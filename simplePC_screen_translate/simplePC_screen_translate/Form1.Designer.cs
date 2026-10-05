#nullable disable
namespace simplePC_screen_translate
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeResources();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _root = new TableLayoutPanel();
            _intro = new Label();
            _tabs = new TabControl();
            _translationPage = new TabPage();
            _translationTable = new TableLayoutPanel();
            _targetLabel = new Label();
            _target = new ComboBox();
            _providerLabel = new Label();
            _provider = new ComboBox();
            _fallback = new CheckBox();
            _languageHint = new Label();
            _captureModeLabel = new Label();
            _captureMode = new ComboBox();
            _hotkeyLabel = new Label();
            _hotkey = new simplePC_screen_translate.UI.HotkeyTextBox();
            _hotkeyHint = new Label();
            _usageHint = new Label();
            _closeToTray = new CheckBox();
            _saveHint = new Label();
            _overlayPage = new TabPage();
            _overlayTable = new TableLayoutPanel();
            _fontRowLabel = new Label();
            _fontRow = new FlowLayoutPanel();
            _font = new Button();
            _bold = new CheckBox();
            _autoSize = new CheckBox();
            _fontSizeLabel = new Label();
            _fontSize = new NumericUpDown();
            _rangeRowLabel = new Label();
            _rangeRow = new FlowLayoutPanel();
            _minFont = new NumericUpDown();
            _rangeDash = new Label();
            _maxFont = new NumericUpDown();
            _colorsRowLabel = new Label();
            _colorsRow = new FlowLayoutPanel();
            _textColor = new Button();
            _backgroundColor = new Button();
            _opacityLabel = new Label();
            _opacity = new NumericUpDown();
            _paddingLabel = new Label();
            _padding = new NumericUpDown();
            _alignmentLabel = new Label();
            _alignment = new ComboBox();
            _secondsLabel = new Label();
            _seconds = new NumericUpDown();
            _overlayHint = new Label();
            _preview = new Panel();
            _recognitionPage = new TabPage();
            _recognitionTable = new TableLayoutPanel();
            _ocrLanguageLabel = new Label();
            _ocrLanguage = new ComboBox();
            _scaleLabel = new Label();
            _scale = new NumericUpDown();
            _merge = new CheckBox();
            _ocrHint = new Label();
            _windowsLanguages = new Button();
            _ocrInstallHint = new Label();
            _privacyHint = new Label();
            _status = new Label();
            _actions = new FlowLayoutPanel();
            _translateButton = new Button();
            _screenButton = new Button();
            _hideButton = new Button();
            _saveButton = new Button();
            _root.SuspendLayout();
            _tabs.SuspendLayout();
            _translationPage.SuspendLayout();
            _translationTable.SuspendLayout();
            _overlayPage.SuspendLayout();
            _overlayTable.SuspendLayout();
            _fontRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_fontSize).BeginInit();
            _rangeRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_minFont).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_maxFont).BeginInit();
            _colorsRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_opacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_padding).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_seconds).BeginInit();
            _recognitionPage.SuspendLayout();
            _recognitionTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_scale).BeginInit();
            _actions.SuspendLayout();
            SuspendLayout();
            // 
            // _root
            // 
            _root.ColumnCount = 1;
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.Controls.Add(_intro, 0, 0);
            _root.Controls.Add(_tabs, 0, 1);
            _root.Controls.Add(_status, 0, 2);
            _root.Controls.Add(_actions, 0, 3);
            _root.Dock = DockStyle.Fill;
            _root.Location = new Point(0, 0);
            _root.Name = "_root";
            _root.Padding = new Padding(18);
            _root.RowCount = 4;
            _root.RowStyles.Add(new RowStyle());
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _root.RowStyles.Add(new RowStyle());
            _root.RowStyles.Add(new RowStyle());
            _root.Size = new Size(720, 710);
            _root.TabIndex = 0;
            // 
            // _intro
            // 
            _intro.AutoSize = true;
            _intro.Dock = DockStyle.Fill;
            _intro.Location = new Point(18, 25);
            _intro.Margin = new Padding(0, 7, 0, 12);
            _intro.MaximumSize = new Size(620, 0);
            _intro.Name = "_intro";
            _intro.Size = new Size(620, 15);
            _intro.TabIndex = 0;
            _intro.Text = "Выделите область или переведите весь экран. Перевод появится поверх текста.";
            // 
            // _tabs
            // 
            _tabs.Controls.Add(_translationPage);
            _tabs.Controls.Add(_overlayPage);
            _tabs.Controls.Add(_recognitionPage);
            _tabs.Dock = DockStyle.Fill;
            _tabs.Location = new Point(21, 55);
            _tabs.Name = "_tabs";
            _tabs.SelectedIndex = 0;
            _tabs.Size = new Size(678, 557);
            _tabs.TabIndex = 1;
            // 
            // _translationPage
            // 
            _translationPage.AutoScroll = true;
            _translationPage.Controls.Add(_translationTable);
            _translationPage.Location = new Point(4, 24);
            _translationPage.Name = "_translationPage";
            _translationPage.Padding = new Padding(15);
            _translationPage.Size = new Size(670, 529);
            _translationPage.TabIndex = 0;
            _translationPage.Text = "Перевод";
            _translationPage.UseVisualStyleBackColor = true;
            // 
            // _translationTable
            // 
            _translationTable.AutoSize = true;
            _translationTable.ColumnCount = 2;
            _translationTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            _translationTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            _translationTable.Controls.Add(_targetLabel, 0, 0);
            _translationTable.Controls.Add(_target, 1, 0);
            _translationTable.Controls.Add(_providerLabel, 0, 1);
            _translationTable.Controls.Add(_provider, 1, 1);
            _translationTable.Controls.Add(_fallback, 0, 2);
            _translationTable.Controls.Add(_languageHint, 0, 3);
            _translationTable.Controls.Add(_captureModeLabel, 0, 4);
            _translationTable.Controls.Add(_captureMode, 1, 4);
            _translationTable.Controls.Add(_hotkeyLabel, 0, 5);
            _translationTable.Controls.Add(_hotkey, 1, 5);
            _translationTable.Controls.Add(_hotkeyHint, 0, 6);
            _translationTable.Controls.Add(_usageHint, 0, 7);
            _translationTable.Controls.Add(_closeToTray, 0, 8);
            _translationTable.Controls.Add(_saveHint, 0, 9);
            _translationTable.Dock = DockStyle.Top;
            _translationTable.Location = new Point(15, 15);
            _translationTable.Name = "_translationTable";
            _translationTable.RowCount = 10;
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.RowStyles.Add(new RowStyle());
            _translationTable.Size = new Size(640, 341);
            _translationTable.TabIndex = 0;
            // 
            // _targetLabel
            // 
            _targetLabel.AutoSize = true;
            _targetLabel.Dock = DockStyle.Fill;
            _targetLabel.Location = new Point(0, 6);
            _targetLabel.Margin = new Padding(0, 6, 12, 8);
            _targetLabel.Name = "_targetLabel";
            _targetLabel.Size = new Size(263, 21);
            _targetLabel.TabIndex = 0;
            _targetLabel.Text = "Переводить на";
            _targetLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _target
            // 
            _target.AccessibleName = "Переводить на";
            _target.Dock = DockStyle.Fill;
            _target.DropDownStyle = ComboBoxStyle.DropDownList;
            _target.Items.AddRange(new object[] { "Русский" });
            _target.Location = new Point(275, 4);
            _target.Margin = new Padding(0, 4, 0, 8);
            _target.Name = "_target";
            _target.Size = new Size(365, 23);
            _target.TabIndex = 1;
            // 
            // _providerLabel
            // 
            _providerLabel.AutoSize = true;
            _providerLabel.Dock = DockStyle.Fill;
            _providerLabel.Location = new Point(0, 41);
            _providerLabel.Margin = new Padding(0, 6, 12, 8);
            _providerLabel.Name = "_providerLabel";
            _providerLabel.Size = new Size(263, 21);
            _providerLabel.TabIndex = 2;
            _providerLabel.Text = "Переводчик";
            _providerLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _provider
            // 
            _provider.AccessibleName = "Переводчик";
            _provider.Dock = DockStyle.Fill;
            _provider.DropDownStyle = ComboBoxStyle.DropDownList;
            _provider.Items.AddRange(new object[] { "Google", "Яндекс" });
            _provider.Location = new Point(275, 39);
            _provider.Margin = new Padding(0, 4, 0, 8);
            _provider.Name = "_provider";
            _provider.Size = new Size(365, 23);
            _provider.TabIndex = 3;
            // 
            // _fallback
            // 
            _fallback.AutoSize = true;
            _translationTable.SetColumnSpan(_fallback, 2);
            _fallback.Location = new Point(3, 73);
            _fallback.Name = "_fallback";
            _fallback.Size = new Size(309, 19);
            _fallback.TabIndex = 4;
            _fallback.Text = "Переключаться на другой переводчик при ошибке";
            // 
            // _languageHint
            // 
            _languageHint.AutoSize = true;
            _translationTable.SetColumnSpan(_languageHint, 2);
            _languageHint.Dock = DockStyle.Fill;
            _languageHint.Location = new Point(0, 102);
            _languageHint.Margin = new Padding(0, 7, 0, 12);
            _languageHint.MaximumSize = new Size(620, 0);
            _languageHint.Name = "_languageHint";
            _languageHint.Size = new Size(620, 15);
            _languageHint.TabIndex = 5;
            _languageHint.Text = "Язык исходного текста определяется автоматически.";
            // 
            // _captureModeLabel
            // 
            _captureModeLabel.AutoSize = true;
            _captureModeLabel.Dock = DockStyle.Fill;
            _captureModeLabel.Location = new Point(0, 135);
            _captureModeLabel.Margin = new Padding(0, 6, 12, 8);
            _captureModeLabel.Name = "_captureModeLabel";
            _captureModeLabel.Size = new Size(263, 21);
            _captureModeLabel.TabIndex = 6;
            _captureModeLabel.Text = "По горячей клавише";
            _captureModeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _captureMode
            // 
            _captureMode.AccessibleName = "По горячей клавише";
            _captureMode.Dock = DockStyle.Fill;
            _captureMode.DropDownStyle = ComboBoxStyle.DropDownList;
            _captureMode.Items.AddRange(new object[] { "Выбрать область", "Весь экран", "Последняя область" });
            _captureMode.Location = new Point(275, 133);
            _captureMode.Margin = new Padding(0, 4, 0, 8);
            _captureMode.Name = "_captureMode";
            _captureMode.Size = new Size(365, 23);
            _captureMode.TabIndex = 7;
            // 
            // _hotkeyLabel
            // 
            _hotkeyLabel.AutoSize = true;
            _hotkeyLabel.Dock = DockStyle.Fill;
            _hotkeyLabel.Location = new Point(0, 170);
            _hotkeyLabel.Margin = new Padding(0, 6, 12, 8);
            _hotkeyLabel.Name = "_hotkeyLabel";
            _hotkeyLabel.Size = new Size(263, 21);
            _hotkeyLabel.TabIndex = 8;
            _hotkeyLabel.Text = "Клавиша перевода";
            _hotkeyLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _hotkey
            // 
            _hotkey.AccessibleDescription = "Нажмите клавишу или сочетание. Escape отменяет изменение.";
            _hotkey.AccessibleName = "Клавиша перевода";
            _hotkey.BackColor = SystemColors.Window;
            _hotkey.Dock = DockStyle.Fill;
            _hotkey.Location = new Point(275, 168);
            _hotkey.Margin = new Padding(0, 4, 0, 8);
            _hotkey.Name = "_hotkey";
            _hotkey.ReadOnly = true;
            _hotkey.ShortcutsEnabled = false;
            _hotkey.Size = new Size(365, 23);
            _hotkey.TabIndex = 9;
            _hotkey.Text = "Ctrl + Alt + T";
            // 
            // _hotkeyHint
            // 
            _hotkeyHint.AutoSize = true;
            _translationTable.SetColumnSpan(_hotkeyHint, 2);
            _hotkeyHint.Dock = DockStyle.Fill;
            _hotkeyHint.Location = new Point(0, 206);
            _hotkeyHint.Margin = new Padding(0, 7, 0, 12);
            _hotkeyHint.MaximumSize = new Size(620, 0);
            _hotkeyHint.Name = "_hotkeyHint";
            _hotkeyHint.Size = new Size(620, 15);
            _hotkeyHint.TabIndex = 10;
            _hotkeyHint.Text = "Нажмите на поле и задайте клавишу или сочетание. Esc отменяет изменение.";
            // 
            // _usageHint
            // 
            _usageHint.AutoSize = true;
            _translationTable.SetColumnSpan(_usageHint, 2);
            _usageHint.Dock = DockStyle.Fill;
            _usageHint.Location = new Point(0, 240);
            _usageHint.Margin = new Padding(0, 7, 0, 12);
            _usageHint.MaximumSize = new Size(620, 0);
            _usageHint.Name = "_usageHint";
            _usageHint.Size = new Size(620, 30);
            _usageHint.TabIndex = 11;
            _usageHint.Text = "Повторное нажатие убирает перевод; во время обработки — отменяет её. Настройки доступны через значок в трее.";
            // 
            // _closeToTray
            // 
            _closeToTray.AutoSize = true;
            _translationTable.SetColumnSpan(_closeToTray, 2);
            _closeToTray.Location = new Point(3, 285);
            _closeToTray.Name = "_closeToTray";
            _closeToTray.Size = new Size(299, 19);
            _closeToTray.TabIndex = 12;
            _closeToTray.Text = "Оставлять приложение в трее при закрытии окна";
            // 
            // _saveHint
            // 
            _saveHint.AutoSize = true;
            _translationTable.SetColumnSpan(_saveHint, 2);
            _saveHint.Dock = DockStyle.Fill;
            _saveHint.Location = new Point(0, 314);
            _saveHint.Margin = new Padding(0, 7, 0, 12);
            _saveHint.MaximumSize = new Size(620, 0);
            _saveHint.Name = "_saveHint";
            _saveHint.Size = new Size(620, 15);
            _saveHint.TabIndex = 13;
            _saveHint.Text = "Нажмите «Сохранить», чтобы применить изменения. После прокрутки экрана запустите перевод снова.";
            // 
            // _overlayPage
            // 
            _overlayPage.AutoScroll = true;
            _overlayPage.Controls.Add(_overlayTable);
            _overlayPage.Location = new Point(4, 24);
            _overlayPage.Name = "_overlayPage";
            _overlayPage.Padding = new Padding(15);
            _overlayPage.Size = new Size(150, 0);
            _overlayPage.TabIndex = 1;
            _overlayPage.Text = "Оверлей";
            _overlayPage.UseVisualStyleBackColor = true;
            // 
            // _overlayTable
            // 
            _overlayTable.AutoSize = true;
            _overlayTable.ColumnCount = 2;
            _overlayTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            _overlayTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            _overlayTable.Controls.Add(_fontRowLabel, 0, 0);
            _overlayTable.Controls.Add(_fontRow, 1, 0);
            _overlayTable.Controls.Add(_autoSize, 0, 1);
            _overlayTable.Controls.Add(_fontSizeLabel, 0, 2);
            _overlayTable.Controls.Add(_fontSize, 1, 2);
            _overlayTable.Controls.Add(_rangeRowLabel, 0, 3);
            _overlayTable.Controls.Add(_rangeRow, 1, 3);
            _overlayTable.Controls.Add(_colorsRowLabel, 0, 4);
            _overlayTable.Controls.Add(_colorsRow, 1, 4);
            _overlayTable.Controls.Add(_opacityLabel, 0, 5);
            _overlayTable.Controls.Add(_opacity, 1, 5);
            _overlayTable.Controls.Add(_paddingLabel, 0, 6);
            _overlayTable.Controls.Add(_padding, 1, 6);
            _overlayTable.Controls.Add(_alignmentLabel, 0, 7);
            _overlayTable.Controls.Add(_alignment, 1, 7);
            _overlayTable.Controls.Add(_secondsLabel, 0, 8);
            _overlayTable.Controls.Add(_seconds, 1, 8);
            _overlayTable.Controls.Add(_overlayHint, 0, 9);
            _overlayTable.Controls.Add(_preview, 0, 10);
            _overlayTable.Dock = DockStyle.Top;
            _overlayTable.Location = new Point(15, 15);
            _overlayTable.Name = "_overlayTable";
            _overlayTable.RowCount = 11;
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.RowStyles.Add(new RowStyle());
            _overlayTable.Size = new Size(120, 932);
            _overlayTable.TabIndex = 0;
            // 
            // _fontRowLabel
            // 
            _fontRowLabel.AutoSize = true;
            _fontRowLabel.Dock = DockStyle.Fill;
            _fontRowLabel.Location = new Point(0, 6);
            _fontRowLabel.Margin = new Padding(0, 6, 12, 8);
            _fontRowLabel.Name = "_fontRowLabel";
            _fontRowLabel.Size = new Size(39, 61);
            _fontRowLabel.TabIndex = 0;
            _fontRowLabel.Text = "Шрифт";
            _fontRowLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _fontRow
            // 
            _fontRow.AccessibleName = "Шрифт";
            _fontRow.AutoSize = true;
            _fontRow.Controls.Add(_font);
            _fontRow.Controls.Add(_bold);
            _fontRow.Dock = DockStyle.Fill;
            _fontRow.Location = new Point(51, 4);
            _fontRow.Margin = new Padding(0, 4, 0, 8);
            _fontRow.Name = "_fontRow";
            _fontRow.Size = new Size(69, 63);
            _fontRow.TabIndex = 1;
            // 
            // _font
            // 
            _font.AutoSize = true;
            _font.Location = new Point(3, 3);
            _font.Name = "_font";
            _font.Size = new Size(95, 32);
            _font.TabIndex = 0;
            _font.Text = "Segoe UI…";
            _font.UseVisualStyleBackColor = true;
            // 
            // _bold
            // 
            _bold.AutoSize = true;
            _bold.Location = new Point(3, 41);
            _bold.Name = "_bold";
            _bold.Size = new Size(101, 19);
            _bold.TabIndex = 1;
            _bold.Text = "Полужирный";
            // 
            // _autoSize
            // 
            _autoSize.AutoSize = true;
            _overlayTable.SetColumnSpan(_autoSize, 2);
            _autoSize.Location = new Point(3, 78);
            _autoSize.Name = "_autoSize";
            _autoSize.Size = new Size(114, 19);
            _autoSize.TabIndex = 2;
            _autoSize.Text = "Подбирать размер по области текста";
            // 
            // _fontSizeLabel
            // 
            _fontSizeLabel.AutoSize = true;
            _fontSizeLabel.Dock = DockStyle.Fill;
            _fontSizeLabel.Location = new Point(0, 106);
            _fontSizeLabel.Margin = new Padding(0, 6, 12, 8);
            _fontSizeLabel.Name = "_fontSizeLabel";
            _fontSizeLabel.Size = new Size(39, 90);
            _fontSizeLabel.TabIndex = 3;
            _fontSizeLabel.Text = "Размер вручную, пиксели";
            _fontSizeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _fontSize
            // 
            _fontSize.AccessibleName = "Размер вручную, пиксели";
            _fontSize.Location = new Point(51, 104);
            _fontSize.Margin = new Padding(0, 4, 0, 8);
            _fontSize.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            _fontSize.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            _fontSize.Name = "_fontSize";
            _fontSize.Size = new Size(69, 23);
            _fontSize.TabIndex = 4;
            _fontSize.Value = new decimal(new int[] { 18, 0, 0, 0 });
            // 
            // _rangeRowLabel
            // 
            _rangeRowLabel.AutoSize = true;
            _rangeRowLabel.Dock = DockStyle.Fill;
            _rangeRowLabel.Location = new Point(0, 210);
            _rangeRowLabel.Margin = new Padding(0, 6, 12, 8);
            _rangeRowLabel.Name = "_rangeRowLabel";
            _rangeRowLabel.Size = new Size(39, 90);
            _rangeRowLabel.TabIndex = 5;
            _rangeRowLabel.Text = "Диапазон размера, пиксели";
            _rangeRowLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _rangeRow
            // 
            _rangeRow.AccessibleName = "Диапазон размера, пиксели";
            _rangeRow.AutoSize = true;
            _rangeRow.Controls.Add(_minFont);
            _rangeRow.Controls.Add(_rangeDash);
            _rangeRow.Controls.Add(_maxFont);
            _rangeRow.Dock = DockStyle.Fill;
            _rangeRow.Location = new Point(51, 208);
            _rangeRow.Margin = new Padding(0, 4, 0, 8);
            _rangeRow.Name = "_rangeRow";
            _rangeRow.Size = new Size(69, 92);
            _rangeRow.TabIndex = 6;
            // 
            // _minFont
            // 
            _minFont.Location = new Point(3, 3);
            _minFont.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            _minFont.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            _minFont.Name = "_minFont";
            _minFont.Size = new Size(85, 23);
            _minFont.TabIndex = 0;
            _minFont.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _rangeDash
            // 
            _rangeDash.AutoSize = true;
            _rangeDash.Location = new Point(6, 35);
            _rangeDash.Margin = new Padding(6);
            _rangeDash.Name = "_rangeDash";
            _rangeDash.Size = new Size(19, 15);
            _rangeDash.TabIndex = 1;
            _rangeDash.Text = "—";
            // 
            // _maxFont
            // 
            _maxFont.Location = new Point(3, 59);
            _maxFont.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            _maxFont.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            _maxFont.Name = "_maxFont";
            _maxFont.Size = new Size(85, 23);
            _maxFont.TabIndex = 2;
            _maxFont.Value = new decimal(new int[] { 28, 0, 0, 0 });
            // 
            // _colorsRowLabel
            // 
            _colorsRowLabel.AutoSize = true;
            _colorsRowLabel.Dock = DockStyle.Fill;
            _colorsRowLabel.Location = new Point(0, 314);
            _colorsRowLabel.Margin = new Padding(0, 6, 12, 8);
            _colorsRowLabel.Name = "_colorsRowLabel";
            _colorsRowLabel.Size = new Size(39, 74);
            _colorsRowLabel.TabIndex = 7;
            _colorsRowLabel.Text = "Цвета";
            _colorsRowLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _colorsRow
            // 
            _colorsRow.AccessibleName = "Цвета";
            _colorsRow.AutoSize = true;
            _colorsRow.Controls.Add(_textColor);
            _colorsRow.Controls.Add(_backgroundColor);
            _colorsRow.Dock = DockStyle.Fill;
            _colorsRow.Location = new Point(51, 312);
            _colorsRow.Margin = new Padding(0, 4, 0, 8);
            _colorsRow.Name = "_colorsRow";
            _colorsRow.Size = new Size(69, 76);
            _colorsRow.TabIndex = 8;
            // 
            // _textColor
            // 
            _textColor.AutoSize = true;
            _textColor.Location = new Point(3, 3);
            _textColor.Name = "_textColor";
            _textColor.Size = new Size(95, 32);
            _textColor.TabIndex = 0;
            _textColor.Text = "Цвет текста…";
            _textColor.UseVisualStyleBackColor = true;
            // 
            // _backgroundColor
            // 
            _backgroundColor.AutoSize = true;
            _backgroundColor.Location = new Point(3, 41);
            _backgroundColor.Name = "_backgroundColor";
            _backgroundColor.Size = new Size(95, 32);
            _backgroundColor.TabIndex = 1;
            _backgroundColor.Text = "Цвет фона…";
            _backgroundColor.UseVisualStyleBackColor = true;
            // 
            // _opacityLabel
            // 
            _opacityLabel.AutoSize = true;
            _opacityLabel.Dock = DockStyle.Fill;
            _opacityLabel.Location = new Point(0, 402);
            _opacityLabel.Margin = new Padding(0, 6, 12, 8);
            _opacityLabel.Name = "_opacityLabel";
            _opacityLabel.Size = new Size(39, 75);
            _opacityLabel.TabIndex = 9;
            _opacityLabel.Text = "Непрозрачность фона, %";
            _opacityLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _opacity
            // 
            _opacity.AccessibleName = "Непрозрачность фона, %";
            _opacity.Location = new Point(51, 400);
            _opacity.Margin = new Padding(0, 4, 0, 8);
            _opacity.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            _opacity.Name = "_opacity";
            _opacity.Size = new Size(69, 23);
            _opacity.TabIndex = 10;
            _opacity.Value = new decimal(new int[] { 85, 0, 0, 0 });
            // 
            // _paddingLabel
            // 
            _paddingLabel.AutoSize = true;
            _paddingLabel.Dock = DockStyle.Fill;
            _paddingLabel.Location = new Point(0, 491);
            _paddingLabel.Margin = new Padding(0, 6, 12, 8);
            _paddingLabel.Name = "_paddingLabel";
            _paddingLabel.Size = new Size(39, 60);
            _paddingLabel.TabIndex = 11;
            _paddingLabel.Text = "Отступы, пиксели";
            _paddingLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _padding
            // 
            _padding.AccessibleName = "Отступы, пиксели";
            _padding.Location = new Point(51, 489);
            _padding.Margin = new Padding(0, 4, 0, 8);
            _padding.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            _padding.Name = "_padding";
            _padding.Size = new Size(69, 23);
            _padding.TabIndex = 12;
            _padding.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // _alignmentLabel
            // 
            _alignmentLabel.AutoSize = true;
            _alignmentLabel.Dock = DockStyle.Fill;
            _alignmentLabel.Location = new Point(0, 565);
            _alignmentLabel.Margin = new Padding(0, 6, 12, 8);
            _alignmentLabel.Name = "_alignmentLabel";
            _alignmentLabel.Size = new Size(39, 45);
            _alignmentLabel.TabIndex = 13;
            _alignmentLabel.Text = "Выравнивание";
            _alignmentLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _alignment
            // 
            _alignment.AccessibleName = "Выравнивание";
            _alignment.Dock = DockStyle.Fill;
            _alignment.DropDownStyle = ComboBoxStyle.DropDownList;
            _alignment.Items.AddRange(new object[] { "По левому краю", "По центру" });
            _alignment.Location = new Point(51, 563);
            _alignment.Margin = new Padding(0, 4, 0, 8);
            _alignment.Name = "_alignment";
            _alignment.Size = new Size(69, 23);
            _alignment.TabIndex = 14;
            // 
            // _secondsLabel
            // 
            _secondsLabel.AutoSize = true;
            _secondsLabel.Dock = DockStyle.Fill;
            _secondsLabel.Location = new Point(0, 624);
            _secondsLabel.Margin = new Padding(0, 6, 12, 8);
            _secondsLabel.Name = "_secondsLabel";
            _secondsLabel.Size = new Size(39, 90);
            _secondsLabel.TabIndex = 15;
            _secondsLabel.Text = "Скрывать через, секунды";
            _secondsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _seconds
            // 
            _seconds.AccessibleName = "Скрывать через, секунды";
            _seconds.Location = new Point(51, 622);
            _seconds.Margin = new Padding(0, 4, 0, 8);
            _seconds.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            _seconds.Name = "_seconds";
            _seconds.Size = new Size(69, 23);
            _seconds.TabIndex = 16;
            // 
            // _overlayHint
            // 
            _overlayHint.AutoSize = true;
            _overlayTable.SetColumnSpan(_overlayHint, 2);
            _overlayHint.Dock = DockStyle.Fill;
            _overlayHint.Location = new Point(0, 729);
            _overlayHint.Margin = new Padding(0, 7, 0, 12);
            _overlayHint.MaximumSize = new Size(620, 0);
            _overlayHint.Name = "_overlayHint";
            _overlayHint.Size = new Size(120, 75);
            _overlayHint.TabIndex = 17;
            _overlayHint.Text = "0 — показывать до горячей клавиши или Esc. Прозрачность меняет только фон.";
            // 
            // _preview
            // 
            _preview.BorderStyle = BorderStyle.FixedSingle;
            _overlayTable.SetColumnSpan(_preview, 2);
            _preview.Dock = DockStyle.Top;
            _preview.Location = new Point(3, 819);
            _preview.Name = "_preview";
            _preview.Size = new Size(114, 110);
            _preview.TabIndex = 18;
            // 
            // _recognitionPage
            // 
            _recognitionPage.AutoScroll = true;
            _recognitionPage.Controls.Add(_recognitionTable);
            _recognitionPage.Location = new Point(4, 24);
            _recognitionPage.Name = "_recognitionPage";
            _recognitionPage.Padding = new Padding(15);
            _recognitionPage.Size = new Size(150, 0);
            _recognitionPage.TabIndex = 2;
            _recognitionPage.Text = "Распознавание";
            _recognitionPage.UseVisualStyleBackColor = true;
            // 
            // _recognitionTable
            // 
            _recognitionTable.AutoSize = true;
            _recognitionTable.ColumnCount = 2;
            _recognitionTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            _recognitionTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            _recognitionTable.Controls.Add(_ocrLanguageLabel, 0, 0);
            _recognitionTable.Controls.Add(_ocrLanguage, 1, 0);
            _recognitionTable.Controls.Add(_scaleLabel, 0, 1);
            _recognitionTable.Controls.Add(_scale, 1, 1);
            _recognitionTable.Controls.Add(_merge, 0, 2);
            _recognitionTable.Controls.Add(_ocrHint, 0, 3);
            _recognitionTable.Controls.Add(_windowsLanguages, 0, 4);
            _recognitionTable.Controls.Add(_ocrInstallHint, 0, 5);
            _recognitionTable.Controls.Add(_privacyHint, 0, 6);
            _recognitionTable.Dock = DockStyle.Top;
            _recognitionTable.Location = new Point(15, 15);
            _recognitionTable.Name = "_recognitionTable";
            _recognitionTable.RowCount = 7;
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.RowStyles.Add(new RowStyle());
            _recognitionTable.Size = new Size(120, 673);
            _recognitionTable.TabIndex = 0;
            // 
            // _ocrLanguageLabel
            // 
            _ocrLanguageLabel.AutoSize = true;
            _ocrLanguageLabel.Dock = DockStyle.Fill;
            _ocrLanguageLabel.Location = new Point(0, 6);
            _ocrLanguageLabel.Margin = new Padding(0, 6, 12, 8);
            _ocrLanguageLabel.Name = "_ocrLanguageLabel";
            _ocrLanguageLabel.Size = new Size(39, 60);
            _ocrLanguageLabel.TabIndex = 0;
            _ocrLanguageLabel.Text = "Язык распознавания";
            _ocrLanguageLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _ocrLanguage
            // 
            _ocrLanguage.AccessibleName = "Язык распознавания";
            _ocrLanguage.Dock = DockStyle.Fill;
            _ocrLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            _ocrLanguage.Items.AddRange(new object[] { "Автоматически" });
            _ocrLanguage.Location = new Point(51, 4);
            _ocrLanguage.Margin = new Padding(0, 4, 0, 8);
            _ocrLanguage.Name = "_ocrLanguage";
            _ocrLanguage.Size = new Size(69, 23);
            _ocrLanguage.TabIndex = 1;
            // 
            // _scaleLabel
            // 
            _scaleLabel.AutoSize = true;
            _scaleLabel.Dock = DockStyle.Fill;
            _scaleLabel.Location = new Point(0, 80);
            _scaleLabel.Margin = new Padding(0, 6, 12, 8);
            _scaleLabel.Name = "_scaleLabel";
            _scaleLabel.Size = new Size(39, 105);
            _scaleLabel.TabIndex = 2;
            _scaleLabel.Text = "Увеличение мелкого текста, ×";
            _scaleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _scale
            // 
            _scale.AccessibleName = "Увеличение мелкого текста, ×";
            _scale.Location = new Point(51, 78);
            _scale.Margin = new Padding(0, 4, 0, 8);
            _scale.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            _scale.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _scale.Name = "_scale";
            _scale.Size = new Size(69, 23);
            _scale.TabIndex = 3;
            _scale.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _merge
            // 
            _merge.AutoSize = true;
            _recognitionTable.SetColumnSpan(_merge, 2);
            _merge.Location = new Point(3, 196);
            _merge.Name = "_merge";
            _merge.Size = new Size(114, 19);
            _merge.TabIndex = 4;
            _merge.Text = "Объединять строки одного абзаца";
            // 
            // _ocrHint
            // 
            _ocrHint.AutoSize = true;
            _recognitionTable.SetColumnSpan(_ocrHint, 2);
            _ocrHint.Dock = DockStyle.Fill;
            _ocrHint.Location = new Point(0, 225);
            _ocrHint.Margin = new Padding(0, 7, 0, 12);
            _ocrHint.MaximumSize = new Size(620, 0);
            _ocrHint.Name = "_ocrHint";
            _ocrHint.Size = new Size(120, 135);
            _ocrHint.TabIndex = 5;
            _ocrHint.Text = "Распознавание работает на компьютере. Если текст читается неверно, выберите его язык вручную. Увеличение ×2 помогает с мелким текстом.";
            // 
            // _windowsLanguages
            // 
            _windowsLanguages.AutoSize = true;
            _recognitionTable.SetColumnSpan(_windowsLanguages, 2);
            _windowsLanguages.Location = new Point(3, 375);
            _windowsLanguages.Name = "_windowsLanguages";
            _windowsLanguages.Size = new Size(114, 32);
            _windowsLanguages.TabIndex = 6;
            _windowsLanguages.Text = "Открыть языки Windows";
            _windowsLanguages.UseVisualStyleBackColor = true;
            // 
            // _ocrInstallHint
            // 
            _ocrInstallHint.AutoSize = true;
            _recognitionTable.SetColumnSpan(_ocrInstallHint, 2);
            _ocrInstallHint.Dock = DockStyle.Fill;
            _ocrInstallHint.Location = new Point(0, 417);
            _ocrInstallHint.Margin = new Padding(0, 7, 0, 12);
            _ocrInstallHint.MaximumSize = new Size(620, 0);
            _ocrInstallHint.Name = "_ocrInstallHint";
            _ocrInstallHint.Size = new Size(120, 90);
            _ocrInstallHint.TabIndex = 7;
            _ocrInstallHint.Text = "Для нового языка установите распознавание текста в языковых параметрах Windows.";
            // 
            // _privacyHint
            // 
            _privacyHint.AutoSize = true;
            _recognitionTable.SetColumnSpan(_privacyHint, 2);
            _privacyHint.Dock = DockStyle.Fill;
            _privacyHint.Location = new Point(0, 526);
            _privacyHint.Margin = new Padding(0, 7, 0, 12);
            _privacyHint.MaximumSize = new Size(620, 0);
            _privacyHint.Name = "_privacyHint";
            _privacyHint.Size = new Size(120, 135);
            _privacyHint.TabIndex = 8;
            _privacyHint.Text = "Снимок экрана остаётся на компьютере. Переводчику отправляется только распознанный текст. Для перевода нужен интернет.";
            // 
            // _status
            // 
            _status.AutoSize = true;
            _status.Dock = DockStyle.Fill;
            _status.Location = new Point(18, 629);
            _status.Margin = new Padding(0, 14, 0, 10);
            _status.MaximumSize = new Size(620, 0);
            _status.Name = "_status";
            _status.Size = new Size(620, 15);
            _status.TabIndex = 2;
            _status.Text = "Готово. Ctrl + Alt + T — перевод.";
            // 
            // _actions
            // 
            _actions.AutoSize = true;
            _actions.Controls.Add(_translateButton);
            _actions.Controls.Add(_screenButton);
            _actions.Controls.Add(_hideButton);
            _actions.Controls.Add(_saveButton);
            _actions.Dock = DockStyle.Fill;
            _actions.Location = new Point(18, 654);
            _actions.Margin = new Padding(0);
            _actions.Name = "_actions";
            _actions.Size = new Size(684, 38);
            _actions.TabIndex = 3;
            // 
            // _translateButton
            // 
            _translateButton.AutoSize = true;
            _translateButton.Location = new Point(3, 3);
            _translateButton.Name = "_translateButton";
            _translateButton.Size = new Size(117, 32);
            _translateButton.TabIndex = 0;
            _translateButton.Text = "Выделить область";
            _translateButton.UseVisualStyleBackColor = true;
            // 
            // _screenButton
            // 
            _screenButton.AutoSize = true;
            _screenButton.Location = new Point(126, 3);
            _screenButton.Name = "_screenButton";
            _screenButton.Size = new Size(95, 32);
            _screenButton.TabIndex = 1;
            _screenButton.Text = "Весь экран";
            _screenButton.UseVisualStyleBackColor = true;
            // 
            // _hideButton
            // 
            _hideButton.AutoSize = true;
            _hideButton.Enabled = false;
            _hideButton.Location = new Point(227, 3);
            _hideButton.Name = "_hideButton";
            _hideButton.Size = new Size(103, 32);
            _hideButton.TabIndex = 2;
            _hideButton.Text = "Убрать перевод";
            _hideButton.UseVisualStyleBackColor = true;
            // 
            // _saveButton
            // 
            _saveButton.AutoSize = true;
            _saveButton.Enabled = false;
            _saveButton.Location = new Point(336, 3);
            _saveButton.Name = "_saveButton";
            _saveButton.Size = new Size(95, 32);
            _saveButton.TabIndex = 3;
            _saveButton.Text = "Сохранить";
            _saveButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 710);
            Controls.Add(_root);
            MinimumSize = new Size(660, 600);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Экранный переводчик";
            _root.ResumeLayout(false);
            _root.PerformLayout();
            _tabs.ResumeLayout(false);
            _translationPage.ResumeLayout(false);
            _translationPage.PerformLayout();
            _translationTable.ResumeLayout(false);
            _translationTable.PerformLayout();
            _overlayPage.ResumeLayout(false);
            _overlayPage.PerformLayout();
            _overlayTable.ResumeLayout(false);
            _overlayTable.PerformLayout();
            _fontRow.ResumeLayout(false);
            _fontRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_fontSize).EndInit();
            _rangeRow.ResumeLayout(false);
            _rangeRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_minFont).EndInit();
            ((System.ComponentModel.ISupportInitialize)_maxFont).EndInit();
            _colorsRow.ResumeLayout(false);
            _colorsRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_opacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)_padding).EndInit();
            ((System.ComponentModel.ISupportInitialize)_seconds).EndInit();
            _recognitionPage.ResumeLayout(false);
            _recognitionPage.PerformLayout();
            _recognitionTable.ResumeLayout(false);
            _recognitionTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_scale).EndInit();
            _actions.ResumeLayout(false);
            _actions.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _root;
        private Label _intro;
        private TabControl _tabs;
        private TabPage _translationPage;
        private TableLayoutPanel _translationTable;
        private TabPage _overlayPage;
        private TableLayoutPanel _overlayTable;
        private TabPage _recognitionPage;
        private TableLayoutPanel _recognitionTable;
        private ComboBox _target;
        private Label _targetLabel;
        private ComboBox _provider;
        private Label _providerLabel;
        private CheckBox _fallback;
        private Label _languageHint;
        private ComboBox _captureMode;
        private Label _captureModeLabel;
        private simplePC_screen_translate.UI.HotkeyTextBox _hotkey;
        private Label _hotkeyLabel;
        private Label _hotkeyHint;
        private Label _usageHint;
        private CheckBox _closeToTray;
        private Label _saveHint;
        private Button _font;
        private CheckBox _bold;
        private FlowLayoutPanel _fontRow;
        private Label _fontRowLabel;
        private CheckBox _autoSize;
        private NumericUpDown _fontSize;
        private Label _fontSizeLabel;
        private NumericUpDown _minFont;
        private Label _rangeDash;
        private NumericUpDown _maxFont;
        private FlowLayoutPanel _rangeRow;
        private Label _rangeRowLabel;
        private Button _textColor;
        private Button _backgroundColor;
        private FlowLayoutPanel _colorsRow;
        private Label _colorsRowLabel;
        private NumericUpDown _opacity;
        private Label _opacityLabel;
        private NumericUpDown _padding;
        private Label _paddingLabel;
        private ComboBox _alignment;
        private Label _alignmentLabel;
        private NumericUpDown _seconds;
        private Label _secondsLabel;
        private Label _overlayHint;
        private Panel _preview;
        private ComboBox _ocrLanguage;
        private Label _ocrLanguageLabel;
        private NumericUpDown _scale;
        private Label _scaleLabel;
        private CheckBox _merge;
        private Label _ocrHint;
        private Button _windowsLanguages;
        private Label _ocrInstallHint;
        private Label _privacyHint;
        private Label _status;
        private Button _translateButton;
        private Button _screenButton;
        private Button _hideButton;
        private Button _saveButton;
        private FlowLayoutPanel _actions;
    }
}
