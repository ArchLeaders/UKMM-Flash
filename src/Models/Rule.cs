using Microsoft.Extensions.FileSystemGlobbing;
using UkmmFlash.Models.RuleActions;

namespace UkmmFlash.Models;

public class Rule : ReactiveObject
{
    public static Dictionary<string, RuleAction> Library => RuleAction.GetEnabled();

    private string _pattern;
    public string Pattern {
        get => _pattern;
        set => this.RaiseAndSetIfChanged(ref _pattern, value);
    }

    private KeyValuePair<string, RuleAction> _action;
    public KeyValuePair<string, RuleAction> Action {
        get => _action;
        set => this.RaiseAndSetIfChanged(ref _action, value);
    }

    public void Compile(string path)
    {
        foreach (var file in GetMatches(path)) {
            Action.Value.Compile(file);
        }
    }

    public void Decompile(string path)
    {
        foreach (var file in GetMatches(path)) {
            Action.Value.Decompile(file);
        }
    }

    public IEnumerable<string> GetMatches(string path)
    {
        Matcher matcher = new();
        matcher.AddInclude(Pattern);
        return matcher.GetResultsInFullPath(path);
    }

    public Rule(string pattern, KeyValuePair<string, RuleAction> action)
    {
        _pattern = pattern;
        _action = action;
    }

    public Rule(string pattern, string action)
    {
        _pattern = pattern;
        _action = new(action, Library[action]);
    }
}
