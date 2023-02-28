namespace MosToX86Converter.Test;

public class TestsShifts : TestBase
{
    [Test]
    public void TestRightShiftAccumulator()
    {
        CompileAndRun("LDA #2\nLSR");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestRightShiftZeroPage()
    {
        CompileAndRun("LSR $2\ndb 2");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    public void TestRightShiftZeroPageX()
    {
        CompileAndRun("LDX #1\nLSR $10, X\n jmp end\norg $11\ndb 2\ndb 0\nend:\nnop");
        Assert.AreEqual(1, memory[0x11]);
    }
    
    [Test]
    public void TestRightShiftAbsolute()
    {
        CompileAndRun("LSR $FF02\n jmp end\norg $FF02\ndb 2\ndb 0\nend:\nnop");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    public void TestRightShiftAbsoluteX()
    {
        CompileAndRun("LDX #1\nLSR $FF02, X\njmp end\norg $FF03\ndb 2\ndb 0\nend:\nnop");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    public void TestRightShiftFlags()
    {
        CompileAndRun("LSR\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsTrue(Z);

        CompileAndRun("LDA #1\nLSR\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(C);
        Assert.IsTrue(Z);

        CompileAndRun("LDA #2\nLSR\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$80\nLSR\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);

        CompileAndRun("LDA #$81\nLSR\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
    }
    
    [Test]
    public void TestRotateLeft()
    {
        CompileAndRun("ROL\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsTrue(Z);

        CompileAndRun("SEC\nROL\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);

        CompileAndRun("LDA #$80\nROL\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsTrue(C);
        Assert.IsTrue(Z);

        CompileAndRun("LDA #$40\nROL\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsTrue(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);
    }
    
    [Test]
    public void TestRotateRight()
    {
        CompileAndRun("ROR\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsTrue(Z);

        CompileAndRun("LDA #$1\nROR\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsTrue(C);
        Assert.IsTrue(Z);

        CompileAndRun("LDA #$80\nROR\nPHP");
        Assert.AreEqual(0x40, A);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);

        CompileAndRun("LDA #$80\nSEC\nROR\nPHP");
        Assert.AreEqual(0x40|0x80, A);
        Assert.IsTrue(N);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);
    }
}