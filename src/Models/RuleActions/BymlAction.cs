namespace UkmmFlash.Models.RuleActions;

public class BymlAction : RuleAction
{
    public BymlAction() : base("Byml to Yaml") { }

    public override Task Compile(string path)
    {
        throw new NotImplementedException();
    }

    public override Task Decompile(string path)
    {
        throw new NotImplementedException();
    }
}
