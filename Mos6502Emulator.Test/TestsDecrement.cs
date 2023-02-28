namespace Mos6502Emulator.Test;

public class TestsDecrement : EmulatorTestBase
{
    [Test]
    public void TestDec()
    {
        memory[0] = 1;
        Step("DEC 0");
        Assert.AreEqual(0, memory[0]);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        
        Step("DEC 0");
        Assert.AreEqual(0xFF, memory[0]);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("DEC 0");
        Assert.AreEqual(0xFE, memory[0]);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);

        memory[0] = 0x80;
        Step("DEC 0");
        Assert.AreEqual(0x7F, memory[0]);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestDex()
    {
        cpu.X = 1;
        Step("DEX");
        Assert.AreEqual(0, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        
        Step("DEX");
        Assert.AreEqual(0xFF, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("DEX");
        Assert.AreEqual(0xFE, cpu.X);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.X = 0x80;
        Step("DEX");
        Assert.AreEqual(0x7F, cpu.X);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestDey()
    {
        cpu.Y = 1;
        Step("DEY");
        Assert.AreEqual(0, cpu.Y);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        
        Step("DEY");
        Assert.AreEqual(0xFF, cpu.Y);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("DEY");
        Assert.AreEqual(0xFE, cpu.Y);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);

        cpu.Y = 0x80;
        Step("DEY");
        Assert.AreEqual(0x7F, cpu.Y);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
    }
}