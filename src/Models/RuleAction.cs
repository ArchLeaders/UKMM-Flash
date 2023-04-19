using Cead;
using Microsoft.Extensions.FileSystemGlobbing;
using UkmmFlash.Models.RuleActions;

namespace UkmmFlash.Models;
public abstract class RuleAction
{
    protected Dictionary<string, bool> _isYaz0 = new();
    protected Endianness _endian = Endianness.Big;

    public virtual bool IsEnabled { get; } = true;
    public abstract string Description { get; }

    public virtual void Compile(string path) { }
    public virtual void Decompile(string path) { }
    public virtual void Deploy(string path) { }
    public virtual IEnumerable<string> GetMatches(string pattern, string path)
    {
        Matcher matcher = new();
        matcher.AddInclude(pattern);
        return matcher.GetResultsInFullPath(path);
    }

    public void Execute(Action<string> action, string pattern, string path)
    {
        foreach (var file in GetMatches(pattern, path)) {
            action(file);
        }
    }

    private static readonly Dictionary<string, RuleAction> _library = new() {
        { "Aamp to Yaml", new AampAction() },
        { "Byml to Yaml", new BymlAction() },
        { "Sarc to Folder", new SarcAction() },
        { "Copy Ice Spear Output", new IceSpearAction() },
    };

    public static Dictionary<string, RuleAction> GetEnabled()
    {
        Dictionary<string, RuleAction> enabled = new();
        foreach (var action in _library.Where(x => x.Value.IsEnabled)) {
            enabled[action.Key] = action.Value;
        }

        return enabled;
    }
}
