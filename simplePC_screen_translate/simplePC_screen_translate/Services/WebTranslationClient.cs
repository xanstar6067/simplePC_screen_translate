// Token discovery and request formats adapted from Traduzir-paginas-web,
// src/background/translationService.js (FilipePS and contributors).
// This file is subject to the Mozilla Public License 2.0. See THIRD_PARTY_NOTICES.md.
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.Services;

public interface ITranslationClient
{
    Task<IReadOnlyList<string>> TranslateAsync(TranslationProvider provider, string target,
        IReadOnlyList<string> texts, CancellationToken cancellationToken);
}

public sealed class WebTranslationClient : ITranslationClient, IDisposable
{
    private const string GoogleReferenceScript = "https://translate.googleapis.com/_/translate_http/_/js/k=translate_http.tr.en_US.YusFYy3P_ro.O/am=AAg/d=1/exm=el_conf/ed=1/rs=AN8SPfq1Hb8iJRleQqQc8zhdzXmF9E56eQ/m=el_main";
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _authLock = new(1, 1);
    private readonly Dictionary<TranslationProvider, (string Value, DateTimeOffset Expires)> _tokens = [];
    private readonly Dictionary<TranslationProvider, DateTimeOffset> _failedDiscovery = [];

    public WebTranslationClient(HttpMessageHandler? handler = null)
    {
        _http = handler is null ? new HttpClient() : new HttpClient(handler);
        _http.Timeout = TimeSpan.FromSeconds(18);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) ScreenTranslator/1.0");
    }

    public async Task<IReadOnlyList<string>> TranslateAsync(TranslationProvider provider, string target,
        IReadOnlyList<string> texts, CancellationToken cancellationToken)
    {
        if (texts.Count == 0) return [];
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var token = await GetTokenAsync(provider, cancellationToken);
            using var request = CreateRequest(provider, token, target, texts);
            using var response = await _http.SendAsync(request, cancellationToken);
            if (attempt == 0 && response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                await InvalidateTokenAsync(provider, cancellationToken);
                continue;
            }
            if (!response.IsSuccessStatusCode)
                throw ProviderError(provider, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            if (attempt == 0 && provider == TranslationProvider.Yandex && HasExpiredYandexSid(json))
            {
                await InvalidateTokenAsync(provider, cancellationToken);
                continue;
            }
            try { return ParseResponse(provider, json, texts.Count); }
            catch (Exception ex) when (ex is JsonException or InvalidOperationException or IndexOutOfRangeException or KeyNotFoundException)
            { throw new TranslationException($"{provider}: изменился формат ответа сервиса.", ex); }
        }
        throw new TranslationException($"{provider}: не удалось обновить авторизацию.");
    }

    private static bool HasExpiredYandexSid(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty("code", out var code) && code.GetInt32() is 401 or 402 or 403;
        }
        catch (JsonException) { return false; }
    }

    private async Task<string> GetTokenAsync(TranslationProvider provider, CancellationToken ct)
    {
        await _authLock.WaitAsync(ct);
        try
        {
            if (_tokens.TryGetValue(provider, out var cached) && cached.Expires > DateTimeOffset.UtcNow)
                return cached.Value;
            if (_failedDiscovery.TryGetValue(provider, out var retryAt) && retryAt > DateTimeOffset.UtcNow)
                throw new TranslationException($"{provider}: временная авторизация недоступна. Повторите через минуту или смените сервис.");
            try
            {
                var value = provider == TranslationProvider.Google
                    ? await DiscoverGoogleTokenAsync(ct) : await DiscoverYandexTokenAsync(ct);
                _tokens[provider] = (value, DateTimeOffset.UtcNow.AddMinutes(20));
                _failedDiscovery.Remove(provider);
                return value;
            }
            catch (Exception ex) when (ex is TranslationException or HttpRequestException)
            {
                _failedDiscovery[provider] = DateTimeOffset.UtcNow.AddMinutes(1);
                if (ex is HttpRequestException { StatusCode: not null } httpError)
                    throw ProviderError(provider, httpError.StatusCode.Value);
                throw;
            }
        }
        finally { _authLock.Release(); }
    }

    private static TranslationException ProviderError(TranslationProvider provider, HttpStatusCode status) =>
        status == HttpStatusCode.TooManyRequests
            ? new($"{provider}: сервис ограничил запросы (HTTP 429). Повторите позже или используйте другой переводчик.")
            : new($"{provider}: сервис вернул HTTP {(int)status}. Попробуйте позже или выберите другой переводчик.");

    private async Task InvalidateTokenAsync(TranslationProvider provider, CancellationToken ct)
    {
        await _authLock.WaitAsync(ct);
        try { _tokens.Remove(provider); _failedDiscovery.Remove(provider); }
        finally { _authLock.Release(); }
    }

    private async Task<string> DiscoverGoogleTokenAsync(CancellationToken ct)
    {
        // Resolve the current module instead of relying only on a versioned URL.
        try
        {
            var loader = await _http.GetStringAsync("https://translate.google.com/translate_a/element.js?cb=googleTranslateElementInit", ct);
            var direct = ExtractGoogleKey(loader);
            if (direct is not null) return direct;
            var script = Regex.Match(loader, @"https://translate\.googleapis\.com/[^\s""']+/m=el_main", RegexOptions.CultureInvariant);
            if (script.Success)
            {
                var module = await _http.GetStringAsync(WebUtility.HtmlDecode(script.Value).Replace(@"\/", "/"), ct);
                var key = ExtractGoogleKey(module);
                if (key is not null) return key;
            }
        }
        catch (HttpRequestException) { /* Try the module used by the reference extension. */ }
        var reference = await _http.GetStringAsync(GoogleReferenceScript, ct);
        return ExtractGoogleKey(reference) ?? throw new TranslationException("Google: временный ключ не найден в скрипте переводчика.");
    }

    public static string? ExtractGoogleKey(string script)
    {
        var match = Regex.Match(script, "[\"']x-goog-api-key[\"']\\s*:\\s*[\"'](AIza[\\w-]{35})[\"']", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return match.Success ? match.Groups[1].Value : null;
    }

    private async Task<string> DiscoverYandexTokenAsync(CancellationToken ct)
    {
        var widget = await _http.GetStringAsync("https://translate.yandex.net/website-widget/v1/widget.js?widgetId=ytWidget&pageLang=en&widgetTheme=light&autoMode=false", ct);
        return ExtractYandexSid(widget) ?? throw new TranslationException("Яндекс: временный SID не найден в скрипте виджета.");
    }

    public static string? ExtractYandexSid(string script)
    {
        var match = Regex.Match(script, @"\bsid\s*:\s*['""]([0-9a-f.]+)['""]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static HttpRequestMessage CreateRequest(TranslationProvider provider, string token, string target, IReadOnlyList<string> texts)
    {
        if (provider == TranslationProvider.Google)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://translate-pa.googleapis.com/v1/translateHtml");
            var html = texts.Select(x => "<pre>" + WebUtility.HtmlEncode(x) + "</pre>").ToArray();
            object[] body = [new object[] { html, "auto", target }, "te"];
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json+protobuf");
            request.Headers.Add("X-goog-api-key", token);
            return request;
        }
        target = target switch { "zh-CN" or "zh-TW" => "zh", "pt" => "pt-BR", _ => target };
        var query = "https://translate.yandex.net/api/v1/tr.json/translate?srv=tr-url-widget&id=" +
            Uri.EscapeDataString(token + "-0-0") + "&format=plain&lang=" + Uri.EscapeDataString(target) +
            string.Concat(texts.Select(x => "&text=" + Uri.EscapeDataString(x)));
        return new HttpRequestMessage(HttpMethod.Get, query);
    }

    public static IReadOnlyList<string> ParseResponse(TranslationProvider provider, string json, int expectedCount)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        string[] result;
        if (provider == TranslationProvider.Google)
        {
            result = root[0].EnumerateArray().Select(x => CleanGoogleHtml(x.GetString() ?? "")).ToArray();
        }
        else
        {
            if (root.TryGetProperty("code", out var code) && code.GetInt32() != 200)
                throw new TranslationException($"Яндекс: сервис вернул код {code.GetInt32()}.");
            result = root.GetProperty("text").EnumerateArray().Select(x => x.GetString() ?? "").ToArray();
        }
        if (result.Length != expectedCount || result.Any(string.IsNullOrWhiteSpace))
            throw new TranslationException($"{provider}: получен неполный перевод. Попробуйте другой сервис.");
        return result;
    }

    private static string CleanGoogleHtml(string html)
    {
        // Google can return both translated <b> content and original <i> content.
        html = Regex.Replace(html, @"<i\b[^>]*>.*?</i>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<[^>]+>", "");
        return WebUtility.HtmlDecode(html).Replace('\u200b', ' ').Trim();
    }

    public void Dispose() { _http.Dispose(); _authLock.Dispose(); }
}

public sealed class TranslationException(string message, Exception? inner = null) : Exception(message, inner);
