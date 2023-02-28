using Mos6502Assembler;
using NUnit.Framework;

namespace Mos6502.Test;

public class AssemblerTests
{
    [TestCase(MosOpcode.BRK, MosAddressingMode.Implied, 0x0, new byte[] { 0x00 }, "BRK")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0x01, 0x0F }, "ORA ($0F, X)")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x05, 0x11 }, "ORA $11")]
    [TestCase(MosOpcode.ASL, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x06, 0x11 }, "ASL $11")]
    [TestCase(MosOpcode.PHP, MosAddressingMode.Implied, 0x0, new byte[] { 0x08 }, "PHP")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.Immediate, 0x11, new byte[] { 0x09, 0x11 }, "ORA #$11")]
    [TestCase(MosOpcode.ASL, MosAddressingMode.Accumulator, 0x0, new byte[] { 0x0a }, "ASL")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x0d, 0x10, 0x2C }, "ORA $2C10")]
    [TestCase(MosOpcode.ASL, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x0e, 0x10, 0x2C }, "ASL $2C10")]
    [TestCase(MosOpcode.BPL, MosAddressingMode.Relative, 0x11, new byte[] { 0x10, 0x11 }, "BPL $11")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0x11, 0x0F }, "ORA ($0F), Y")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x15, 0x0F }, "ORA $0F, X")]
    [TestCase(MosOpcode.ASL, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x16, 0x0F }, "ASL $0F, X")]
    [TestCase(MosOpcode.CLC, MosAddressingMode.Implied, 0x0, new byte[] { 0x18 }, "CLC")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0x19, 0x10, 0x2C }, "ORA $2C10, Y")]
    [TestCase(MosOpcode.ORA, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x1d, 0x10, 0x2C }, "ORA $2C10, X")]
    [TestCase(MosOpcode.ASL, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x1e, 0x10, 0x2C }, "ASL $2C10, X")]
    [TestCase(MosOpcode.JSR, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x20, 0x10, 0x2C }, "JSR $2C10")]
    [TestCase(MosOpcode.AND, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0x21, 0x0F }, "AND ($0F, X)")]
    [TestCase(MosOpcode.BIT, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x24, 0x11 }, "BIT $11")]
    [TestCase(MosOpcode.AND, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x25, 0x11 }, "AND $11")]
    [TestCase(MosOpcode.ROL, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x26, 0x11 }, "ROL $11")]
    [TestCase(MosOpcode.PLP, MosAddressingMode.Implied, 0x0, new byte[] { 0x28 }, "PLP")]
    [TestCase(MosOpcode.AND, MosAddressingMode.Immediate, 0x11, new byte[] { 0x29, 0x11 }, "AND #$11")]
    [TestCase(MosOpcode.ROL, MosAddressingMode.Accumulator, 0x0, new byte[] { 0x2a }, "ROL")]
    [TestCase(MosOpcode.BIT, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x2c, 0x10, 0x2C }, "BIT $2C10")]
    [TestCase(MosOpcode.AND, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x2d, 0x10, 0x2C }, "AND $2C10")]
    [TestCase(MosOpcode.ROL, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x2e, 0x10, 0x2C }, "ROL $2C10")]
    [TestCase(MosOpcode.BMI, MosAddressingMode.Relative, 0x11, new byte[] { 0x30, 0x11 }, "BMI $11")]
    [TestCase(MosOpcode.AND, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0x31, 0x0F }, "AND ($0F), Y")]
    [TestCase(MosOpcode.AND, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x35, 0x0F }, "AND $0F, X")]
    [TestCase(MosOpcode.ROL, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x36, 0x0F }, "ROL $0F, X")]
    [TestCase(MosOpcode.SEC, MosAddressingMode.Implied, 0x0, new byte[] { 0x38 }, "SEC")]
    [TestCase(MosOpcode.AND, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0x39, 0x10, 0x2C }, "AND $2C10, Y")]
    [TestCase(MosOpcode.AND, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x3d, 0x10, 0x2C }, "AND $2C10, X")]
    [TestCase(MosOpcode.ROL, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x3e, 0x10, 0x2C }, "ROL $2C10, X")]
    [TestCase(MosOpcode.RTI, MosAddressingMode.Implied, 0x0, new byte[] { 0x40 }, "RTI")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0x41, 0x0F }, "EOR ($0F, X)")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x45, 0x11 }, "EOR $11")]
    [TestCase(MosOpcode.LSR, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x46, 0x11 }, "LSR $11")]
    [TestCase(MosOpcode.PHA, MosAddressingMode.Implied, 0x0, new byte[] { 0x48 }, "PHA")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.Immediate, 0x11, new byte[] { 0x49, 0x11 }, "EOR #$11")]
    [TestCase(MosOpcode.LSR, MosAddressingMode.Accumulator, 0x0, new byte[] { 0x4a }, "LSR")]
    [TestCase(MosOpcode.JMP, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x4c, 0x10, 0x2C }, "JMP $2C10")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x4d, 0x10, 0x2C }, "EOR $2C10")]
    [TestCase(MosOpcode.LSR, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x4e, 0x10, 0x2C }, "LSR $2C10")]
    [TestCase(MosOpcode.BVC, MosAddressingMode.Relative, 0x11, new byte[] { 0x50, 0x11 }, "BVC $11")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0x51, 0x0F }, "EOR ($0F), Y")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x55, 0x0F }, "EOR $0F, X")]
    [TestCase(MosOpcode.LSR, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x56, 0x0F }, "LSR $0F, X")]
    [TestCase(MosOpcode.CLI, MosAddressingMode.Implied, 0x0, new byte[] { 0x58 }, "CLI")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0x59, 0x10, 0x2C }, "EOR $2C10, Y")]
    [TestCase(MosOpcode.EOR, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x5d, 0x10, 0x2C }, "EOR $2C10, X")]
    [TestCase(MosOpcode.LSR, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x5e, 0x10, 0x2C }, "LSR $2C10, X")]
    [TestCase(MosOpcode.RTS, MosAddressingMode.Implied, 0x0, new byte[] { 0x60 }, "RTS")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0x61, 0x0F }, "ADC ($0F, X)")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x65, 0x11 }, "ADC $11")]
    [TestCase(MosOpcode.ROR, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x66, 0x11 }, "ROR $11")]
    [TestCase(MosOpcode.PLA, MosAddressingMode.Implied, 0x0, new byte[] { 0x68 }, "PLA")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.Immediate, 0x11, new byte[] { 0x69, 0x11 }, "ADC #$11")]
    [TestCase(MosOpcode.ROR, MosAddressingMode.Accumulator, 0x0, new byte[] { 0x6a }, "ROR")]
    [TestCase(MosOpcode.JMP, MosAddressingMode.Indirect, 0x2010, new byte[] { 0x6c, 0x10, 0x20 }, "JMP ($2010)")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x6d, 0x10, 0x2C }, "ADC $2C10")]
    [TestCase(MosOpcode.ROR, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x6e, 0x10, 0x2C }, "ROR $2C10")]
    [TestCase(MosOpcode.BVS, MosAddressingMode.Relative, 0x11, new byte[] { 0x70, 0x11 }, "BVS $11")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0x71, 0x0F }, "ADC ($0F), Y")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x75, 0x0F }, "ADC $0F, X")]
    [TestCase(MosOpcode.ROR, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x76, 0x0F }, "ROR $0F, X")]
    [TestCase(MosOpcode.SEI, MosAddressingMode.Implied, 0x0, new byte[] { 0x78 }, "SEI")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0x79, 0x10, 0x2C }, "ADC $2C10, Y")]
    [TestCase(MosOpcode.ADC, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x7d, 0x10, 0x2C }, "ADC $2C10, X")]
    [TestCase(MosOpcode.ROR, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x7e, 0x10, 0x2C }, "ROR $2C10, X")]
    [TestCase(MosOpcode.STA, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0x81, 0x0F }, "STA ($0F, X)")]
    [TestCase(MosOpcode.STY, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x84, 0x11 }, "STY $11")]
    [TestCase(MosOpcode.STA, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x85, 0x11 }, "STA $11")]
    [TestCase(MosOpcode.STX, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0x86, 0x11 }, "STX $11")]
    [TestCase(MosOpcode.DEY, MosAddressingMode.Implied, 0x0, new byte[] { 0x88 }, "DEY")]
    [TestCase(MosOpcode.TXA, MosAddressingMode.Implied, 0x0, new byte[] { 0x8a }, "TXA")]
    [TestCase(MosOpcode.STY, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x8c, 0x10, 0x2C }, "STY $2C10")]
    [TestCase(MosOpcode.STA, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x8d, 0x10, 0x2C }, "STA $2C10")]
    [TestCase(MosOpcode.STX, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0x8e, 0x10, 0x2C }, "STX $2C10")]
    [TestCase(MosOpcode.BCC, MosAddressingMode.Relative, 0x11, new byte[] { 0x90, 0x11 }, "BCC $11")]
    [TestCase(MosOpcode.STA, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0x91, 0x0F }, "STA ($0F), Y")]
    [TestCase(MosOpcode.STY, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x94, 0x0F }, "STY $0F, X")]
    [TestCase(MosOpcode.STA, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0x95, 0x0F }, "STA $0F, X")]
    [TestCase(MosOpcode.STX, MosAddressingMode.ZeroPageY, 0x0F, new byte[] { 0x96, 0x0F }, "STX $0F, Y")]
    [TestCase(MosOpcode.TYA, MosAddressingMode.Implied, 0x0, new byte[] { 0x98 }, "TYA")]
    [TestCase(MosOpcode.STA, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0x99, 0x10, 0x2C }, "STA $2C10, Y")]
    [TestCase(MosOpcode.TXS, MosAddressingMode.Implied, 0x0, new byte[] { 0x9a }, "TXS")]
    [TestCase(MosOpcode.STA, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0x9d, 0x10, 0x2C }, "STA $2C10, X")]
    [TestCase(MosOpcode.LDY, MosAddressingMode.Immediate, 0x11, new byte[] { 0xa0, 0x11 }, "LDY #$11")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0xa1, 0x0F }, "LDA ($0F, X)")]
    [TestCase(MosOpcode.LDX, MosAddressingMode.Immediate, 0x11, new byte[] { 0xa2, 0x11 }, "LDX #$11")]
    [TestCase(MosOpcode.LDY, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xa4, 0x11 }, "LDY $11")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xa5, 0x11 }, "LDA $11")]
    [TestCase(MosOpcode.LDX, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xa6, 0x11 }, "LDX $11")]
    [TestCase(MosOpcode.TAY, MosAddressingMode.Implied, 0x0, new byte[] { 0xa8 }, "TAY")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.Immediate, 0x11, new byte[] { 0xa9, 0x11 }, "LDA #$11")]
    [TestCase(MosOpcode.TAX, MosAddressingMode.Implied, 0x0, new byte[] { 0xaa }, "TAX")]
    [TestCase(MosOpcode.LDY, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xac, 0x10, 0x2C }, "LDY $2C10")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xad, 0x10, 0x2C }, "LDA $2C10")]
    [TestCase(MosOpcode.LDX, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xae, 0x10, 0x2C }, "LDX $2C10")]
    [TestCase(MosOpcode.BCS, MosAddressingMode.Relative, 0x11, new byte[] { 0xb0, 0x11 }, "BCS $11")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0xb1, 0x0F }, "LDA ($0F), Y")]
    [TestCase(MosOpcode.LDY, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xb4, 0x0F }, "LDY $0F, X")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xb5, 0x0F }, "LDA $0F, X")]
    [TestCase(MosOpcode.LDX, MosAddressingMode.ZeroPageY, 0x0F, new byte[] { 0xb6, 0x0F }, "LDX $0F, Y")]
    [TestCase(MosOpcode.CLV, MosAddressingMode.Implied, 0x0, new byte[] { 0xb8 }, "CLV")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0xb9, 0x10, 0x2C }, "LDA $2C10, Y")]
    [TestCase(MosOpcode.TSX, MosAddressingMode.Implied, 0x0, new byte[] { 0xba }, "TSX")]
    [TestCase(MosOpcode.LDY, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xbc, 0x10, 0x2C }, "LDY $2C10, X")]
    [TestCase(MosOpcode.LDA, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xbd, 0x10, 0x2C }, "LDA $2C10, X")]
    [TestCase(MosOpcode.LDX, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0xbe, 0x10, 0x2C }, "LDX $2C10, Y")]
    [TestCase(MosOpcode.CPY, MosAddressingMode.Immediate, 0x11, new byte[] { 0xc0, 0x11 }, "CPY #$11")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0xc1, 0x0F }, "CMP ($0F, X)")]
    [TestCase(MosOpcode.CPY, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xc4, 0x11 }, "CPY $11")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xc5, 0x11 }, "CMP $11")]
    [TestCase(MosOpcode.DEC, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xc6, 0x11 }, "DEC $11")]
    [TestCase(MosOpcode.INY, MosAddressingMode.Implied, 0x0, new byte[] { 0xc8 }, "INY")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.Immediate, 0x11, new byte[] { 0xc9, 0x11 }, "CMP #$11")]
    [TestCase(MosOpcode.DEX, MosAddressingMode.Implied, 0x0, new byte[] { 0xca }, "DEX")]
    [TestCase(MosOpcode.CPY, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xcc, 0x10, 0x2C }, "CPY $2C10")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xcd, 0x10, 0x2C }, "CMP $2C10")]
    [TestCase(MosOpcode.DEC, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xce, 0x10, 0x2C }, "DEC $2C10")]
    [TestCase(MosOpcode.BNE, MosAddressingMode.Relative, 0x11, new byte[] { 0xd0, 0x11 }, "BNE $11")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0xd1, 0x0F }, "CMP ($0F), Y")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xd5, 0x0F }, "CMP $0F, X")]
    [TestCase(MosOpcode.DEC, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xd6, 0x0F }, "DEC $0F, X")]
    [TestCase(MosOpcode.CLD, MosAddressingMode.Implied, 0x0, new byte[] { 0xd8 }, "CLD")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0xd9, 0x10, 0x2C }, "CMP $2C10, Y")]
    [TestCase(MosOpcode.CMP, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xdd, 0x10, 0x2C }, "CMP $2C10, X")]
    [TestCase(MosOpcode.DEC, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xde, 0x10, 0x2C }, "DEC $2C10, X")]
    [TestCase(MosOpcode.CPX, MosAddressingMode.Immediate, 0x11, new byte[] { 0xe0, 0x11 }, "CPX #$11")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.IndirectX, 0x0F, new byte[] { 0xe1, 0x0F }, "SBC ($0F, X)")]
    [TestCase(MosOpcode.CPX, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xe4, 0x11 }, "CPX $11")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xe5, 0x11 }, "SBC $11")]
    [TestCase(MosOpcode.INC, MosAddressingMode.ZeroPage, 0x11, new byte[] { 0xe6, 0x11 }, "INC $11")]
    [TestCase(MosOpcode.INX, MosAddressingMode.Implied, 0x0, new byte[] { 0xe8 }, "INX")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.Immediate, 0x11, new byte[] { 0xe9, 0x11 }, "SBC #$11")]
    [TestCase(MosOpcode.NOP, MosAddressingMode.Implied, 0x0, new byte[] { 0xea }, "NOP")]
    [TestCase(MosOpcode.CPX, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xec, 0x10, 0x2C }, "CPX $2C10")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xed, 0x10, 0x2C }, "SBC $2C10")]
    [TestCase(MosOpcode.INC, MosAddressingMode.Absolute, 0x2C10, new byte[] { 0xee, 0x10, 0x2C }, "INC $2C10")]
    [TestCase(MosOpcode.BEQ, MosAddressingMode.Relative, 0x11, new byte[] { 0xf0, 0x11 }, "BEQ $11")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.IndirectY, 0x0F, new byte[] { 0xf1, 0x0F }, "SBC ($0F), Y")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xf5, 0x0F }, "SBC $0F, X")]
    [TestCase(MosOpcode.INC, MosAddressingMode.ZeroPageX, 0x0F, new byte[] { 0xf6, 0x0F }, "INC $0F, X")]
    [TestCase(MosOpcode.SED, MosAddressingMode.Implied, 0x0, new byte[] { 0xf8 }, "SED")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.AbsoluteY, 0x2C10, new byte[] { 0xf9, 0x10, 0x2C }, "SBC $2C10, Y")]
    [TestCase(MosOpcode.SBC, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xfd, 0x10, 0x2C }, "SBC $2C10, X")]
    [TestCase(MosOpcode.INC, MosAddressingMode.AbsoluteX, 0x2C10, new byte[] { 0xfe, 0x10, 0x2C }, "INC $2C10, X")]
    [TestCase(MosOpcode.JMP, MosAddressingMode.Absolute, 0x1, new byte[] { 0x4c, 0x1, 0x00 }, "JMP $0001")]
    public void TestEncode(MosOpcode opcode, MosAddressingMode addressingMode, int operand, byte[] bytes, string str)
    {
        var instr = new MosInstruction(opcode, addressingMode, (ushort)operand);
        var assembled = instr.Encode();
        CollectionAssert.AreEqual(bytes, assembled, str);

        var toString = instr.ToString();
        Assert.AreEqual(str, toString);

        var assembler = new Assembler();
        var assembledBytes = assembler.Assemble(str, out _);
        CollectionAssert.AreEqual(bytes, assembledBytes, str);
    }

    [TestCase(MosOpcode.JMP, MosAddressingMode.Absolute, 0x1, new byte[] { 0x4c, 0x1, 0x00 }, "JMP 1")]
    public void TestAssemble(MosOpcode opcode, MosAddressingMode addressingMode, int operand, byte[] bytes, string str)
    {
        var assembler = new Assembler();
        var assembledBytes = assembler.Assemble(str, out _);
        CollectionAssert.AreEqual(bytes, assembledBytes, str);
    }

    [Test]
    public void TestCompoundProgram()
    {
        string program = @"
  JSR init
  JSR loop
  JSR end

init:
  LDX #$00
  RTS

loop:
  INX
  CPX #$05
  BNE loop
  RTS

end:
  BRK
";
        var assembler = new Assembler();
        var assembly = assembler.Assemble(program, out _);
        var bytes = new byte[]
        {
            /* $0000 */ 0x20, 0x09, 0x00, // JSR $0609
            /* $0003 */ 0x20, 0x0c, 0x00, // JSR $060c
            /* $0006 */ 0x20, 0x12, 0x00, // JSR $0612
            /* $0009 */ 0xa2, 0x00, // LDX #$00
            /* $000b */ 0x60, // RTS
            /* $000c */ 0xe8, // INX
            /* $000d */ 0xe0, 0x05, // CPX #$05
            /* $000f */ 0xd0, 0xfb, // BNE $060c
            /* $0011 */ 0x60, // RTS
            /* $0012 */ 0x00, // BRK
        };
        CollectionAssert.AreEqual(bytes, assembly);
    }

    [Test]
    public void TestOrg()
    {
        string program = @"
org $0b
RTS
org $03
RTS
";
        var assembler = new Assembler();
        var assembly = assembler.Assemble(program, out _);
        var bytes = new byte[]
        {
            /* $0000 */ 0x00, 0x00, 0x00, // JSR $0609
            /* $0003 */ 0x60, 0x00, 0x00, // JSR $060c
            /* $0006 */ 0x00, 0x00, 0x00, // JSR $0612
            /* $0009 */ 0x00, 0x00, // LDX #$00
            /* $000b */ 0x60, // RTS
        };
        CollectionAssert.AreEqual(bytes, assembly);
    }

    [Test]
    public void TestDefines()
    {
        string program = @"
define abc    1
define abc2   $10
define abc3   $100
lda #abc
lda #abc2
lda abc3
";
        var assembler = new Assembler();
        var assembly = assembler.Assemble(program, out _);
        var bytes = new byte[]
        {
            0xa9, 1, // LDA #ABC
            0xa9, 16, // LDA #ABC2
            0xad, 0, 1 // LDA ABC3
        };
        CollectionAssert.AreEqual(bytes, assembly);
    }
    
    [Test]
    public void TestComments()
    {
        string program = @"
; asdadsda __ ___ 
; asdas d
   ;  asdsds
lda #$1
";
        var assembler = new Assembler();
        var assembly = assembler.Assemble(program, out _);
        var bytes = new byte[]
        {
            0xa9, 1, // LDA 1
        };
        CollectionAssert.AreEqual(bytes, assembly);
    }
}
