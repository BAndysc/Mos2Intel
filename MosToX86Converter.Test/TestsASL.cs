namespace MosToX86Converter.Test;

public class TestsASL : TestBase
{
    [Test]
    public void TestASLZero()
    {
        CompileAndRun("SEC\nASL\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
    }

    [Test]
    public void TestASLOne()
    {
        CompileAndRun("LDA #$1\nASL\nPHP");
        Assert.AreEqual(2, A);
        Assert.IsFalse(Z);
        Assert.IsFalse(N);
        Assert.IsFalse(C);
    }

    [Test]
    public void TestASLNegative()
    {
        CompileAndRun("LDA #$40\nASL\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
        Assert.IsFalse(C);
    }

    [Test]
    public void TestASLCarry()
    {
        CompileAndRun("LDA #$80\nASL\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsTrue(Z);
        Assert.IsFalse(N);
        Assert.IsTrue(C);
    }

    [Test]
    public void TestASL()
    {
        CompileAndRun("LDA #$FF\nASL\nPHP");
        Assert.AreEqual(0xFE, A);
        Assert.IsFalse(Z);
        Assert.IsTrue(N);
        Assert.IsTrue(C);
    }
}