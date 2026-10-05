using System.Text.Json.Serialization;

namespace simplePC_screen_translate.Models;

public enum TranslationProvider { Google, Yandex }
public enum CaptureMode { SelectRegion, Monitor, LastRegion }
public enum OverlayAlignment { Left, Center }

public sealed record AppSettings
{
    public string TargetLanguage { get; init; } = "ru";
    public TranslationProvider Provider { get; init; } = TranslationProvider.Google;
    public bool UseFallback { get; init; } = true;
    public CaptureMode CaptureMode { get; init; } = CaptureMode.SelectRegion;
    public HotkeySettings Hotkey { get; init; } = new();
    public string OcrLanguage { get; init; } = "auto";
    public int OcrScale { get; init; } = 2;
    public bool MergeLines { get; init; } = true;
    public string FontFamily { get; init; } = "Segoe UI";
    public bool AutoFontSize { get; init; } = true;
    public int FontSize { get; init; } = 18;
    public int MinimumFontSize { get; init; } = 10;
    public int MaximumFontSize { get; init; } = 28;
    public bool Bold { get; init; }
    public string TextColor { get; init; } = "#FFFFFF";
    public string BackgroundColor { get; init; } = "#181818";
    public int BackgroundOpacity { get; init; } = 85;
    public int Padding { get; init; } = 4;
    public OverlayAlignment Alignment { get; init; }
    public int OverlaySeconds { get; init; }
    public bool CloseToTray { get; init; } = true;

    public AppSettings Normalize() => this with
    {
        TargetLanguage = Languages.All.Any(x => x.Code == TargetLanguage) ? TargetLanguage : "ru",
        Provider = Enum.IsDefined(Provider) ? Provider : TranslationProvider.Google,
        CaptureMode = Enum.IsDefined(CaptureMode) ? CaptureMode : CaptureMode.SelectRegion,
        Alignment = Enum.IsDefined(Alignment) ? Alignment : OverlayAlignment.Left,
        Hotkey = Hotkey is not null && Hotkey.IsValid ? Hotkey : new(),
        OcrLanguage = string.IsNullOrWhiteSpace(OcrLanguage) ? "auto" : OcrLanguage,
        OcrScale = Math.Clamp(OcrScale, 1, 3),
        FontFamily = string.IsNullOrWhiteSpace(FontFamily) ? "Segoe UI" : FontFamily,
        FontSize = Math.Clamp(FontSize, 8, 64),
        MinimumFontSize = Math.Clamp(MinimumFontSize, 8, 32),
        MaximumFontSize = Math.Clamp(MaximumFontSize, Math.Clamp(MinimumFontSize, 8, 32), 64),
        TextColor = IsColor(TextColor) ? TextColor : "#FFFFFF",
        BackgroundColor = IsColor(BackgroundColor) ? BackgroundColor : "#181818",
        BackgroundOpacity = Math.Clamp(BackgroundOpacity, 10, 100),
        Padding = Math.Clamp(Padding, 0, 20),
        OverlaySeconds = Math.Clamp(OverlaySeconds, 0, 300)
    };

    private static bool IsColor(string? color) => color is { Length: 7 } && color[0] == '#' &&
        int.TryParse(color.AsSpan(1), System.Globalization.NumberStyles.HexNumber, null, out _);
}

public sealed record HotkeySettings
{
    public Keys Key { get; init; } = Keys.T;
    public bool Control { get; init; } = true;
    public bool Alt { get; init; } = true;
    public bool Shift { get; init; }
    public bool Windows { get; init; }

    [JsonIgnore]
    public uint Modifiers => (Alt ? 1u : 0) | (Control ? 2u : 0) | (Shift ? 4u : 0) | (Windows ? 8u : 0);
    [JsonIgnore]
    public bool IsValid => Key is >= Keys.A and <= Keys.Z or >= Keys.D0 and <= Keys.D9 or
        >= Keys.F1 and <= Keys.F24 or >= Keys.NumPad0 and <= Keys.NumPad9 or
        Keys.Space or Keys.Tab or Keys.Enter or Keys.Back or Keys.Delete or Keys.Insert or
        Keys.Home or Keys.End or Keys.PageUp or Keys.PageDown or Keys.Left or Keys.Right or Keys.Up or Keys.Down or
        Keys.PrintScreen or Keys.Multiply or Keys.Add or Keys.Subtract or Keys.Decimal or Keys.Divide or
        Keys.Oemtilde or Keys.OemMinus or Keys.Oemplus or Keys.Oemcomma or Keys.OemPeriod or
        Keys.Oem1 or Keys.Oem2 or Keys.Oem4 or Keys.Oem5 or Keys.Oem6 or Keys.Oem7 or Keys.Oem102;
    public override string ToString() => string.Join(" + ", new[]
    {
        Control ? "Ctrl" : null, Alt ? "Alt" : null, Shift ? "Shift" : null, Windows ? "Win" : null,
        DisplayKey(Key)
    }.Where(x => x is not null));

    private static string DisplayKey(Keys key) => key switch
    {
        >= Keys.D0 and <= Keys.D9 => ((int)key - (int)Keys.D0).ToString(),
        >= Keys.NumPad0 and <= Keys.NumPad9 => "Num " + ((int)key - (int)Keys.NumPad0),
        Keys.Space => "Пробел", Keys.Back => "Backspace", Keys.PrintScreen => "Print Screen",
        Keys.Left => "←", Keys.Right => "→", Keys.Up => "↑", Keys.Down => "↓",
        Keys.Oemtilde => "~", Keys.OemMinus or Keys.Subtract => "−", Keys.Oemplus or Keys.Add => "+",
        Keys.Oemcomma => ",", Keys.OemPeriod or Keys.Decimal => ".", Keys.Oem1 => ";",
        Keys.Oem2 or Keys.Divide => "/", Keys.Oem4 => "[", Keys.Oem5 or Keys.Oem102 => "\\",
        Keys.Oem6 => "]", Keys.Oem7 => "'", Keys.Multiply => "*",
        _ => key.ToString()
    };
}

public sealed record LanguageOption(string Code, string Name)
{
    public override string ToString() => Name;
}

public static class Languages
{
    public static readonly LanguageOption[] All =
    [
        new("ru", "Русский"), new("en", "Английский"), new("be", "Белорусский"),
        new("uk", "Украинский"), new("de", "Немецкий"), new("fr", "Французский"),
        new("es", "Испанский"), new("it", "Итальянский"), new("pt", "Португальский"),
        new("pl", "Польский"), new("cs", "Чешский"), new("sk", "Словацкий"),
        new("nl", "Нидерландский"), new("sv", "Шведский"), new("da", "Датский"),
        new("fi", "Финский"), new("no", "Норвежский"), new("hu", "Венгерский"),
        new("ro", "Румынский"), new("bg", "Болгарский"), new("el", "Греческий"),
        new("tr", "Турецкий"), new("ar", "Арабский"), new("he", "Иврит"),
        new("fa", "Персидский"), new("hi", "Хинди"), new("ja", "Японский"),
        new("ko", "Корейский"), new("zh-CN", "Китайский (упрощённый)"),
        new("zh-TW", "Китайский (традиционный)"), new("vi", "Вьетнамский"),
        new("th", "Тайский"), new("id", "Индонезийский"), new("kk", "Казахский")
    ];
}
