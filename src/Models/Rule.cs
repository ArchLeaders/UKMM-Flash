using Microsoft.Extensions.FileSystemGlobbing;
using UkmmFlash.Models.RuleActions;

namespace UkmmFlash.Models;

public class Rule : ReactiveObject
{
    public static RuleAction[] Library { get; } = {
        new AampAction(),
        new BymlAction(),
        new SarcAction(),
    };

    private string _pattern;
    public string Pattern {
        get => _pattern;
        set => this.RaiseAndSetIfChanged(ref _pattern, value);
    }

    private RuleAction _action;
    public RuleAction Action {
        get => _action;
        set => this.RaiseAndSetIfChanged(ref _action, value);
    }

    public void Compile(string path)
    {
        foreach (var file in GetMatches(path)) {
            Action.Compile(file);
        }
    }

    public void Decompile(string path)
    {
        foreach (var file in GetMatches(path)) {
            Action.Decompile(file);
        }
    }

    public IEnumerable<string> GetMatches(string path)
    {
        Matcher matcher = new();
        matcher.AddInclude(Pattern);
        return matcher.GetResultsInFullPath(path);
    }

    public Rule(string pattern, RuleAction action)
    {
        _pattern = pattern;
        _action = action;
    }
}
