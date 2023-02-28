namespace Mos6502Emulator.Test;

public class TestTransfers : EmulatorTestBase
{
    [Test]
    public void TransferAtoX()
    {
        cpu.A = 0;
        Step("TAX");
        Assert.AreEqual(0, cpu.X);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.A = 1;
        Step("TAX");
        Assert.AreEqual(1, cpu.X);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.A = 0x80;
        Step("TAX");
        Assert.AreEqual(0x80, cpu.X);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }
    
    [Test]
    public void TransferAtoY()
    {
        cpu.A = 0;
        Step("TAY");
        Assert.AreEqual(0, cpu.Y);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.A = 1;
        Step("TAY");
        Assert.AreEqual(1, cpu.Y);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.A = 0x80;
        Step("TAY");
        Assert.AreEqual(0x80, cpu.Y);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }
    
    [Test]
    public void TransferXtoA()
    {
        cpu.X = 0;
        Step("TXA");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.X = 1;
        Step("TXA");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.X = 0x80;
        Step("TXA");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }
    
    [Test]
    public void TransferYtoA()
    {
        cpu.Y = 0;
        Step("TYA");
        Assert.AreEqual(0, cpu.A);
        Assert.IsTrue(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.Y = 1;
        Step("TYA");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsFalse(cpu.N);
        
        cpu.Y = 0x80;
        Step("TYA");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsFalse(cpu.Z);
        Assert.IsTrue(cpu.N);
    }

    [Test]
    public void TransferXToStackPointer()
    {
        cpu.X = 0x20;
        cpu.A = 1;
        Step("TXS\nPHA");
        Assert.AreEqual(0x1, memory[0x100 + 0x20]);
    }

    [Test]
    public void TransferStackPointerToX()
    {
        Step("TSX");
        Assert.AreEqual(0xFF, cpu.X);
    }
}