namespace VirtualMachine;

public class Machine
{
    public readonly IMemory Memory;
    private readonly ICpu cpu;

    public Machine(IMemory memory, ICpu cpu)
    {
        this.Memory = memory;
        this.cpu = cpu;
    }

    public void Step(int cycles = 1)
    {
        for (int i = 0; i < cycles; ++i)
            cpu.Step(Memory);
    }
}