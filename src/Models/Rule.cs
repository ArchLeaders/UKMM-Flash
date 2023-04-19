namespace UkmmFlash.Models;

public class Rule : ReactiveObject
{
    public static Dictionary<string, RuleAction> Library => RuleAction.GetEnabled();


    private bool _isEnabled;
    public bool IsEnabled {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

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
        Action.Value.Execute(Action.Value.Compile, Pattern, path);
    }

    public void Decompile(string path)
    {
        Action.Value.Execute(Action.Value.Decompile, Pattern, path);
    }

    public void Deploy(string path)
    {
        Action.Value.Execute(Action.Value.Deploy, Pattern, path);
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
