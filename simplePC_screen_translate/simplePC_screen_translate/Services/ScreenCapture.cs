using System.Drawing.Imaging;

namespace simplePC_screen_translate.Services;

public static class ScreenCapture
{
    public static Bitmap Capture(Rectangle bounds)
    {
        if (bounds.Width < 8 || bounds.Height < 8)
            throw new InvalidOperationException("Выберите область размером хотя бы 8 × 8 пикселей.");
        var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
        try
        {
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
            return bitmap;
        }
        catch { bitmap.Dispose(); throw; }
    }
}
