namespace Mos6502Emulator.Test;

public class TestsIncrement : EmulatorTestBase
{
    [Test]
    public void TestInc()
    {
        memory[0] = 1;
        Step("INC 0");
        Assert.AreEqual(2, memory[0]);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);

        memory[0] = 0x7F;
        Step("INC 0");
        Assert.AreEqual(0x80, memory[0]);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("INC 0");
        Assert.AreEqual(0x81, memory[0]);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);

        memory[0] = 0xFF;
        Step("INC 0");
        Assert.AreEqual(0, memory[0]);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
    }
    
    [Test]
    public void TestInx()
    {
        cpu.X = 0xFF;
        Step("INX");
        Assert.AreEqual(0, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        
        Step("INX");
        Assert.AreEqual(1, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("INX");
        Assert.AreEqual(2, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.X = 0x7F;
        Step("INX");
        Assert.AreEqual(0x80, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestIny()
    {
        cpu.Y = 0xFF;
        Step("INY");
        Assert.AreEqual(0, cpu.Y);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        
        Step("INY");
        Assert.AreEqual(1, cpu.Y);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("INY");
        Assert.AreEqual(2, cpu.Y);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.Y = 0x7F;
        Step("INY");
        Assert.AreEqual(0x80, cpu.Y);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestINX()
    {
        string program = "INX\nINX\nINX\nINX\nINX\nINX\nINX";
        UseProgram(program);

        machine.Step();
        Assert.AreEqual(1, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        machine.Step();
        Assert.AreEqual(2, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.X = 0x7E;
        machine.Step();
        Assert.AreEqual(0x7F, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        machine.Step();
        Assert.AreEqual(0x80, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        machine.Step();
        Assert.AreEqual(0x81, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.X = 0xFE;
        machine.Step();
        Assert.AreEqual(0xFF, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        machine.Step();
        Assert.AreEqual(0, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
    }
}