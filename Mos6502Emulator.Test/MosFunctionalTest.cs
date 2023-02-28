using System;
using System.IO;
using VirtualMachine;

namespace Mos6502Emulator.Test;

public class MosFunctionalTest : EmulatorTestBase
{
    [Test]
    public void TestFunctional()
    {
        var src = File.ReadAllBytes("mos/6502_functional_test.bin");
        memory.Fill(src);
        cpu.IP = 0x400;
        var previousIp = cpu.IP;
        while (cpu.IP != 0x3469)
        {
            if (cpu.IP == 0x1b90)
                Console.WriteLine("saa");
            machine.Step(1);
            if (previousIp == cpu.IP && cpu.IP != 0x3469)
            {
                Assert.Fail($"Fail at address {cpu.IP:X}");
                break;
            }

            previousIp = cpu.IP;
        }
    }
}