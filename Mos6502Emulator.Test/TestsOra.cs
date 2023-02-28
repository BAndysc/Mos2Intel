namespace Mos6502Emulator.Test;

public class TestsOra : EmulatorTestBase
{
    [Test]
    public void TestOr()
    {
        Step("ORA #1");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        Step("ORA #2");
        Assert.AreEqual(3, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        Step("ORA #$7F");
        Assert.AreEqual(0x7F, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        Step("ORA #$80");
        Assert.AreEqual(0xFF, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
        
        Step("ORA 0");
        Assert.AreEqual(0xFF, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);

        cpu.A = 0;
        Step("ORA 0");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
    }
}