namespace Mos6502Emulator.Test;

public class TestsASL : EmulatorTestBase
{
    [Test]
    public void TestASLZero()
    {
        cpu.C = true;
        cpu.N = true;
        cpu.Z = false;
        
        Step("ASL");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
    }

    [Test]
    public void TestASLOne()
    {
        cpu.A = 1;
        Step("ASL");
        Assert.AreEqual(2, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
    }

    [Test]
    public void TestASLNegative()
    {
        cpu.A = 0x40;
        Step("ASL");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.C);
    }

    [Test]
    public void TestASLCarry()
    {
        cpu.A = 0x80;
        Step("ASL");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.C);
    }

    [Test]
    public void TestASL()
    {
        cpu.A = 0xFF;
        Step("ASL");
        Assert.AreEqual(0xFE, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
        Assert.IsTrue(cpu.C);
    }
}