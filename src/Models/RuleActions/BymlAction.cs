using Cead;
using Cead.Interop;

namespace UkmmFlash.Models.RuleActions;

public class BymlAction : RuleAction
{
    public BymlAction() : base("Byml to Yaml") { }

    public override void Compile(string path)
    {
        string text = File.ReadAllText(path + ".yml");
        File.Delete(path + ".yml");
        Byml byml = Byml.FromText(text);

        DataHandle handle = byml.ToBinary(_endian == Endianness.Big);
        if (_isYaz0) {
            handle = Yaz0.Compress(handle);
        }

        using FileStream stream = File.Create(path);
        stream.Write(handle);
    }

    public override void Decompile(string path)
    {
        Span<byte> data = File.ReadAllBytes(path);
        data = Yaz0.TryDecompress(data, out _isYaz0);

        Byml byml = Byml.FromBinary(data);
        _endian = data[0..2].SequenceEqual("BY"u8) ? Endianness.Big : Endianness.Little;

        using FileStream stream = File.Create(path + ".yml");
        stream.Write(byml.ToText().AsSpan());
    }
}
