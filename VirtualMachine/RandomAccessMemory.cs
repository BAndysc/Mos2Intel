namespace VirtualMachine;

public class RandomAccessMemory : IMemory
{
    private byte[] ram;
    public int Size => ram.Length;

    public RandomAccessMemory(int maxSize)
    {
        ram = new byte[maxSize];
    }

    public byte this[int offset]
    {
        get => ram[offset];
        set => ram[offset] = value;
    }

    public ushort Get16(int offset)
    {
        return (ushort)(ram[offset] | (ram[(offset + 1)] << 8));
    }
}