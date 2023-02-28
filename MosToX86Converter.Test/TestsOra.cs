namespace MosToX86Converter.Test;

public class TestsOra : TestBase
{
    [Test]
    public void TestOr()
    {
        CompileAndRun("ORA #1\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$1\nORA #2\nPHP");
        Assert.AreEqual(3, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$3\nORA #$7F\nPHP");
        Assert.AreEqual(0x7F, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        
        CompileAndRun("LDA #$7F\nORA #$80\nPHP");
        Assert.AreEqual(0xFF, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
        
        CompileAndRun("LDA #$FF\nORA #0\nPHP");
        Assert.AreEqual(0xFF, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);

        CompileAndRun("ORA #0\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
    }
}