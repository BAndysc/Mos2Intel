namespace MosToX86Converter.Test;

public class TestsAND : TestBase
{
    [Test]
    public void TestAndZero()
    {
        CompileAndRun("AND #1\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
    }
    
    [Test]
    public void TestAnd()
    {
        CompileAndRun("LDA #$3\nAND #1\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
    }
    
    [Test]
    public void TestAndNegative()
    {
        CompileAndRun("LDA #$FF\nAND #$80\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }
}