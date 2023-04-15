using System.Text.Json;

namespace UkmmFlash;

public class GameConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "botw", "config.json");

    // Note: the order of the properties matter (see GameFiles.cs)
    public string UpdatePath { get; set; } = string.Empty;
    public string DlcPath { get; set; } = string.Empty;
    public string GamePath { get; set; } = string.Empty;
    public string GamePathNx { get; set; } = string.Empty;
    public string DlcPathNx { get; set; } = string.Empty;

    public static GameConfig Load()
    {
        if (!File.Exists(_path)) {
            new GameConfig().Save();
        }

        using FileStream fs = File.OpenRead(_path);
        GameConfig config = JsonSerializer.Deserialize<GameConfig>(fs)!;
        return config;
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using FileStream wfs = File.Create(_path);
        JsonSerializer.Serialize(wfs, this);
    }
}
