namespace Mos6502Emulator.Test;

public class TestSubtractWithBorrow : EmulatorTestBase
{
    [Test]
    public void TestSubtract()
    {
        Step("SBC #$1");
        Assert.AreEqual(0xFE, cpu.A);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsTrue(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0xFC, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsTrue(cpu.N);

        cpu.A = 0x81;
        cpu.C = true;
        Step("SBC #$1");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsTrue(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0x7f, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.V);
        Assert.IsFalse(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0x7e, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsFalse(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0x7d, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsFalse(cpu.N);

        cpu.A = 2;
        cpu.C = true;
        Step("SBC #$1");
        Assert.AreEqual(1, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsFalse(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsFalse(cpu.N);
        
        Step("SBC #$1");
        Assert.AreEqual(0xff, cpu.A);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.V);
        Assert.IsTrue(cpu.N);
    }
    
}