namespace MosToX86Converter.Test;

public class TestQuirks : TestBase
{
    [Test]
    public void TestIndirectXRollover()
    {
        CompileAndRun("org $600\nLDA #4\nSTA $0\nLDA #$80\nSTA $0400\nLDX #0\nLDA ($FF,X)", 0x600);
        Assert.AreEqual(0x80, A);
    }
    
    [Test]
    public void TestIndirectYRollover()
    {
        CompileAndRun("org $600\nLDA #4\nSTA $0\nLDA #$80\nSTA $0400\nLDY #0\nLDA ($FF), Y", 0x600);
        Assert.AreEqual(0x80, A);
    }
    
    [Test]
    public void TestIndirectJumpWithFFLowByte()
    {
        CompileAndRun("HLT\nHLT\norg $600\nLDA #$BF\nSTA $0200\nLDA #1\nSTA $02FF\nJMP ($02FF)\norg $BF01\nLDA #$10\nHLT", 0x600);
        Assert.AreEqual(0x10, A);
    }
}