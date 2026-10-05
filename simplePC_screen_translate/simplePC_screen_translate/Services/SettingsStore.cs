using System.Text.Json;
using System.Text.Json.Serialization;
using simplePC_screen_translate.Models;

namespace simplePC_screen_translate.Services;

public sealed class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
    public string FilePath { get; }

    public SettingsStore(string? path = null) => FilePath = path ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScreenTranslator", "settings.json");

    public AppSettings Load(out string? warning)
    {
        warning = null;
        if (!File.Exists(FilePath)) return new();
        try { return Read(FilePath); }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            warning = "Не удалось прочитать настройки. Использованы настройки по умолчанию.";
            try
            {
                if (File.Exists(FilePath + ".bak"))
                {
                    var recovered = Read(FilePath + ".bak");
                    warning = "Настройки восстановлены из резервной копии.";
                    return recovered;
                }
            }
            catch (Exception backupEx) when (backupEx is JsonException or IOException or UnauthorizedAccessException) { }
            return new();
        }
    }

    private static AppSettings Read(string path) =>
        (JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(path), JsonOptions) ?? throw new JsonException()).Normalize();

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(FilePath))!);
        var temporary = FilePath + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(settings.Normalize(), JsonOptions));
            if (File.Exists(FilePath))
            {
                // Preserve the last valid version, even after loading a damaged primary file.
                try { _ = Read(FilePath); File.Copy(FilePath, FilePath + ".bak", true); }
                catch (JsonException) { File.Copy(FilePath, FilePath + ".damaged", true); }
            }
            File.Move(temporary, FilePath, true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
