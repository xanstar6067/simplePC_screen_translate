using System.Net.Http;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.Services;

public sealed class TranslationService(ITranslationClient client)
{
    private readonly Dictionary<(TranslationProvider Provider, string Target, string Text), (string Text, DateTimeOffset Expires)> _cache = [];

    public async Task<TranslationResult> TranslateAsync(IReadOnlyList<string> texts, AppSettings settings, CancellationToken ct)
    {
        try
        {
            return new(await TranslateWithProviderAsync(texts, settings.Provider, settings.TargetLanguage, ct), settings.Provider, false);
        }
        catch (Exception ex) when (settings.UseFallback && !ct.IsCancellationRequested &&
            ex is TranslationException or HttpRequestException or TaskCanceledException)
        {
            var alternative = settings.Provider == TranslationProvider.Google ? TranslationProvider.Yandex : TranslationProvider.Google;
            try { return new(await TranslateWithProviderAsync(texts, alternative, settings.TargetLanguage, ct), alternative, true); }
            catch (Exception fallbackEx) when (!ct.IsCancellationRequested && fallbackEx is TranslationException or HttpRequestException or TaskCanceledException)
            { throw new TranslationException("Оба переводчика недоступны. " + ex.Message + " " + fallbackEx.Message, fallbackEx); }
        }
    }

    private async Task<IReadOnlyList<string>> TranslateWithProviderAsync(IReadOnlyList<string> texts,
        TranslationProvider provider, string target, CancellationToken ct)
    {
        var pieces = texts.Select(SplitText).ToArray();
        var unique = pieces.SelectMany(x => x).Distinct().ToArray();
        var now = DateTimeOffset.UtcNow;
        var translated = new Dictionary<string, string>();
        var missing = new List<string>();
        foreach (var text in unique)
        {
            if (_cache.TryGetValue((provider, target, text), out var cached) && cached.Expires > now)
                translated[text] = cached.Text;
            else missing.Add(text);
        }
        foreach (var batch in Batch(missing))
        {
            ct.ThrowIfCancellationRequested();
            var result = await client.TranslateAsync(provider, target, batch, ct);
            if (result.Count != batch.Count) throw new TranslationException("Переводчик вернул неверное число строк.");
            for (var i = 0; i < batch.Count; i++) translated[batch[i]] = result[i];
        }
        // Commit only a complete response so a fallback never leaves a half-filled cache.
        if (_cache.Count + translated.Count > 2000) _cache.Clear();
        foreach (var item in translated) _cache[(provider, target, item.Key)] = (item.Value, now.AddMinutes(30));
        return pieces.Select(x => string.Join(" ", x.Select(p => translated[p]))).ToArray();
    }

    private static string[] SplitText(string text)
    {
        var pieces = new List<string>();
        while (text.Length > 0)
        {
            var length = Math.Min(1000, text.Length);
            while (Uri.EscapeDataString(text[..length]).Length > 4500) length = length * 3 / 4;
            if (length < text.Length)
            {
                var space = text.LastIndexOf(' ', length - 1, Math.Min(length, 300));
                if (space >= length * 0.7) length = space;
            }
            if (char.IsHighSurrogate(text[length - 1])) length--;
            pieces.Add(text[..length]);
            text = text[length..].TrimStart();
        }
        return pieces.ToArray();
    }

    private static IEnumerable<IReadOnlyList<string>> Batch(IEnumerable<string> texts)
    {
        var batch = new List<string>();
        var size = 0;
        foreach (var text in texts)
        {
            var encodedSize = Uri.EscapeDataString(text).Length + 6;
            if (batch.Count > 0 && (size + encodedSize > 5500 || batch.Count >= 30))
            { yield return batch.ToArray(); batch.Clear(); size = 0; }
            batch.Add(text);
            size += encodedSize;
        }
        if (batch.Count > 0) yield return batch.ToArray();
    }
}
