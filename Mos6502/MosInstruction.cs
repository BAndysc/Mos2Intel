namespace Mos6502;

public readonly struct MosInstruction
{
    public readonly MosOpcode Opcode;
    public readonly MosAddressingMode AddressingMode;
    public readonly ushort Operand;

    public MosInstruction(MosOpcode opcode, MosAddressingMode addressingMode, ushort operand = 0)
    {
        Opcode = opcode;
        AddressingMode = addressingMode;
        Operand = operand;
    }

    public override string ToString()
    {
        switch (AddressingMode)
        {
            case MosAddressingMode.Absolute:
                return $"{Opcode} ${Operand:X4}";
            case MosAddressingMode.AbsoluteX:
                return $"{Opcode} ${Operand:X4}, X";
            case MosAddressingMode.AbsoluteY:
                return $"{Opcode} ${Operand:X4}, Y";
            case MosAddressingMode.Immediate:
                return $"{Opcode} #${Operand:X2}";
            case MosAddressingMode.Implied:
            case MosAddressingMode.Accumulator:
                return Opcode.ToString();
            case MosAddressingMode.Indirect:
                return $"{Opcode} (${Operand:X4})";
            case MosAddressingMode.IndirectX:
                return $"{Opcode} (${Operand:X2}, X)";
            case MosAddressingMode.IndirectY:
                return $"{Opcode} (${Operand:X2}), Y";
            case MosAddressingMode.Relative:
                return $"{Opcode} ${Operand:X2}";
            case MosAddressingMode.ZeroPage:
                return $"{Opcode} ${Operand:X2}";
            case MosAddressingMode.ZeroPageX:
                return $"{Opcode} ${Operand:X2}, X";
            case MosAddressingMode.ZeroPageY:
                return $"{Opcode} ${Operand:X2}, Y";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}