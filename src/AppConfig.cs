using System.Text.Json;

namespace UkmmFlash;
public class AppConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ukmm-flash", "config.json");

    public string UkmmPath { get; set; } = string.Empty;
    public string ModsPath { get; set; } = "Mods";
    public GameConfig Game { get; set; } = new();

    public static AppConfig Load()
    {
        if (!File.Exists(_path)) {
            new AppConfig().Save();
        }

        using FileStream fs = File.OpenRead(_path);
        AppConfig config = JsonSerializer.Deserialize<AppConfig>(fs)!;
        config.Game = GameConfig.Load();
        return config;
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using FileStream wfs = File.Create(_path);
        JsonSerializer.Serialize(wfs, this);

        Game.Save();
    }
}
