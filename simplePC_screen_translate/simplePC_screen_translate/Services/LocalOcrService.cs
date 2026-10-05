using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Security.Cryptography;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.Services;

public sealed class LocalOcrService
{
    public static IReadOnlyList<LanguageOption> GetLanguages() => OcrEngine.AvailableRecognizerLanguages
        .Select(x => new LanguageOption(x.LanguageTag, x.NativeName)).OrderBy(x => x.Name).ToArray();

    public async Task<IReadOnlyList<TextBlock>> RecognizeAsync(Bitmap screenshot, AppSettings settings,
        IProgress<string>? progress, CancellationToken ct)
    {
        var available = OcrEngine.AvailableRecognizerLanguages.ToArray();
        if (available.Length == 0)
            throw new InvalidOperationException("В Windows не установлен ни один язык OCR. Откройте параметры языка Windows и установите компонент «Оптическое распознавание символов» для нужного языка.");
        var languages = settings.OcrLanguage == "auto" ? available : available
            .Where(x => x.LanguageTag.Equals(settings.OcrLanguage, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (languages.Length == 0)
            throw new InvalidOperationException("Выбранный язык OCR больше не установлен. Выберите «Автоматически» в настройках распознавания.");
        var engines = languages.Select(x => OcrEngine.TryCreateFromLanguage(x)).Where(x => x is not null).ToArray();
        if (engines.Length == 0) throw new InvalidOperationException("Не удалось запустить локальное распознавание Windows.");

        // Ownership by word centre removes duplicates in overlapping tiles and preserves
        // small text on large monitors without shrinking the entire screenshot.
        const int overlap = 64;
        var coreSize = Math.Min(1200, (int)OcrEngine.MaxImageDimension / settings.OcrScale - 2 * overlap);
        var columns = (int)Math.Ceiling(screenshot.Width / (double)coreSize);
        var rows = (int)Math.Ceiling(screenshot.Height / (double)coreSize);
        var words = new List<RecognizedWord>();
        var tileIndex = 0;
        for (var top = 0; top < screenshot.Height; top += coreSize)
        for (var left = 0; left < screenshot.Width; left += coreSize)
        {
            ct.ThrowIfCancellationRequested();
            var core = Rectangle.Intersect(new(left, top, coreSize, coreSize), new(Point.Empty, screenshot.Size));
            var tile = Rectangle.Intersect(Rectangle.Inflate(core, overlap, overlap), new(Point.Empty, screenshot.Size));
            using var image = ResizeTile(screenshot, tile, settings.OcrScale);
            using var bitmap = ToSoftwareBitmap(image);
            OcrResult? best = null;
            var bestScore = double.MinValue;
            foreach (var engine in engines)
            {
                ct.ThrowIfCancellationRequested();
                progress?.Report($"Распознавание {tileIndex + 1}/{columns * rows} · {engine!.RecognizerLanguage.NativeName}");
                var result = await engine.RecognizeAsync(bitmap).AsTask(ct);
                var score = Score(result);
                if (score > bestScore) { best = result; bestScore = score; }
            }
            if (best is not null)
            foreach (var word in best.Lines.SelectMany(x => x.Words))
            {
                var bounds = new RectangleF(tile.Left + (float)word.BoundingRect.X / settings.OcrScale,
                    tile.Top + (float)word.BoundingRect.Y / settings.OcrScale,
                    (float)word.BoundingRect.Width / settings.OcrScale, (float)word.BoundingRect.Height / settings.OcrScale);
                if (core.Contains((int)(bounds.Left + bounds.Width / 2), (int)(bounds.Top + bounds.Height / 2)))
                    words.Add(new(word.Text, bounds));
            }
            tileIndex++;
        }
        var lines = BuildLines(words);
        return settings.MergeLines ? MergeParagraphs(lines) : lines;
    }

    private static double Score(OcrResult result)
    {
        var text = result.Text;
        var letters = text.Count(char.IsLetterOrDigit);
        var noise = text.Count(x => !char.IsLetterOrDigit(x) && !char.IsWhiteSpace(x) && !char.IsPunctuation(x));
        var single = result.Lines.SelectMany(x => x.Words).Count(x => x.Text.Length == 1 && char.IsLetter(x.Text[0]));
        return letters - noise * 3 - single * 0.25;
    }

    private static Bitmap ResizeTile(Bitmap source, Rectangle tile, int scale)
    {
        var image = new Bitmap(tile.Width * scale, tile.Height * scale, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(image);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(source, new Rectangle(Point.Empty, image.Size), tile, GraphicsUnit.Pixel);
        return image;
    }

    private static SoftwareBitmap ToSoftwareBitmap(Bitmap image)
    {
        var data = image.LockBits(new(Point.Empty, image.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            var bytes = new byte[image.Width * image.Height * 4];
            for (var y = 0; y < image.Height; y++)
                Marshal.Copy(data.Scan0 + y * data.Stride, bytes, y * image.Width * 4, image.Width * 4);
            return SoftwareBitmap.CreateCopyFromBuffer(CryptographicBuffer.CreateFromByteArray(bytes),
                BitmapPixelFormat.Bgra8, image.Width, image.Height, BitmapAlphaMode.Ignore);
        }
        finally { image.UnlockBits(data); }
    }

    private sealed record RecognizedWord(string Text, RectangleF Bounds);

    private static IReadOnlyList<TextBlock> BuildLines(List<RecognizedWord> words)
    {
        var rows = new List<List<RecognizedWord>>();
        foreach (var word in words.OrderBy(x => x.Bounds.Top).ThenBy(x => x.Bounds.Left))
        {
            var row = rows.LastOrDefault(r => Math.Abs(CenterY(r[0].Bounds) - CenterY(word.Bounds)) <
                Math.Max(3, Math.Min(r[0].Bounds.Height, word.Bounds.Height) * 0.45f));
            if (row is null) rows.Add([word]); else row.Add(word);
        }
        var lines = new List<TextBlock>();
        foreach (var row in rows)
        {
            var group = new List<RecognizedWord>();
            foreach (var word in row.OrderBy(x => x.Bounds.Left))
            {
                if (group.Count > 0 && word.Bounds.Left - group[^1].Bounds.Right > Math.Max(24, word.Bounds.Height * 2.8f))
                { AddLine(group, lines); group.Clear(); }
                group.Add(word);
            }
            AddLine(group, lines);
        }
        return lines.OrderBy(x => x.Bounds.Top).ThenBy(x => x.Bounds.Left).ToArray();
    }

    private static float CenterY(RectangleF r) => r.Top + r.Height / 2;

    private static void AddLine(List<RecognizedWord> words, List<TextBlock> lines)
    {
        if (words.Count == 0) return;
        var bounds = words.Select(x => x.Bounds).Aggregate(RectangleF.Union);
        var combined = string.Concat(words.Select(x => x.Text));
        var rtl = combined.Count(x => x is >= '\u0590' and <= '\u08ff') > combined.Count(char.IsLetter) / 2;
        var cjk = combined.Count(x => x is >= '\u3040' and <= '\u9fff' or >= '\uac00' and <= '\ud7af') > combined.Length / 2;
        var ordered = rtl ? words.OrderByDescending(x => x.Bounds.Left) : words.OrderBy(x => x.Bounds.Left);
        var text = string.Join(cjk ? "" : " ", ordered.Select(x => x.Text)).Trim();
        if (text.Length > 0 && text.Any(char.IsLetterOrDigit))
            lines.Add(new(text, bounds, words.Select(x => x.Bounds.Height).Order().ElementAt(words.Count / 2)));
    }

    public static IReadOnlyList<TextBlock> MergeParagraphs(IReadOnlyList<TextBlock> lines)
    {
        var paragraphs = new List<TextBlock>();
        foreach (var line in lines.OrderBy(x => x.Bounds.Top).ThenBy(x => x.Bounds.Left))
        {
            var index = paragraphs.FindLastIndex(previous =>
            {
                var gap = line.Bounds.Top - previous.Bounds.Bottom;
                var height = Math.Max(previous.LineHeight, line.LineHeight);
                var overlapWidth = Math.Min(previous.Bounds.Right, line.Bounds.Right) - Math.Max(previous.Bounds.Left, line.Bounds.Left);
                return gap >= -height * 0.15f && gap <= height * 0.8f &&
                    Math.Abs(previous.LineHeight - line.LineHeight) < height * 0.3f &&
                    Math.Abs(previous.Bounds.Left - line.Bounds.Left) < height * 1.5f &&
                    overlapWidth >= Math.Min(previous.Bounds.Width, line.Bounds.Width) * 0.6f &&
                    previous.Text.Length >= 15 && line.Text.Length >= 10;
            });
            if (index < 0) paragraphs.Add(line);
            else
            {
                var previous = paragraphs[index];
                var separator = previous.Text.EndsWith('-') ? "" : " ";
                paragraphs[index] = previous with { Text = previous.Text.TrimEnd('-') + separator + line.Text,
                    Bounds = RectangleF.Union(previous.Bounds, line.Bounds) };
            }
        }
        return paragraphs;
    }
}
