namespace Mos6502;

[Flags]
public enum Mos6502SR : byte
{
    N = 0b10000000,
    V = 0b01000000,
    Unused = 0b00100000,
    B = 0b00010000,
    D = 0b00001000,
    I = 0b00000100,
    Z = 0b00000010,
    C = 0b00000001,
    
    NVDIZC = N | V | D | I | Z | C,
    All = N | V | Unused | B | D | I | Z | C ,
}