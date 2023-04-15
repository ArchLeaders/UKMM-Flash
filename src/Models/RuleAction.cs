using Cead;

namespace UkmmFlash.Models;
public abstract class RuleAction
{
    protected bool _isYaz0 = false;
    protected Endianness _endian = Endianness.Big;

    public string Name { get; }

    public RuleAction(string name)
    {
        Name = name;
    }

    public abstract void Compile(string path);
    public abstract void Decompile(string path);

    public override string ToString() => Name;
}
