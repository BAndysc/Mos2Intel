using System.Text;
using Mos6502;

namespace Mos6502Disassembler;

public class Program
{
    
    
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ./Disassembler [BINARY] [OUTPUT] [START_OFFSET = 0] [LENGTH = max] [OUTPUT_OFFSET = 0]");
            return;
        }

        var binary = File.ReadAllBytes(args[0]);
        var output = args[1];
        var startOffset = args.Length >= 3 ? int.Parse(args[2]) : 0;
        var length = args.Length >= 4 ? int.Parse(args[3]) : binary.Length - startOffset;
        var outputOffset = args.Length >= 5 ? int.Parse(args[4]) : 0;

        List<(int, string)> lines = new();

        Dictionary<int, string> makeLabel = new();

        int i = 0;
        int brkInRow = 0;
        while (i < length)
        {
            try
            {
                var instruction = MosDisassembler.DecodeInstruction(binary.AsSpan(i + startOffset, Math.Min(3, binary.Length - i - startOffset)), out var readBytes);
                var bytes = binary.AsSpan(i + startOffset, readBytes).ToArray().Select(x => $"{x:X2}").ToArray();

                
                
                i += readBytes;
                
                if (instruction.Opcode == MosOpcode.BRK)
                {
                    brkInRow++;
                }
                else
                {
                    brkInRow = 0;
                }

                if (brkInRow >= 3)
                    continue;

                if (instruction.Opcode.IsBranchOpcode() || instruction.Opcode == MosOpcode.JMP)
                {
                    var destAddr = (instruction.Opcode == MosOpcode.JMP ? instruction.Operand : i + (instruction.Operand <= 127 ? instruction.Operand : -255+instruction.Operand - 1));
                    var destLabel = "L" + destAddr.ToString("X4");
                    makeLabel[destAddr] = destLabel;
                    lines.Add((i - readBytes + outputOffset, $"{string.Join(" ", bytes),-12} " + instruction.Opcode + " " + destLabel));
                }
                else
                    lines.Add((i - readBytes + outputOffset, $"{string.Join(" ", bytes),-12} " + instruction));
            }
            catch (Exception)
            {       
                brkInRow = 0;
                var bytes = binary.AsSpan(i + startOffset, 1).ToArray().Select(x => $"{x:X2}").ToArray();
                lines.Add((i + outputOffset, $"{string.Join(" ", bytes),-12} " + "???"));
                i++;
            }

        }

        StringBuilder sb = new();
        foreach (var line in lines)
        {
            if (makeLabel.TryGetValue(line.Item1, out var lbl))
            {
                sb.AppendLine(lbl + ":");
            }
            sb.AppendLine($"0x{line.Item1:X4}   " + line.Item2);
        }
        File.WriteAllText(output, sb.ToString());
    }
}