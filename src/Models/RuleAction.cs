using Cead;

namespace UkmmFlash.Models;
public abstract class RuleAction
{
    protected Dictionary<string, bool> _isYaz0 = new();
    protected Endianness _endian = Endianness.Big;

    public abstract void Compile(string path);
    public abstract void Decompile(string path);
}
