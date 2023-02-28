namespace Mos6502Emulator.Test;

public class TestJumps : EmulatorTestBase
{
    [Test]
    public void JumpAbsolute()
    {
        Step("JMP $FF00");
        Assert.AreEqual(0xFF00, cpu.IP);
    }
    
    [Test]
    public void JumpIndirect()
    {
        memory[0xFF00] = 0x01;
        memory[0xFF01] = 0x02;
        Step("JMP ($FF00)");
        Assert.AreEqual(0x201, cpu.IP);
    }
    
    [Test]
    public void Jsr()
    {
        Step("JSR $FF00");
        Assert.AreEqual(0x602, memory.Get16(0x1FE));
        Assert.AreEqual(0xFF00, cpu.IP);
    }
    
    [Test]
    public void JsrRts()
    {
        Step("JSR end\nLDA #1\nend:\nRTS", 3);
        Assert.AreEqual(1, cpu.A);
    }
    
    [Test]
    public void BrkRti()
    {
        memory[0x5] = 0xA9; // LDA # 
        memory[0x6] = 0x1;  //      $1
        memory[0x7] = 0x40; // RTI
        memory[0xFFFE] = 0x5;
        cpu.Z = true;
        Step("BRK", 2);
        Assert.IsFalse(cpu.Z);
        machine.Step(1);
        Assert.AreEqual(1, cpu.A);
        Assert.IsTrue(cpu.Z);
    }
}