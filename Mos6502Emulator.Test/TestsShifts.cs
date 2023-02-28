namespace Mos6502Emulator.Test;

public class TestsShifts : EmulatorTestBase
{
    [Test]
    public void TestRightShiftAccumulator()
    {
        cpu.A = 2;
        Step("LSR");
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestRightShiftZeroPage()
    {
        memory[2] = 2;
        Step("LSR $2");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    public void TestRightShiftZeroPageX()
    {
        memory[3] = 2;
        cpu.X = 1;
        Step("LSR $2, X");
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    public void TestRightShiftAbsolute()
    {
        memory[0xFF02] = 2;
        Step("LSR $FF02");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    public void TestRightShiftAbsoluteX()
    {
        memory[0xFF03] = 2;
        cpu.X = 1;
        Step("LSR $FF02, X");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    public void TestRightShiftFlags()
    {
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.A = 1;
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.A = 2;
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);
        
        cpu.A = 0x80;
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);

        cpu.N = true;
        cpu.A = 0x80;
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);

        cpu.A = 0x81;
        Step("LSR");
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.C);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestRotateLeft()
    {
        Step("ROL");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.C = true;
        Step("ROL");
        Assert.AreEqual(1, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);

        cpu.A = 0x80;
        Step("ROL");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.C = false;
        cpu.A = 0x40;
        Step("ROL");
        Assert.AreEqual(0x80, cpu.A);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);
    }
    
    [Test]
    public void TestRotateRight()
    {
        Step("ROR");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.A = 1;
        Step("ROR");
        Assert.AreEqual(0, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsTrue(cpu.C);
        Assert.IsTrue(cpu.Z);

        cpu.C = false;
        cpu.A = 0x80;
        Step("ROR");
        Assert.AreEqual(0x40, cpu.A);
        Assert.IsFalse(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);

        cpu.C = true;
        cpu.A = 0x80;
        Step("ROR");
        Assert.AreEqual(0x40|0x80, cpu.A);
        Assert.IsTrue(cpu.N);
        Assert.IsFalse(cpu.C);
        Assert.IsFalse(cpu.Z);
    }
}