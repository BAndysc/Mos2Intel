namespace MosToX86Converter.Test;

public class TestSubtractWithBorrow : TestBase
{
    [Test]
    public void TestSubtract()
    {
        // CompileAndRun("SBC #$1");
        // Assert.AreEqual(0xFE, A);
        // Assert.IsFalse(C);
        // Assert.IsFalse(Z);
        // Assert.IsFalse(V);
        // Assert.IsTrue(N);
        
        CompileAndRun("LDA #$FE\nSBC #$1\nPHP");
        Assert.AreEqual(0xFC, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsTrue(N);

        CompileAndRun("LDA #$81\nSEC\nSBC #$1\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsTrue(N);
        
        CompileAndRun("LDA #$81\nSEC\nSBC #$1\nSBC #$1\nPHP");
        Assert.AreEqual(0x7f, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsTrue(V);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$81\nSEC\nSBC #$1\nSBC #$1\nSBC #$1\nPHP");
        Assert.AreEqual(0x7e, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$81\nSEC\nSBC #$1\nSBC #$1\nSBC #$1\nSBC #$1\nPHP");
        Assert.AreEqual(0x7d, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsFalse(N);

        CompileAndRun("LDA #$2\nSEC\nSBC #$1\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsTrue(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$2\nSEC\nSBC #$1\nSBC #$1\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(C);
        Assert.IsTrue(Z);
        Assert.IsFalse(V);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$2\nSEC\nSBC #$1\nSBC #$1\nSBC #$1\nPHP");
        Assert.AreEqual(0xff, A);
        Assert.IsFalse(C);
        Assert.IsFalse(Z);
        Assert.IsFalse(V);
        Assert.IsTrue(N);
    }
    
}