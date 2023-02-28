namespace Mos6502Emulator.Test;

public class TestsSetFlags : EmulatorTestBase
{
    [Test]
    public void TestCLC()
    {
        cpu.C = true;
        Step("CLC");
        Assert.IsFalse(cpu.C);
        
        Step("CLC");
        Assert.IsFalse(cpu.C);
    }
    
    [Test]
    public void TestSEC()
    {
        Step("SEC");
        Assert.IsTrue(cpu.C);
        
        Step("SEC");
        Assert.IsTrue(cpu.C);
    }
    
    [Test]
    public void TestCLD()
    {
        cpu.D = true;
        Step("CLD");
        Assert.IsFalse(cpu.D);
        
        Step("CLD");
        Assert.IsFalse(cpu.D);
    }
    
    [Test]
    public void TestSED()
    {
        Step("SED");
        Assert.IsTrue(cpu.D);
        
        Step("SED");
        Assert.IsTrue(cpu.D);
    }
    
    [Test]
    public void TestCLI()
    {
        cpu.I = true;
        Step("CLI");
        Assert.IsFalse(cpu.I);
        
        Step("CLI");
        Assert.IsFalse(cpu.I);
    }
    
    [Test]
    public void TestSEI()
    {
        Step("SEI");
        Assert.IsTrue(cpu.I);
        
        Step("SEI");
        Assert.IsTrue(cpu.I);
    }
    
    [Test]
    public void TestCLV()
    {
        cpu.V = true;
        Step("CLV");
        Assert.IsFalse(cpu.V);
        
        Step("CLV");
        Assert.IsFalse(cpu.V);
    }
}