using System.Linq;
using System.Text;
using Mos6502;

namespace MosToX86Converter.Test;

public class TestStack : TestBase
{
    [Test]
    public void TestPush()
    {
        CompileAndRun("LDA #1\nPHA\nLDA #2\nPHA");
        Assert.AreEqual(1, memory[511]);
        Assert.AreEqual(2, memory[510]);
    }
    
    [Test]
    public void TestPushStatusRegister()
    {
        CompileAndRun("PHP");
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused, (Mos6502SR)memory[511]);
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused, Flags);

        CompileAndRun("LDA #$80\nPHP");
        Assert.AreEqual(Mos6502SR.B | Mos6502SR.Unused | Mos6502SR.N, (Mos6502SR)memory[511]);
        Assert.AreEqual(Mos6502SR.N | Mos6502SR.B | Mos6502SR.Unused, Flags);
    }
    
    [Test]
    public void TestPushOverflow()
    {
        StringBuilder program = new();
        program.AppendLine("org $600");
        program.AppendLine("LDA #$1");
        program.AppendLine(string.Join("\n", Enumerable.Repeat("PHA", 256)));
        program.AppendLine("LDA #$2");
        program.AppendLine("PHA");
        CompileAndRun(program.ToString(), 0x600);

        Assert.AreEqual(1, memory[256]);
        Assert.AreEqual(2, memory[511]);
    }
    
    [Test]
    public void TestPull()
    {
        CompileAndRun("LDA #$1\nPHA\nLDA #$0\nPLA\nPHP");
        Assert.AreEqual(1, A);
        Assert.IsFalse(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$80\nPHA\nLDA #$0\nPLA\nPHP");
        Assert.AreEqual(0x80, A);
        Assert.IsTrue(N);
        Assert.IsFalse(Z);
        
        CompileAndRun("LDA #$0\nPHA\nLDA #$1\nPLA\nPHP");
        Assert.AreEqual(0, A);
        Assert.IsFalse(N);
        Assert.IsTrue(Z);
    }
    
    [Test]
    public void TestPullOverflow()
    {
        CompileAndRun("LDA #$1\nSTA $100\nLDA #0\nPLA");
        Assert.AreEqual(1, A);
    }
    
    [Test]
    public void TestPullSR()
    {
        byte flags = (byte)(Mos6502SR.C | Mos6502SR.B);
        CompileAndRun($"LDA #${flags:x}\nSTA $100\nSTA $101\nPLP");
        Assert.IsTrue((Flags & Mos6502SR.C) != 0);
        Assert.IsTrue((Flags & Mos6502SR.B) != 0);
    }
}