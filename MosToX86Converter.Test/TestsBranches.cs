namespace MosToX86Converter.Test;

public class TestsBranches : TestBase
{
    [Test]
    public void TestBCCOnClear()
    {
        CompileAndRun("BCC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBCCOnSet()
    {
        CompileAndRun("SEC\nBCC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBCSOnClear()
    {
        CompileAndRun("BCS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBCSOnSet()
    {
        CompileAndRun("SEC\nBCS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBEQOnClear()
    {
        CompileAndRun("BEQ lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBEQOnSet()
    {
        CompileAndRun("LDA #0\nBEQ lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBNEOnClear()
    {
        CompileAndRun("BNE lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBNEOnSet()
    {
        CompileAndRun("LDA #0\nBNE lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBMIOnClear()
    {
        CompileAndRun("BMI lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBMIOnSet()
    {
        CompileAndRun("LDA #$80\nBMI lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBPLOnClear()
    {
        CompileAndRun("BPL lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBPLOnSet()
    {
        CompileAndRun("LDA #$80\nBPL lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBVCOnClear()
    {
        CompileAndRun("BVC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestBVCOnSet()
    {
        CompileAndRun("ADC #$70\nADC #$10\nBVC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBVSOnClear()
    {
        CompileAndRun("BVS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestBVSOnSet()
    {
        // ADC to make V flag up
        CompileAndRun("ADC #$70\nADC #$10\nBVS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP\nHLT");
        Assert.AreEqual(2, A);
    }
    
}