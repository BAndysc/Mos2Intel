using Mos6502;

namespace Mos6502Disassembler;

public class MosDisassembler
{
    private static readonly MosOpcode[] OpcodesGroup01 = {
        MosOpcode.ORA, MosOpcode.AND, MosOpcode.EOR, MosOpcode.ADC, MosOpcode.STA, MosOpcode.LDA, MosOpcode.CMP, MosOpcode.SBC
    };
    private static readonly MosAddressingMode[] AddressingModesGroup01 = new MosAddressingMode[8]
    {
        MosAddressingMode.IndirectX,
        MosAddressingMode.ZeroPage,
        MosAddressingMode.Immediate,
        MosAddressingMode.Absolute,
        MosAddressingMode.IndirectY,
        MosAddressingMode.ZeroPageX,
        MosAddressingMode.AbsoluteY,
        MosAddressingMode.AbsoluteX
    };
    
    private static readonly MosOpcode[] OpcodesGroup10 = {
        MosOpcode.ASL, MosOpcode.ROL, MosOpcode.LSR, MosOpcode.ROR, MosOpcode.STX, MosOpcode.LDX, MosOpcode.DEC, MosOpcode.INC
    };
    private static readonly MosAddressingMode[] AddressingModesGroup10 = new MosAddressingMode[8]
    {
        MosAddressingMode.Immediate, MosAddressingMode.ZeroPage, MosAddressingMode.Accumulator, MosAddressingMode.Absolute, MosAddressingMode.Illegal, MosAddressingMode.ZeroPageX, MosAddressingMode.Illegal, MosAddressingMode.AbsoluteX
    };
    
    private static readonly MosOpcode[] OpcodesGroup00 = {
        0, MosOpcode.BIT, MosOpcode.JMP, MosOpcode.JMP_INDIRECT, MosOpcode.STY, MosOpcode.LDY, MosOpcode.CPY, MosOpcode.CPX
    };
    private static readonly MosAddressingMode[] AddressingModesGroup00 = new MosAddressingMode[8]
    {
        MosAddressingMode.Immediate, MosAddressingMode.ZeroPage, MosAddressingMode.Illegal, MosAddressingMode.Absolute, MosAddressingMode.Illegal, MosAddressingMode.ZeroPageX, MosAddressingMode.Illegal, MosAddressingMode.AbsoluteX
    };

    private static Dictionary<byte, (MosOpcode, MosAddressingMode)> Exceptions = new()
    {
        { 0x10, (MosOpcode.BPL, MosAddressingMode.Relative) },
        { 0x30, (MosOpcode.BMI, MosAddressingMode.Relative) },
        { 0x50, (MosOpcode.BVC, MosAddressingMode.Relative) },
        { 0x70, (MosOpcode.BVS, MosAddressingMode.Relative) },
        { 0x90, (MosOpcode.BCC, MosAddressingMode.Relative) },
        { 0xB0, (MosOpcode.BCS, MosAddressingMode.Relative) },
        { 0xD0, (MosOpcode.BNE, MosAddressingMode.Relative) },
        { 0xF0, (MosOpcode.BEQ, MosAddressingMode.Relative) },

        { 0x0,  (MosOpcode.BRK, MosAddressingMode.Implied) },
        { 0x20, (MosOpcode.JSR, MosAddressingMode.Indirect) },
        { 0x40, (MosOpcode.RTI, MosAddressingMode.Implied) },
        { 0x60, (MosOpcode.RTS, MosAddressingMode.Implied) },

        { 0x08, (MosOpcode.PHP, MosAddressingMode.Implied) },
        { 0x28, (MosOpcode.PLP, MosAddressingMode.Implied) },
        { 0x48, (MosOpcode.PHA, MosAddressingMode.Implied) },
        { 0x68, (MosOpcode.PLA, MosAddressingMode.Implied) },
        { 0x88, (MosOpcode.DEY, MosAddressingMode.Implied) },
        { 0xA8, (MosOpcode.TAY, MosAddressingMode.Implied) },
        { 0xC8, (MosOpcode.INY, MosAddressingMode.Implied) },
        { 0xE8, (MosOpcode.INX, MosAddressingMode.Implied) },
        { 0x18, (MosOpcode.CLC, MosAddressingMode.Implied) },
        { 0x38, (MosOpcode.SEC, MosAddressingMode.Implied) },
        { 0x58, (MosOpcode.CLI, MosAddressingMode.Implied) },
        { 0x78, (MosOpcode.SEI, MosAddressingMode.Implied) },
        { 0x98, (MosOpcode.TYA, MosAddressingMode.Implied) },
        { 0xB8, (MosOpcode.CLV, MosAddressingMode.Implied) },
        { 0xD8, (MosOpcode.CLD, MosAddressingMode.Implied) },
        { 0xF8, (MosOpcode.SED, MosAddressingMode.Implied) },
        { 0x8A, (MosOpcode.TXA, MosAddressingMode.Implied) },
        { 0x9A, (MosOpcode.TXS, MosAddressingMode.Implied) },
        { 0xAA, (MosOpcode.TAX, MosAddressingMode.Implied) },
        { 0xBA, (MosOpcode.TSX, MosAddressingMode.Implied) },
        { 0xCA, (MosOpcode.DEX, MosAddressingMode.Implied) },
        { 0xEA, (MosOpcode.NOP, MosAddressingMode.Implied) },
        
        { 0x02, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x12, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x22, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x32, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x42, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x52, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x62, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x72, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0x92, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0xB2, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0xD2, (MosOpcode.HLT, MosAddressingMode.Implied) },
        { 0xF2, (MosOpcode.HLT, MosAddressingMode.Implied) }
    };

    public static MosInstruction DecodeInstruction(ReadOnlySpan<byte> memory, out byte bytesRead)
    {
        bytesRead = 0;
        if (memory.Length <= 0)
            throw new IndexOutOfRangeException("Memory less than 1 bytes length");

        byte b = memory[bytesRead++];
        byte opcodePart = (byte)(b >> 5);
        byte groupPart = (byte)(b & 0b11);
        byte addressingPart = (byte)((b >> 2) & 0b111);

        MosOpcode opcode = MosOpcode.Illegal;
        MosAddressingMode addressingMode = MosAddressingMode.Illegal;

        if (Exceptions.TryGetValue(b, out var pair))
        {
            opcode = pair.Item1;
            addressingMode = pair.Item2;
            if (addressingMode == MosAddressingMode.Implied)
                return new MosInstruction(opcode, addressingMode);
        }
        else if (groupPart == 0b01)
        {
            opcode = OpcodesGroup01[opcodePart];
            addressingMode = AddressingModesGroup01[addressingPart];
            if (opcode == MosOpcode.STA && addressingMode == MosAddressingMode.Immediate)
                throw new IllegalInstructionException("Immediate STA is not legal MOS instruction");
        }
        else if (groupPart == 0b10)
        {
            opcode = OpcodesGroup10[opcodePart];
            addressingMode = AddressingModesGroup10[addressingPart];
            if (opcode is MosOpcode.STX or MosOpcode.LDX && addressingMode == MosAddressingMode.ZeroPageX)
                addressingMode = MosAddressingMode.ZeroPageY;
            if (opcode is MosOpcode.LDX && addressingMode == MosAddressingMode.AbsoluteX)
                addressingMode = MosAddressingMode.AbsoluteY;
            if (addressingMode == MosAddressingMode.Immediate && opcode != MosOpcode.LDX)
                throw new IllegalInstructionException($"Opcode {opcode} doesn't support immediate mode");
            if (addressingMode is MosAddressingMode.Accumulator && opcode is MosOpcode.STX or MosOpcode.LDX or MosOpcode.DEC or MosOpcode.INC)
                throw new IllegalInstructionException($"Opcode {opcode} doesn't support absolute mode");
            if (opcode is MosOpcode.STX && addressingMode == MosAddressingMode.AbsoluteX)
                throw new IllegalInstructionException($"Opcode {opcode} doesn't support absolute, X mode");
        }
        else if (groupPart == 0b00)
        {
            opcode = OpcodesGroup00[opcodePart];
            addressingMode = AddressingModesGroup00[addressingPart];
            switch (addressingMode, opcode)
            {
                case (MosAddressingMode.Immediate, MosOpcode.BIT):
                case (MosAddressingMode.Immediate, MosOpcode.JMP):
                case (MosAddressingMode.Immediate, MosOpcode.JMP_INDIRECT):
                case (MosAddressingMode.Immediate, MosOpcode.STY):
                    
                case (MosAddressingMode.ZeroPage, MosOpcode.JMP):
                case (MosAddressingMode.ZeroPage, MosOpcode.JMP_INDIRECT):

                case (MosAddressingMode.ZeroPageX, MosOpcode.BIT):
                case (MosAddressingMode.ZeroPageX, MosOpcode.JMP):
                case (MosAddressingMode.ZeroPageX, MosOpcode.JMP_INDIRECT):
                case (MosAddressingMode.ZeroPageX, MosOpcode.CPX):
                case (MosAddressingMode.ZeroPageX, MosOpcode.CPY):
                    throw new IllegalInstructionException($"Opcode {opcode} doesn't support {addressingMode}");
                    
                case (MosAddressingMode.AbsoluteX, _):
                    if (opcode != MosOpcode.LDY)
                        throw new IllegalInstructionException($"Opcode {opcode} doesn't support {addressingMode}");
                    break;
            }
        }
        
        if (opcode == MosOpcode.Illegal || addressingMode == MosAddressingMode.Illegal)
            throw new IllegalInstructionException($"Byte 0x{b:X} is ńot a valid opcode ({opcode}, {addressingMode})");

        if (opcode == MosOpcode.JMP_INDIRECT)
        {
            opcode = MosOpcode.JMP;
            addressingMode = MosAddressingMode.Indirect;
        }
        
        ushort operand = 0;

        if (addressingMode is MosAddressingMode.Immediate or
            MosAddressingMode.Relative or
            MosAddressingMode.ZeroPage or
            MosAddressingMode.ZeroPageX or
            MosAddressingMode.ZeroPageY or
            MosAddressingMode.IndirectX or
            MosAddressingMode.IndirectY)
        {
            if (memory.Length < 2)
                throw new IndexOutOfRangeException();
            operand = memory[bytesRead++];
        }
        else if (addressingMode is MosAddressingMode.Absolute or
                 MosAddressingMode.AbsoluteX or
                 MosAddressingMode.AbsoluteY or
                 MosAddressingMode.Indirect)
        {
            if (memory.Length < 3)
                throw new IndexOutOfRangeException();
            var lowByte = memory[bytesRead++];
            var highByte = memory[bytesRead++];
            operand = (ushort)(lowByte | highByte << 8);
        }
        
        return new MosInstruction(opcode, addressingMode, operand);
    }
}