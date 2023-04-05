using System.Text.Json;

namespace UkmmFlash;
public class AppConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ukmm-flash", "config.json");

    public string UkmmPath { get; set; } = string.Empty;
    public string ModsPath { get; set; } = "Mods";

    public static AppConfig Load()
    {
        if (!File.Exists(_path)) {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            using FileStream wfs = File.Create(_path);
            JsonSerializer.Serialize(wfs, new AppConfig());
        }

        using FileStream fs = File.OpenRead(_path);
        return JsonSerializer.Deserialize<AppConfig>(fs)!;
    }

    public async Task Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using FileStream wfs = File.Create(_path);
        await JsonSerializer.SerializeAsync(wfs, this);
    }
}
