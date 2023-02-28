using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Mos6502Assembler;
using VirtualMachine;

namespace Mos6502Emulator.Test;

public class MosNesTest
{
    protected IMemory memory = null!;
    protected Mos6502Cpu cpu = null!;
    protected Assembler assembler = null!;
    protected Machine machine = null!;
    
    
    [SetUp]
    public void Setup()
    {
        memory = new RandomAccessMemory(0x10000);
        cpu = new Mos6502Cpu(0x600);
        assembler = new Assembler();
        machine = new Machine(memory, cpu);
    }
    
    [Test]
    public void NesTest()
    {
        var src = File.ReadAllBytes("mos/nestest.nes");
        var rom = src.AsSpan(0x10, 16384);
        var ram = src.AsSpan(0x10 + 16384, 8192);
        Regex logRegex = new Regex("^(....)  .*A:(..) X:(..) Y:(..) P:(..) SP:(..) PPU: *(\\d+), *(\\d+) CYC:(\\d+)");
        var correctLog = File.ReadAllLines("mos/nestest.log");

        byte[] originalProgramMemory = new byte[0x10000];
        rom.CopyTo(originalProgramMemory.AsSpan(0xC000));
        ram.CopyTo(originalProgramMemory.AsSpan(0x6000));
        
        memory.FillSpan(originalProgramMemory);

        cpu.IP = 0xC000;
        cpu.StackPointer = 0xFD; // apparently that's how NES works?
        cpu.DisableDecimalMode();
        
        int i = 0;
        int cycles = 7;
        while (cpu.IP != 0xC6BD)
        {
            var ip = cpu.IP;

            var log = correctLog[i++];
            var match = logRegex.Match(log);

            Assert.IsTrue(match.Success);
            ushort IP = ushort.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
            byte A = byte.Parse(match.Groups[2].Value, NumberStyles.HexNumber);
            byte X = byte.Parse(match.Groups[3].Value, NumberStyles.HexNumber);
            byte Y = byte.Parse(match.Groups[4].Value, NumberStyles.HexNumber);
            byte P = byte.Parse(match.Groups[5].Value, NumberStyles.HexNumber);
            byte SP = byte.Parse(match.Groups[6].Value, NumberStyles.HexNumber);
            int CYC = int.Parse(match.Groups[9].Value);

            var msg = $"Fail at {ip:X4} at step {i}";
            Assert.AreEqual(IP, ip, msg);
            Assert.AreEqual(A, cpu.A, msg);
            Assert.AreEqual(X, cpu.X, msg);
            Assert.AreEqual(Y, cpu.Y, msg);
            Assert.AreEqual(SP, cpu.StackPointer, msg);
            Assert.IsTrue((cycles - CYC) <= 1, msg);
            cycles = CYC;
            
            cycles += cpu.Step(memory);
        }
    }
}