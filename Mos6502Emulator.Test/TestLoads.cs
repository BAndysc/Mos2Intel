namespace Mos6502Emulator.Test;

public class TestLoads : EmulatorTestBase
{
    [Test]
    public void TestSelfModifyingProgram()
    {
        Step("LDA #2\nSTA $606\nLDA #$20");
        Assert.AreEqual(0x2, cpu.A);
    }
    
    [Test]
    public void TestLdaImmediate()
    {
        Step("LDA #$20");
        Assert.AreEqual(0x20, cpu.A);
    }
    
    [Test]
    public void TestLdaZeropage()
    {
        memory[0x20] = 0x30;
        Step("LDA $20");
        Assert.AreEqual(0x30, cpu.A);
    }
    
    [Test]
    public void TestLdaZeropageX()
    {
        memory[0x1F] = 0x30;
        cpu.X = 0xFF;
        Step("LDA $20, X");
        Assert.AreEqual(0x30, cpu.A);
    }
    
    [Test]
    public void TestLdaAbsolute()
    {
        memory[0xFF20] = 0x30;
        Step("LDA $FF20");
        Assert.AreEqual(0x30, cpu.A);
    }
    
    [Test]
    public void TestLdaAbsoluteX()
    {
        memory[0xFF21] = 0x30;
        cpu.X = 1;
        Step("LDA $FF20, X");
        Assert.AreEqual(0x30, cpu.A);
    }
    
    [Test]
    public void TestLdaAbsoluteY()
    {
        memory[0xFF21] = 0x30;
        cpu.Y = 1;
        Step("LDA $FF20, Y");
        Assert.AreEqual(0x30, cpu.A);
    }
    
    [Test]
    public void TestLdaIndirectX()
    {
        memory[1] = 2;
        memory[0x20] = 1;
        cpu.X = 0x60;
        Step("LDA ($C0, X)");
        Assert.AreEqual(2, cpu.A);
    }

    [Test]
    public void TestLdaIndirectY()
    {
        memory[1] = 2;
        cpu.Y = 1;
        Step("LDA ($20), Y");
        Assert.AreEqual(2, cpu.A);
    }

    [Test]
    public void TestLdaFlags()
    {
        Step("LDA #$1");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("LDA #$7F");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("LDA #$80");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("LDA #$FF");
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.Z);
        
        Step("LDA #$0");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.Z);
    }

    [Test]
    public void TestLdxImmediate()
    {
        Step("LDX #$20");
        Assert.AreEqual(0x20, cpu.X);
    }
    
    [Test]
    public void TestLdxZeropage()
    {
        memory[0x20] = 0x30;
        Step("LDX $20");
        Assert.AreEqual(0x30, cpu.X);
    }
    
    [Test]
    public void TestLdxZeropageY()
    {
        memory[0x1F] = 0x30;
        cpu.Y = 0xFF;
        Step("LDX $20, Y");
        Assert.AreEqual(0x30, cpu.X);
    }
    
    [Test]
    public void TestLdxAbsolute()
    {
        memory[0xFF20] = 0x30;
        Step("LDX $FF20");
        Assert.AreEqual(0x30, cpu.X);
    }
    
    [Test]
    public void TestLdxAbsoluteY()
    {
        memory[0xFF21] = 0x30;
        cpu.Y = 1;
        Step("LDX $FF20, Y");
        Assert.AreEqual(0x30, cpu.X);
    }
    
    [Test]
    public void TestLdyImmediate()
    {
        Step("LDY #$20");
        Assert.AreEqual(0x20, cpu.Y);
    }
    
    [Test]
    public void TestLdyZeropage()
    {
        memory[0x20] = 0x30;
        Step("LDY $20");
        Assert.AreEqual(0x30, cpu.Y);
    }
    
    [Test]
    public void TestLdyZeropageX()
    {
        memory[0x1F] = 0x30;
        cpu.X = 0xFF;
        Step("LDY $20, X");
        Assert.AreEqual(0x30, cpu.Y);
    }
    
    [Test]
    public void TestLdyAbsolute()
    {
        memory[0xFF20] = 0x30;
        Step("LDY $FF20");
        Assert.AreEqual(0x30, cpu.Y);
    }
    
    [Test]
    public void TestLdyAbsoluteX()
    {
        memory[0xFF21] = 0x30;
        cpu.X = 1;
        Step("LDY $FF20, X");
        Assert.AreEqual(0x30, cpu.Y);
    }
}