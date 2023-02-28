namespace MosToX86Converter.Test;

public class TestsBit : TestBase
{
    [Test]
    public void TestASLZero()
    {
        CompileAndRun("LDA #$C0\nSTA $78\nLDA #$05\nBIT $78\nPHP");
        Assert.AreEqual(0x5, A);
        Assert.IsTrue(Z);
        Assert.IsTrue(N);
        Assert.IsFalse(C);
    }
}