global using NUnit.Framework;
using Mos6502Assembler;
using VirtualMachine;

namespace Mos6502Emulator.Test;

public class EmulatorTestBase
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

    protected void Step(string instructions, int count = 0)
    {
        var assembly = assembler.Assemble(instructions, out var instrCount, cpu.IP);
        memory.Fill(assembly, cpu.IP);
        if (count == 0)
            count = instrCount;
        machine.Step(count);
    }

    protected void UseProgram(string program, ushort baseAddr = 0x600)
    {
        var assembly = assembler.Assemble(program, out _, baseAddr);
        memory.Fill(assembly, baseAddr);
    }
}