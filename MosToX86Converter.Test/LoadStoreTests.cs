
using Mos6502Testing;

namespace MosToX86Converter.Test;

public class LoadStoreTests : TestBase
{
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreZeroPage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDA #$64\nSTA $10\nHLT");
        Assert.AreEqual(0x64, memory[0x10]);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDA #$64\nSTA $1000\nHLT");
        Assert.AreEqual(0x64, memory[0x1000]);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreZeroPageX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nLDA #$1\nSTA $2, X\nHLT");
        Assert.AreEqual(1, memory[3]);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreAbsoluteX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nLDA #$1\nSTA $FF02, X\nHLT");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreAbsoluteY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #$1\nLDA #$1\nSTA $FF02, Y\nHLT");
        Assert.AreEqual(1, memory[0xFF03]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreIndirectX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDX #$1\nLDA #$1\nSTA ($20, X)\nHLT\norg $21\ndb $FE\ndb $FF", 0x600);
        Assert.AreEqual(1, memory[0xFFFE]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreIndirectY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDY #$1\nLDA #$1\nSTA ($20), Y\nHLT\norg $20\ndb $FD\ndb $FF", 0x600);
        Assert.AreEqual(1, memory[0xFFFE]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreXZeroPage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nSTX $2\nHLT");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreXZeroPageY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nLDY #$1\nSTX $2, Y\nHLT");
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreXAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nSTX $FF02\nHLT");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreYZeroPage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #$1\nSTY $2\nHLT");
        Assert.AreEqual(1, memory[2]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreYZeroPageX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$1\nLDY $1\nSTY $2, X\nHLT"); 
        Assert.AreEqual(1, memory[3]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestStoreYAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #$1\nSTY $FF02\nHLT");
        Assert.AreEqual(1, memory[0xFF02]);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestSelfModifyingProgram(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDA #2\nSTA $5\nLDA #$20\nHLT");
        Assert.AreEqual(0x2, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaImmediate(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDA #$20\nHLT");
        Assert.AreEqual(0x20, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaZeropage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDA $20\nHLT\norg $20\ndb $30", 0x600);
        Assert.AreEqual(0x30, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaZeropageX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDX #$FF\nLDA $20, X\nHLT\norg $1F\ndb $30", 0x600);
        Assert.AreEqual(0x30, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDA $FF20\nJMP end\norg $FF20\ndb $30\n db 0\nend:\nNOP\nHLT");
        Assert.AreEqual(0x30, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaAbsoluteX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #1\nLDA $FF20, X\njmp end\norg $FF21\ndb $EA\nend:\nNOP\nHLT");
        Assert.AreEqual(0xEA, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaAbsoluteY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #1\nLDA $FF20, Y\njmp end\norg $FF21\ndb $EA\nend:\nNOP\nHLT");
        Assert.AreEqual(0xEA, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaIndirectX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDX #$60\nLDA ($C0, X)\nHLT\norg $1\ndb 2\norg $20\ndb 1", 0x600);
        Assert.AreEqual(2, A);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaIndirectY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDY #$1\nLDA ($20), Y\nHLT\norg $1\ndb 2", 0x600);
        Assert.AreEqual(2, A);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdaFlags(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod).WithOptimizeFlags(false);
        CompileAndRun("LDA #$1\nHLT");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$7F\nHLT");
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$80\nHLT");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$FF\nHLT");
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$0\nHLT");
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdxImmediate(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #$20\nHLT");
        Assert.AreEqual(0x20, X);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdxZeropage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDX $20\nHLT\norg $20\ndb $30",0x600);
        Assert.AreEqual(0x30, X);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdxZeropageY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDY #$FF\nLDX $20, Y\nHLT\norg $1F\ndb $30", 0x600);
        Assert.AreEqual(0x30, X);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdxAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX $FF20\njmp end\norg $FF20\ndb $30\ndb $0\nend:\n nop\nHLT");
        Assert.AreEqual(0x30, X);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdxAbsoluteY(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #1\nLDX $FF20, Y\njmp end\norg $FF21\ndb $30\ndb $0\nend:\nnop\nHLT");
        Assert.AreEqual(0x30, X);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdyImmediate(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY #$20\nHLT");
        Assert.AreEqual(0x20, Y);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdyZeropage(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDY $20\nHLT\norg $20\ndb $30", 0x600);
        Assert.AreEqual(0x30, Y);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdyZeropageX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("org $600\nLDX #$FF\nLDY $20, X\nHLT\norg $1F\ndb $30",0x600);
        Assert.AreEqual(0x30, Y);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdyAbsolute(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDY $FF20\njmp end\norg $FF20\ndb $30\ndb $0\n end:\nnop\nHLT");
        Assert.AreEqual(0x30, Y);
    }
    
    [Test]
    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public void TestLdyAbsoluteX(bool dma, bool cycleMethod)
    {
        DefaultOptions = MosConverterOptions.Default.WithDirectMemoryAccess(dma).WithCycleMethod(cycleMethod);
        CompileAndRun("LDX #1\nLDY $FF20, X\njmp end\norg $FF21\ndb $30\ndb $0\nend:\n nop\nHLT");
        Assert.AreEqual(0x30, Y);
    }
}