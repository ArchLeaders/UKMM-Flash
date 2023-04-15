using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace UkmmFlash.Helpers;

public static class GameFiles
{
    public static Dictionary<string, List<string>> Fetch(string pattern, bool isNx = false, string? outPath = null)
    {
        IEnumerable<string> paths = typeof(GameConfig).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => isNx ? x.Name.EndsWith("Nx") : !x.Name.EndsWith("Nx"))
            .Select(x => (string)x.GetValue(App.Config.Game)!);

        Dictionary<string, List<string>> matches = new();
        List<string> found = new();
        foreach (var path in paths) {
            Matcher matcher = new();
            matcher.AddInclude(pattern);

            foreach (var match in matcher.GetResultsInFullPath(path)) {
                if (!matches.ContainsKey(path)) {
                    matches[path] = new();
                }

                string relFilePath = Path.GetRelativePath(path, match);
                foreach (var prefix in new string[] { "0010", "0011", "0012" }) {
                    if (relFilePath.StartsWith(prefix)) {
                        string knownRel = Path.GetRelativePath(prefix, relFilePath);
                        if (!found.Contains(knownRel)) {
                            matches[path].Add(relFilePath);
                            found.Add(knownRel);
                        }

                        goto ContinueLoop;
                    }
                }

                if (!found.Contains(relFilePath) && !relFilePath.Contains("Map")) {
                    matches[path].Add(relFilePath);
                    found.Add(relFilePath);
                }

            ContinueLoop:
                continue;
            }
        }

        return matches;
    }

    public static async Task CopyInto(this Dictionary<string, List<string>> fetch, string path)
    {
        File.WriteAllText("D:\\Bin\\fetch.json", JsonSerializer.Serialize(fetch, new JsonSerializerOptions() {
            WriteIndented = true,
        }));

        foreach ((var root, var files) in fetch) {
            foreach (var _file in files) {
                string file = _file; 
                if (file.StartsWith("0010") || file.StartsWith("0011") || file.StartsWith("0012")) {
                    file = Path.Combine("aoc", _file);
                }

                Directory.CreateDirectory(Path.Combine(path, Path.GetDirectoryName(file)!));
                string outputFile = Path.Combine(path, file);

                if (File.Exists(outputFile)) {
                    ContentDialog dlg = new() {
                        Content = new ScrollViewer {
                            MaxHeight = 150,
                            Content = new TextBlock {
                                Text = $"The file '{file}' already exists, overwrite it?",
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap
                            }
                        },
                        DefaultButton = ContentDialogButton.Secondary,
                        PrimaryButtonText = "Yes",
                        SecondaryButtonText = "No",
                        Title = "File Already Exists"
                    };

                    if (await dlg.ShowAsync() != ContentDialogResult.Primary) {
                        continue;
                    }
                }

                File.Copy(Path.Combine(root, _file), outputFile, true);
            }
        }
    }
}
