using System;

namespace Mos6502Emulator.Test;

public class TestQuirks : EmulatorTestBase
{
    [Test]
    public void TestIndirectXRollover()
    {
        Step("LDA #4");
        Step("STA $0");
        Step("LDA #$80");
        Step("STA $0400");
        Step("LDX #0");
        Step("LDA ($FF,X)");
        Assert.AreEqual(0x80, cpu.A);
    }
    
    [Test]
    public void TestIndirectYRollover()
    {
        Step("LDA #4");
        Step("STA $0");
        Step("LDA #$80");
        Step("STA $0400");
        Step("LDY #0");
        Step("LDA ($FF), Y");
        Assert.AreEqual(0x80, cpu.A);
    }
    
    [Test]
    public void TestIndirectJumpWithFFLowByte()
    {
        UseProgram("HLT\nHLT\norg $600\nLDA #$BF\nSTA $0200\nLDA #1\nSTA $02FF\nJMP ($02FF)\norg $BF01\nLDA #$10\nHLT", 0);
        cpu.IP = 0x600;
        while (true)
        {
            try
            {
                machine.Step();
            }
            catch (Exception)
            {
                break;
            }
        }
        
        Assert.AreEqual(0x10, cpu.A);
    }
}