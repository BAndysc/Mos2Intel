namespace Mos6502Emulator;

public class MosRuntimeException : Exception
{
    public MosRuntimeException(string message, Exception? inner = null) : base(message, inner) {}
}