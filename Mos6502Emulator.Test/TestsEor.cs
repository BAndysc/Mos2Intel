namespace Mos6502Emulator.Test;

public class TestsEor : EmulatorTestBase
{
    [Test]
    public void TestEor()
    {
        cpu.A = 1;
        Step("EOR #2");
        Assert.AreEqual(1 ^ 2, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
    }
    
    [Test]
    public void TestEorNegative()
    {
        cpu.A = 1;
        Step("EOR #$80");
        Assert.AreEqual(1 ^ 0x80, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }
    
    [Test]
    public void TestEorZero()
    {
        cpu.A = 0x81;
        Step("EOR #$81");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
    }
}