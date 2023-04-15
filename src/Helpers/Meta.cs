using System.Text.Json;
using System.Text.Json.Serialization;

namespace UkmmFlash.Helpers;

public enum Platform
{
    WiiU, Switch
}

public class Meta
{
    public static string SetNextVersion(string meta)
    {
        string[] lines = File.ReadAllLines(meta);

        int line = 1;
        string versionStr = lines.Where((x, i) => {
            line = i;
            return x.StartsWith("version: ");
        }).First();

        string[] vArgs = versionStr.Split(':')[1].Trim().Split('.');
        int vBuild = int.Parse(vArgs[^1]);

        vBuild++;
        vArgs[^1] = vBuild.ToString();
        versionStr = string.Join('.', vArgs);

        lines[line] = $"version: {versionStr}";
        File.WriteAllLines(meta, lines);

        return versionStr;
    }

    public static string GetMetaFile(string path, string name, Platform platform = Platform.WiiU)
    {
        string meta = Path.Combine(path, "meta.yml");
        if (!File.Exists(meta)) {
            File.WriteAllText(meta, $$"""
                name: "{{name}}"
                version: 0.1.0
                author: ""
                category: ""
                description: ""
                platform: !Specific {{(platform == Platform.WiiU ? "Wii U" : "Switch")}}
                url: null
                option_groups: []
                masters: {}
                """);
        }

        return meta;
    }

    public static string SetNextVersion((string path, Info obj) info)
    {
        string[] vArgs = info.obj.Version.Split('.');
        int vBuild = int.Parse(vArgs[^1]);

        vBuild++;
        vArgs[^1] = vBuild.ToString();
        info.obj.Version = string.Join('.', vArgs);

        File.WriteAllText(info.path, JsonSerializer.Serialize(info.obj, new JsonSerializerOptions() {
            WriteIndented = true
        }));

        return info.obj.Version;
    }

    public static (string path, Info obj) GetInfoFile(string path, string name, Platform platform = Platform.WiiU)
    {
        string info = Path.Combine(path, "build", "info.json");
        if (!File.Exists(info)) {
            File.WriteAllText(info, JsonSerializer.Serialize(new Info {
                Name = name,
                Platform = platform == Platform.WiiU ? "wiiu" : "nx"
            }, new JsonSerializerOptions() {
                WriteIndented = true
            }));
        }

        return (info, JsonSerializer.Deserialize<Info>(File.ReadAllText(info))!);
    }

    public class Info
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("desc")]
        public string Desc { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        [JsonPropertyName("options")]
        public Dictionary<string, object> Options { get; set; } = new();

        [JsonPropertyName("depends")]
        public List<object> Depends { get; set; } = new();

        [JsonPropertyName("showcompare")]
        public bool ShowCompare { get; set; } = false;

        [JsonPropertyName("showconvert")]
        public bool ShowConvert { get; set; } = false;

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = "wiiu";

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
