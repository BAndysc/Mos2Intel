using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mos6502;
using Mos6502Assembler;
using Mos6502Disassembler;
using Mos6502Emulator;
using Mos6502Testing;
using VirtualMachine;

namespace MosToX86Converter.Test;

public class PerformanceTest : TestBase
{
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void Perf_MultiAdd_Native_Emulator(bool o3)
    {
        const int REPEAT = 3;
        var src = File.ReadAllText("mos/multi_add.mos");
        var binary = new Assembler().Assemble(src, out _).ToArray();
        RunNativeEmulator(binary, 0x600, o3, REPEAT, timeout: 65000);

        var oflags = o3 ? "-O3" : "-O0";
        Summary($"MultiAdd (X86 Emulator {oflags})");
    }
    
    [Test]
    public void Perf_MultiAdd_Managed_Emulator()
    {
        var src = File.ReadAllText("mos/multi_add.mos");
        var binary = new Assembler().Assemble(src, out _).ToArray();

        const int REPEATS = 3;
        
        for (int i = 0; i < REPEATS; ++i)
        {
            Mos6502Cpu cpu = new Mos6502Cpu(0x600);
            RandomAccessMemory mem = new RandomAccessMemory(0xFFF);
            mem.Fill(binary);

            var timer = timeCalculator.StartRun();
            while (true)
            {
                try
                {
                    cpu.Step(mem);
                }
                catch (MosHaltedException)
                {
                    break;
                }
            }
            timer.Dispose();
        }
        
        Summary("MultiAdd (C# Emulator)");
    }

    [Test]
    public void Perf_MultiAdd_Translated()
    {
        var src = File.ReadAllText("mos/multi_add.mos");
        var binary = new Assembler().Assemble(src, out _).ToArray();

        const int REPEATS = 10;
        
        CompileAndRun(binary, 0x600, new MosConverterOptions()
        {
            UseNativeFlags = true,
            UseNativeCallAsJsr = true
        }, repeat: REPEATS);

        Summary("MultiAdd (Translated)");
    }
    
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void Perf_MultiAdd_Native(bool o3)
    {
        const int REPEATS = 10;
        
        string flags = o3 ? "-O3" : "-O0";
        Compiler.BuildCObjectFile("mos/multi_add.c", "mos/multi_add.o", flags);
        Compiler.LinkExecutable( "mos/multi_add.exe", null, Language.C, "mos/multi_add.o");
        
        for (int i = 0; i < REPEATS; ++i)
        {
            var timer = timeCalculator.StartRun();
            var tuple = RunAndReadOutput("mos/multi_add.exe",  "", 10000);
            if (!tuple.HasValue)
                throw new Exception("Timeout while running multi_add.exe");
            timer.Dispose();
            int exitCode = 0;
            (output, exitCode) = tuple.Value;
            Assert.AreEqual(0, exitCode, output);
        }
        
        var oflags = o3 ? "-O3" : "-O0";
        Summary($"MultiAdd (X86 Native {oflags})");
    }
    
    private byte[] GCD_Prepare()
    {
        var bin = File.ReadAllBytes("mos/gcd");
        byte[] programMemory = new byte[0x10000];
        bin.CopyTo(programMemory.AsSpan(0));
        return programMemory;
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_GCD_Native_Emulator(bool optimize)
    {
        var programMemory = GCD_Prepare();
        const int REPEATS = 10;
        RunNativeEmulator(programMemory, 0x600, optimize, REPEATS, 9_135000);

        Summary("GCD x86 Emulator" + (optimize ? " (-O3)" : "(-O0)"));
    }

    [Test]
    public void Perf_GCD_Native_Translated_Test()
    {
        var programMemory = GCD_Prepare();
        const int REPEATS = 20;
        
        CompileAndRun(programMemory, 0x600, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = true,
            UseNativeCallAsJsr = false,
            UseNativeFlags = true
        },  repeat: REPEATS, timeout: 135000);
        
        
        Summary("GCD Translated");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_GCD_Native_X86_Test(bool optimize)
    {
        const int REPEATS = 30;

        string flags = optimize ? "-O3" : "-O0";
        Compiler.BuildCObjectFile("mos/gcd.c", "mos/gcd.o", flags);
        Compiler.LinkExecutable( "mos/gcd.exe", null, Language.C, "mos/gcd.o");
        
        for (int i = 0; i < REPEATS; ++i)
        {
            var timer = timeCalculator.StartRun();
            var tuple = RunAndReadOutput("mos/gcd.exe",  "", 10000);
            if (!tuple.HasValue)
                throw new Exception("Timeout while running gcd.exe");
            timer.Dispose();
            int exitCode = 0;
            (output, exitCode) = tuple.Value;
            Assert.AreEqual(0, exitCode, output);
        }
        
        Summary("GCD Native (x86): " + flags);
    }
    
    [Test]
    public void Perf_GCD_Emulator_Test()
    {
        var programMemory = GCD_Prepare();

        const int REPEATS = 5;

        for (int i = 0; i < REPEATS; ++i)
        {
            Mos6502Cpu cpu = new Mos6502Cpu(0x600);
            RandomAccessMemory mem = new RandomAccessMemory(0x10000);
            mem.Fill(programMemory);

        
            var timer = timeCalculator.StartRun();
            while (true)
            {
                try
                {
                    cpu.Step(mem);
                }
                catch (MosHaltedException)
                {
                    break;
                }
            }
            timer.Dispose();
        }

        Summary($"GCD C# Emulator ({Configuration})");
    }
    
#if DEBUG
    private string Configuration => "DEBUG";
#else
    private string Configuration => "RELEASE";
#endif

    private byte[] PrepareMosFunctionalTestNoDecimal()
    {
        var programMemory = File.ReadAllBytes("mos/6502_functional_test_no_decimal.bin");
        // change last instruction to HALT
        Encoder.Encode(new MosInstruction(MosOpcode.HLT, MosAddressingMode.Implied), programMemory.AsSpan(0x3244));
        return programMemory;
    }
    
    [Test]
    public void Perf_MosFunctionalTestNoDecimal_EMULATOR()
    {
        var programMemory = PrepareMosFunctionalTestNoDecimal();
        const int REPEATS = 20;
        
        RandomAccessMemory mem = new RandomAccessMemory(0x10000);
        for (int i = 0; i < REPEATS; ++i)
        {
            mem.Fill(programMemory);
            Mos6502Cpu cpu = new Mos6502Cpu(0x400);
            var timer = timeCalculator.StartRun();
            while (true)
            {
                try
                {
                    cpu.Step(mem);
                }
                catch (MosHaltedException)
                {
                    break;
                }
            }
            timer.Dispose();
        }
        Summary($"MosFunctionalTestNoDecimal C# Emulator ({Configuration})");
    }
    
    
    [Test]
    public void Perf_MosFunctionalTestNoDecimal_Translated()
    {
        var programMemory = PrepareMosFunctionalTestNoDecimal();
        const int REPEATS = 50;
        
        CompileAndRun(programMemory, 0x400, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = true,
            UseNativeCallAsJsr = true,
            UseNativeFlags = true
        },  repeat: REPEATS, timeout: 135000);
        Summary("MosFunctionalTestNoDecimal Translated");
    }
    
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_MosFunctionalTestNoDecimal_NativeEmulator(bool o3)
    {
        var programMemory = PrepareMosFunctionalTestNoDecimal();
        const int REPEATS = 50;
        
        RunNativeEmulator(programMemory, 0x400, o3,  repeat: REPEATS, timeout: 135000);
        
        Summary("MosFunctionalTestNoDecimal Native Emulator " + (o3 ? "-O3" : "-O0"));
    }
    
    
    private byte[] PrepareNesTest()
    {
        var src = File.ReadAllBytes("mos/nestest.nes");
        var rom = src.AsSpan(0x10, 16384);
        var ram = src.AsSpan(0x10 + 16384, 8192);
        
        byte[] programMemory = new byte[0x10000];
        rom.CopyTo(programMemory.AsSpan(0xC000));
        ram.CopyTo(programMemory.AsSpan(0x6000));

        // this test program invokes code that is saved
        // to the ram dynamically, I don't support it by design
        // so fill this memory ahead of time
        programMemory[0x300] = 169;
        programMemory[0x301] = 170;
        programMemory[0x302] = 96;
        programMemory[0x303] = 172;
        
        // change last instruction to HALT
        Mos6502Assembler.Encoder.Encode(new MosInstruction(MosOpcode.HLT, MosAddressingMode.Implied), programMemory.AsSpan(0xC6BD));
        return programMemory;
    }
    
    [Test]
    public void Perf_NesTest_EMULATOR()
    {
        var programMemory = PrepareNesTest();
        const int REPEATS = 25;
        
        RandomAccessMemory mem = new RandomAccessMemory(0x10000);
        for (int i = 0; i < REPEATS; ++i)
        {
            mem.Fill(programMemory);
            Mos6502Cpu cpu = new Mos6502Cpu(0xC000);
            var timer = timeCalculator.StartRun();
            while (true)
            {
                try
                {
                    cpu.Step(mem);
                }
                catch (MosHaltedException)
                {
                    break;
                }
            }
            timer.Dispose();
        }
        Summary($"MosFunctionalTestNoDecimal C# Emulator ({Configuration})");
    }
    
    
    [Test]
    public void Perf_NesTest_Translated()
    {
        var programMemory = PrepareNesTest();
        const int REPEATS = 50;
        
        CompileAndRun(programMemory, 0xC000, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = true,
            UseNativeCallAsJsr = false,
            UseNativeFlags = true
        },  repeat: REPEATS, timeout: 135000);
        Summary("MosFunctionalTestNoDecimal Translated");
    }
    
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_NesTest_NativeEmulator(bool o3)
    {
        var programMemory = PrepareNesTest();
        const int REPEATS = 25;
        
        RunNativeEmulator(programMemory, 0xC000, o3,  repeat: REPEATS, timeout: 135000);
        
        Summary("MosFunctionalTestNoDecimal Native Emuleator " + (o3 ? "-O3" : "-O0"));
    }

    private void VerifyMatrix(Func<int, byte> memGetter)
    {
        const int SRC_DATA = 0x1000;
        const int DIM = 120;
        const int DST_DATA = 0x4b19;

        for (int row = DIM - 1; row >=0 ; --row)
        {
            for (int col = DIM - 1; col >= 0; --col)
            {
                int result = 0;
                for (int k = 0; k < DIM; ++k)
                {
                    byte a = memGetter(SRC_DATA + row * DIM + k);
                    byte b = memGetter(SRC_DATA + k * DIM + col);
                    result += a * b;
                }

                var offset = DST_DATA + ((DIM - 1 - row) * DIM + (DIM - 1 - col)) * 3;
                var memResult = memGetter(offset) + (memGetter(offset + 1) << 8) + (memGetter(offset + 2) << 16);
                Assert.AreEqual(result, memResult, $"[{row}, {col}]");
            }
        }
    }
    
    [Test]
    public void Perf_Matrix()
    {
        var binary = File.ReadAllBytes("mos/matrix");

        const int REPEATS = 5;

        int j = 0;
        for (int i = 0; i < REPEATS; ++i)
        {
            Mos6502Cpu cpu = new Mos6502Cpu(0x600);
            RandomAccessMemory mem = new RandomAccessMemory(0x10000);
            mem.Fill(binary);

            var timer = timeCalculator.StartRun();
            while (true)
            {
                try
                {
                    cpu.Step(mem);
                }
                catch (MosHaltedException)
                {
                    break;
                }
            }

            VerifyMatrix(addr => mem[addr]);
            
            timer.Dispose();
        }
        
        Summary("Matrix (C# Emulator)");
    }

    
    
    [Test]
    public void Perf_Matrix_Translated()
    {
        var programMemory = File.ReadAllBytes("mos/matrix");
        const int REPEATS = 50;
        
        CompileAndRun(programMemory, 0x600, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = true,
            UseNativeCallAsJsr = true,
            UseNativeFlags = true
        },  repeat: REPEATS, timeout: 135000);

        VerifyMatrix(addr => memory[addr]);
        
        Summary("Matrix Translated");
    }
    
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_Matrix_NativeEmulator(bool o3)
    {
        var programMemory = File.ReadAllBytes("mos/matrix");
        const int REPEATS = 20;
        
        RunNativeEmulator(programMemory, 0x600, o3,  repeat: REPEATS, timeout: 135000);
        
        Summary("Matrix Native Emuleator " + (o3 ? "-O3" : "-O0"));
    }
    
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Perf_Matrix_Native_X86_Test(bool optimize)
    {
        const int REPEATS = 50;

        string flags = optimize ? "-Ofast" : "-O0";
        Compiler.BuildCObjectFile("mos/matrix.c", "mos/matrix.o", flags);
        Compiler.LinkExecutable( "mos/matrix.exe", null, Language.C, "mos/matrix.o");
        
        for (int i = 0; i < REPEATS; ++i)
        {
            var timer = timeCalculator.StartRun();
            var tuple = RunAndReadOutput("mos/matrix.exe",  "", 10000);
            if (!tuple.HasValue)
                throw new Exception("Timeout while running matrix.exe");
            timer.Dispose();
            int exitCode = 0;
            (output, exitCode) = tuple.Value;
            Assert.AreEqual(0, exitCode, output);
        }
        
        Summary("Matrix Native (x86): " + flags);
    }
}