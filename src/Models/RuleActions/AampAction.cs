namespace UkmmFlash.Models.RuleActions;

internal class AampAction : RuleAction
{
    public AampAction() : base("Aamp to Yaml") { }

    public override Task Compile(string path)
    {
        throw new NotImplementedException();
    }

    public override Task Decompile(string path)
    {
        throw new NotImplementedException();
    }
}
