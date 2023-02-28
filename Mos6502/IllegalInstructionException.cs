namespace Mos6502;

public class IllegalInstructionException : Exception
{
    public IllegalInstructionException(string message) : base(message)
    {
        
    }
}