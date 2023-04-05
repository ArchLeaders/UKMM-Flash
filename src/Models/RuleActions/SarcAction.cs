namespace UkmmFlash.Models.RuleActions;

public class SarcAction : RuleAction
{
    public SarcAction() : base("Sarc to Folder") { }

    public override Task Compile(string path)
    {
        throw new NotImplementedException();
    }

    public override Task Decompile(string path)
    {
        throw new NotImplementedException();
    }
}
