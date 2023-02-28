namespace Mos6502Emulator.Test;

public class TestsAddWithCarry : EmulatorTestBase
{
    [Test]
    public void TestADCImmediate()
    {
        UseProgram("ADC #1");
        machine.Step();
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestADCZeroPage()
    {
        UseProgram("ADC 1\nADC 1");
        machine.Step();
        Assert.AreEqual(0, cpu.A);
        memory[1] = 2;
        machine.Step();
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCZeroPageX()
    {
        cpu.X = 1;
        Step("ADC 0,x");
        Assert.AreEqual(0, cpu.A);
        memory[1] = 2;
        Step("ADC 0,x");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCZeroPageXOverflow()
    {
        cpu.X = 0x60;
        memory[0x20] = 2;
        Step("ADC $C0,x");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCAbsolute()
    {
        memory[256] = 2;
        Step("ADC 256");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCAbsoluteX()
    {
        memory[257] = 2;
        cpu.X = 1;
        Step("ADC 256, X");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCAbsoluteY()
    {
        memory[257] = 2;
        cpu.Y = 1;
        Step("ADC 256, Y");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCIndirectX()
    {
        memory[1] = 2;
        memory[0x20] = 1;
        cpu.X = 0x60;
        Step("ADC ($C0, X)");
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestADCIndirectY()
    {
        memory[1] = 2;
        cpu.Y = 1;
        Step("ADC ($20), Y");
        Assert.AreEqual(2, cpu.A);
    }

    [Test]
    public void TestADCFlags()
    {
        Step("ADC #1");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.V);
        
        cpu.A = 0x7E;
        Step("ADC #1");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.V);
        
        Step("ADC #1");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsTrue(cpu.V);
        
        Step("ADC #1");
        Assert.AreEqual(0x81, cpu.A);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.V);
        
        Step("ADC #$7E");
        Assert.AreEqual(0xFF, cpu.A);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.V);
        
        Step("ADC #1");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.V);

        Step("ADC #1");
        Assert.AreEqual(2, cpu.A); // becuase carry
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.V);
    }
}