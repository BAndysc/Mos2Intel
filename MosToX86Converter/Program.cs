using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Mos6502Testing;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: conveter [source_bin_path] [start_address] [output_asm_path] ");
            return;
        }

        string SOURCE = args[0];
        int START_ADDR = 0;
        if (args[1].StartsWith("0x"))
            START_ADDR = int.Parse(args[1].Substring(2), NumberStyles.HexNumber);
        else
            START_ADDR = int.Parse(args[1]);
        string OUTPUT = args[2];

        
        var src = File.ReadAllBytes(SOURCE);

        MosConverter c = new MosConverter();
        var result = c.Convert(src, START_ADDR, new MosConverterOptions()
        {
            DebugMode = false
        });
        File.WriteAllText(OUTPUT, result);
    }
}