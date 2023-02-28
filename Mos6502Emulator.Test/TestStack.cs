using Mos6502;

namespace Mos6502Emulator.Test;

public class TestStack : EmulatorTestBase
{
    [Test]
    public void TestPush()
    {
        cpu.A = 1;
        Step("PHA");
        Assert.AreEqual(1, memory[511]);
        
        cpu.A = 2;
        Step("PHA");
        Assert.AreEqual(2, memory[510]);
    }
    
    [Test]
    public void TestPushStatusRegister()
    {
        Step("PHP");
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused, (Mos6502SR)memory[511]);
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused, cpu.SR);

        cpu.N = true;
        Step("PHP");
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused | Mos6502SR.N, (Mos6502SR)memory[510]);
        Assert.AreEqual(Mos6502SR.N | Mos6502SR.B | Mos6502SR.Unused, cpu.SR);
    }
    
    [Test]
    public void TestPushOverflow()
    {
        cpu.A = 1;
        for (int i = 0; i < 256; ++i)
            Step("PHA");
    
        Assert.AreEqual(1, memory[256]);

        cpu.A = 2;
        Step("PHA");
        Assert.AreEqual(2, memory[511]);
    }
    
    [Test]
    public void TestPull()
    {
        cpu.A = 1;
        Step("PHA");
        cpu.A = 0;
        Step("PLA");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        cpu.A = 0x80;
        Step("PHA");
        cpu.A = 0;
        Step("PLA");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        cpu.A = 0;
        Step("PHA");
        cpu.A = 1;
        Step("PLA");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
    }
    
    [Test]
    public void TestPullOverflow()
    {
        memory[256] = 1;
        Step("PLA");
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestPullSR()
    {
        memory[256] = memory[257] = (byte)(Mos6502SR.C | Mos6502SR.B);
        Step("PLP");
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.B);

        cpu.B = true;
        Step("PLP");
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.B);
    }
}