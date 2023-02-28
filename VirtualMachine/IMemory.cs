namespace VirtualMachine;

public interface IMemory
{
    int Size { get; }
    byte this[int offset] { get; set; }
    ushort Get16(int offset);
}