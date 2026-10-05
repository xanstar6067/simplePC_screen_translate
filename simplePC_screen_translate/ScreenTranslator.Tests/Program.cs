using System.Drawing.Imaging;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;
using simplePC_screen_translate;
using simplePC_screen_translate.Models;
using simplePC_screen_translate.Native;
using simplePC_screen_translate.Services;
using simplePC_screen_translate.UI;

internal static class Program
{
    private static int _passed;
    private static string _output = "";
    [STAThread]
    private static int Main(string[] args)
    {
        _output = Path.GetFullPath("artifacts/checks");
        Directory.CreateDirectory(_output);
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetDefaultFont(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont);
        try
        {
            if (args.Contains("--live-only"))
            {
                Task.Run(LiveAsync).GetAwaiter().GetResult();
                return 0;
            }
            SettingsRoundtrip();
            Parsers();
            ParagraphGeometry();
            OverlayAlpha();
            HotkeyConflict();
            HotkeyRecording();
            DesignerInitialization();
            Task.Run(CoreAsync).GetAwaiter().GetResult();
            SettingsLayout();
            if (args.Contains("--live")) Task.Run(LiveAsync).GetAwaiter().GetResult();
            Console.WriteLine($"PASS: {_passed} checks. Artifacts: {_output}");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
    private static void Assert(bool condition, string message)
    { if (!condition) throw new Exception(message); }
    private static void Pass(string name) { _passed++; Console.WriteLine("PASS " + name); }

    private static void SettingsRoundtrip()
    {
        var path = Path.Combine(_output, "settings-roundtrip.json");
        foreach (var suffix in new[] { "", ".bak", ".damaged", ".tmp" }) if (File.Exists(path + suffix)) File.Delete(path + suffix);
        var store = new SettingsStore(path);
        var original = new AppSettings { TargetLanguage = "ja", BackgroundOpacity = 63, Padding = 7,
            Provider = TranslationProvider.Yandex, Hotkey = new() { Key = Keys.F8, Alt = false, Shift = true }, CloseToTray = false };
        store.Save(original);
        Assert(store.Load(out _) == original, "Settings changed after roundtrip");
        store.Save(original with { TargetLanguage = "fr" });
        File.WriteAllText(path, "{ damaged");
        var recovered = store.Load(out var warning);
        Assert(recovered == original && warning is not null, "Backup recovery failed");
        store.Save(recovered);
        Assert(File.ReadAllText(path + ".damaged") == "{ damaged", "Damaged settings were not preserved");
        Assert((original with { BackgroundOpacity = -5, FontSize = 999, TextColor = "invalid", Hotkey = null! }).Normalize()
            is { BackgroundOpacity: 10, FontSize: 64, TextColor: "#FFFFFF", Hotkey.IsValid: true }, "Settings validation failed");
        Pass("settings persistence, backup recovery and validation");
    }
    private static void Parsers()
    {
        var key = "AIza" + new string('a', 35);
        Assert(WebTranslationClient.ExtractGoogleKey("{'x-goog-api-key': '" + key + "'}") == key, "Google key discovery failed");
        Assert(WebTranslationClient.ExtractYandexSid("sid: 'a1.b2.c3'") == "a1.b2.c3", "Yandex SID discovery failed");
        Assert(WebTranslationClient.ExtractGoogleKey("unexpected script") is null, "Accepted invalid key");
        var google = WebTranslationClient.ParseResponse(TranslationProvider.Google,
            JsonSerializer.Serialize(new object[] { new[] { "<pre><b>Привет &amp; мир</b><i>Hello world</i></pre>" }, new[] { "en" } }), 1);
        Assert(google.Single() == "Привет & мир", "Google included source text or markup");
        Assert(WebTranslationClient.ParseResponse(TranslationProvider.Yandex, "{\"code\":200,\"text\":[\"Привет\"]}", 1).Single() == "Привет", "Yandex parse failed");
        try { WebTranslationClient.ParseResponse(TranslationProvider.Google, "[[\"only one\"]]", 2); throw new Exception("Accepted incomplete translation"); }
        catch (TranslationException) { }
        Pass("key extraction and provider response contracts");
    }
    private static void ParagraphGeometry()
    {
        var merged = LocalOcrService.MergeParagraphs([
            new("A sentence continues", new(10, 10, 180, 20), 20),
            new("onto the second line", new(10, 35, 180, 20), 20),
            new("Separate column text", new(300, 35, 180, 20), 20)
        ]);
        Assert(merged.Count == 2 && merged[0].Text.Contains("second line") && merged[0].Bounds.Height == 45, "Paragraphs crossed columns or lost bounds");
        Pass("paragraph grouping preserves columns and coordinates");
    }
    private static void OverlayAlpha()
    {
        var settings = new AppSettings { BackgroundOpacity = 50 };
        var rendered = OverlayRenderer.Render(new(640, 240),
            [new(new("Hello", new(30, 30, 240, 30), 24), "Привет, мир")], settings);
        using var bitmap = rendered.Bitmap;
        Assert(bitmap.GetPixel(0, 0).A == 0, "Overlay covers unrelated pixels");
        Assert(Math.Abs(bitmap.GetPixel(28, 28).A - 127) <= 1, "Background alpha is incorrect");
        var opaqueText = false;
        for (var y = 32; y < 60; y++) for (var x = 32; x < 270; x++)
            if (bitmap.GetPixel(x, y).A > 240) opaqueText = true;
        Assert(opaqueText, "Text inherited background transparency");
        bitmap.Save(Path.Combine(_output, "overlay.png"), ImageFormat.Png);
        using var layer = new OverlayForm(new(50, 50, 640, 240));
        layer.CreateControl();
        layer.SetBitmap(bitmap);
        Pass("overlay alpha, text opacity and native layered window");
    }
    private static void HotkeyConflict()
    {
        using var first = new HotkeyManager();
        using var second = new HotkeyManager();
        var key = new HotkeySettings { Key = Keys.F19, Windows = false, Alt = true, Control = true };
        first.Register(key);
        try { second.Register(key); throw new Exception("Hotkey conflict was ignored"); }
        catch (InvalidOperationException) { }
        first.Register(key with { Key = Keys.F20 });
        second.Register(key);
        first.Suspend();
        second.Register(key with { Key = Keys.F20 });
        try { first.Resume(); throw new Exception("A hotkey stolen during recording was silently accepted"); }
        catch (InvalidOperationException) { }
        second.Register(key);
        first.Resume();
        Pass("global hotkey conflict detection and replacement");
    }
    private static void HotkeyRecording()
    {
        using var editor = new TestHotkeyTextBox();
        var before = editor.Value;
        var entered = 0;
        var left = 0;
        editor.CaptureStarted += (_, _) => entered++;
        editor.CaptureFinished += (_, _) => left++;
        editor.BeginCapture();
        editor.Press(Keys.ControlKey | Keys.Control);
        Assert(editor.Value == before, "A modifier alone changed the saved hotkey");
        editor.Press(Keys.F8 | Keys.Control | Keys.Shift);
        Assert(editor.Value is { Key: Keys.F8, Control: true, Shift: true, Alt: false, Windows: false }, "A recorded combination is incorrect");
        Assert(editor.Text == "Ctrl + Shift + F8", "Recorded hotkey is not readable");
        editor.Command(Keys.Control | Keys.Tab);
        Assert(editor.Value is { Key: Keys.Tab, Control: true }, "Navigation keys were not recorded");
        editor.Press(Keys.LWin);
        editor.Press(Keys.F9);
        Assert(editor.Value is { Key: Keys.F9, Windows: true }, "Windows modifier was lost");
        editor.Release(Keys.LWin);
        editor.Press(Keys.F10);
        Assert(editor.Value is { Key: Keys.F10, Windows: false, Alt: false, Control: false }, "A single key inherited modifiers");
        editor.Press(Keys.Escape);
        Assert(editor.Value == before, "Escape did not restore the previous hotkey");
        editor.EndCapture();
        editor.BeginCapture();
        editor.Press(Keys.F12);
        editor.EndCapture();
        Assert(editor.Value.Key == Keys.F12 && editor.Text == "F12" && entered == 2 && left == 2, "Recorded key was lost on leaving the field");
        var path = Path.Combine(_output, "recorded-hotkey.json");
        var store = new SettingsStore(path);
        store.Save(new AppSettings { Hotkey = editor.Value });
        Assert(store.Load(out _).Hotkey == editor.Value, "Recorded key was lost after restarting");
        Pass("hotkey recording: combinations, navigation, Win, Escape and persistence");
    }
    private static void DesignerInitialization()
    {
        var previous = System.ComponentModel.LicenseManager.CurrentContext;
        try
        {
            System.ComponentModel.LicenseManager.CurrentContext = new TestDesignContext();
            using var form = (Form1)Activator.CreateInstance(typeof(Form1))!;
            var tabs = Descendants(form).OfType<TabControl>().Single();
            Assert(tabs.TabPages.Count == 3 && Descendants(form).OfType<HotkeyTextBox>().Count() == 1, "Designer has no settings controls");
            foreach (var name in new[] { "_client", "_hotkeys", "_tray" })
                Assert(typeof(Form1).GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(form) is null,
                    "Designer started runtime service " + name);
        }
        finally { System.ComponentModel.LicenseManager.CurrentContext = previous; }
        Pass("parameterless designer initialization without HTTP, tray or global hotkeys");
    }
    private static async Task CoreAsync()
    {
        var handler = new GoogleHandler();
        using (var web = new WebTranslationClient(handler))
        {
            Assert((await web.TranslateAsync(TranslationProvider.Google, "ru", ["Hello"], default)).Single() == "Привет", "Google translation failed");
            await web.TranslateAsync(TranslationProvider.Google, "ru", ["Hello"], default);
            Assert(handler.Discoveries == 2 && handler.Translations == 3, "Token refresh or token cache failed");
        }
        Pass("expired Google key refresh and in-memory token reuse");
        var yandexHandler = new YandexHandler();
        using (var yandex = new WebTranslationClient(yandexHandler))
        {
            await yandex.TranslateAsync(TranslationProvider.Yandex, "zh-CN", ["Hello & goodbye"], default);
            await yandex.TranslateAsync(TranslationProvider.Yandex, "zh-CN", ["Hello & goodbye"], default);
            Assert(yandexHandler.Discoveries == 2 && yandexHandler.Translations == 3, "Yandex SID refresh or cache failed");
        }
        Pass("Yandex SID refresh, target language mapping and request escaping");
        var client = new FakeClient();
        var service = new TranslationService(client);
        var result = await service.TranslateAsync(["Hello", "Hello", "World"], new(), default);
        Assert(result.UsedFallback && result.Provider == TranslationProvider.Yandex && result.Texts[0] == result.Texts[1], "Fallback or deduplication failed");
        var count = client.YandexRequests;
        await service.TranslateAsync(["Hello", "World"], new(), default);
        Assert(client.YandexRequests == count, "Translation cache was ignored");
        client.FailGoogle = false;
        var longText = string.Concat(Enumerable.Repeat("Перевод текста 中文 🚀 ", 250));
        var longResult = await service.TranslateAsync([longText], new() { UseFallback = false }, default);
        Assert(longResult.Texts.Single().Length > 0 && client.MaximumEncodedBatch <= 5500, "Long Unicode text exceeded request limits");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try { await service.TranslateAsync(["Uncached"], new(), cancellation.Token); throw new Exception("Cancellation was ignored"); }
        catch (OperationCanceledException) { }
        Pass("fallback, deduplication, bounded batches, cache and cancellation");

        var languages = LocalOcrService.GetLanguages();
        Console.WriteLine("OCR languages: " + string.Join(", ", languages.Select(x => x.Code)));
        Assert(languages.Count > 0, "No local OCR language is installed");
        using var image = new Bitmap(800, 150);
        using (var graphics = Graphics.FromImage(image))
        {
            graphics.Clear(Color.White);
            using var font = new Font("Segoe UI", 28, GraphicsUnit.Pixel);
            graphics.DrawString("Hello world. Screen translation works.", font, Brushes.Black, 35, 40);
        }
        image.Save(Path.Combine(_output, "ocr-input.png"), ImageFormat.Png);
        var blocks = await new LocalOcrService().RecognizeAsync(image, new() { MergeLines = false }, null, default);
        Assert(blocks.Any(x => x.Text.Contains("Hello", StringComparison.OrdinalIgnoreCase)), "Local OCR did not recognize generated text");
        Assert(blocks.All(x => x.Bounds.Left >= 0 && x.Bounds.Right <= 800 && x.Bounds.Top >= 0 && x.Bounds.Bottom <= 150), "OCR coordinates are outside the screenshot");
        Console.WriteLine("OCR sample: " + string.Join(" | ", blocks.Select(x => x.Text)));
        Pass("real Windows OCR and physical pixel coordinates");
        using var largeImage = new Bitmap(2800, 220);
        using (var graphics = Graphics.FromImage(largeImage))
        {
            graphics.Clear(Color.White);
            using var font = new Font("Segoe UI", 26, GraphicsUnit.Pixel);
            graphics.DrawString("A sentence crosses the tile boundary correctly", font, Brushes.Black, 1080, 40);
            graphics.DrawString("Это русский текст для проверки", font, Brushes.Black, 60, 120);
        }
        var tiled = await new LocalOcrService().RecognizeAsync(largeImage, new() { MergeLines = false }, null, default);
        var tiledText = string.Join(" ", tiled.Select(x => x.Text));
        Assert(tiledText.Contains("boundary", StringComparison.OrdinalIgnoreCase), "OCR lost text across tile boundary");
        Assert(tiledText.Contains("русский", StringComparison.OrdinalIgnoreCase), "Automatic OCR lost Russian text");
        Assert(tiledText.Split("boundary", StringSplitOptions.None).Length == 2, "OCR duplicated tile boundary text");
        Pass("large screenshot tiling and automatic English/Russian OCR");
    }
    private static void SettingsLayout()
    {
        using var form = new Form1(new SettingsStore(Path.Combine(_output, "ui-settings.json")));
        var systemFont = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
        Console.WriteLine($"Windows UI font: {systemFont.Name}, {systemFont.SizeInPoints} pt. Form font: {form.Font.Name}");
        Assert(form.Font.Name == systemFont.Name, "Settings do not use the Windows UI font");
        Assert(Descendants(form).OfType<Button>().Any(x => x.Text == "Весь экран"), "Direct full screen translation is missing");
        Assert(Descendants(form).OfType<HotkeyTextBox>().Single().Value == new HotkeySettings(), "Default hotkey recording field is missing");
        Assert(!Descendants(form).OfType<CheckBox>().Any(x => x.Text is "Ctrl" or "Alt" or "Shift" or "Win"), "Old modifier checkboxes remain");
        form.ShowInTaskbar = false;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new(-10000, -10000);
        form.Show();
        form.CreateControl();
        var tabs = Descendants(form).OfType<TabControl>().Single();
        foreach (TabPage page in tabs.TabPages)
        {
            tabs.SelectedTab = page;
            form.PerformLayout();
            Application.DoEvents();
            using var bitmap = new Bitmap(form.Width, form.Height);
            form.DrawToBitmap(bitmap, new(Point.Empty, form.Size));
            bitmap.Save(Path.Combine(_output, "settings-" + page.TabIndex + ".png"), ImageFormat.Png);
            Assert(tabs.ClientRectangle.Height > 300, "Settings tabs collapsed");
            Assert(Descendants(page).OfType<ComboBox>().All(x => x.Width > 100), "Settings controls collapsed");
        }
        form.Size = new(660, 600);
        form.PerformLayout();
        using var minimum = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(minimum, new(Point.Empty, form.Size));
        minimum.Save(Path.Combine(_output, "settings-minimum.png"), ImageFormat.Png);
        Pass("settings window and all tabs render at default and minimum sizes");
    }
    private static IEnumerable<Control> Descendants(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            yield return child;
            foreach (var nested in Descendants(child)) yield return nested;
        }
    }
    private static async Task LiveAsync()
    {
        using var client = new WebTranslationClient();
        foreach (var provider in Enum.GetValues<TranslationProvider>())
        {
            try
            {
                var translated = await client.TranslateAsync(provider, "ru", ["Hello world", "Open settings"], default);
                Assert(translated.All(x => x.Any(ch => ch is >= '\u0400' and <= '\u04ff')), $"{provider} returned no Russian translation");
                Console.WriteLine($"LIVE {provider}: " + string.Join(" | ", translated));
                Pass($"live {provider} translation");
            }
            catch (TranslationException ex) when (provider == TranslationProvider.Yandex && ex.Message.Contains("429"))
            { Console.WriteLine("UNAVAILABLE LIVE Yandex: " + ex.Message); }
        }
        var fallback = await new TranslationService(client).TranslateAsync(["Hello world"],
            new() { Provider = TranslationProvider.Yandex, UseFallback = true }, default);
        Assert(fallback.Texts.Single().Contains("мир"), "Live fallback returned no Russian text");
        Console.WriteLine($"LIVE preferred Yandex, actual {fallback.Provider}, fallback {fallback.UsedFallback}");
        Pass("live preferred Yandex with automatic Google fallback when unavailable");
    }
    private sealed class GoogleHandler : HttpMessageHandler
    {
        public int Discoveries, Translations;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.RequestUri!.Host == "translate.google.com")
            {
                Discoveries++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent("{'x-goog-api-key':'AIza" + new string('a', 35) + "'}") });
            }
            Translations++;
            Assert(request.Headers.Contains("X-goog-api-key") && request.Method == HttpMethod.Post, "Google request is missing authentication");
            return Task.FromResult(new HttpResponseMessage(Translations == 1 ? HttpStatusCode.Forbidden : HttpStatusCode.OK)
            { Content = new StringContent("[[\"<pre>Привет</pre>\"],[\"en\"]]") });
        }
    }
    private sealed class FakeClient : ITranslationClient
    {
        public bool FailGoogle = true;
        public int YandexRequests, MaximumEncodedBatch;
        public Task<IReadOnlyList<string>> TranslateAsync(TranslationProvider provider, string target, IReadOnlyList<string> texts, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (provider == TranslationProvider.Google && FailGoogle) throw new TranslationException("Simulated unavailable Google");
            if (provider == TranslationProvider.Yandex) YandexRequests++;
            MaximumEncodedBatch = Math.Max(MaximumEncodedBatch, texts.Sum(x => Uri.EscapeDataString(x).Length + 6));
            return Task.FromResult<IReadOnlyList<string>>(texts.Select(x => "translated " + x).ToArray());
        }
    }
    private sealed class YandexHandler : HttpMessageHandler
    {
        public int Discoveries, Translations;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.RequestUri!.AbsolutePath.EndsWith("widget.js"))
            {
                Discoveries++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("sid: 'abc.123'") });
            }
            Translations++;
            var query = request.RequestUri.Query;
            Assert(query.Contains("lang=zh") && query.Contains("id=abc.123-0-0") && query.Contains("text=Hello%20%26%20goodbye"), "Yandex request was malformed");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(Translations == 1 ? "{\"code\":403}" : "{\"code\":200,\"text\":[\"你好\"]}") });
        }
    }
    private sealed class TestHotkeyTextBox : HotkeyTextBox
    {
        public void BeginCapture() => OnEnter(EventArgs.Empty);
        public void EndCapture() => OnLeave(EventArgs.Empty);
        public void Press(Keys key) => OnKeyDown(new KeyEventArgs(key));
        public void Release(Keys key) => OnKeyUp(new KeyEventArgs(key));
        public void Command(Keys key)
        {
            var message = Message.Create(0, 0x100, (nint)(key & Keys.KeyCode), 0);
            ProcessCmdKey(ref message, key);
        }
        protected override bool IsWindowsKeyDown() => false;
    }
    private sealed class TestDesignContext : System.ComponentModel.LicenseContext
    {
        public override System.ComponentModel.LicenseUsageMode UsageMode => System.ComponentModel.LicenseUsageMode.Designtime;
    }
}
