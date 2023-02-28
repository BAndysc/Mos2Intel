using System.Diagnostics;
using Mos6502;
using Mos6502Disassembler;
using VirtualMachine;

namespace Mos6502Emulator;

public class Mos6502Cpu : ICpu
{
    private static Mos6502SR NZ = Mos6502SR.N | Mos6502SR.Z;
    private static Mos6502SR NZC = NZ | Mos6502SR.C;
    private static Mos6502SR NZCV = NZC | Mos6502SR.V;
    
    public byte A, X, Y;
    public ushort IP;
    private ushort SP = 0x1FF;
    private bool decimalModeEnabled = true;

    public byte StackPointer
    {
        get => (byte)(SP - 0x100);
        set => SP = (ushort)(value + 0x100);
    }

    public Mos6502Cpu(ushort startIP = 0)
    {
        IP = startIP;
        B = true;
        UnusedFlag = true;
    }
    
    public bool N
    {
        get => statusRegister.HasFlag(Mos6502SR.N);
        set => ToggleFlag(Mos6502SR.N, value);
    }
    
    public bool V
    {
        get => statusRegister.HasFlag(Mos6502SR.V);
        set => ToggleFlag(Mos6502SR.V, value);
    }
    
    public bool B
    {
        get => statusRegister.HasFlag(Mos6502SR.B);
        set => ToggleFlag(Mos6502SR.B, value);
    }

    public bool D
    {
        get => statusRegister.HasFlag(Mos6502SR.D);
        set => ToggleFlag(Mos6502SR.D, value);
    }
    
    public bool UnusedFlag
    {
        get => statusRegister.HasFlag(Mos6502SR.Unused);
        set => ToggleFlag(Mos6502SR.Unused, value);
    }
    
    public bool I
    {
        get => statusRegister.HasFlag(Mos6502SR.I);
        set => ToggleFlag(Mos6502SR.I, value);
    }
    
    public bool Z
    {
        get => statusRegister.HasFlag(Mos6502SR.Z);
        set => ToggleFlag(Mos6502SR.Z, value);
    }
    
    public bool C
    {
        get => statusRegister.HasFlag(Mos6502SR.C);
        set => ToggleFlag(Mos6502SR.C, value);
    }

    private void ToggleFlag(Mos6502SR flag, bool value)
    {
        if (value)
            statusRegister |= flag;
        else
            statusRegister &= ~flag;
    }

    private Mos6502SR statusRegister;

    public Mos6502SR SR => statusRegister;
    
    private ushort Pop16(IMemory memory)
    {
        var low = Pop8(memory);
        var high = Pop8(memory);
        return (ushort)(low | (high << 8));
    }
    
    private ushort Pop8(IMemory memory)
    {
        ++SP;
        if (SP > 0x1FF)
            SP = 0x100;
        var low = memory[SP];
        return low;
    }
    
    private void Push(IMemory memory, int val)
    {
        if (val < 0 || val > ushort.MaxValue)
            throw new Exception("Stack value out of bounds");
            
        Push(memory, (byte)(val >> 8));
        Push(memory, (byte)(val & 0xFF));
    }
    
    private void Push(IMemory memory, byte val)
    {
        memory[SP--] = val;
        if (SP < 0x100)
            SP = 0x1FF;
    }
    
    public int Step(IMemory memory)
    {
        Span<byte> currentBytes = stackalloc byte[3];
        currentBytes[0] = memory[IP];
        currentBytes[1] = memory[IP + 1];
        currentBytes[2] = memory[IP + 2];

        MosInstruction instr;
        byte bytesRead;
        try
        {
            instr = MosDisassembler.DecodeInstruction(currentBytes, out bytesRead);
        }
        catch (IllegalInstructionException e)
        {
            throw new MosRuntimeException($"Can't decode instruction at address 0x{IP:x}.", e);
        }

        int cycles = instr.CyclesCount();
        var cyclesMeta = instr.CyclesMetaData();
        var startIp = IP;
        IP += bytesRead;
        int value = 0;
        int indirectAddress = 0;
        bool indirectAddressIsAccumulator = false;
        switch (instr.AddressingMode)
        {
            case MosAddressingMode.Accumulator:
                value = A;
                indirectAddressIsAccumulator = true;
                break;
            case MosAddressingMode.Absolute:
                value = memory[instr.Operand];
                indirectAddress = instr.Operand;
                break;
            case MosAddressingMode.AbsoluteX:
                value = memory[instr.Operand + X];
                indirectAddress = instr.Operand + X;
                if (cyclesMeta == MosCycle.PageBoundary)
                {
                    if (instr.Operand / 256 != indirectAddress / 256)
                        cycles++;
                }
                break;
            case MosAddressingMode.AbsoluteY:
                indirectAddress = (instr.Operand + Y) & 0xFFFF;
                value = memory[indirectAddress];
                if (cyclesMeta == MosCycle.PageBoundary)
                {
                    if (instr.Operand / 256 != indirectAddress / 256)
                        cycles++;
                }
                break;
            case MosAddressingMode.Immediate:
                value = instr.Operand;
                break;
            case MosAddressingMode.Indirect:
                var low = memory[instr.Operand];
                var high = memory[instr.Operand + 1];
                if ((instr.Operand & 0xFF) == 0xFF)
                    high = memory[instr.Operand & 0xFF00];
                value = indirectAddress = (low | high << 8);
                break;
            case MosAddressingMode.IndirectX:
                indirectAddress = memory[(instr.Operand + X) & 0xFF] | memory[(instr.Operand + X + 1) & 0xFF] << 8;
                value = memory[indirectAddress];
                break;
            case MosAddressingMode.IndirectY:
                indirectAddress = (memory[instr.Operand & 0xFF] | (memory[(instr.Operand + 1) & 0xFF] << 8)) + Y;
                value = memory[indirectAddress & 0xFFFF];
                if (cyclesMeta == MosCycle.PageBoundary)
                {
                    if (memory.Get16(instr.Operand) / 256 != indirectAddress / 256)
                        cycles++;
                }
                break;
            case MosAddressingMode.Relative:
                value = IP + (instr.Operand <= 127 ? instr.Operand : (instr.Operand - 255 - 1));
                break;
            case MosAddressingMode.ZeroPage:
                value = memory[instr.Operand];
                indirectAddress = instr.Operand;
                break;
            case MosAddressingMode.ZeroPageX:
                indirectAddress = (instr.Operand + X) & 0xFF;
                value = memory[indirectAddress];
                break;
            case MosAddressingMode.ZeroPageY:
                indirectAddress = (instr.Operand + Y) & 0xFF;
                value = memory[indirectAddress];
                break;
        }

        void SetOperand(byte b)
        {
            if (indirectAddressIsAccumulator)
                A = b;
            else
                memory[indirectAddress] = b;
        }

        bool branched = false;
        switch (instr.Opcode)
        {
            case MosOpcode.ADC:
            {
                byte m = (byte)value;
                uint tmp = (uint)(m + A + (C ? 1 : 0));
                Z = (tmp & 0xFF) == 0;
                if (D && decimalModeEnabled)
                {
                    if ((A & 0xF) + (m & 0xF) + (C ? 1 : 0) > 9) 
                        tmp += 6;
                    N = (tmp & 0x80) != 0;
                    V = ((A ^ m) & 0x80) == 0 && ((A ^ tmp) & 0x80) != 0;
                    if (tmp > 0x99)
                        tmp += 96;
                    C  = tmp > 0x99;
                }
                else
                {
                    N = (tmp & 0x80) != 0;
                    V = ((A ^ m) & 0x80) == 0 && ((A ^ tmp) & 0x80) != 0;
                    C = tmp > 0xFF;
                }

                A = (byte)(tmp & 0xFF);
                break;
            }
            case MosOpcode.AND:
                A = UpdateFlags(A & value, NZ);
                break;
            case MosOpcode.ASL:
                SetOperand(UpdateFlags(value << 1, NZC));
                break;
            case MosOpcode.BCC:
                if (!C)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BCS:
                if (C)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BEQ:
                if (Z)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BIT:
                statusRegister = (statusRegister & (Mos6502SR)0x3F) | (Mos6502SR)(value & 0xC0) | Mos6502SR.Unused | Mos6502SR.B;
                UpdateFlags(A & value, Mos6502SR.Z);
                break;
            case MosOpcode.BMI:
                if (N)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BNE:
                if (!Z)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BPL:
                if (!N)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BRK:
                Push(memory, IP + 1);
                Push(memory, (byte)(statusRegister | Mos6502SR.B | Mos6502SR.Unused));
                I = true;
                IP = memory.Get16(0xFFFE);
                break;
            case MosOpcode.BVC:
                if (!V)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.BVS:
                if (V)
                {
                    IP = (ushort)value;
                    branched = true;
                }
                break;
            case MosOpcode.CLC:
                C = false;
                break;
            case MosOpcode.CLD:
                D = false;
                break;
            case MosOpcode.CLI:
                I = false;
                break;
            case MosOpcode.CLV:
                V = false;
                break;
            case MosOpcode.CMP:
                UpdateFlagsCompare((uint)(A - (byte)value), A, (byte)value);
                break;
            case MosOpcode.CPX:
                UpdateFlagsCompare((uint)(X - (byte)value), X, (byte)value);
                break;
            case MosOpcode.CPY:
                UpdateFlagsCompare((uint)(Y - (byte)value), Y, (byte)value);
                break;
            case MosOpcode.DEC:
                SetOperand(UpdateFlags(value - 1, NZ));
                break;
            case MosOpcode.DEX:
                X = UpdateFlags(X - 1, NZ);
                break;
            case MosOpcode.DEY:
                Y = UpdateFlags(Y - 1, NZ);
                break;
            case MosOpcode.EOR:
                A = UpdateFlags(A ^ value, NZ);
                break;
            case MosOpcode.INC:
                SetOperand(UpdateFlags(value + 1, NZ));
                break;
            case MosOpcode.INX:
                X = UpdateFlags(X + 1, NZ);
                break;
            case MosOpcode.INY:
                Y = UpdateFlags(Y + 1, NZ);
                break;
            case MosOpcode.JMP:
                IP = (ushort)indirectAddress;
                break;
            case MosOpcode.JSR:
                Debug.Assert(instr.AddressingMode == MosAddressingMode.Indirect);
                Push(memory, IP - 1);
                IP = instr.Operand;
                break;
            case MosOpcode.LDA:
                A = UpdateFlags(value, NZ);
                break;
            case MosOpcode.LDX:
                X = UpdateFlags(value, NZ);
                break;
            case MosOpcode.LDY:
                Y = UpdateFlags(value, NZ);
                break;
            case MosOpcode.LSR:
            {
                var carry = (value & 1) == 1;
                SetOperand(UpdateFlags(value >> 1, NZC));
                C = carry;
                N = false;
                break;
            }
            case MosOpcode.NOP:
                break;
            case MosOpcode.ORA:
                A = UpdateFlags(A | value, NZ);
                break;
            case MosOpcode.PHA:
                Push(memory, A);
                break;
            case MosOpcode.PHP:
                Push(memory, (byte)(statusRegister | Mos6502SR.B | Mos6502SR.Unused));
                break;
            case MosOpcode.PLA:
                A = UpdateFlags(Pop8(memory), NZ);
                break;
            case MosOpcode.PLP:
            {
                var flag = (Mos6502SR)Pop8(memory);
                statusRegister = flag | Mos6502SR.B | Mos6502SR.Unused;
                break;
            }
            case MosOpcode.ROL:
            {
                SetOperand(UpdateFlags((value << 1) | (C ? 1 : 0), NZC));
                break;
            }
            case MosOpcode.ROR:
            {
                var carry = (value & 1) == 1;
                SetOperand(UpdateFlags((value >> 1) | (C ? 0x80 : 0), NZC));
                C = carry;
                break;
            }
            case MosOpcode.RTI:
            {
                var flag = (Mos6502SR)Pop8(memory);
                statusRegister = (flag & (Mos6502SR.NVDIZC)) | Mos6502SR.B | Mos6502SR.Unused;
                IP = Pop16(memory);
                break;
            }
            case MosOpcode.RTS:
                IP = (ushort)(Pop16(memory) + 1);
                break;
            case MosOpcode.SBC:
            {
                byte m = (byte)value;
                uint tmp = (uint)(A - m - (C ? 0 : 1));
                N = (tmp & 0x80) != 0;
                Z = (tmp & 0xFF) == 0;
                V = (((A ^ tmp) & 0x80) != 0 && ((A ^ m) & 0x80) != 0);

                if (D && decimalModeEnabled)
                {
                    if ( ((A & 0x0F) - (C ? 0 : 1)) < (m & 0x0F)) tmp -= 6;
                    if (tmp > 0x99)
                    {
                        tmp -= 0x60;
                    }
                }
                C = tmp < 0x100;
                A = (byte)(tmp & 0xFF);
                break;
            }
            case MosOpcode.SEC:
                C = true;
                break;
            case MosOpcode.SED:
                D = true;
                break;
            case MosOpcode.SEI:
                I = true;
                break;
            case MosOpcode.STA:
                SetOperand(A);
                break;
            case MosOpcode.STX:
                SetOperand(X);
                break;
            case MosOpcode.STY:
                SetOperand(Y);
                break;
            case MosOpcode.TAX:
                X = UpdateFlags(A, NZ);
                break;
            case MosOpcode.TAY:
                Y = UpdateFlags(A, NZ);
                break;
            case MosOpcode.TSX:
                X = UpdateFlags(SP & 0xFF, NZ);
                break;
            case MosOpcode.TXA:
                A = UpdateFlags(X, NZ);
                break;
            case MosOpcode.TXS:
                SP = (ushort)(0x100 + X);
                break;
            case MosOpcode.TYA:
                A = UpdateFlags(Y, NZ);
                break;
            case MosOpcode.HLT:
                throw new MosHaltedException();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (branched && cyclesMeta == MosCycle.BranchPage)
        {
            var newPage = value / 256;
            var oldPage = startIp / 256;
            cycles += newPage == oldPage ? 1 : 2;
        }
        
        return cycles;
    }

    private void UpdateFlagsCompare(uint diff, byte a, byte b)
    {
        N = (sbyte)((sbyte)a - (sbyte)b) < 0;// (diff & 0x80) == 0x80;
        Z = a == b;//(diff & 0xFF) == 0;
        C = (uint)(a-b) < 0x100;
    }

    private byte UpdateFlags(int val, Mos6502SR flags)
    {
        if ((flags & Mos6502SR.C) != 0)
            C = val > 255;

        val = (sbyte)val;// & 0xFF;
        
        if ((flags & Mos6502SR.N) != 0)
            N = val < 0 || val >= 128;
        
        if ((flags & Mos6502SR.Z) != 0)
            Z = val == 0;
        return (byte)val;
    }

    public void DisableDecimalMode()
    {
        decimalModeEnabled = false;
    }
}

public class MosHaltedException : Exception
{
    
}