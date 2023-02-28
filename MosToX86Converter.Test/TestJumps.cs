using Mos6502Testing;

namespace MosToX86Converter.Test;

public class TestJumps : TestBase
{
    [Test]
    public void JumpAbsolute()
    {
        CompileAndRun("JMP $FF00\nJMP 3\norg $FF00\nLDA #1");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void JumpIndirect()
    {
        CompileAndRun("JMP ($FF00)\nJMP @\norg $201\nLDA #2\nJMP $FFFE\norg $FF00\ndb 1\ndb 2\norg $FFFE\nNOP");
        Assert.AreEqual(2, A);
    }
    
    [Test]
    public void Jsr()
    {
        CompileAndRun("org $600\nJSR $FF00\nINX\nJMP end\norg $FF00\nINX\nRTS\nend:\nNOP", 0x600);
        Assert.AreEqual(0x2, memory[0x1FE]);
        Assert.AreEqual(0x6, memory[0x1FE + 1]);
        Assert.AreEqual(2, X);
    }
    
    [Test]
    public void JsrRts()
    {
        CompileAndRun("JSR end\nLDA #1\nJMP realEnd\nend:\nRTS\nrealEnd:\nNOP");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void BrkRti()
    {
        CompileAndRun("LDA #0\nBRK\nJMP end\norg $10\n INX\nRTI\norg $1000\n db $10\n db 0\nend:\nNOP", 0, new MosConverterOptions()
        {
            BrkInterruptAddress = 0x1000
        });
        Assert.AreEqual(1, X);
        Assert.IsTrue(Z);
    }
}