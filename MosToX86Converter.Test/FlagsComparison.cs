using System;
using System.IO;
using System.Linq;
using Mos6502;
using Mos6502Assembler;
using Mos6502Testing;

namespace MosToX86Converter.Test;

public class FlagsComparison : TestBase
{
    
    [Test]
    [TestCase(true, true, true)]
    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, false, false)]
    [TestCase(false, true, true)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    public void GCD_Translated_MultipleOptions(bool nativeCall, bool useArrayMem, bool useNativeFlags)
    {
        var gcd = File.ReadAllBytes("mos/gcd");
        const int REPEATS = 30;
        
        CompileAndRun(gcd, 0x600, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = useArrayMem,
            UseNativeCallAsJsr = nativeCall,
            UseNativeFlags = useNativeFlags
        },  repeat: REPEATS);
        
        Summary($"GCD Translated (nativeCall: {nativeCall}, use array mem: {useArrayMem}, use native flags: {useNativeFlags})");
    }
    
    [Test]
    [TestCase(true, true, true)]
    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, false, false)]
    [TestCase(false, true, true)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    public void MosFunctionalTestNoDecimalPerformance(bool nativeCall, bool useArrayMem, bool useNativeFlags)
    {
        var programMemory = File.ReadAllBytes("mos/6502_functional_test_no_decimal.bin");
        // change last instruction to HALT
        Encoder.Encode(new MosInstruction(MosOpcode.HLT, MosAddressingMode.Implied), programMemory.AsSpan(0x3244));

        const int REPEATS = 50;
        
        CompileAndRun(programMemory, 0x400, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = useArrayMem,
            UseNativeCallAsJsr = nativeCall,
            UseNativeFlags = useNativeFlags
        },  repeat: REPEATS);
        
        Summary($"6502_functional_test_no_decimal native (nativeCall: {nativeCall}, use array mem: {useArrayMem}, use native flags: {useNativeFlags})");
    }
    
    
    [Test]
    [TestCase(true, true, true)]
    [TestCase(true, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, false, false)]
    [TestCase(false, true, true)]
    [TestCase(false, true, false)]
    [TestCase(false, false, true)]
    [TestCase(false, false, false)]
    public void MultiAdd_Options(bool nativeCall, bool useArrayMem, bool useNativeFlags)
    {
        var src = File.ReadAllText("mos/multi_add.mos");
        Assembler mosAsm = new Assembler();
        var programMemory = mosAsm.Assemble(src, out _).ToArray();
       
        const int REPEATS = 25;
        
        CompileAndRun(programMemory, 0x600, new MosConverterOptions()
        {
            DebugMode = false,
            CycleMethod = false,
            UseArrayAsMem = useArrayMem,
            UseNativeCallAsJsr = nativeCall,
            UseNativeFlags = useNativeFlags
        },  repeat: REPEATS, timeout:15000);

        Summary($"MultiAdd (nativeCall: {nativeCall}, use array mem: {useArrayMem}, use native flags: {useNativeFlags})");
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void NES_Native_Translation_Multiple_Options(bool useArrayMem, bool useNativeFlags)
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
        
        const int REPEATS = 50;
        
        CompileAndRun(programMemory, 0xC000, new MosConverterOptions()
        {
            DebugMode = false,
            StartStackPointer = 0xFD,
            UseArrayAsMem = useArrayMem,
            UseNativeCallAsJsr = false, // NES TEST REQUIRES THIS TO BE FALSE (modifies return addresses)
            UseNativeFlags = useNativeFlags
        }, repeat: REPEATS);
        
        Summary($"Nes Test native (nativeCall: {false}, use array mem: {useArrayMem}, use native flags: {useNativeFlags})");
    }
}