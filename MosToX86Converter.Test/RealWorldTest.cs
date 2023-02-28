using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Mos6502;
using Mos6502Testing;

namespace MosToX86Converter.Test;

public class RealWorldTest : TestBase
{
    [Test]
    public void MosFunctionalTestNoDecimal()
    {
        var programMemory = File.ReadAllBytes("mos/6502_functional_test_no_decimal.bin");
        // change last instruction to HALT
        Mos6502Assembler.Encoder.Encode(new MosInstruction(MosOpcode.HLT, MosAddressingMode.Implied), programMemory.AsSpan(0x3244));

        CompileAndRun(programMemory, 0x400, new MosConverterOptions()
        {
            DebugMode = false
        });
        
        Assert.Pass();
    }

    [Test]
    public void NesTestX86()
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
        
        CompileAndRun(programMemory, 0xC000, new MosConverterOptions()
        {
            DebugMode = true,
            StartStackPointer = 0xFD
        });

        Regex myLogRegex = new Regex("^(....) A:(..) X:(..) Y:(..) SP:(..)");
        Regex correctLogRegex = new Regex("^(....)  .*A:(..) X:(..) Y:(..) P:(..) SP:(..) PPU: *(\\d+), *(\\d+) CYC:(\\d+)");
        var correctLog = File.ReadAllLines("mos/nestest.log");
        var outputLines = output.Split("\n");
        for (int i = 0; i < outputLines.Length; ++i)
        {
            var myMatch = myLogRegex.Match(outputLines[i]);
            var correctMatch = correctLogRegex.Match(correctLog[i]);
            Assert.IsTrue(myMatch.Success, outputLines[i]);
            Assert.IsTrue(correctMatch.Success, correctLog[i]);

            ushort IP = ushort.Parse(correctMatch.Groups[1].Value, NumberStyles.HexNumber);
            byte A = byte.Parse(correctMatch.Groups[2].Value, NumberStyles.HexNumber);
            byte X = byte.Parse(correctMatch.Groups[3].Value, NumberStyles.HexNumber);
            byte Y = byte.Parse(correctMatch.Groups[4].Value, NumberStyles.HexNumber);
            byte SP = byte.Parse(correctMatch.Groups[6].Value, NumberStyles.HexNumber);
            
            ushort myIP = ushort.Parse(myMatch.Groups[1].Value, NumberStyles.HexNumber);
            byte myA = byte.Parse(myMatch.Groups[2].Value, NumberStyles.HexNumber);
            byte myX = byte.Parse(myMatch.Groups[3].Value, NumberStyles.HexNumber);
            byte myY = byte.Parse(myMatch.Groups[4].Value, NumberStyles.HexNumber);
            byte mySP = byte.Parse(myMatch.Groups[5].Value, NumberStyles.HexNumber);

            var mismatch = $"Mismatch in log in line {i+1}, originally in IP: {IP:X4}, my IP was: {myIP:X4}";
            Assert.AreEqual(IP, myIP, mismatch);
            Assert.AreEqual(A, myA, mismatch);
            Assert.AreEqual(X, myX, mismatch);
            Assert.AreEqual(Y, myY, mismatch);
            Assert.AreEqual(SP, mySP, mismatch);

            if (myIP == 0xC6BD)
                break;
        }
    }
}