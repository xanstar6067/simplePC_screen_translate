using System.ComponentModel;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.Native;

public sealed class HotkeyManager : NativeWindow, IDisposable
{
    private const int TranslateId = 1;
    private const int EscapeId = 2;
    private HotkeySettings? _registered;
    public event EventHandler? TranslatePressed;
    public event EventHandler? EscapePressed;

    public HotkeyManager() => CreateHandle(new CreateParams { Caption = "ScreenTranslator.Hotkeys", Parent = new nint(-3) });

    public void Register(HotkeySettings hotkey)
    {
        if (!hotkey.IsValid) throw new InvalidOperationException("Выберите букву, цифру, пробел или клавишу F1–F24.");
        if (_registered == hotkey) return;
        var previous = _registered;
        NativeMethods.UnregisterHotKey(Handle, TranslateId);
        if (!NativeMethods.RegisterHotKey(Handle, TranslateId, hotkey.Modifiers | 0x4000, (uint)hotkey.Key))
        {
            if (previous is not null) NativeMethods.RegisterHotKey(Handle, TranslateId, previous.Modifiers | 0x4000, (uint)previous.Key);
            throw new InvalidOperationException($"Сочетание {hotkey} уже занято или недоступно. Выберите другое.", new Win32Exception());
        }
        _registered = hotkey;
    }

    public bool EnableEscape() => NativeMethods.RegisterHotKey(Handle, EscapeId, 0x4000, (uint)Keys.Escape);
    public void DisableEscape() => NativeMethods.UnregisterHotKey(Handle, EscapeId);
    protected override void WndProc(ref Message message)
    {
        if (message.Msg == NativeMethods.WmHotkey)
        {
            if (message.WParam.ToInt32() == TranslateId) TranslatePressed?.Invoke(this, EventArgs.Empty);
            if (message.WParam.ToInt32() == EscapeId) EscapePressed?.Invoke(this, EventArgs.Empty);
        }
        base.WndProc(ref message);
    }
    public void Dispose()
    {
        NativeMethods.UnregisterHotKey(Handle, TranslateId);
        DisableEscape();
        DestroyHandle();
    }
}
