namespace MosToX86Converter.Test;

public class TestsDecrement : TestBase
{
    [Test]
    public void TestDec()
    {
        CompileAndRun("org $600\nLDA #$1\nSTA $0\nDEC 0\nPHP", 0x600);
        Assert.AreEqual(0, memory[0]);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        
        CompileAndRun("org $600\nDEC 0\nPHP", 0x600);
        Assert.AreEqual(0xFF, memory[0]);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("org $600\nLDA #$FF\nSTA $0\nDEC 0\nPHP", 0x600);
        Assert.AreEqual(0xFE, memory[0]);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);

        CompileAndRun("org $600\nLDA #$80\nSTA $0\nDEC 0\nPHP", 0x600);
        Assert.AreEqual(0x7F, memory[0]);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
    }
    
    [Test]
    public void TestDex()
    {
        CompileAndRun("LDX #1\nDEX\nPHP");
        Assert.AreEqual(0, X);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        
        CompileAndRun("DEX\nPHP");
        Assert.AreEqual(0xFF, X);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDX #$FF\nDEX\nPHP");
        Assert.AreEqual(0xFE, X);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);

        CompileAndRun("LDX #$80\nDEX\nPHP");
        Assert.AreEqual(0x7F, X);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
    }
    
    [Test]
    public void TestDey()
    {
        CompileAndRun("LDY #1\nDEY\nPHP");
        Assert.AreEqual(0, Y);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        
        CompileAndRun("DEY\nPHP");
        Assert.AreEqual(0xFF, Y);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDY #$FF\nDEY\nPHP");
        Assert.AreEqual(0xFE, Y);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);

        CompileAndRun("LDY #$80\nDEY\nPHP");
        Assert.AreEqual(0x7F, Y);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
    }
}