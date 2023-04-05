using Microsoft.Extensions.FileSystemGlobbing;

namespace UkmmFlash.Models;

public class Rule : ReactiveObject
{

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

    public async Task Compile(string path)
    {
        // for match in rule => Action.Compile(path);
        foreach (var file in GetMatches(path)) {
            await Action.Compile(file);
        }
    }

    public async Task Decompile(string path)
    {
        // for match in rule => Action.Decompile(path);
        foreach (var file in GetMatches(path)) {
            await Action.Decompile(file);
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

    public static Rule Create<T>(string pattern) where T : RuleAction, new()
    {
        return new(pattern, new T());
    }
}
