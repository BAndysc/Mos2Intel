namespace VirtualMachine;

public interface ICpu
{
    int Step(IMemory memory);
}