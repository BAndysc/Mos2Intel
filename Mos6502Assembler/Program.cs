namespace Mos6502Assembler;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ./Assebler [SOURCE] [OUTPUT]");
            return;
        }

        var source = File.ReadAllLines(args[0]);
        var output = args[1];

        Assembler asm = new Assembler();
        var bytes = asm.Assemble(source, out _, 0);
        File.WriteAllBytes(output, bytes.ToArray());
    }
}