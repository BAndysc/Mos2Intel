// ReSharper disable InconsistentNaming
namespace Mos6502;

public enum MosOpcode
{
    Illegal,
    
    ADC,
    AND,
    ASL,
    BCC,
    BCS,
    BEQ,
    BIT,
    BMI,
    BNE,
    BPL,
    BRK,
    BVC,
    BVS,
    CLC,
    CLD,
    CLI,
    CLV,
    CMP,
    CPX,
    CPY,
    DEC,
    DEX,
    DEY,
    EOR,
    INC,
    INX,
    INY,
    JMP,
    JMP_INDIRECT,
    JSR,
    LDA,
    LDX,
    LDY,
    LSR,
    NOP,
    ORA,
    PHA,
    PHP,
    PLA,
    PLP,
    ROL,
    ROR,
    RTI,
    RTS,
    SBC,
    SEC,
    SED,
    SEI,
    STA,
    STX,
    STY,
    TAX,
    TAY,
    TSX,
    TXA,	
    TXS,
    TYA,
    HLT
}

public static class OpcodeExtensions
{
    
    public static bool IsBranchOpcode(this MosOpcode opcode)
    {
        switch (opcode)
        {
            case MosOpcode.BPL:
            case MosOpcode.BMI:
            case MosOpcode.BVC:
            case MosOpcode.BVS:
            case MosOpcode.BCC:
            case MosOpcode.BCS:
            case MosOpcode.BNE:
            case MosOpcode.BEQ:
                return true;
        }

        return false;
    }
}