using CommandLine;
using Mos6502Testing;

public class Program
{
    public class Options
    {
        [Value(0, Required = true)]
        public string Input { get; set; }
        
        [Option('o', "output", Required = true, HelpText = "Set output file path")]
        public string Output { get; set; }
    }
    
    public static void Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);
        result.WithParsed(RunProgram);
        result.WithNotParsed(ShowHelp);
    }

    private static void ShowHelp(IEnumerable<Error> obj)
    {
    }

    private static void RunProgram(Options ops)
    {
        var src = File.ReadAllBytes(ops.Input);

        MosConverter c = new MosConverter();
        var result = c.Convert(src, 0x600, new MosConverterOptions()
        {
            CycleMethod = true
        });

        string asmTemp = Path.GetTempFileName();
        string asmBin = Path.GetTempFileName();
        string runtimeSource = new FileInfo("runtime_easy.c").FullName;
        string runtimeBin = Path.GetTempFileName();
        string output = new FileInfo(ops.Output).FullName;

        string buildFlags = "-I/usr/local/homebrew/Cellar/sdl2/2.0.22/include";
        string linkFlags = "-lSDL2main -lSDL2 -L/usr/local/homebrew/Cellar/sdl2/2.0.22/lib";
        
        try
        {
            File.WriteAllText(asmTemp, result);
        
            Compiler.AssembleX86(asmTemp, asmBin);
            Compiler.BuildCObjectFile(runtimeSource, runtimeBin, buildFlags);
            Compiler.LinkExecutable(output, linkFlags, Language.C, runtimeBin, asmBin);
        }
        finally
        {
            if (File.Exists(asmTemp)) 
                File.Delete(asmTemp);
            if (File.Exists(asmBin))
                File.Delete(asmBin);
            if (File.Exists(runtimeBin)) 
                File.Delete(runtimeBin);
        }
    }
}