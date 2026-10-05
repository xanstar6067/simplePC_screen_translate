using System.Runtime.InteropServices;

namespace simplePC_screen_translate.Native;

internal static class NativeMethods
{
    internal const int WmHotkey = 0x0312;
    internal const int WmNcHitTest = 0x0084;
    internal const int WmMouseActivate = 0x0021;
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(nint hWnd, int id, uint modifiers, uint key);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(nint hWnd, int id);
    [DllImport("user32.dll")]
    internal static extern nint GetDC(nint hWnd);
    [DllImport("user32.dll")]
    internal static extern int ReleaseDC(nint hWnd, nint hDc);
    [DllImport("gdi32.dll")]
    internal static extern nint CreateCompatibleDC(nint hDc);
    [DllImport("gdi32.dll")]
    internal static extern nint SelectObject(nint hDc, nint hObject);
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(nint hObject);
    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(nint hDc);
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UpdateLayeredWindow(nint window, nint destinationDc, ref NativePoint destination,
        ref NativeSize size, nint sourceDc, ref NativePoint source, uint colorKey, ref BlendFunction blend, uint flags);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowDisplayAffinity(nint window, uint affinity);
    [DllImport("dwmapi.dll")]
    internal static extern int DwmFlush();

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePoint(int x, int y) { public int X = x; public int Y = y; }
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeSize(int width, int height) { public int Width = width; public int Height = height; }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BlendFunction { public byte Operation, Flags, ConstantAlpha, AlphaFormat; }
}
