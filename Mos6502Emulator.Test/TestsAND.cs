namespace Mos6502Emulator.Test;

public class TestsAND : EmulatorTestBase
{
    [Test]
    public void TestAndZero()
    {
        Step("AND #1");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
    }
    
    [Test]
    public void TestAnd()
    {
        cpu.A = 3;
        Step("AND #1");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
    }
    
    [Test]
    public void TestAndNegative()
    {
        cpu.A = 0xFF;
        Step("AND #$80");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }
}