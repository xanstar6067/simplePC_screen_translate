using System.Diagnostics;
using simplePC_screen_translate.Models;
using simplePC_screen_translate.Services;
using simplePC_screen_translate.UI;

namespace simplePC_screen_translate;

public partial class Form1
{
    private string _fontName = "Segoe UI";
    private Color _foreground = Color.White, _background = Color.FromArgb(24, 24, 24);

    private void OpenWindowsLanguages()
    {
        try { Process.Start(new ProcessStartInfo("ms-settings:regionlanguage") { UseShellExecute = true }); }
        catch (Exception ex) { ShowError("Не удалось открыть параметры Windows: " + ex.Message); }
    }

    private void PopulateControls()
    {
        _target.Items.Clear(); _provider.Items.Clear(); _captureMode.Items.Clear(); _alignment.Items.Clear(); _ocrLanguage.Items.Clear();
        _target.Items.AddRange(Languages.All);
        _provider.Items.AddRange(["Google", "Яндекс"]);
        _captureMode.Items.AddRange(["Выбрать область", "Весь экран", "Последняя область"]);
        _alignment.Items.AddRange(["По левому краю", "По центру"]);
        _ocrLanguage.Items.Add(new LanguageOption("auto", "Автоматически"));
        try
        {
            _ocrLanguage.Items.AddRange(LocalOcrService.GetLanguages().Cast<object>().ToArray());
            if (_ocrLanguage.Items.Count == 1) SetStatus("Установите компонент OCR в параметрах языка Windows.");
        }
        catch (Exception ex) { SetStatus("OCR недоступен: " + ex.Message); }
    }
    private void LoadSettings(AppSettings settings)
    {
        _target.SelectedItem = Languages.All.First(x => x.Code == settings.TargetLanguage);
        _provider.SelectedIndex = (int)settings.Provider; _captureMode.SelectedIndex = (int)settings.CaptureMode;
        _alignment.SelectedIndex = (int)settings.Alignment;
        _hotkey.Value = settings.Hotkey;
        _fallback.Checked = settings.UseFallback; _merge.Checked = settings.MergeLines;
        _autoSize.Checked = settings.AutoFontSize; _bold.Checked = settings.Bold; _closeToTray.Checked = settings.CloseToTray;
        _scale.Value = settings.OcrScale; _fontSize.Value = settings.FontSize;
        _minFont.Value = settings.MinimumFontSize; _maxFont.Value = settings.MaximumFontSize;
        _opacity.Value = settings.BackgroundOpacity; _padding.Value = settings.Padding; _seconds.Value = settings.OverlaySeconds;
        _ocrLanguage.SelectedItem = _ocrLanguage.Items.Cast<LanguageOption>().FirstOrDefault(x => x.Code == settings.OcrLanguage);
        if (_ocrLanguage.SelectedIndex < 0) _ocrLanguage.SelectedIndex = 0;
        _fontName = settings.FontFamily; _font.Text = _fontName + "…";
        _foreground = ColorTranslator.FromHtml(settings.TextColor); _background = ColorTranslator.FromHtml(settings.BackgroundColor);
        UpdateSizeControls(); UpdateTranslateCaption();
    }
    private AppSettings ReadSettings() => _settings with
    {
        TargetLanguage = ((LanguageOption)_target.SelectedItem!).Code,
        Provider = (TranslationProvider)_provider.SelectedIndex, CaptureMode = (CaptureMode)_captureMode.SelectedIndex,
        Hotkey = _hotkey.Value,
        UseFallback = _fallback.Checked, OcrLanguage = ((LanguageOption)_ocrLanguage.SelectedItem!).Code,
        OcrScale = (int)_scale.Value, MergeLines = _merge.Checked,
        FontFamily = _fontName, AutoFontSize = _autoSize.Checked, Bold = _bold.Checked,
        FontSize = (int)_fontSize.Value, MinimumFontSize = (int)_minFont.Value, MaximumFontSize = (int)_maxFont.Value,
        TextColor = $"#{_foreground.R:X2}{_foreground.G:X2}{_foreground.B:X2}",
        BackgroundColor = $"#{_background.R:X2}{_background.G:X2}{_background.B:X2}",
        BackgroundOpacity = (int)_opacity.Value, Padding = (int)_padding.Value,
        Alignment = (OverlayAlignment)_alignment.SelectedIndex, OverlaySeconds = (int)_seconds.Value,
        CloseToTray = _closeToTray.Checked
    };
    private void TrackChanges(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is ComboBox combo) combo.SelectedIndexChanged += (_, _) => MarkChanged();
            if (control is CheckBox check) check.CheckedChanged += (_, _) => MarkChanged();
            if (control is NumericUpDown number) number.ValueChanged += (_, _) => MarkChanged();
            TrackChanges(control);
        }
    }
    private void MarkChanged()
    {
        if (_loading) return;
        _dirty = true; _saveButton.Enabled = _operation is null;
        UpdateSizeControls(); _preview.Invalidate();
        SetStatus("Есть изменения. Нажмите «Сохранить», чтобы применить их.");
    }
    private void UpdateSizeControls()
    { _fontSize.Enabled = !_autoSize.Checked; _minFont.Enabled = _maxFont.Enabled = _autoSize.Checked; }
    private void UpdateTranslateCaption() => _translateButton.Text = "Выделить область";
    private void ChooseFont()
    {
        using var current = new Font(_fontName, (float)_fontSize.Value, _bold.Checked ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
        using var dialog = new FontDialog { Font = current, FontMustExist = true, ShowEffects = false, MinSize = 8, MaxSize = 64 };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        _fontName = dialog.Font.FontFamily.Name; _font.Text = _fontName + "…";
        _bold.Checked = dialog.Font.Bold;
        _fontSize.Value = Math.Clamp((decimal)(dialog.Font.SizeInPoints * 96 / 72), _fontSize.Minimum, _fontSize.Maximum);
        MarkChanged();
    }
    private void ChooseColor(bool foreground)
    {
        using var dialog = new ColorDialog { Color = foreground ? _foreground : _background, FullOpen = true };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        if (foreground) _foreground = dialog.Color; else _background = dialog.Color;
        MarkChanged();
    }
    private void PaintPreview(object? sender, PaintEventArgs e)
    {
        if (_target.SelectedItem is null) return;
        e.Graphics.Clear(SystemColors.Window);
        using var sampleFont = new Font("Segoe UI", 17, GraphicsUnit.Pixel);
        e.Graphics.DrawString("This text will be translated on your screen.", sampleFont, SystemBrushes.WindowText, 12, 18);
        var source = new TextBlock("This text will be translated on your screen.", new(12, 18, Math.Max(80, _preview.Width - 35), 25), 18);
        using var bitmap = OverlayRenderer.Render(new(Math.Max(1, _preview.Width - 2), _preview.Height - 2),
            [new(source, "Перевод появится поверх текста на экране.")], ReadSettings().Normalize()).Bitmap;
        e.Graphics.DrawImageUnscaled(bitmap, Point.Empty);
        e.Graphics.DrawString("Предпросмотр оверлея", Font, SystemBrushes.GrayText, 12, 80);
    }
}
