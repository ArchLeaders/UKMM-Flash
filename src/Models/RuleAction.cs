namespace UkmmFlash.Models;
public abstract class RuleAction
{
    public string Name { get; }

    public RuleAction(string name)
    {
        Name = name;
    }

    public abstract Task Compile(string path);
    public abstract Task Decompile(string path);

    public override string ToString() => Name;
}
