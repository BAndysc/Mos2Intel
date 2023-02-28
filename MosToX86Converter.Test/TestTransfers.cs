namespace MosToX86Converter.Test;

public class TestTransfers : TestBase
{
    [Test]
    public void TransferAtoX()
    {
        CompileAndRun("TAX\nPHP");
        Assert.AreEqual(0, X);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #1\nTAX\nPHP");
        Assert.AreEqual(1, X);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$80\nTAX\nPHP");
        Assert.AreEqual(0x80, X);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }
    
    [Test]
    public void TransferAtoY()
    {
        CompileAndRun("LDA #0\nTAY\nPHP");
        Assert.AreEqual(0, Y);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #1\nTAY\nPHP");
        Assert.AreEqual(1, Y);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$80\nTAY\nPHP");
        Assert.AreEqual(0x80, Y);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }
    
    [Test]
    public void TransferXtoA()
    {
        CompileAndRun("LDX #$0\nTXA\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDX #$1\nTXA\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDX #$80\nTXA\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }
    
    [Test]
    public void TransferYtoA()
    {
        CompileAndRun("LDY #$0\nTYA\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDY #$1\nTYA\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDY #$80\nTYA\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }

    [Test]
    public void TransferXToStackPointer()
    {
        CompileAndRun("LDX #$20\nLDA #1\nTXS\nPHA");
        Assert.AreEqual(0x1, memory[0x100 + 0x20]);
    }

    [Test]
    public void TransferStackPointerToX()
    {
        CompileAndRun("TSX");
        Assert.AreEqual(0xFF, X);
    }
}