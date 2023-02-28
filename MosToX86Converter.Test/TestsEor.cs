namespace MosToX86Converter.Test;

public class TestsEor : TestBase
{
    [Test]
    public void TestEor()
    {
        CompileAndRun("LDA #1\nEOR #2\nPHP");
        Assert.AreEqual(1 ^ 2, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
    }
    
    [Test]
    public void TestEorNegative()
    {
        CompileAndRun("LDA #1\nEOR #$80\nPHP");
        Assert.AreEqual(1 ^ 0x80, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
    }
    
    [Test]
    public void TestEorZero()
    {
        CompileAndRun("LDA #$81\nEOR #$81\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
    }
}