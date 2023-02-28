using System.Diagnostics;
using System.Text;
using Mos6502Testing;
using NesConverter;

if (args.Length != 2 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
{
    Console.WriteLine("Usage: ./nesconverter <path> <outputPath>");
    Environment.Exit(-1);
}

var path = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);

if (!File.Exists(Path.GetFullPath("runtime/CMakeLists.txt")))
{
    Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
}

if (File.Exists(outputPath))
    throw new Exception("File " + outputPath + " exists. Remove it before continuing.");

var temp = Path.GetTempFileName();
File.Delete(temp);
Directory.CreateDirectory(temp);

if (!File.Exists(path))
{
    Console.WriteLine("File doesn't exist");
    Environment.Exit(-1);
}

var bytes = File.ReadAllBytes(path);

var prgRomSize = bytes[4] * 0x4000;
var chrRomSize = bytes[5] * 0x2000;

var header = bytes.AsSpan(0, 16);

var offset = 16;
var romData = bytes.AsSpan(offset, prgRomSize);
offset += prgRomSize;
var ramData = bytes.AsSpan(offset, chrRomSize);

MosConverter c = new MosConverter();
byte[] mem = new byte[0x8000 + romData.Length *(romData.Length  == 16384 ? 2 : 1)];
romData.CopyTo(mem.AsSpan(0x8000));
if (romData.Length == 16384)
    romData.CopyTo(mem.AsSpan(0x8000 + 0x4000)); // mirror
var result = c.Convert(mem, 0x8000, new MosConverterOptions()
{
    DebugMode = false,
    CycleMethod = true,
    UseArrayAsMem = false,
    OptimizeFlags = true,
    StartStackPointer = 0xfd,
    UseNativeFlags = true,
    UseNativeCallAsJsr = false
});

var character = new StringBuilder();
character.AppendLine("global _getCharacterRam");
character.AppendLine("global _getHeader");
character.AppendLine("section .text");
character.AppendLine("_getHeader:");
character.AppendLine("  LEA RAX, [rel _header]");
character.AppendLine("  ret");

character.AppendLine("_getCharacterRam:");
character.AppendLine("  LEA RAX, [rel _characterRam]");
character.AppendLine("  ret");

character.AppendLine("section .data");
character.AppendLine("_header:");
character.AppendLine("  db " + string.Join(", ", header.ToArray().Select(x => $"0x{x:x2}")));
character.AppendLine("_characterRam:");
character.Append("  db " + string.Join(", ", ramData.ToArray().Select(x => $"0x{x:x2}")));
if (ramData.IsEmpty)
    character.AppendLine(" 0");
else
    character.AppendLine("");

File.WriteAllText(Path.Join(temp, "mos6502.asm"), result);
File.WriteAllText(Path.Join(temp, "mos6502.chr.asm"), character.ToString());
CMake.Configure("runtime/", temp, ("TITLE", Path.GetFileNameWithoutExtension(path)), ("CMAKE_BUILD_TYPE", "Release"));
CMake.Build(temp);
File.Copy(Path.Join(temp, "NativeNes"), outputPath);
Directory.Delete(temp, true);