using Mos6502;

namespace Mos6502Assembler;

public static class Encoder
{
    private static readonly Dictionary<(MosOpcode, MosAddressingMode), byte> Opcodes = new()
    {
        { (MosOpcode.BRK, MosAddressingMode.Implied), 0x00 },
        { (MosOpcode.ORA, MosAddressingMode.IndirectX), 0x01 },
        { (MosOpcode.ORA, MosAddressingMode.ZeroPage), 0x05 },
        { (MosOpcode.ASL, MosAddressingMode.ZeroPage), 0x06 },
        { (MosOpcode.PHP, MosAddressingMode.Implied), 0x08 },
        { (MosOpcode.ORA, MosAddressingMode.Immediate), 0x09 },
        { (MosOpcode.ASL, MosAddressingMode.Accumulator), 0x0a },
        { (MosOpcode.ORA, MosAddressingMode.Absolute), 0x0d },
        { (MosOpcode.ASL, MosAddressingMode.Absolute), 0x0e },
        { (MosOpcode.BPL, MosAddressingMode.Relative), 0x10 },
        { (MosOpcode.ORA, MosAddressingMode.IndirectY), 0x11 },
        { (MosOpcode.ORA, MosAddressingMode.ZeroPageX), 0x15 },
        { (MosOpcode.ASL, MosAddressingMode.ZeroPageX), 0x16 },
        { (MosOpcode.CLC, MosAddressingMode.Implied), 0x18 },
        { (MosOpcode.ORA, MosAddressingMode.AbsoluteY), 0x19 },
        { (MosOpcode.ORA, MosAddressingMode.AbsoluteX), 0x1d },
        { (MosOpcode.ASL, MosAddressingMode.AbsoluteX), 0x1e },
        { (MosOpcode.JSR, MosAddressingMode.Absolute), 0x20 },
        { (MosOpcode.AND, MosAddressingMode.IndirectX), 0x21 },
        { (MosOpcode.BIT, MosAddressingMode.ZeroPage), 0x24 },
        { (MosOpcode.AND, MosAddressingMode.ZeroPage), 0x25 },
        { (MosOpcode.ROL, MosAddressingMode.ZeroPage), 0x26 },
        { (MosOpcode.PLP, MosAddressingMode.Implied), 0x28 },
        { (MosOpcode.AND, MosAddressingMode.Immediate), 0x29 },
        { (MosOpcode.ROL, MosAddressingMode.Accumulator), 0x2a },
        { (MosOpcode.BIT, MosAddressingMode.Absolute), 0x2c },
        { (MosOpcode.AND, MosAddressingMode.Absolute), 0x2d },
        { (MosOpcode.ROL, MosAddressingMode.Absolute), 0x2e },
        { (MosOpcode.BMI, MosAddressingMode.Relative), 0x30 },
        { (MosOpcode.AND, MosAddressingMode.IndirectY), 0x31 },
        { (MosOpcode.AND, MosAddressingMode.ZeroPageX), 0x35 },
        { (MosOpcode.ROL, MosAddressingMode.ZeroPageX), 0x36 },
        { (MosOpcode.SEC, MosAddressingMode.Implied), 0x38 },
        { (MosOpcode.AND, MosAddressingMode.AbsoluteY), 0x39 },
        { (MosOpcode.AND, MosAddressingMode.AbsoluteX), 0x3d },
        { (MosOpcode.ROL, MosAddressingMode.AbsoluteX), 0x3e },
        { (MosOpcode.RTI, MosAddressingMode.Implied), 0x40 },
        { (MosOpcode.EOR, MosAddressingMode.IndirectX), 0x41 },
        { (MosOpcode.EOR, MosAddressingMode.ZeroPage), 0x45 },
        { (MosOpcode.LSR, MosAddressingMode.ZeroPage), 0x46 },
        { (MosOpcode.PHA, MosAddressingMode.Implied), 0x48 },
        { (MosOpcode.EOR, MosAddressingMode.Immediate), 0x49 },
        { (MosOpcode.LSR, MosAddressingMode.Accumulator), 0x4a },
        { (MosOpcode.JMP, MosAddressingMode.Absolute), 0x4c },
        { (MosOpcode.EOR, MosAddressingMode.Absolute), 0x4d },
        { (MosOpcode.LSR, MosAddressingMode.Absolute), 0x4e },
        { (MosOpcode.BVC, MosAddressingMode.Relative), 0x50 },
        { (MosOpcode.EOR, MosAddressingMode.IndirectY), 0x51 },
        { (MosOpcode.EOR, MosAddressingMode.ZeroPageX), 0x55 },
        { (MosOpcode.LSR, MosAddressingMode.ZeroPageX), 0x56 },
        { (MosOpcode.CLI, MosAddressingMode.Implied), 0x58 },
        { (MosOpcode.EOR, MosAddressingMode.AbsoluteY), 0x59 },
        { (MosOpcode.EOR, MosAddressingMode.AbsoluteX), 0x5d },
        { (MosOpcode.LSR, MosAddressingMode.AbsoluteX), 0x5e },
        { (MosOpcode.RTS, MosAddressingMode.Implied), 0x60 },
        { (MosOpcode.ADC, MosAddressingMode.IndirectX), 0x61 },
        { (MosOpcode.ADC, MosAddressingMode.ZeroPage), 0x65 },
        { (MosOpcode.ROR, MosAddressingMode.ZeroPage), 0x66 },
        { (MosOpcode.PLA, MosAddressingMode.Implied), 0x68 },
        { (MosOpcode.ADC, MosAddressingMode.Immediate), 0x69 },
        { (MosOpcode.ROR, MosAddressingMode.Accumulator), 0x6a },
        { (MosOpcode.JMP, MosAddressingMode.Indirect), 0x6c },
        { (MosOpcode.ADC, MosAddressingMode.Absolute), 0x6d },
        { (MosOpcode.ROR, MosAddressingMode.Absolute), 0x6e },
        { (MosOpcode.BVS, MosAddressingMode.Relative), 0x70 },
        { (MosOpcode.ADC, MosAddressingMode.IndirectY), 0x71 },
        { (MosOpcode.ADC, MosAddressingMode.ZeroPageX), 0x75 },
        { (MosOpcode.ROR, MosAddressingMode.ZeroPageX), 0x76 },
        { (MosOpcode.SEI, MosAddressingMode.Implied), 0x78 },
        { (MosOpcode.ADC, MosAddressingMode.AbsoluteY), 0x79 },
        { (MosOpcode.ADC, MosAddressingMode.AbsoluteX), 0x7d },
        { (MosOpcode.ROR, MosAddressingMode.AbsoluteX), 0x7e },
        { (MosOpcode.STA, MosAddressingMode.IndirectX), 0x81 },
        { (MosOpcode.STY, MosAddressingMode.ZeroPage), 0x84 },
        { (MosOpcode.STA, MosAddressingMode.ZeroPage), 0x85 },
        { (MosOpcode.STX, MosAddressingMode.ZeroPage), 0x86 },
        { (MosOpcode.DEY, MosAddressingMode.Implied), 0x88 },
        { (MosOpcode.TXA, MosAddressingMode.Implied), 0x8a },
        { (MosOpcode.STY, MosAddressingMode.Absolute), 0x8c },
        { (MosOpcode.STA, MosAddressingMode.Absolute), 0x8d },
        { (MosOpcode.STX, MosAddressingMode.Absolute), 0x8e },
        { (MosOpcode.BCC, MosAddressingMode.Relative), 0x90 },
        { (MosOpcode.STA, MosAddressingMode.IndirectY), 0x91 },
        { (MosOpcode.STY, MosAddressingMode.ZeroPageX), 0x94 },
        { (MosOpcode.STA, MosAddressingMode.ZeroPageX), 0x95 },
        { (MosOpcode.STX, MosAddressingMode.ZeroPageY), 0x96 },
        { (MosOpcode.TYA, MosAddressingMode.Implied), 0x98 },
        { (MosOpcode.STA, MosAddressingMode.AbsoluteY), 0x99 },
        { (MosOpcode.TXS, MosAddressingMode.Implied), 0x9a },
        { (MosOpcode.STA, MosAddressingMode.AbsoluteX), 0x9d },
        { (MosOpcode.LDY, MosAddressingMode.Immediate), 0xa0 },
        { (MosOpcode.LDA, MosAddressingMode.IndirectX), 0xa1 },
        { (MosOpcode.LDX, MosAddressingMode.Immediate), 0xa2 },
        { (MosOpcode.LDY, MosAddressingMode.ZeroPage), 0xa4 },
        { (MosOpcode.LDA, MosAddressingMode.ZeroPage), 0xa5 },
        { (MosOpcode.LDX, MosAddressingMode.ZeroPage), 0xa6 },
        { (MosOpcode.TAY, MosAddressingMode.Implied), 0xa8 },
        { (MosOpcode.LDA, MosAddressingMode.Immediate), 0xa9 },
        { (MosOpcode.TAX, MosAddressingMode.Implied), 0xaa },
        { (MosOpcode.LDY, MosAddressingMode.Absolute), 0xac },
        { (MosOpcode.LDA, MosAddressingMode.Absolute), 0xad },
        { (MosOpcode.LDX, MosAddressingMode.Absolute), 0xae },
        { (MosOpcode.BCS, MosAddressingMode.Relative), 0xb0 },
        { (MosOpcode.LDA, MosAddressingMode.IndirectY), 0xb1 },
        { (MosOpcode.LDY, MosAddressingMode.ZeroPageX), 0xb4 },
        { (MosOpcode.LDA, MosAddressingMode.ZeroPageX), 0xb5 },
        { (MosOpcode.LDX, MosAddressingMode.ZeroPageY), 0xb6 },
        { (MosOpcode.CLV, MosAddressingMode.Implied), 0xb8 },
        { (MosOpcode.LDA, MosAddressingMode.AbsoluteY), 0xb9 },
        { (MosOpcode.TSX, MosAddressingMode.Implied), 0xba },
        { (MosOpcode.LDY, MosAddressingMode.AbsoluteX), 0xbc },
        { (MosOpcode.LDA, MosAddressingMode.AbsoluteX), 0xbd },
        { (MosOpcode.LDX, MosAddressingMode.AbsoluteY), 0xbe },
        { (MosOpcode.CPY, MosAddressingMode.Immediate), 0xc0 },
        { (MosOpcode.CMP, MosAddressingMode.IndirectX), 0xc1 },
        { (MosOpcode.CPY, MosAddressingMode.ZeroPage), 0xc4 },
        { (MosOpcode.CMP, MosAddressingMode.ZeroPage), 0xc5 },
        { (MosOpcode.DEC, MosAddressingMode.ZeroPage), 0xc6 },
        { (MosOpcode.INY, MosAddressingMode.Implied), 0xc8 },
        { (MosOpcode.CMP, MosAddressingMode.Immediate), 0xc9 },
        { (MosOpcode.DEX, MosAddressingMode.Implied), 0xca },
        { (MosOpcode.CPY, MosAddressingMode.Absolute), 0xcc },
        { (MosOpcode.CMP, MosAddressingMode.Absolute), 0xcd },
        { (MosOpcode.DEC, MosAddressingMode.Absolute), 0xce },
        { (MosOpcode.BNE, MosAddressingMode.Relative), 0xd0 },
        { (MosOpcode.CMP, MosAddressingMode.IndirectY), 0xd1 },
        { (MosOpcode.CMP, MosAddressingMode.ZeroPageX), 0xd5 },
        { (MosOpcode.DEC, MosAddressingMode.ZeroPageX), 0xd6 },
        { (MosOpcode.CLD, MosAddressingMode.Implied), 0xd8 },
        { (MosOpcode.CMP, MosAddressingMode.AbsoluteY), 0xd9 },
        { (MosOpcode.CMP, MosAddressingMode.AbsoluteX), 0xdd },
        { (MosOpcode.DEC, MosAddressingMode.AbsoluteX), 0xde },
        { (MosOpcode.CPX, MosAddressingMode.Immediate), 0xe0 },
        { (MosOpcode.SBC, MosAddressingMode.IndirectX), 0xe1 },
        { (MosOpcode.CPX, MosAddressingMode.ZeroPage), 0xe4 },
        { (MosOpcode.SBC, MosAddressingMode.ZeroPage), 0xe5 },
        { (MosOpcode.INC, MosAddressingMode.ZeroPage), 0xe6 },
        { (MosOpcode.INX, MosAddressingMode.Implied), 0xe8 },
        { (MosOpcode.SBC, MosAddressingMode.Immediate), 0xe9 },
        { (MosOpcode.NOP, MosAddressingMode.Implied), 0xea },
        { (MosOpcode.CPX, MosAddressingMode.Absolute), 0xec },
        { (MosOpcode.SBC, MosAddressingMode.Absolute), 0xed },
        { (MosOpcode.INC, MosAddressingMode.Absolute), 0xee },
        { (MosOpcode.BEQ, MosAddressingMode.Relative), 0xf0 },
        { (MosOpcode.SBC, MosAddressingMode.IndirectY), 0xf1 },
        { (MosOpcode.SBC, MosAddressingMode.ZeroPageX), 0xf5 },
        { (MosOpcode.INC, MosAddressingMode.ZeroPageX), 0xf6 },
        { (MosOpcode.SED, MosAddressingMode.Implied), 0xf8 },
        { (MosOpcode.SBC, MosAddressingMode.AbsoluteY), 0xf9 },
        { (MosOpcode.SBC, MosAddressingMode.AbsoluteX), 0xfd },
        { (MosOpcode.INC, MosAddressingMode.AbsoluteX), 0xfe },
        { (MosOpcode.HLT, MosAddressingMode.Implied), 0x02 },
    };

    public static int CountBytes(this MosInstruction instr)
    {
        return instr.AddressingMode.OperandLength() + 1;
    }

    public static void Encode(this MosInstruction instr, Span<byte> bytes)
    {
        var size = instr.CountBytes();
        if (bytes.Length < size)
            throw new IndexOutOfRangeException();
        if (!Opcodes.TryGetValue((instr.Opcode, instr.AddressingMode), out var b))
            throw new IllegalInstructionException($"({instr.Opcode}, {instr.AddressingMode}) is not a valid combination");
        
        bytes[0] = b;
        
        switch (instr.AddressingMode)
        {
            case MosAddressingMode.Absolute:
            case MosAddressingMode.AbsoluteX:
            case MosAddressingMode.AbsoluteY:
            case MosAddressingMode.Indirect:
                bytes[1] = (byte)(instr.Operand & 0xFF); // low
                bytes[2] = (byte)(instr.Operand >> 8); // high 
                break;
            case MosAddressingMode.IndirectX:
            case MosAddressingMode.ZeroPage:
            case MosAddressingMode.Relative:
            case MosAddressingMode.IndirectY:
            case MosAddressingMode.ZeroPageX:
            case MosAddressingMode.ZeroPageY:
            case MosAddressingMode.Immediate:
                if (instr.Operand > 0xFF)
                    throw new IllegalInstructionException("Operand must be a one byte value");
                bytes[1] = (byte)instr.Operand;
                break;
        }
    }

    public static byte[] Encode(this MosInstruction instr)
    {
        var count = instr.CountBytes();
        var bytes = new byte[count];
        instr.Encode(bytes);
        return bytes;
    }

    public static bool IsOnlyTwoBytesOperandSupported(this MosOpcode opcode)
    {
        switch (opcode)
        {
            case MosOpcode.JMP:
            case MosOpcode.JSR:
                return true;
        }

        return false;
    }

    public static bool IsImplied(this MosOpcode opcode)
    {
        switch (opcode)
        {
            case MosOpcode.BRK:
            case MosOpcode.PHP:
            case MosOpcode.CLC:
            case MosOpcode.PLP:
            case MosOpcode.SEC:
            case MosOpcode.RTI:
            case MosOpcode.PHA:
            case MosOpcode.CLI:
            case MosOpcode.RTS:
            case MosOpcode.PLA:
            case MosOpcode.SEI:
            case MosOpcode.DEY:
            case MosOpcode.TXA:
            case MosOpcode.TYA:
            case MosOpcode.TXS:
            case MosOpcode.TAY:
            case MosOpcode.TAX:
            case MosOpcode.CLV:
            case MosOpcode.TSX:
            case MosOpcode.INY:
            case MosOpcode.DEX:
            case MosOpcode.CLD:
            case MosOpcode.INX:
            case MosOpcode.NOP:
            case MosOpcode.SED:
            case MosOpcode.HLT:
                return true;
        }

        return false;
    }

    public static int OperandLength(this MosAddressingMode addressingMode)
    {
        switch (addressingMode)
        {
            case MosAddressingMode.Absolute:
            case MosAddressingMode.AbsoluteX:
            case MosAddressingMode.AbsoluteY:
            case MosAddressingMode.Indirect:
                return 2;
            case MosAddressingMode.IndirectX:
            case MosAddressingMode.ZeroPage:
            case MosAddressingMode.Relative:
            case MosAddressingMode.IndirectY:
            case MosAddressingMode.ZeroPageX:
            case MosAddressingMode.ZeroPageY:
            case MosAddressingMode.Immediate:
                return 1;
        }

        return 0;
    }
}