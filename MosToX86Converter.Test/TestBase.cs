global using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mos6502;
using Mos6502Assembler;
using Mos6502Testing;

namespace MosToX86Converter.Test;

public abstract class TestBase
{
    private string asmSource = null!;
    private string asmBinary = null!;
    private string runtimeSource = null!;
    private string runtimeObject = null!;
    private string exe = null!;
    protected string output = null!;

    protected byte[] memory = null!;
    protected byte A = 0;
    protected byte X = 0;
    protected byte Y = 0;
    protected bool N = false;
    protected bool Z = false;
    protected bool C = false;
    protected bool V = false;
    protected bool D = false;
    protected bool I = false;
    protected Mos6502SR Flags = 0;

    protected MosConverterOptions? DefaultOptions;

    protected TimeCalculator timeCalculator;

    protected void Summary(string name)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Console.WriteLine("Test: {0}", name);
        Console.WriteLine("   Runs: {0}", timeCalculator.Runs);
        Console.WriteLine("   Average: {0}", timeCalculator.Average.TotalSeconds + "s");
        Console.WriteLine("   Min: {0}", timeCalculator.Worst.TotalSeconds + "s");
        Console.WriteLine("   Max: {0}", timeCalculator.Best.TotalSeconds + "s");
        Console.WriteLine("   StdDev: {0}", timeCalculator.StdDev.TotalSeconds + "s");
        Console.WriteLine("   [Values: {0}", string.Join(", ", timeCalculator.Scores.Select(s => s.TotalSeconds))+"]");
    }
    
    [SetUp]
    public void Setup()
    {
        asmSource = Path.GetTempFileName() + ".asm";
        asmBinary = Path.GetTempFileName() + ".o";
        runtimeSource = new FileInfo("runtime_sync.c").FullName;
        runtimeObject = Path.GetTempFileName() + ".o";
        exe = Path.GetTempFileName();
        timeCalculator = new TimeCalculator();
    }

    protected static (string, int)? RunAndReadOutput(string exe, string arguments, int timeout)
    {
        async Task<(string, int)> Run(Process process)
        {
            var output = await process.StandardOutput.ReadToEndAsync();
            var outputError = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            if (!string.IsNullOrEmpty(outputError))
                Console.WriteLine(outputError);
            return (output, process.ExitCode);
        }

        async Task<(string, int)?> RunOrWait()
        {
            var process = Process.Start(new ProcessStartInfo(exe, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            })!;
            
            var run = Run(process);
            var wait = Task.Delay(timeout);
            var completed = await Task.WhenAny(run, wait);
            if (completed == wait)
            {
                process.Kill();
                return null;
            }
            return await run;
        }

        var t = RunOrWait();
        t.Wait();
        return t.Result;
    }
    
    protected void CompileAndRun(byte[] binary, ushort startInstruction = 0, MosConverterOptions? options = null, uint? maxInstr = null, int? repeat = null, int timeout = 5000)
    {
        options ??= DefaultOptions;
        Debug.Assert(maxInstr == null || options != null && options.CycleMethod); // either no max Instr or use cycling
        MosConverter converter = new MosConverter();
        var result = converter.Convert(binary, startInstruction, options);
        File.WriteAllText(asmSource, result);
        
        Compiler.AssembleX86(asmSource, asmBinary);

        string defines = "";
        if ((options ?? MosConverterOptions.Default).CycleMethod)
            defines = $"-DSTART_INSTR={startInstruction} -DCYCLES={maxInstr ?? 0}";

        Compiler.BuildCObjectFile(runtimeSource, runtimeObject, defines);
        Compiler.LinkExecutable(exe, null, Language.C, runtimeObject, asmBinary);

        for (int i = 0; i < (repeat ?? 1); ++i)
        {
            var timer = timeCalculator.StartRun();
            var tuple = RunAndReadOutput(exe,  "", timeout);
            if (!tuple.HasValue)
                throw new Exception("Timeout while running " + exe);
            timer.Dispose();
            int exitCode = 0;
            (output, exitCode) = tuple.Value;
            Assert.AreEqual(0, exitCode, output);
        }

        if (options == null || !options.DebugMode)
        {
            var bytesStr = output.Split(new char[] { ' ', '\n' });
            memory = bytesStr.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
            A = memory[0];
            X = memory[1];
            Y = memory[2];
            Flags = (Mos6502SR)memory[3];
            Z = (Flags & Mos6502SR.Z) != 0;
            N = (Flags & Mos6502SR.N) != 0;
            C = (Flags & Mos6502SR.C) != 0;
            V = (Flags & Mos6502SR.V) != 0;
            D = (Flags & Mos6502SR.D) != 0;
            I = (Flags & Mos6502SR.I) != 0;
            memory = memory.Skip(4).ToArray();
        }
    }
    
    protected void CompileAndRun(string asm, ushort startInstruction = 0, MosConverterOptions? options = null, uint? maxInstr = null)
    {
        Assembler mosAsm = new Assembler();
        var binary = mosAsm.Assemble(asm, out _).ToArray();
        CompileAndRun(binary, startInstruction, options, maxInstr);
    }

    protected void RunNativeEmulator(byte[] binary, ushort startInstruction = 0, bool optimize3 = false, int? repeat = null, int timeout = 5000)
    {
        var nativeMos6502Source = Path.GetFullPath("native/mos6502/mos6502.cpp");
        var nativeMos6502Object = Path.GetTempFileName() + ".o";
        var nativeEmulatorSource = Path.GetFullPath("native/runtime.cpp");
        var nativeEmulatorObject = Path.GetTempFileName() + ".o";
        var nativeEmulatorExe = Path.GetTempFileName();
        var binaryFile = Path.GetTempFileName() + ".bin";
        
        string flags = optimize3 ? "-O3" : "-O0";
        flags += " -g";
        Compiler.BuildCObjectFile(nativeMos6502Source, nativeMos6502Object, extra: flags, lang: Language.CPP);
        Compiler.BuildCObjectFile(nativeEmulatorSource, nativeEmulatorObject, extra: flags, lang: Language.CPP);
        Compiler.LinkExecutable(nativeEmulatorExe, null, Language.CPP, nativeEmulatorObject, nativeMos6502Object);

        File.WriteAllBytes(binaryFile, binary);
        
        if (File.Exists(nativeMos6502Object))
            File.Delete(nativeMos6502Object);
        
        if (File.Exists(nativeEmulatorObject))
            File.Delete(nativeEmulatorObject);

        for (int i = 0; i < (repeat ?? 1); ++i)
        {
            var timer = timeCalculator.StartRun();
            var tuple = RunAndReadOutput(nativeEmulatorExe, "\"" + binaryFile + "\" " + startInstruction, timeout);
            if (!tuple.HasValue)
                throw new Exception("Timeout while running " + exe);
            timer.Dispose();
            int exitCode = 0;
            (output, exitCode) = tuple.Value;
            Assert.AreEqual(0, exitCode, output);
        }
        
        if (File.Exists(nativeEmulatorExe))
            File.Delete(nativeEmulatorExe);
        
        if (File.Exists(binaryFile))
            File.Delete(binaryFile);
    }
    
    [TearDown]
    public void Teardown()
    {
        //Console.WriteLine(asmSource);
        if (File.Exists(asmSource))
            File.Delete(asmSource);
        if (File.Exists(asmBinary))
            File.Delete(asmBinary);
        if (File.Exists(runtimeObject))
            File.Delete(runtimeObject);
        if (File.Exists(exe))
          File.Delete(exe);
        //Console.WriteLine(exe);
    }
}