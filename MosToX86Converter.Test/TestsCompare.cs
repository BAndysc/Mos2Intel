namespace MosToX86Converter.Test;

public class TestsCompare : TestBase
{
    [Test]
    public void TestEqual()
    {
        CompileAndRun("LDA #$20\nCMP #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestLesser()
    {
        CompileAndRun("LDA #$19\nCMP #$20\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
    }
    
    [Test]
    public void TestHigher()
    {
        CompileAndRun("LDA #$21\nCMP #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestEqualNegative()
    {
        CompileAndRun("LDA #$90\nCMP #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestLesserNegative()
    {
        CompileAndRun("LDA #$8F\nCMP #$90\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
    }
    
    [Test]
    public void TestHigherNegative()
    {
        CompileAndRun("LDA #$91\nCMP #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
    }

    [Test]
    public void TestHigherThanZero()
    {
        CompileAndRun("LDA #$80\nCMP #0\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestX()
    {
        CompileAndRun("LDX #$20\nCPX #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDX #$19\nCPX #$20\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        
        CompileAndRun("LDX #$21\nCPX #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDX #$90\nCPX #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDX #$8F\nCPX #$90\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        
        CompileAndRun("LDX #$91\nCPX #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDX #$80\nCPX #$7F\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
    }
    
    [Test]
    public void TestY()
    {
        CompileAndRun("LDY #$20\nCPY #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDY #$19\nCPY #$20\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        
        CompileAndRun("LDY #$21\nCPY #$20\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDY #$90\nCPY #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
        
        CompileAndRun("LDY #$8F\nCPY #$90\nPHP");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        
        CompileAndRun("LDY #$91\nCPY #$90\nPHP");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsTrue(C);
    }
}