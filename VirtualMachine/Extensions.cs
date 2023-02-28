namespace VirtualMachine;

public static class Extensions
{
    public static void Fill(this IMemory memory, IEnumerable<byte> bytes, int offset = 0)
    {
        int i = 0;
        foreach (var b in bytes)
            memory[offset + i++] = b;
    }
    
    public static void FillSpan(this IMemory memory, ReadOnlySpan<byte> bytes, int offset = 0)
    {
        int i = 0;
        foreach (var b in bytes)
            memory[offset + i++] = b;
    }
}