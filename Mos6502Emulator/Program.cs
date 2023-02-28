using System.Globalization;
using VirtualMachine;

namespace Mos6502Emulator;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ./emulator [bin file] [start location]");
            return;
        }

        ushort startLocation = 0;
        if (args[^1].StartsWith("$"))
            startLocation = ushort.Parse(args[^1].Substring(1), NumberStyles.HexNumber);
        else
            startLocation = ushort.Parse(args[^1]);

        string binFile = args[^2];
        
        var bytes = File.ReadAllBytes(binFile);

        for (int i = 0; i < 1000; ++i)
        {
            var memory = new RandomAccessMemory(0x10000);
            var cpu = new Mos6502Cpu(startLocation);
            var machine = new Machine(memory, cpu);
            memory.Fill(bytes);

            while (true)
            {
                var ip = cpu.IP;
                machine.Step();
                if (ip == 0x3244)
                    break;
            }   
        }
    }
}