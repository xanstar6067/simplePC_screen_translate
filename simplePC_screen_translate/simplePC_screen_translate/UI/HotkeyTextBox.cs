using System.ComponentModel;
using simplePC_screen_translate.Models;
using simplePC_screen_translate.Native;

namespace simplePC_screen_translate.UI;

[DesignerCategory("Code")]
public class HotkeyTextBox : TextBox
{
    private HotkeySettings _value = new();
    private HotkeySettings _beforeFocus = new();
    private bool _windowsDown;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public HotkeySettings Value
    {
        get => _value;
        set
        {
            if (!value.IsValid) throw new ArgumentException("Недопустимая клавиша перевода.", nameof(value));
            var changed = _value != value;
            _value = value;
            Text = value.ToString();
            if (changed) ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? ValueChanged;
    public event EventHandler? CaptureStarted;
    public event EventHandler? CaptureFinished;

    public HotkeyTextBox()
    {
        ReadOnly = true;
        ShortcutsEnabled = false;
        BackColor = SystemColors.Window;
        Text = _value.ToString();
        AccessibleName = "Клавиша перевода";
        AccessibleDescription = "Нажмите клавишу или сочетание. Escape отменяет изменение.";
    }

    protected override void OnEnter(EventArgs e)
    {
        _beforeFocus = _value;
        _windowsDown = false;
        base.OnEnter(e);
        CaptureStarted?.Invoke(this, EventArgs.Empty);
        Text = "Нажмите нужную клавишу…";
    }

    protected override void OnLeave(EventArgs e)
    {
        _windowsDown = false;
        Text = _value.ToString();
        base.OnLeave(e);
        CaptureFinished?.Invoke(this, EventArgs.Empty);
    }

    protected override bool ProcessCmdKey(ref Message message, Keys keyData)
    {
        RecordKey(keyData);
        return true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        RecordKey(e.KeyData);
        e.Handled = true;
        e.SuppressKeyPress = true;
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.KeyCode is Keys.LWin or Keys.RWin) _windowsDown = false;
        e.Handled = true;
        e.SuppressKeyPress = true;
        base.OnKeyUp(e);
    }

    private void RecordKey(Keys keyData)
    {
        var key = keyData & Keys.KeyCode;
        if (key == Keys.Escape)
        {
            Value = _beforeFocus;
            Parent?.SelectNextControl(this, true, true, true, true);
            return;
        }
        if (key is Keys.LWin or Keys.RWin) _windowsDown = true;
        var candidate = new HotkeySettings
        {
            Key = key,
            Control = keyData.HasFlag(Keys.Control),
            Alt = keyData.HasFlag(Keys.Alt),
            Shift = keyData.HasFlag(Keys.Shift),
            Windows = _windowsDown || IsWindowsKeyDown()
        };
        if (candidate.IsValid) { Value = candidate; SelectionStart = TextLength; return; }
        if (key is Keys.ControlKey or Keys.LControlKey or Keys.RControlKey or Keys.Menu or Keys.LMenu or Keys.RMenu
            or Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey or Keys.LWin or Keys.RWin)
        {
            var modifiers = new List<string>();
            if (candidate.Control || key is Keys.ControlKey or Keys.LControlKey or Keys.RControlKey) modifiers.Add("Ctrl");
            if (candidate.Alt || key is Keys.Menu or Keys.LMenu or Keys.RMenu) modifiers.Add("Alt");
            if (candidate.Shift || key is Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey) modifiers.Add("Shift");
            if (candidate.Windows) modifiers.Add("Win");
            Text = string.Join(" + ", modifiers.Append("…"));
        }
        else Text = "Выберите другую клавишу";
    }

    protected virtual bool IsWindowsKeyDown() =>
        NativeMethods.GetAsyncKeyState((int)Keys.LWin) < 0 || NativeMethods.GetAsyncKeyState((int)Keys.RWin) < 0;
}
