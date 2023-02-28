namespace Mos6502Emulator.Test;

public class TestStores : EmulatorTestBase
{
    [Test]
    public void TestStoreZeroPage()
    {
        cpu.A = 1;
        Step("STA $2");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    public void TestStoreZeroPageX()
    {
        cpu.X = 1;
        cpu.A = 1;
        Step("STA $2, X");
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    public void TestStoreAbsolute()
    {
        cpu.A = 1;
        Step("STA $FF02");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    public void TestStoreAbsoluteX()
    {
        cpu.A = 1;
        cpu.X = 1;
        Step("STA $FF02, X");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    public void TestStoreAbsoluteY()
    {
        cpu.A = 1;
        cpu.Y = 1;
        Step("STA $FF02, Y");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    public void TestStoreIndirectX()
    {
        cpu.A = 1;
        cpu.X = 1;
        memory[0x21] = 0xFE;
        memory[0x22] = 0xFF;
        Step("STA ($20, X)");
        Assert.AreEqual(1, memory[0xFFFE]);
    }
    
    [Test]
    public void TestStoreIndirectY()
    {
        cpu.A = 1;
        cpu.Y = 1;
        memory[0x20] = 0xFD;
        memory[0x21] = 0xFF;
        Step("STA ($20), Y");
        Assert.AreEqual(1, memory[0xFFFE]);
    }
    
    [Test]
    public void TestStoreXZeroPage()
    {
        cpu.X = 1;
        Step("STX $2");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    public void TestStoreXZeroPageY()
    {
        cpu.Y = 1;
        cpu.X = 1;
        Step("STX $2, Y");
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    public void TestStoreXAbsolute()
    {
        cpu.X = 1;
        Step("STX $FF02");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    public void TestStoreYZeroPage()
    {
        cpu.Y = 1;
        Step("STY $2");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    public void TestStoreYZeroPageX()
    {
        cpu.X = 1;
        cpu.Y = 1;
        Step("STY $2, X");
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    public void TestStoreYAbsolute()
    {
        cpu.Y = 1;
        Step("STY $FF02");
        Assert.AreEqual(1, memory[0xFF02]);
    }
}