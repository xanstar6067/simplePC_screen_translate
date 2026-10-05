using simplePC_screen_translate.Models;
using simplePC_screen_translate.Native;
using simplePC_screen_translate.Services;
using simplePC_screen_translate.UI;

namespace simplePC_screen_translate;

public partial class Form1 : Form
{
    private readonly SettingsStore _store;
    private readonly WebTranslationClient _client = null!;
    private readonly TranslationService _translator = null!;
    private readonly LocalOcrService _ocr = new();
    private readonly HotkeyManager _hotkeys = null!;
    private readonly NotifyIcon _tray = null!;
    private readonly System.Windows.Forms.Timer _overlayTimer = new();
    private AppSettings _settings;
    private CancellationTokenSource? _operation;
    private OverlayForm? _overlay;
    private RegionSelectionForm? _selection;
    private Rectangle? _lastRegion;
    private bool _exiting, _hotkeyAvailable, _restoring, _dirty;
    private bool _loading = true;
    private bool _resourcesDisposed;

    public Form1() : this(null) { }

    public Form1(SettingsStore? store)
    {
        InitializeComponent();
        Font = SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont;
        Icon = SystemIcons.Application;
        _store = store ?? new SettingsStore();
        _settings = new();
        if (DesignMode || System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            return;
        _settings = _store.Load(out var warning);
        _client = new WebTranslationClient();
        _hotkeys = new HotkeyManager();
        _translator = new(_client);
        PopulateControls();
        LoadSettings(_settings);
        TrackChanges(this);
        _preview.Paint += PaintPreview;
        _translateButton.Click += async (_, _) =>
        {
            if (_operation is not null) { CancelOperation(); return; }
            if (SaveSettings()) await TranslateScreenAsync(CaptureMode.SelectRegion);
        };
        _screenButton.Click += async (_, _) =>
        {
            if (SaveSettings()) await TranslateScreenAsync(CaptureMode.Monitor);
        };
        _saveButton.Click += (_, _) => SaveSettings();
        _hideButton.Click += (_, _) => HideOverlay();
        _font.Click += (_, _) => ChooseFont();
        _textColor.Click += (_, _) => ChooseColor(true);
        _backgroundColor.Click += (_, _) => ChooseColor(false);
        _windowsLanguages.Click += (_, _) => OpenWindowsLanguages();
        _hotkey.ValueChanged += (_, _) => MarkChanged();
        _hotkey.CaptureStarted += (_, _) =>
        {
            if (_exiting) return;
            _hotkeys.Suspend();
            _hotkeys.DisableEscape();
        };
        _hotkey.CaptureFinished += (_, _) =>
        {
            if (_exiting) return;
            try { _hotkeys.Resume(); _hotkeyAvailable = true; }
            catch (InvalidOperationException ex) { _hotkeyAvailable = false; SetStatus(ex.Message); }
            if (_operation is not null || _overlay is not null) _hotkeys.EnableEscape();
        };
        _hotkeys.TranslatePressed += async (_, _) =>
        {
            if (_restoring || _exiting) return;
            if (_operation is not null) { CancelOperation(); return; }
            if (_overlay is not null) { HideOverlay(); return; }
            await TranslateScreenAsync();
        };
        _hotkeys.EscapePressed += (_, _) => { CancelOperation(); HideOverlay(); };
        _overlayTimer.Tick += (_, _) => HideOverlay();
        var menu = new ContextMenuStrip();
        menu.Items.Add("Настройки", null, (_, _) => ShowSettings());
        menu.Items.Add("Перевести", null, async (_, _) =>
        {
            if (_operation is not null) CancelOperation(); else await TranslateScreenAsync();
        });
        menu.Items.Add("Убрать перевод", null, (_, _) => HideOverlay());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Выход", null, (_, _) => { _exiting = true; Close(); });
        _tray = new NotifyIcon(components!) { Icon = Icon, Text = "Экранный переводчик", Visible = true, ContextMenuStrip = menu };
        _tray.DoubleClick += (_, _) => ShowSettings();
        _tray.BalloonTipClicked += (_, _) => ShowSettings();
        TryRegisterHotkey();
        _loading = false;
        _saveButton.Enabled = false;
        if (warning is not null) SetStatus(warning);
        else if (_hotkeyAvailable) SetStatus($"Готово. {_settings.Hotkey} — перевод; повторное нажатие — убрать оверлей.");
    }

    private bool SaveSettings()
    {
        if (_operation is not null) return false;
        try
        {
            var candidate = ReadSettings();
            if (candidate.MinimumFontSize > candidate.MaximumFontSize)
                throw new InvalidOperationException("Минимальный размер шрифта не должен превышать максимальный.");
            _hotkeys.Register(candidate.Hotkey);
            try { _store.Save(candidate); }
            catch { _hotkeys.Register(_settings.Hotkey); throw; }
            _settings = candidate;
            _hotkeyAvailable = true;
            _dirty = false;
            _saveButton.Enabled = false;
            SetStatus($"Настройки сохранены. {_settings.Hotkey} — перевод.");
            UpdateTranslateCaption();
            HideOverlay(false);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        { ShowError(ex.Message); return false; }
    }
    private void TryRegisterHotkey()
    {
        try { _hotkeys.Register(_settings.Hotkey); _hotkeyAvailable = true; }
        catch (InvalidOperationException ex) { _hotkeyAvailable = false; SetStatus(ex.Message); }
    }

    private async Task TranslateScreenAsync(CaptureMode? modeOverride = null)
    {
        if (_operation is not null || _exiting) return;
        HideOverlay(false);
        var settings = modeOverride is CaptureMode mode ? _settings with { CaptureMode = mode } : _settings;
        var operation = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        _operation = operation;
        _translateButton.Text = "Отменить";
        _saveButton.Enabled = false;
        _screenButton.Enabled = false;
        var wasVisible = Visible && WindowState != FormWindowState.Minimized;
        Bitmap? screenshot = null;
        try
        {
            var monitor = Screen.FromPoint(Cursor.Position).Bounds;
            Hide();
            NativeMethods.DwmFlush();
            await Task.Delay(180, operation.Token);
            Rectangle region;
            if (settings.CaptureMode == CaptureMode.Monitor) region = monitor;
            else if (settings.CaptureMode == CaptureMode.LastRegion && _lastRegion is Rectangle saved &&
                Screen.AllScreens.Any(x => x.Bounds.IntersectsWith(saved)))
                region = Rectangle.Intersect(saved, SystemInformation.VirtualScreen);
            else
            {
                var desktop = SystemInformation.VirtualScreen;
                using var frozen = ScreenCapture.Capture(desktop);
                using var selection = new RegionSelectionForm(frozen, desktop);
                _selection = selection;
                var selected = selection.ShowDialog();
                _selection = null;
                if (selected != DialogResult.OK)
                { SetStatus("Выделение отменено."); if (wasVisible) ShowSettings(); return; }
                region = selection.SelectedBounds;
                _lastRegion = region;
                screenshot = frozen.Clone(new(region.X - desktop.X, region.Y - desktop.Y, region.Width, region.Height), frozen.PixelFormat);
            }
            operation.Token.ThrowIfCancellationRequested();
            screenshot ??= ScreenCapture.Capture(region);
            _hotkeys.EnableEscape();
            var progress = new Progress<string>(text => { if (!_exiting) SetStatus(text); });
            var blocks = await Task.Run(() => _ocr.RecognizeAsync(screenshot, settings, progress, operation.Token), operation.Token);
            if (blocks.Count == 0)
            {
                SetStatus("Текст не найден. Выберите другую область или язык OCR.");
                Notify("Текст не найден", "Выберите другую область или язык OCR в настройках.");
                if (wasVisible) ShowSettings();
                return;
            }
            SetStatus($"Перевод {blocks.Count} блоков · {settings.Provider}…");
            var result = await _translator.TranslateAsync(blocks.Select(x => x.Text).ToArray(), settings, operation.Token);
            operation.Token.ThrowIfCancellationRequested();
            var translated = blocks.Select((block, i) => new TranslatedBlock(block, result.Texts[i])).ToArray();
            var rendered = OverlayRenderer.Render(region.Size, translated, settings);
            using (rendered.Bitmap)
            {
                _overlay = new OverlayForm(region);
                _overlay.Show();
                _overlay.SetBitmap(rendered.Bitmap);
            }
            _hideButton.Enabled = true;
            if (settings.OverlaySeconds > 0)
            { _overlayTimer.Interval = settings.OverlaySeconds * 1000; _overlayTimer.Start(); }
            _hotkeys.DisableEscape();
            var escape = _hotkeys.EnableEscape() ? "Esc или горячая клавиша — убрать." : "Горячая клавиша или меню в трее — убрать.";
            var details = result.UsedFallback ? $" Использован резервный сервис {result.Provider}." : $" {result.Provider}.";
            if (rendered.TruncatedBlocks > 0) details += $" Не вместилось блоков: {rendered.TruncatedBlocks}; уменьшите шрифт или отступы.";
            SetStatus($"Переведено блоков: {blocks.Count}.{details} {escape}");
            if (result.UsedFallback || rendered.TruncatedBlocks > 0) Notify("Перевод готов", _status.Text);
        }
        catch (OperationCanceledException)
        {
            if (!_exiting)
            {
                SetStatus("Перевод отменён или превышено время ожидания.");
                if (wasVisible) ShowSettings();
            }
        }
        catch (Exception ex)
        {
            HideOverlay(false);
            if (!_exiting)
            {
                var message = ex is System.Net.Http.HttpRequestException ? "Не удалось подключиться к переводчику. Проверьте интернет. " + ex.Message : ex.Message;
                SetStatus(message);
                Notify("Не удалось перевести", message, ToolTipIcon.Warning);
                ShowSettings();
            }
        }
        finally
        {
            screenshot?.Dispose();
            _selection = null;
            if (_overlay is null) _hotkeys.DisableEscape();
            _operation = null;
            operation.Dispose();
            if (!_exiting) { UpdateTranslateCaption(); _saveButton.Enabled = _dirty; _screenButton.Enabled = true; }
        }
    }

    private void CancelOperation()
    {
        _operation?.Cancel();
        if (_selection is not null) { _selection.DialogResult = DialogResult.Cancel; _selection.Close(); }
    }
    private void HideOverlay(bool updateStatus = true)
    {
        _overlayTimer.Stop();
        var hadOverlay = _overlay is not null;
        _overlay?.Close(); _overlay?.Dispose(); _overlay = null;
        _hideButton.Enabled = false;
        _hotkeys?.DisableEscape();
        if (hadOverlay && updateStatus) SetStatus("Перевод убран. Горячая клавиша — новый перевод.");
    }
    private void ShowSettings()
    {
        if (_exiting) return;
        HideOverlay(false);
        _restoring = true;
        Show(); WindowState = FormWindowState.Normal; Activate();
        _restoring = false;
    }
    private void SetStatus(string text)
    {
        _status.Text = text;
        if (_tray is not null) _tray.Text = text.Length > 63 ? text[..60] + "…" : text;
    }
    private void Notify(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
    { _tray.ShowBalloonTip(5000, title, text, icon); }
    private void ShowError(string text)
    { SetStatus(text); MessageBox.Show(this, text, "Экранный переводчик", MessageBoxButtons.OK, MessageBoxIcon.Information); }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_tray is null) { base.OnFormClosing(e); return; }
        if (!_exiting && e.CloseReason == CloseReason.UserClosing && _settings.CloseToTray)
        {
            e.Cancel = true; Hide();
            Notify("Приложение работает в трее", $"{_settings.Hotkey} — перевод. Двойной клик по значку — настройки; «Выход» в меню — закрыть приложение.");
            return;
        }
        _exiting = true;
        CancelOperation(); HideOverlay(false); _tray.Visible = false;
        base.OnFormClosing(e);
    }
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        DisposeResources();
        base.OnFormClosed(e);
    }
    private void DisposeResources()
    {
        if (_resourcesDisposed) return;
        _resourcesDisposed = true;
        _exiting = true;
        CancelOperation(); HideOverlay(false);
        _tray?.Dispose(); _overlayTimer.Dispose(); _hotkeys?.Dispose(); _client?.Dispose();
    }
}
