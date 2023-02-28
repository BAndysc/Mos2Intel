using Mos6502Testing;

namespace MosToX86Converter.Test;

public class TestsAddWithCarry : TestBase
{
    [Test]
    public void TestADCImmediate()
    {
        CompileAndRun("ADC #1\nPHP");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestADCImmediateWithCarry()
    {
        CompileAndRun("SEC\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCOverflow()
    {
        CompileAndRun("LDA #$FE\nADC #3");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestADCZeroPage()
    {
        CompileAndRun("ADC 1\nLDA #$2\nSTA 1\nLDA #0\nADC 1");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCZeroPageX()
    {
        CompileAndRun("org $600\nLDX #1\nADC 0,x", 0x600);
        Assert.AreEqual(0, A);
        
        CompileAndRun("org $600\nLDX #1\nADC 0,x\nLDA #2\nSTA 1\nLDA #0\nADC 0,x", 0x600);
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCZeroPageXOverflow()
    {
        CompileAndRun("LDA #2\nSTA $20\nLDA #0\nLDX #$60\nADC $C0,x");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCAbsolute()
    {
        CompileAndRun("LDA #2\nSTA $100\nLDA #0\nADC 256");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCAbsoluteX()
    {
        CompileAndRun("LDX #1\nLDA #2\nSTA $101\nLDA #0\nADC 256, X");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCAbsoluteY()
    {
        CompileAndRun("LDY #1\nLDA #2\nSTA $101\nLDA #0\nADC 256, Y");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCIndirectX()
    {
        CompileAndRun("org $600\nLDX #$60\nLDA #2\nSTA $1\nLDA #1\nSTA $20\nLDA #0\nADC ($C0, X)", 0x600);
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void TestADCIndirectY()
    {
        CompileAndRun("org $600\nLDY #1\nLDA #2\nSTA $1\nLDA #0\nADC ($20), Y", 0x600);
        Assert.AreEqual(2, A);
    }

    [Test]
    public void TestADCFlags()
    {
        CompileAndRun("ADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(1, A);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsFalse(V);
        
        CompileAndRun("LDA #$7E\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsFalse(V);
        
        CompileAndRun("LDA #$7F\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsTrue(V);
        
        CompileAndRun("LDA #$80\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(0x81, A);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsFalse(V);
        
        CompileAndRun("LDA #$81\nADC #$7E", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(0xFF, A);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsFalse(V);
        
        CompileAndRun("LDA #$FF\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
        Assert.IsTrue(C);
        Assert.IsFalse(V);

        CompileAndRun("SEC\nADC #1", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.AreEqual(2, A); // becuase carry
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsFalse(V);
        
        CompileAndRun("ADC #$70\nADC #$10", 0, MosConverterOptions.Default.WithOptimizeFlags(false));
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        Assert.IsFalse(C);
        Assert.IsTrue(V);
    }
}