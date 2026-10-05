using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.UI;

public sealed record OverlayRenderResult(Bitmap Bitmap, int TruncatedBlocks);

public static class OverlayRenderer
{
    public static OverlayRenderResult Render(Size canvas, IReadOnlyList<TranslatedBlock> blocks, AppSettings settings)
    {
        var bitmap = new Bitmap(canvas.Width, canvas.Height, PixelFormat.Format32bppPArgb);
        try
        {
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var background = new SolidBrush(Color.FromArgb(settings.BackgroundOpacity * 255 / 100,
                ColorTranslator.FromHtml(settings.BackgroundColor)));
            using var foreground = new SolidBrush(ColorTranslator.FromHtml(settings.TextColor));
            var truncated = 0;
            var occupied = new List<RectangleF>();
            foreach (var block in blocks.OrderBy(x => x.Source.Bounds.Top).ThenBy(x => x.Source.Bounds.Left))
            {
                var box = RectangleF.Intersect(RectangleF.Inflate(block.Source.Bounds, settings.Padding, settings.Padding),
                    new(PointF.Empty, canvas));
                // Allow translations to grow into free space, while stopping before neighbouring text.
                var right = Math.Min(canvas.Width, Math.Max(box.Right, box.Left + Math.Max(80, box.Width * 1.35f)));
                var bottom = Math.Min(canvas.Height, Math.Max(box.Bottom, box.Top + Math.Max(28, box.Height * 2.2f)));
                foreach (var other in blocks)
                {
                    if (ReferenceEquals(block, other)) continue;
                    var r = other.Source.Bounds;
                    if (r.Left >= block.Source.Bounds.Right && r.Top < box.Bottom && r.Bottom > box.Top)
                        right = Math.Min(right, Math.Max(block.Source.Bounds.Right, r.Left - settings.Padding));
                    if (r.Top >= block.Source.Bounds.Bottom && r.Left < right && r.Right > box.Left)
                        bottom = Math.Min(bottom, Math.Max(block.Source.Bounds.Bottom, r.Top - settings.Padding));
                }
                foreach (var r in occupied)
                {
                    if (r.Right > box.Left && r.Left < right && r.Bottom > box.Top && r.Top < box.Top)
                        box.Y = Math.Min(box.Bottom, r.Bottom);
                }
                box.Width = Math.Max(1, right - box.Left);
                box.Height = Math.Max(1, bottom - box.Top);
                var inner = RectangleF.Inflate(box, -settings.Padding, -settings.Padding);
                if (inner.Width <= 1 || inner.Height <= 1) { truncated++; continue; }
                using var format = new StringFormat(StringFormat.GenericTypographic)
                {
                    Alignment = settings.Alignment == OverlayAlignment.Center ? StringAlignment.Center : StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.LineLimit
                };
                if (IsRtl(block.Translation)) format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                var size = settings.AutoFontSize
                    ? Math.Clamp(block.Source.LineHeight * 1.05f, settings.MinimumFontSize, settings.MaximumFontSize)
                    : settings.FontSize;
                var minimum = settings.AutoFontSize ? settings.MinimumFontSize : size;
                using var font = FitFont(graphics, block.Translation, inner, settings, format, size, minimum, out var fits);
                if (!fits) truncated++;
                // Use only the height actually needed, so nearby rows retain their screen positions.
                var measured = graphics.MeasureString(block.Translation, font, new SizeF(inner.Width, 30000), format);
                box.Height = Math.Min(box.Height, Math.Max(block.Source.Bounds.Height + settings.Padding * 2,
                    measured.Height + settings.Padding * 2));
                inner = RectangleF.Inflate(box, -settings.Padding, -settings.Padding);
                graphics.FillRectangle(background, box);
                graphics.SetClip(inner);
                graphics.DrawString(block.Translation, font, foreground, inner, format);
                graphics.ResetClip();
                occupied.Add(box);
            }
            return new(bitmap, truncated);
        }
        catch { bitmap.Dispose(); throw; }
    }

    private static Font FitFont(Graphics g, string text, RectangleF bounds, AppSettings settings,
        StringFormat format, float initial, float minimum, out bool fits)
    {
        Font Create(float size)
        {
            try { return new Font(settings.FontFamily, size, settings.Bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel); }
            catch (ArgumentException) { return new Font(FontFamily.GenericSansSerif, size, settings.Bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel); }
        }
        var size = initial;
        while (true)
        {
            var font = Create(size);
            var measured = g.MeasureString(text, font, new SizeF(bounds.Width, 30000), format);
            fits = measured.Height <= bounds.Height + 0.5f && measured.Width <= bounds.Width + 1;
            if (fits || size <= minimum) return font;
            font.Dispose();
            size = Math.Max(minimum, size - 1);
        }
    }

    private static bool IsRtl(string text) => text.Count(x => x is >= '\u0590' and <= '\u08ff') > text.Count(char.IsLetter) / 2;
}
