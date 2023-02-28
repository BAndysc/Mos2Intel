namespace Mos6502Emulator.Test;

public class TestsBranches : EmulatorTestBase
{
    [Test]
    public void TestBCCOnClear()
    {
        UseProgram("BCC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBCCOnSet()
    {
        cpu.C = true;
        UseProgram("BCC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBCSOnClear()
    {
        UseProgram("BCS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBCSOnSet()
    {
        cpu.C = true;
        UseProgram("BCS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBEQOnClear()
    {
        UseProgram("BEQ lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBEQOnSet()
    {
        cpu.Z = true;
        UseProgram("BEQ lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBNEOnClear()
    {
        UseProgram("BNE lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBNEOnSet()
    {
        cpu.Z = true;
        UseProgram("BNE lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBMIOnClear()
    {
        UseProgram("BMI lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBMIOnSet()
    {
        cpu.N = true;
        UseProgram("BMI lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBPLOnClear()
    {
        UseProgram("BPL lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBPLOnSet()
    {
        cpu.N = true;
        UseProgram("BPL lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBVCOnClear()
    {
        UseProgram("BVC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
    [Test]
    public void TestBVCOnSet()
    {
        cpu.V = true;
        UseProgram("BVC lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBVSOnClear()
    {
        UseProgram("BVS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void TestBVSOnSet()
    {
        cpu.V = true;
        UseProgram("BVS lbl\nLDA #1\nJMP end\nlbl:\nLDA #2\nend:\nNOP\nNOP");
        machine.Step(3);
        Assert.AreEqual(2, cpu.A);
    }
    
}