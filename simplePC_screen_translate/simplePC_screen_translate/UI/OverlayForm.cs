using System.ComponentModel;
using simplePC_screen_translate.Native;

namespace simplePC_screen_translate.UI;

public sealed class OverlayForm : Form
{
    protected override bool ShowWithoutActivation => true;
    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            parameters.ExStyle |= 0x80000 | 0x20 | 0x80 | 0x8000000; // layered, click-through, tool window, no activation
            return parameters;
        }
    }
    public OverlayForm(Rectangle bounds)
    {
        AutoScaleMode = AutoScaleMode.None;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Bounds = bounds;
        TopMost = true;
    }
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        // Exclude the translated layer from subsequent screenshots on Windows 10 2004+.
        NativeMethods.SetWindowDisplayAffinity(Handle, 0x11);
    }
    public void SetBitmap(Bitmap bitmap)
    {
        var screenDc = NativeMethods.GetDC(0);
        var memoryDc = NativeMethods.CreateCompatibleDC(screenDc);
        var handle = bitmap.GetHbitmap(Color.FromArgb(0));
        var previous = NativeMethods.SelectObject(memoryDc, handle);
        try
        {
            var destination = new NativeMethods.NativePoint(Left, Top);
            var source = new NativeMethods.NativePoint(0, 0);
            var size = new NativeMethods.NativeSize(bitmap.Width, bitmap.Height);
            var blend = new NativeMethods.BlendFunction { ConstantAlpha = 255, AlphaFormat = 1 };
            if (!NativeMethods.UpdateLayeredWindow(Handle, screenDc, ref destination, ref size, memoryDc, ref source, 0, ref blend, 2))
                throw new Win32Exception();
        }
        finally
        {
            NativeMethods.SelectObject(memoryDc, previous);
            NativeMethods.DeleteObject(handle);
            NativeMethods.DeleteDC(memoryDc);
            NativeMethods.ReleaseDC(0, screenDc);
        }
    }
    protected override void WndProc(ref Message message)
    {
        if (message.Msg == NativeMethods.WmNcHitTest) { message.Result = new nint(-1); return; }
        if (message.Msg == NativeMethods.WmMouseActivate) { message.Result = new nint(3); return; }
        base.WndProc(ref message);
    }
}
