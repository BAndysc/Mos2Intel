namespace MosToX86Converter.Test;

public class TestsSetFlags : TestBase
{
    [Test]
    public void TestCLC()
    {
        CompileAndRun("SEC\nCLC\nPHP");
        Assert.IsFalse(C);
        
        CompileAndRun("CLC\nPHP");
        Assert.IsFalse(C);
    }
    
    [Test]
    public void TestSEC()
    {
        CompileAndRun("SEC\nPHP");
        Assert.IsTrue(C);
        
        CompileAndRun("SEC\nCLC\nSEC\nPHP");
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestCLD()
    {
        CompileAndRun("SED\nCLD\nPHP");
        Assert.IsFalse(D);
        
        CompileAndRun("CLD\nPHP");
        Assert.IsFalse(D);
    }
    
    [Test]
    public void TestSED()
    {
        CompileAndRun("SED\nPHP");
        Assert.IsTrue(D);
        
        CompileAndRun("CLD\nSED\nPHP");
        Assert.IsTrue(D);
    }
    
    [Test]
    public void TestCLI()
    {
        CompileAndRun("SEI\nCLI\nPHP");
        Assert.IsFalse(I);
        
        CompileAndRun("CLI\nPHP");
        Assert.IsFalse(I);
    }
    
    [Test]
    public void TestSEI()
    {
        CompileAndRun("CLI\nSEI\nPHP");
        Assert.IsTrue(I);
        
        CompileAndRun("SEI\nPHP");
        Assert.IsTrue(I);
    }
    
    [Test]
    public void TestCLV()
    {
        CompileAndRun("LDA #$5\nADC #$70\nADC #$10\nPHP");
        Assert.IsTrue(V);
        
        CompileAndRun("LDA #$5\nADC #$70\nADC #$10\nCLV\nPHP");
        Assert.IsFalse(V);
    }
}