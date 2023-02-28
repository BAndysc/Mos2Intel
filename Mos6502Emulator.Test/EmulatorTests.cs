using Mos6502Assembler;
using VirtualMachine;

namespace Mos6502Emulator.Test;

public class ComplexTests
{
    private IMemory memory = null!;
    private Mos6502Cpu cpu = null!;
    private Assembler assembler = null!;
    private Machine machine = null!;
    
    [SetUp]
    public void Setup()
    {
        memory = new RandomAccessMemory(0x10000);
        cpu = new Mos6502Cpu(0x600);
        assembler = new Assembler();
        machine = new Machine(memory, cpu);
    }
    
    [Test]
    public void Test()
    {
        string program = @"
  JSR init
  JSR loop
  JSR end

init:
  LDX #$00
  RTS

loop:
  INX
  CPX #$05
  BNE loop
  RTS

end:
  BRK
"; 
        var assembly = assembler.Assemble(program, out _,0x600);
        memory.Fill(assembly, 0x600);

        while (!cpu.I)
        { 
            machine.Step();
        }
        Assert.AreEqual(5, cpu.X);
    }
}