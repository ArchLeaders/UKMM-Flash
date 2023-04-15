using Cead;

namespace UkmmFlash.Models.RuleActions;

public class SarcAction : RuleAction
{
    public override void Compile(string path)
    {
        var dirPath = path + ".dir";

        Sarc sarc = new(_endian);
        foreach (var file in Directory.EnumerateFiles(dirPath, "*.*", SearchOption.AllDirectories)) {
            sarc.Add(Path.GetRelativePath(dirPath, file).Replace(Path.DirectorySeparatorChar, '/').Replace(".slash", "/"), File.ReadAllBytes(file));
        }

        Directory.Delete(dirPath, true);
        using FileStream fs = File.Create(path);
        fs.Write(sarc.ToBinary());
    }

    public override void Decompile(string path)
    {
        Span<byte> data = File.ReadAllBytes(path);
        data = Yaz0.TryDecompress(data, out _isYaz0);
        Sarc sarc = Sarc.FromBinary(data);
        _endian = sarc.Endian;

        path += ".dir";
        foreach ((string file, Sarc.SarcFile fileData) in sarc) {
            var filePath = Path.Combine(path, file);
            if (file.StartsWith('/')) {
                filePath = Path.Combine(path, "!slash", file.Remove(0, 1));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using FileStream fs = File.Create(filePath);
            fs.Write(fileData.AsSpan());
        }
    }
}
