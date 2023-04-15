using Cead;
using Cead.Interop;

namespace UkmmFlash.Models.RuleActions;

internal class AampAction : RuleAction
{
    public AampAction() : base("Aamp to Yaml") { }

    public override void Compile(string path)
    {
        string text = File.ReadAllText(path + ".yml");
        File.Delete(path + ".yml");
        ParameterIO pio = ParameterIO.FromText(text);

        DataHandle handle = pio.ToBinary();
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

        ParameterIO pio = ParameterIO.FromBinary(data);

        using FileStream stream = File.Create(path + ".yml");
        stream.Write(pio.ToText().AsSpan());
    }
}
