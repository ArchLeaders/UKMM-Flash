using System.Collections.ObjectModel;
using System.Text.Json;
using UkmmFlash.Models;

namespace UkmmFlash.Helpers;

public static class RuleSerializer
{
    public static void Load(this ObservableCollection<Rule> rules, string path)
    {
        path = Path.Combine(path, "compiler-rules.json");

        rules.Clear();
        if (File.Exists(path)) {
            using FileStream fs = File.OpenRead(path);
            Dictionary<string, string> ruleset = JsonSerializer.Deserialize<Dictionary<string, string>>(fs)!;
            foreach ((var pattern, var rule) in ruleset) {
                rules.Add(new(pattern, rule));
            }
        }
    }

    public static async Task Save(this ObservableCollection<Rule> rules, string path)
    {
        path = Path.Combine(path, "compiler-rules.json");

        Dictionary<string, string> ruleset = new();
        foreach (var rule in rules) {
            ruleset.Add(rule.Pattern, rule.Action.Key);
        }

        using FileStream fs = File.Create(path);
        await JsonSerializer.SerializeAsync(fs, ruleset, new JsonSerializerOptions() {
            WriteIndented = true
        });
    }
}
