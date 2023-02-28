namespace X86Assembly;

public static class X86
{
    public static X86Instruction Lea(OperandX86 dest, OperandX86 src) => new(OpcodesX86.LEA, dest, src);
    public static X86Instruction Add(OperandX86 dest, OperandX86 src) => new(OpcodesX86.ADD, dest, src);
    public static X86Instruction Adc(OperandX86 dest, OperandX86 src) => new(OpcodesX86.ADC, dest, src);
    public static X86Instruction Sub(OperandX86 dest, OperandX86 src) => new(OpcodesX86.SUB, dest, src);
    public static X86Instruction Mov(OperandX86 dest, OperandX86 src) => new(OpcodesX86.MOV, dest, src);
    public static X86Instruction Or(OperandX86 dest, OperandX86 src) => new(OpcodesX86.OR, dest, src);
    public static X86Instruction Shl(OperandX86 dest, byte bits) => new(OpcodesX86.SHL, dest, bits);
    public static X86Instruction Shr(OperandX86 dest, byte bits) => new(OpcodesX86.SHR, dest, bits);
    public static X86Instruction Rcl(OperandX86 dest, byte bits) => new(OpcodesX86.RCL, dest, bits);
    public static X86Instruction Rcr(OperandX86 dest, byte bits) => new(OpcodesX86.RCR, dest, bits);
    public static X86Instruction And(OperandX86 dest, OperandX86 src) => new(OpcodesX86.AND, dest, src);
    public static X86Instruction Xor(OperandX86 dest, OperandX86 src) => new(OpcodesX86.XOR, dest, src);
    public static X86Instruction Test(OperandX86 dest, OperandX86 src) => new(OpcodesX86.TEST, dest, src);
    public static X86Instruction Cmp(OperandX86 dest, OperandX86 src) => new(OpcodesX86.CMP, dest, src);
    public static X86Instruction Dec(OperandX86 src) => new(OpcodesX86.DEC, src);
    public static X86Instruction Inc(OperandX86 src, RegisterWidth? widthHint = null) => new(OpcodesX86.INC, widthHint, src);
    public static X86Instruction Push(OperandX86 src) => new(OpcodesX86.PUSH, src);
    public static X86Instruction Pop(OperandX86 dest) => new(OpcodesX86.POP, dest);
    public static X86Instruction Not(OperandX86 dest) => new(OpcodesX86.NOT, dest);
    public static X86Instruction ConditionalSetLower(OperandX86 dest) => new(OpcodesX86.SETL, dest);
    public static X86Instruction ConditionalSetSigned(OperandX86 dest) => new(OpcodesX86.SETS, dest);
    public static X86Instruction ConditionalSetOverflow(OperandX86 dest) => new(OpcodesX86.SETO, dest);
    public static X86Instruction ConditionalSetZero(OperandX86 dest) => new(OpcodesX86.SETZ, dest);
    public static X86Instruction ConditionalSetNonZero(OperandX86 dest) => new(OpcodesX86.SETNZ, dest);
    public static X86Instruction ConditionalSetCarry(OperandX86 dest) => new(OpcodesX86.SETC, dest);
    public static X86Instruction JumpNonZero(string label) => new(OpcodesX86.JNZ, label);
    public static X86Instruction JumpZero(string label) => new(OpcodesX86.JZ, label);
    public static X86Instruction JumpSign(string label) => new(OpcodesX86.JS, label);
    public static X86Instruction JumpIfCarry(string label) => new(OpcodesX86.JC, label);
    public static X86Instruction JumpIfNoCarry(string label) => new(OpcodesX86.JNC, label);
    public static X86Instruction JumpLower(string label) => new(OpcodesX86.JL, label);
    public static X86Instruction JumpEqual(string label) => new(OpcodesX86.JE, label);
    public static X86Instruction JumpGreaterEqual(string label) => new(OpcodesX86.JGE, label);
    public static X86Instruction JumpGreaterEqualUnsigned(string label) => new(OpcodesX86.JAE, label);
    public static X86Instruction JumpLowerUnsigned(string label) => new(OpcodesX86.JB, label);
    public static X86Instruction JumpOverflow(string label) => new(OpcodesX86.JO, label);
    public static X86Instruction JumpNoOverflow(string label) => new(OpcodesX86.JNO, label);
    public static X86Instruction Jump(string label) => new(OpcodesX86.JMP, label);
    public static X86Instruction Call(string label) => new(OpcodesX86.CALL, label);
    public static X86Instruction Nop() => new(OpcodesX86.NOP);
    public static X86Instruction Ret() => new(OpcodesX86.RET);
    public static X86Instruction Leave() => new(OpcodesX86.LEAVE);
    public static X86Instruction PushEFlags() => new(OpcodesX86.PUSHF);
    public static X86Instruction PopEFlags() => new(OpcodesX86.POPF);
    public static X86Instruction SetCarry() => new(OpcodesX86.STC);
    public static X86Instruction ClearCarry() => new(OpcodesX86.CLC);

    
    public static X86Instruction Label(string label) => new(OpcodesX86.MetaLabel, label);

    
    public static OperandX86 Register(RegisterX86 r)
    {
        return new OperandX86(OperandType.Register, 0, r, false, 0, 0, null);
    }
    
    public static OperandX86 Value(ulong value)
    {
        return new OperandX86(OperandType.ImmediateValue, value, 0, false, 0, 0, null);
    }
    
    public static OperandX86 Indirect(RegisterX86 reg, uint displacement = 0, byte scale = 1, string? label = null)
    {
        return new OperandX86(OperandType.Indirect, 0, reg, true, scale, displacement, label);
    }
}

public struct X86Instruction
{
    public readonly OpcodesX86 Opcode;
    public readonly OperandX86? Operand1;
    public readonly OperandX86? Operand2;
    public readonly string? Label;
    public readonly RegisterWidth? WidthHint;

    public X86Instruction(OpcodesX86 opcode, string label)
    {
        Opcode = opcode;
        Label = label;
        Operand1 = null;
        Operand2 = null;
        WidthHint = null;
    }

    public X86Instruction(OpcodesX86 opcode, RegisterWidth? widthHint, OperandX86? operand1 = null, OperandX86? operand2 = null)
    {
        Opcode = opcode;
        WidthHint = widthHint;
        Operand1 = operand1;
        Operand2 = operand2;
        Label = null;
    }

    public X86Instruction(OpcodesX86 opcode, OperandX86? operand1 = null, OperandX86? operand2 = null)
    {
        Opcode = opcode;
        WidthHint = null;
        Operand1 = operand1;
        Operand2 = operand2;
        Label = null;
    }

    public override string ToString()
    {
        if (Opcode == OpcodesX86.MetaLabel)
            return $"{Label}:";
        if (Label != null)
            return $"{Opcode} {Label}";
        string hint = "";
        if (WidthHint.HasValue)
        {
            switch (WidthHint)
            {
                case RegisterWidth.QWord:
                    hint = "qword ";
                    break;
                case RegisterWidth.DWord:
                    hint = "dword ";
                    break;
                case RegisterWidth.Word:
                    hint = "word ";
                    break;
                case RegisterWidth.Byte:
                    hint = "byte ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        if (Operand2.HasValue)
            return $"{Opcode} {hint}{Operand1.ToString()}, {Operand2.ToString()}";
        if (Operand1.HasValue)
            return $"{Opcode} {hint}{Operand1.ToString()}";
        return $"{Opcode}";
    }
}

public enum OperandType
{
    Register,
    ImmediateValue,
    Indirect
}

public struct OperandX86
{
    public bool Equals(OperandX86 other)
    {
        return Type == other.Type && Val == other.Val && Reg == other.Reg && IsIndirect == other.IsIndirect && Scale == other.Scale && Displacement == other.Displacement && DisplacementLabel == other.DisplacementLabel;
    }

    public override bool Equals(object? obj)
    {
        return obj is OperandX86 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, Val, (int)Reg, IsIndirect, Scale, Displacement, DisplacementLabel);
    }

    public static bool operator ==(OperandX86 left, OperandX86 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OperandX86 left, OperandX86 right)
    {
        return !left.Equals(right);
    }

    public readonly OperandType Type;
    public readonly ulong Val;
    public readonly RegisterX86 Reg;
    public readonly bool IsIndirect;
    public readonly byte Scale;
    public readonly uint Displacement;
    public readonly string? DisplacementLabel;
    
    public static implicit operator OperandX86(RegisterX86 reg) => X86.Register(reg);
    public static implicit operator OperandX86(int d) => X86.Value((uint)d);
    
    public OperandX86(OperandType type, ulong value, RegisterX86 register, bool indirect, byte scale, uint displacement, string? displacementLabel)
    {
        Type = type;
        Val = value;
        Reg = register;
        IsIndirect = indirect;
        Scale = scale;
        Displacement = displacement;
        DisplacementLabel = displacementLabel;
    }

    public override string ToString()
    {
        switch (Type)
        {
            case OperandType.Register:
                return Reg.ToString();
            case OperandType.ImmediateValue:
                return "0x" + Val.ToString("x");
            case OperandType.Indirect:
                if (DisplacementLabel == null)
                {
                    if (Reg != 0)
                    {
                        if (Scale != 1)
                        {
                            if (Displacement != 0)
                                return $"[{Scale} * {Reg} + 0x{Displacement:x}]";
                            else
                                return $"[{Scale} * {Reg}]";
                        }
                        else
                        {
                            if (Displacement != 0)
                                return $"[{Reg} + 0x{Displacement:x}]";
                            else
                                return $"[{Reg}]";
                        }
                    }
                    else
                        return $"[0x{Displacement:x}]";
                }
                else
                {
                    if (Reg != 0)
                    {
                        if (Scale != 1)
                        {
                            if (Displacement != 0)
                                return $"[rel {DisplacementLabel} + {Scale} * {Reg} + 0x{Displacement:x}]";
                            else            
                                return $"[rel {DisplacementLabel} + {Scale} * {Reg}]";
                        }
                        else
                        {
                            if (Displacement != 0)
                                return $"[rel {DisplacementLabel} + {Reg} + 0x{Displacement:x}]";
                            else
                                return $"[rel {DisplacementLabel} + {Reg}]";
                        }
                    }
                    else
                    {
                        if (Displacement != 0)
                            return $"[rel {DisplacementLabel} + 0x{Displacement:x}]";
                        else
                            return $"[rel {DisplacementLabel}]";
                    }
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum RegisterX86
{
    RAX = 0X8 |  (1 << 4), EAX  = 0X4 |  (1 << 4),   AX = 0X2 |  (1 << 4),  AL  = 0X1 |  (1 << 4),
    RCX = 0X8 |  (2 << 4), ECX  = 0X4 |  (2 << 4),   CX = 0X2 |  (2 << 4),  CL  = 0X1 |  (2 << 4),
    RDX = 0X8 |  (3 << 4), EDX  = 0X4 |  (3 << 4),   DX = 0X2 |  (3 << 4),  DL  = 0X1 |  (3 << 4),
    RBX = 0X8 |  (4 << 4), EBX  = 0X4 |  (4 << 4),   BX = 0X2 |  (4 << 4),  BL  = 0X1 |  (4 << 4),
    RSI = 0X8 |  (5 << 4), ESI  = 0X4 |  (5 << 4),   SI = 0X2 |  (5 << 4),  SIL = 0X1 |  (5 << 4),
    RDI = 0X8 |  (6 << 4), EDI  = 0X4 |  (6 << 4),   DI = 0X2 |  (6 << 4),  DIL = 0X1 |  (6 << 4),
    RSP = 0X8 |  (7 << 4), ESP  = 0X4 |  (7 << 4),   SP = 0X2 |  (7 << 4),  SPL = 0X1 |  (7 << 4),
    RBP = 0X8 |  (8 << 4), EBP  = 0X4 |  (8 << 4),   BP = 0X2 |  (8 << 4),  BPL = 0X1 |  (8 << 4),
     R8 = 0X8 |  (9 << 4), R8D  = 0X4 |  (9 << 4),  R8W = 0X2 |  (9 << 4),  R8B = 0X1 |  (9 << 4),
     R9 = 0X8 | (10 << 4), R9D  = 0X4 | (10 << 4),  R9W = 0X2 | (10 << 4),  R9B = 0X1 | (10 << 4),
    R10 = 0X8 | (11 << 4), R10D = 0X4 | (11 << 4), R10W = 0X2 | (11 << 4), R10B = 0X1 | (11 << 4),
    R11 = 0X8 | (12 << 4), R11D = 0X4 | (12 << 4), R11W = 0X2 | (12 << 4), R11B = 0X1 | (12 << 4),
    R12 = 0X8 | (13 << 4), R12D = 0X4 | (13 << 4), R12W = 0X2 | (13 << 4), R12B = 0X1 | (13 << 4),
    R13 = 0X8 | (14 << 4), R13D = 0X4 | (14 << 4), R13W = 0X2 | (14 << 4), R13B = 0X1 | (14 << 4),
    R14 = 0X8 | (15 << 4), R14D = 0X4 | (15 << 4), R14W = 0X2 | (15 << 4), R14B = 0X1 | (15 << 4),
    R15 = 0X8 | (16 << 4), R15D = 0X4 | (16 << 4), R15W = 0X2 | (16 << 4), R15B = 0X1 | (16 << 4),
}

public enum RegisterWidth
{
    QWord = 8,
    DWord = 4,
    Word = 2,
    Byte = 1,
    
    _64 = 8,
    _32 = 4,
    _16 = 2,
    _8 = 1
}

public static class Extensions
{
    public static RegisterX86 Create(int id, RegisterWidth width)
    {
        return (RegisterX86)(id << 4 | (int)width);
    }
    
    public static RegisterWidth GetWidth(this RegisterX86 reg)
    {
        return (RegisterWidth)((int)reg & 0b1111);
    }

    public static int GetRegisterId(this RegisterX86 reg)
    {
        return (int)reg >> 4;
    }
    
    public static RegisterX86 WithWidth(this RegisterX86 reg, RegisterWidth width)
    {
        return Create(GetRegisterId(reg), width);
    }
    
    public static bool IsSame(this RegisterX86 a, RegisterX86 b)
    {
        return GetRegisterId(a) == GetRegisterId(b);
    }
}

public enum OpcodesX86
{
    MetaLabel,
    MOV,
    XOR,
    LEA,
    ADD,
    AND,
    RET,
    PUSH,
    POP,
    SHL,
    OR,
    TEST,
    JNZ,
    JMP,
    JS,
    LEAVE,
    DEC,
    INC,
    SHR,
    JNC,
    JC,
    NOP,
    CALL,
    JZ,
    CMP,
    JL,
    JE,
    SETL,
    JGE,
    SETZ,
    SETNZ,
    SUB,
    JAE,
    JB,
    ADC,
    POPF,
    PUSHF,
    STC,
    CLC,
    JO,
    JNO,
    SETC,
    SETS,
    SETO,
    NOT,
    RCL,
    RCR
}