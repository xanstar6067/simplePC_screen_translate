namespace simplePC_screen_translate.Models;

// Coordinates are physical pixels relative to the captured rectangle.
public sealed record TextBlock(string Text, RectangleF Bounds, float LineHeight);
public sealed record TranslatedBlock(TextBlock Source, string Translation);
public sealed record TranslationResult(IReadOnlyList<string> Texts, TranslationProvider Provider, bool UsedFallback);
