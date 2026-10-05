using System.Diagnostics;
using simplePC_screen_translate.Models;
using simplePC_screen_translate.Services;
using simplePC_screen_translate.UI;

namespace simplePC_screen_translate;

public partial class Form1
{
    private readonly ComboBox _target = DropDown(), _provider = DropDown(), _captureMode = DropDown(),
        _ocrLanguage = DropDown(), _key = DropDown(), _alignment = DropDown();
    private readonly CheckBox _ctrl = Check("Ctrl"), _alt = Check("Alt"), _shift = Check("Shift"), _win = Check("Win");
    private readonly CheckBox _fallback = Check("Использовать другой сервис при ошибке"),
        _merge = Check("Объединять строки абзаца перед переводом"),
        _autoSize = Check("Подбирать размер по распознанной области"), _bold = Check("Полужирный"),
        _closeToTray = Check("При закрытии окна оставлять приложение в трее");
    private readonly NumericUpDown _scale = Number(1, 3), _fontSize = Number(8, 64), _minFont = Number(8, 32),
        _maxFont = Number(8, 64), _opacity = Number(10, 100), _padding = Number(0, 20), _seconds = Number(0, 300);
    private readonly Button _font = new() { AutoSize = true, Text = "Segoe UI" },
        _textColor = new() { AutoSize = true, Text = "Цвет текста…" }, _backgroundColor = new() { AutoSize = true, Text = "Цвет фона…" };
    private readonly Panel _preview = new() { Height = 110, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };
    private readonly Label _status = new() { AutoSize = true, Dock = DockStyle.Fill, MaximumSize = new(630, 0) };
    private readonly Button _translateButton = new() { Text = "Выделить область", AutoSize = true, Height = 36 },
        _screenButton = new() { Text = "Весь экран", AutoSize = true, Height = 36 },
        _saveButton = new() { Text = "Сохранить", AutoSize = true, Height = 36 },
        _hideButton = new() { Text = "Убрать перевод", AutoSize = true, Enabled = false, Height = 36 };
    private string _fontName = "Segoe UI";
    private Color _foreground = Color.White, _background = Color.FromArgb(24, 24, 24);

    private static ComboBox DropDown() => new() { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
    private static CheckBox Check(string text) => new() { Text = text, AutoSize = true, Margin = new(0, 5, 4, 5) };
    private static NumericUpDown Number(int min, int max) => new() { Minimum = min, Maximum = max, Width = 85 };
    private static Label Description(string text) => new() { Text = text, AutoSize = true, Dock = DockStyle.Fill, Margin = new(0, 7, 0, 13), MaximumSize = new(620, 0) };
    private static FlowLayoutPanel Flow(params Control[] controls)
    {
        var flow = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Margin = System.Windows.Forms.Padding.Empty, WrapContents = true };
        flow.Controls.AddRange(controls);
        return flow;
    }
    private static TableLayoutPanel Page(TabControl tabs, string title)
    {
        var tab = new TabPage(title) { Padding = new(15), AutoScroll = true, UseVisualStyleBackColor = true };
        tabs.TabPages.Add(tab);
        var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        table.ColumnStyles.Add(new(SizeType.Percent, 43));
        table.ColumnStyles.Add(new(SizeType.Percent, 57));
        tab.Controls.Add(table);
        return table;
    }
    private static void Row(TableLayoutPanel table, string caption, Control control)
    {
        var row = table.RowCount++;
        table.RowStyles.Add(new(SizeType.AutoSize));
        table.Controls.Add(new Label { Text = caption, AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Margin = new(0, 6, 12, 8) }, 0, row);
        control.Margin = new(0, 4, 0, 8);
        control.AccessibleName = caption;
        table.Controls.Add(control, 1, row);
    }
    private static void Wide(TableLayoutPanel table, Control control)
    {
        var row = table.RowCount++;
        table.RowStyles.Add(new(SizeType.AutoSize));
        table.Controls.Add(control, 0, row);
        table.SetColumnSpan(control, 2);
    }

    private void BuildInterface()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new(18), ColumnCount = 1, RowCount = 4 };
        root.ColumnStyles.Add(new(SizeType.Percent, 100));
        root.RowStyles.Add(new(SizeType.AutoSize)); root.RowStyles.Add(new(SizeType.Percent, 100));
        root.RowStyles.Add(new(SizeType.AutoSize)); root.RowStyles.Add(new(SizeType.AutoSize));
        Controls.Add(root);
        root.Controls.Add(Description("Нажмите горячую клавишу и выделите текст на экране. Перевод появится поверх оригинала; клики пройдут в открытое приложение."), 0, 0);
        var tabs = new TabControl { Dock = DockStyle.Fill };
        root.Controls.Add(tabs, 0, 1);
        var translation = Page(tabs, "Перевод");
        Row(translation, "Переводить на", _target); Row(translation, "Переводчик", _provider);
        Wide(translation, _fallback);
        Wide(translation, Description("Язык исходного текста определяется автоматически. Временные ключи Google и Яндекса приложение получает само."));
        Row(translation, "Что переводить", _captureMode);
        Row(translation, "Горячая клавиша", Flow(_ctrl, _alt, _shift, _win));
        Row(translation, "Основная клавиша", _key);
        Wide(translation, Description("Повторное нажатие горячей клавиши убирает перевод; во время обработки — отменяет её. Esc убирает оверлей. Настройки доступны через значок в трее."));
        Wide(translation, _closeToTray);
        Wide(translation, Description("Новые настройки применяются кнопкой «Сохранить». Перевод по кнопке также сохраняет настройки. Оверлей относится к текущему снимку; после прокрутки сделайте новый перевод."));

        var overlay = Page(tabs, "Оверлей");
        Row(overlay, "Шрифт", Flow(_font, _bold)); Wide(overlay, _autoSize);
        Row(overlay, "Размер вручную, пиксели", _fontSize);
        Row(overlay, "Автоматический размер, пиксели", Flow(_minFont, new Label { Text = "—", AutoSize = true, Margin = new(6, 6, 6, 0) }, _maxFont));
        Row(overlay, "Цвета", Flow(_textColor, _backgroundColor));
        Row(overlay, "Непрозрачность фона, %", _opacity); Row(overlay, "Отступ от текста, пиксели", _padding);
        Row(overlay, "Выравнивание", _alignment); Row(overlay, "Скрывать через, секунды", _seconds);
        Wide(overlay, Description("0 секунд — показывать до горячей клавиши или Esc. Полупрозрачным будет только фон; текст остаётся непрозрачным."));
        Wide(overlay, _preview);

        var recognition = Page(tabs, "Распознавание");
        Row(recognition, "Язык OCR", _ocrLanguage); Row(recognition, "Увеличение изображения, ×", _scale);
        Wide(recognition, _merge);
        Wide(recognition, Description("OCR работает на компьютере. В режиме «Автоматически» сравниваются результаты установленных языков Windows. Если текст распознаётся неверно, выберите его язык вручную. Увеличение ×2 помогает с мелким текстом."));
        var windowsSettings = new Button { Text = "Открыть языки Windows", AutoSize = true };
        windowsSettings.Click += (_, _) =>
        {
            try { Process.Start(new ProcessStartInfo("ms-settings:regionlanguage") { UseShellExecute = true }); }
            catch (Exception ex) { ShowError("Не удалось открыть параметры Windows: " + ex.Message); }
        };
        Wide(recognition, windowsSettings);
        Wide(recognition, Description("Для нужного языка в Windows должен быть установлен компонент OCR. Снимок экрана остаётся в памяти и не отправляется в интернет. Google или Яндекс получает только распознанный текст. Для перевода нужен интернет."));
        Wide(recognition, Description("Файл настроек: " + _store.FilePath));
        root.Controls.Add(_status, 0, 2); _status.Margin = new(0, 14, 0, 10);
        root.Controls.Add(Flow(_translateButton, _screenButton, _hideButton, _saveButton), 0, 3);
    }

    private void PopulateControls()
    {
        _target.Items.AddRange(Languages.All);
        _provider.Items.AddRange(["Google", "Яндекс"]);
        _captureMode.Items.AddRange(["Выделять область мышью", "Монитор под курсором", "Последняя выделенная область"]);
        _alignment.Items.AddRange(["По левому краю", "По центру"]);
        _key.Items.AddRange(Enumerable.Range((int)Keys.A, 26).Select(x => (object)(Keys)x).ToArray());
        _key.Items.AddRange(Enumerable.Range((int)Keys.F1, 24).Select(x => (object)(Keys)x).ToArray());
        _key.Items.AddRange(Enumerable.Range((int)Keys.D0, 10).Select(x => (object)(Keys)x).ToArray());
        _key.Items.AddRange([Keys.Space, Keys.Oemtilde, Keys.OemMinus, Keys.Oemplus]);
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
        _alignment.SelectedIndex = (int)settings.Alignment; _key.SelectedItem = settings.Hotkey.Key;
        _ctrl.Checked = settings.Hotkey.Control; _alt.Checked = settings.Hotkey.Alt;
        _shift.Checked = settings.Hotkey.Shift; _win.Checked = settings.Hotkey.Windows;
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
        Hotkey = new() { Key = (Keys)_key.SelectedItem!, Control = _ctrl.Checked, Alt = _alt.Checked, Shift = _shift.Checked, Windows = _win.Checked },
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
