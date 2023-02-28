using Mos6502Testing;

namespace MosToX86Converter.Test;

public class TestCycling : TestBase
{
    [Test]
    public void TestRunTwoCycles()
    {
        CompileAndRun("org $600\nLDA #1\nLDX #2\nLDY #3", 0x600, new MosConverterOptions()
        {
            CycleMethod = true
        }, 2);
        Assert.AreEqual(1, A);
        Assert.AreEqual(2, X);
        Assert.AreEqual(0, Y);
    }
    
    [Test]
    public void TestJump()
    {
        CompileAndRun("org $600\nJMP lbl\nLDA #1\nlbl:\nLDA #2\nLDX #3", 0x600, new MosConverterOptions()
        {
            CycleMethod = true
        }, 3);
        Assert.AreEqual(2, A);
        Assert.AreEqual(3, X);
    }
    
    [Test]
    public void TestCyclingHalt()
    {
        CompileAndRun("org $600\nLDA #1\nHLT\nLDY #3", 0x600, new MosConverterOptions()
        {
            CycleMethod = true
        }, 3);
        Assert.AreEqual(1, A);
        Assert.AreEqual(0, X);
        Assert.AreEqual(0, Y);
    }
    
    [Test]
    public void TestCyclingSubroutine()
    {
        CompileAndRun("org $600\nJSR func\nLDX #1\nHLT\nfunc:\nLDA #1\nRTS", 0x600, new MosConverterOptions()
        {
            CycleMethod = true
        });
        Assert.AreEqual(1, A);
        Assert.AreEqual(1, X);
        Assert.AreEqual(0, Y);
    }
}