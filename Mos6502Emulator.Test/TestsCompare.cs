namespace Mos6502Emulator.Test;

public class TestsCompare : EmulatorTestBase
{
    [Test]
    public void TestEqual()
    {
        cpu.A = 0x20;
        Step("CMP #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestLesser()
    {
        cpu.A = 0x19;
        Step("CMP #$20");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
    }
    
    [Test]
    public void TestHigher()
    {
        cpu.A = 0x21;
        Step("CMP #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestEqualNegative()
    {
        cpu.A = 0x90;
        Step("CMP #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestLesserNegative()
    {
        cpu.A = 0x8F;
        Step("CMP #$90");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
    }
    
    [Test]
    public void TestHigherNegative()
    {
        cpu.A = 0x91;
        Step("CMP #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestHigherThanZero()
    {
        cpu.A = 0x80;
        Step("CMP #0");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
    }

    [Test]
    public void TestX()
    {
        cpu.X = 0x20;
        Step("CPX #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.X = 0x19;
        Step("CPX #$20");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        
        cpu.X = 0x21;
        Step("CPX #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.X = 0x90;
        Step("CPX #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.X = 0x8F;
        Step("CPX #$90");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        
        cpu.X = 0x91;
        Step("CPX #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.X = 0x80;
        Step("CPX #$7F");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestY()
    {
        cpu.Y = 0x20;
        Step("CPY #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.Y = 0x19;
        Step("CPY #$20");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        
        cpu.Y = 0x21;
        Step("CPY #$20");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.Y = 0x90;
        Step("CPY #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
        
        cpu.Y = 0x8F;
        Step("CPY #$90");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        
        cpu.Y = 0x91;
        Step("CPY #$90");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.C);
    }
}