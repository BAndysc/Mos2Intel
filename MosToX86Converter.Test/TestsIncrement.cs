namespace MosToX86Converter.Test;

public class TestsIncrement : TestBase
{
    [Test]
    public void TestInc()
    {
        CompileAndRun("org $600\nLDA #$1\nSTA $0\nINC 0\nPHP", 0x600);
        Assert.AreEqual(2, memory[0]);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);

        CompileAndRun("org $600\nLDA #$7F\nSTA $0\nINC 0\nPHP", 0x600);
        Assert.AreEqual(0x80, memory[0]);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("org $600\nLDA #$80\nSTA $0\nINC 0\nPHP", 0x600);
        Assert.AreEqual(0x81, memory[0]);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);

        CompileAndRun("org $600\nLDA #$FF\nSTA $0\nINC 0\nPHP", 0x600);
        Assert.AreEqual(0, memory[0]);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
    }
    
    [Test]
    public void TestInx()
    {
        CompileAndRun("LDX #$FF\nINX\nPHP");
        Assert.AreEqual(0, X);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        
        CompileAndRun("INX\nPHP");
        Assert.AreEqual(1, X);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDX #$1\nINX\nPHP");
        Assert.AreEqual(2, X);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);

        CompileAndRun("LDX #$7F\nINX\nPHP");
        Assert.AreEqual(0x80, X);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
    }
    
    [Test]
    public void TestIny()
    {
        CompileAndRun("LDY #$FF\nINY\nPHP");
        Assert.AreEqual(0, Y);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        
        CompileAndRun("INY\nPHP");
        Assert.AreEqual(1, Y);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDY #$1\nINY\nPHP");
        Assert.AreEqual(2, Y);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);

        CompileAndRun("LDY #$7F\nINY\nPHP");
        Assert.AreEqual(0x80, Y);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
    }
}