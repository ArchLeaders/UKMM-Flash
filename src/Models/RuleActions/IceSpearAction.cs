using Microsoft.Extensions.FileSystemGlobbing;
using System.Text.Json;
using UkmmFlash.ViewModels;

using IceSpearConfig = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>>;

namespace UkmmFlash.Models.RuleActions;

public sealed class IceSpearAction : RuleAction
{
    public override string Description { get; } = """
        Copies the targeted map unit files from Ice-Spear into your mod folder before deploying.

        Note: The search pattern must include the Ice-Spear project name.
        """;

    private static readonly string? _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ice-spear", "config.json");
    private static readonly string? _path = Load()?["projects"]["path"].GetString();
    private static IceSpearConfig? Load()
    {
        if (File.Exists(_configPath)) {
            using FileStream fs = File.OpenRead(_configPath);
            return JsonSerializer.Deserialize<IceSpearConfig>(fs);
        }

        return null;
    }

    public override bool IsEnabled => File.Exists(_configPath);

    public override void Deploy(string path)
    {
        string outFolder = Path.GetRelativePath(_path!, Path.GetDirectoryName(path)!);
        outFolder = outFolder[outFolder.IndexOf(Path.DirectorySeparatorChar)..].Remove(0, 1);

        if (!outFolder.StartsWith("field")) {
            // Probably an unrelated file, we only
            // want the field files (hopefully)
            return;
        }

        outFolder = Path.GetRelativePath(Path.Combine("field", "data"), outFolder);
        outFolder = Path.Combine(ShellViewModel.Shared.ModPath, "build", "aoc", "0010", "Map", "MainField", outFolder);
        Directory.CreateDirectory(outFolder);

        string outPath = Path.Combine(outFolder, Path.GetFileName(path));
        File.Copy(path, outPath, true);
    }

    public override IEnumerable<string> GetMatches(string pattern, string _)
    {
        if (_path != null) {
            Matcher matcher = new();
            matcher.AddInclude(pattern);
            return matcher.GetResultsInFullPath(_path);
        }

        throw new InvalidDataException("Invalid path argument: The Ice Spear settings file might be corrupt");
    }
}
