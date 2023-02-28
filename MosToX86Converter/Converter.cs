using System.Diagnostics;
using System.Text;
using Mos6502;
using Mos6502Disassembler;
using X86Assembly;

namespace Mos6502Testing;

public class MosConverterOptions
{
    public MosConverterOptions()
    {
    }
    
    public MosConverterOptions(MosConverterOptions other)
    {
        BrkInterruptAddress = other.BrkInterruptAddress;
        DebugMode = other.DebugMode;
        StartStackPointer = other.StartStackPointer;
        UseNativeCallAsJsr = other.UseNativeCallAsJsr;
        OptimizeFlags = other.OptimizeFlags;
        CycleMethod = other.CycleMethod;
        UseArrayAsMem = other.UseArrayAsMem;
        UseNativeFlags = other.UseNativeFlags;
    }

    public ushort BrkInterruptAddress { get; set; } = 0xFFFE;

    public bool DebugMode { get; set; } = false;

    public byte StartStackPointer { get; set; } = 0xFF;

    public bool UseNativeCallAsJsr { get; set; } = false;
    
    public bool OptimizeFlags { get; set; } = true;

    public bool UseNativeFlags { get; set; } = true;
    
    /**
     * If false, generate a single big mos6502 function that executes all the function sequentially
     * if true, generates a function 'cycle', which invokes exactly 1 mos instruction
     *      and can be executed consecutively
     */
    public bool CycleMethod { get; set; } = false;

    public static MosConverterOptions Default => new();

    /**
     * if true, will use _memory array instead of using GetValue and SetValue functions
     */
    public bool UseArrayAsMem { get; set; } = true;

    public MosConverterOptions WithOptimizeFlags(bool optimize)
    {
        var @new = new MosConverterOptions(this);
        @new.OptimizeFlags = optimize;
        return @new;
    }
    
    public MosConverterOptions WithDirectMemoryAccess(bool dma)
    {
        var @new = new MosConverterOptions(this);
        @new.UseArrayAsMem = dma;
        return @new;
    }
    
    public MosConverterOptions WithCycleMethod(bool cycle)
    {
        var @new = new MosConverterOptions(this);
        @new.CycleMethod = cycle;
        return @new;
    }
}

public class MosConverter
{
    private MosConverterOptions options = new();
    private int labelCounter;
    private List<int> legalInstructions = new();
    private HashSet<ushort> jumpLabels = new();

    public string GetNextLabel()
    {
        return $"label{labelCounter++:X4}";
    }
    
    private StringBuilder sb = new();
    private void Emit(X86Instruction instr)
    {
        sb.AppendLine(instr.ToString());
    }

    private void Emit(string str)
    {
        sb.AppendLine(str);
    }

    public string Convert(ReadOnlySpan<byte> source, int startInstruction, MosConverterOptions? options = null)
    {
        this.options = options ??= MosConverterOptions.Default;

        if (options.CycleMethod)
        {
            
        }
        else
        {
            
        }
        
        Emit("global _mosInit");
        Emit("global _mos6502");
        Emit("global _getMemory");
        
        if (options.CycleMethod)
            Emit("global _mosCycle");
        if (options.DebugMode)
        {
            Emit("extern _debugMos");
        }

        if (!options.UseArrayAsMem)
        {
            Emit("extern _getValue");
            Emit("extern _getValue16");
            Emit("extern _setValue");
        }
        
        Emit("section .text");
        if (options.CycleMethod)
        {
            Emit(X86.Label("_mosCycle"));
            Emit(X86.Push(RegisterX86.RBX));
            Emit(X86.Push(RegisterX86.R12));
            Emit(X86.Push(RegisterX86.R13));
            Emit(X86.Push(RegisterX86.R14));
            Emit(X86.Push(RegisterX86.R15));
            Emit(X86.Push(RegisterX86.RBP));
            Emit(X86.Xor(CycleCounter_64, CycleCounter_64));
            EmitRegisterToCpuState(RegisterX86.RDI, true);
            EmitCpuStateToRegister(RegisterX86.RAX, Temp16, CycleCounter);
            Emit(X86.Pop(RegisterX86.RBP));
            Emit(X86.Pop(RegisterX86.R15));
            Emit(X86.Pop(RegisterX86.R14));
            Emit(X86.Pop(RegisterX86.R13));
            Emit(X86.Pop(RegisterX86.R12));
            Emit(X86.Pop(RegisterX86.RBX));
            Emit("ret");
        }
        
        Emit("_getMemory:");
        Emit("LEA RAX, [rel _memory]");
        Emit("ret");
        
        Emit("_mos6502:");
        Emit(X86.Push(RegisterX86.RBX));
        Emit(X86.Push(RegisterX86.R12));
        Emit(X86.Push(RegisterX86.R13));
        Emit(X86.Push(RegisterX86.R14));
        Emit(X86.Push(RegisterX86.R15));
        Emit(X86.Push(RegisterX86.RBP));
        Emit(X86.Xor(Flags_64, Flags_64));
        Emit(X86.Xor(X_64, X_64));
        Emit(X86.Xor(Y_64, Y_64));
        Emit(X86.Xor(Temp64, Temp64));
        Emit(X86.Xor(Temp64_2, Temp64_2));
        Emit(X86.Mov(Flags_64, (int)(Mos6502SR.B | Mos6502SR.Unused)));
        Emit(X86.Xor(Accumulator_64, Accumulator_64));
        if (options.UseArrayAsMem)
            Emit(X86.Lea(StackPointer_64, X86.Indirect(0, (uint)(0x100 + options.StartStackPointer), 1, "_memory")));
        else
            Emit(X86.Mov(StackPointer_64, 0x100 + options.StartStackPointer));
        Emit($"JMP instr{startInstruction:X4}");

        bool[] visitedBytes = new bool[source.Length];

        bool anyVisited = false;
        do
        {
            anyVisited = false;
            for (int i = 0; i < source.Length; ++i)
            {
                if (visitedBytes[i])
                    continue;

                visitedBytes[i] = true;
                anyVisited = true;
                if (i >= 2 && source[i] == 0 && source[i - 1] == 0 && source[i - 2] == 0)
                {
                    continue;
                }

                var span = source.Slice(i);
                if (!TryDecodeMosInstruction(span, out var bytesRead, out var instr))
                {
                    if (legalInstructions.Contains(i))
                    {
                        Console.WriteLine("aa");
                    }
                    legalInstructions.Add(i);
                    Emit($"instr{i:X4}:          ; " + $"{span[0]:X}");
                    Emit($"ret");
                    continue;
                }

                bool canSkipFlags = options.OptimizeFlags;
                if (canSkipFlags)
                {
                    // if can, we need to make sure if we really can
                    var instrWriteFlags = instr.GetWriteFlags();
                    int fakePos = i + bytesRead;
                    while (fakePos < source.Length)
                    {
                        if (!TryDecodeMosInstruction(source.Slice(fakePos), out var bytesRead2, out var instr2))
                            break;
                        if (instr2.Opcode == MosOpcode.HLT)
                            break;
                        fakePos += bytesRead2;
                        var nextInstrReadFlags = instr2.GetReadFlags();
                        var nextInstrWriteFlags = instr2.GetWriteFlags();
                        if ((nextInstrReadFlags & instrWriteFlags) != 0)
                        {
                            canSkipFlags = false;
                            break;
                        }

                        instrWriteFlags &= ~nextInstrWriteFlags;
                        if (instrWriteFlags == 0)
                            break;
                    }
                }

                var output = Convert(instr, i, !canSkipFlags).ToList();

                if (output.Count > 0)
                {
                    Emit($"instr{i:X4}:          ; " + instr.ToString());
                    if (legalInstructions.Contains(i))
                        Console.WriteLine("ups");
                    legalInstructions.Add(i);

                    if (options.DebugMode)
                    {
                        EmitCpuStateToRegister(RegisterX86.RDI, (ushort)i, (byte)instr.CyclesCount());
                        Emit(X86.Call("_debugMos"));
                    }

                    foreach (var asm in output)
                        Emit(asm);

                    if (options.CycleMethod)
                    {
                        foreach (var ins in LeaveFromCycle(i + bytesRead, instr))
                            Emit(ins);
                    }
                    else
                    {
                        //Emit(X86.Jump(GetLabelForJump(i + bytesRead)));
                    }
                }

                i += bytesRead - 1;
            }
            Emit(X86.Jump("nesend"));
        } while (anyVisited);

        

        foreach (var m in jumpLabels)
        {
            if (!legalInstructions.Contains(m))
            {
                Emit(X86.Label($"instr{m:X4}"));
            }
        }

        Emit(X86.Label("nesend"));
        if (options.CycleMethod)
        {
            Emit(X86.Or(CycleCounter, 0x80)); // HLT
        }
        else
        {
            EmitCpuStateToRegister(RegisterX86.RAX, null, 0x80); // HLT
            Emit(X86.Pop(RegisterX86.RBP));
            Emit(X86.Pop(RegisterX86.R15));
            Emit(X86.Pop(RegisterX86.R14));
            Emit(X86.Pop(RegisterX86.R13));
            Emit(X86.Pop(RegisterX86.R12));
            Emit(X86.Pop(RegisterX86.RBX));
        }

        Emit(X86.Ret());

        Emit(X86.Label("_mosInit"));
        Emit(X86.Lea(RegisterX86.RAX, X86.Indirect(0, 0, 1, "addrMap")));
        foreach (var m in legalInstructions)
        {
            Emit(X86.Lea(RegisterX86.RCX, X86.Indirect(0, 0, 1, $"instr{m:X4}")));
            Emit(X86.Mov(X86.Indirect(RegisterX86.RAX, (uint)(8 * m), 1), RegisterX86.RCX));
        }

        // IF NES & MIRROR
        // foreach (var m in legalInstructions)
        // {
        //     Emit(X86.Lea(RegisterX86.RCX, X86.Indirect(0, 0, 1, $"instr{m:X4}")));
        //     Emit(X86.Mov(X86.Indirect(RegisterX86.RAX, (uint)(8*(m+0x4000)), 1), RegisterX86.RCX));
        // }
        // END IF
        Emit(X86.Ret());

        Emit("section .data");
        Emit("addrMap: times 1000000 db 0 ");
        Emit("align 1024");
        Emit("_memory:");
        bool db = true;
        bool comma = false;
        for (int i = 0; i < source.Length; ++i)
        {
            if (i < source.Length - 3 && source[i] == 0 && source[i + 1] == 0 && source[i + 2] == 0)
            {
                int count0 = 0;
                while (i < source.Length && source[i] == 0)
                {
                    count0++;
                    i++;
                }

                i--;
                Emit("");
                Emit($"times {count0} db 0 ");
                comma = false;
                db = true;
            }
            else
            {
                if (db)
                {
                    sb.Append("db ");
                    db = false;
                }

                if (comma)
                    sb.Append(", ");
                sb.Append($"0x{source[i]:x}");
                comma = true;
                if ((i + 1) % 8 == 0)
                {
                    Emit("");
                    comma = false;
                    db = true;
                }
            }
        }
        
        Emit("");
        if (source.Length < 0x10000)
            Emit($"times {0x10000 - source.Length} db 0 ");

        return sb.ToString();
    }

    private IEnumerable<X86Instruction> LeaveFromCycle(OperandX86 operand, MosInstruction instr)
    {
        yield return (X86.Add(CycleCounter, instr.CyclesCount()));
        yield return (X86.Mov(Temp16, operand));
        yield return (X86.Ret());
    }

    private void EmitRegisterToCpuState(RegisterX86 src, bool callToIp)
    {
        // firstly clear the cpu state registers
        Emit(X86.Xor(Accumulator_64, Accumulator_64));
        Emit(X86.Xor(X_64, X_64));
        Emit(X86.Xor(Y_64, Y_64));
        Emit(X86.Mov(Flags_64, (int)(Mos6502SR.B | Mos6502SR.Unused)));
        if (options.UseArrayAsMem)
            Emit(X86.Lea(StackPointer_64, X86.Indirect(0, (uint)(0x100 + options.StartStackPointer), 1, "_memory")));
        else
            Emit(X86.Mov(StackPointer_64, 0x100));

        // now extract the data from the src register
        var src_8 = src.WithWidth(RegisterWidth._8);
        Emit(X86.Shr(src, 8)); // cycles

        Emit(X86.Xor(Temp64, Temp64));
        Emit(X86.Mov(Temp16, src.WithWidth(RegisterWidth._16)));
        Emit(X86.Lea(Temp64_2, X86.Indirect(0, 0, 1, "addrMap")));
        Emit(X86.Shr(src, 16));
        
        Emit(X86.Mov(StackPointer, src_8));
        Emit(X86.Shr(src, 8));
        Emit(X86.Mov(Flags, src_8));
        Emit(X86.Shr(src, 8));
        Emit(X86.Mov(Y, src_8));
        Emit(X86.Shr(src, 8));
        Emit(X86.Mov(X, src_8));
        Emit(X86.Shr(src, 8));
        Emit(X86.Mov(Accumulator, src_8));
        
        if (callToIp)
            Emit(X86.Call($"[{Temp64_2} + 8 * {Temp64}]"));
    }
    
    private void EmitCpuStateToRegister(RegisterX86 dest, OperandX86? ip, OperandX86? cycles)
    {
        var dest8 = dest.WithWidth(RegisterWidth._8);
        Emit(X86.Xor(dest, dest));
        Emit(X86.Mov(dest8, Accumulator));
        Emit(X86.Shl(dest, 8));
        Emit(X86.Or(dest8, X));
        Emit(X86.Shl(dest, 8));
        Emit(X86.Or(dest8, Y));
        Emit(X86.Shl(dest, 8));
        Emit(X86.Or(dest8, Flags));
        Emit(X86.Shl(dest, 8));
        Emit(X86.Mov(dest8, StackPointer)); // set low bytes
        Emit(X86.Shl(dest, 16));
        if (ip.HasValue)
            Emit(X86.Mov(dest.WithWidth(RegisterWidth._16), ip.Value));
        Emit(X86.Shl(dest, 8));
        if (cycles.HasValue)
            Emit(X86.Mov(dest8, cycles.Value)); // set low bytes
    }

    private string GetLabelForJump(int i)
    {
        jumpLabels.Add((ushort)i);
        return $"instr{(ushort)i:X4}";
    }


    private static bool TryDecodeMosInstruction(ReadOnlySpan<byte> bytes, out byte bytesRead, out MosInstruction instr)
    {
        try
        {
            instr = MosDisassembler.DecodeInstruction(bytes, out bytesRead);
            return true;
        }
        catch (IllegalInstructionException)
        {
            bytesRead = 0;
            instr = new MosInstruction();
            return false;
        }
        catch (IndexOutOfRangeException)
        {
            bytesRead = 0;
            instr = new MosInstruction();
            return false;
        }
    }

    // REG  SAVE?   USAGE
    // rax    -       Operand address
    // rbx    Y        STACK POINTER
    // rcx    -         temp 1
    // rdx    -         Operand
    // rbp    Y         CYCLE_COUNTER
    // rsi    -       
    // rdi    -       
    // r8     -         temp 2
    // r9     -         temp 3
    // r10    -         temp 4
    // r11    -       
    // r12    Y         ACC
    // r13    Y          Y
    // r14    Y          X
    // r15    Y          FLAGS
    private RegisterX86 CycleCounter = RegisterX86.BPL;
    private RegisterX86 Accumulator = RegisterX86.R12B;
    private RegisterX86 Y = RegisterX86.R13B;
    private RegisterX86 X = RegisterX86.R14B;
    private RegisterX86 StackPointer = RegisterX86.BL;
    private RegisterX86 Flags = RegisterX86.R15B;
    private RegisterX86 OperandAddress_8 = RegisterX86.AL;
    private RegisterX86 Temp8 = RegisterX86.CL;
    private RegisterX86 Operand_8 = RegisterX86.DL;
    private RegisterX86 Temp8_2 = RegisterX86.R8B;
    private RegisterX86 Temp8_3 = RegisterX86.R9B;
    private RegisterX86 Temp8_4 = RegisterX86.R10B;
    
    
    private RegisterX86 CycleCounter_64 => RegisterX86.RBP.WithWidth(RegisterWidth._64);
    private RegisterX86 Accumulator_64 => Accumulator.WithWidth(RegisterWidth._64);
    private RegisterX86 Y_16 => Y.WithWidth(RegisterWidth._16);
    private RegisterX86 X_16 => X.WithWidth(RegisterWidth._16);
    private RegisterX86 Y_64 => Y.WithWidth(RegisterWidth._64);
    private RegisterX86 X_64 => X.WithWidth(RegisterWidth._64);
    private RegisterX86 Flags_64 => Flags.WithWidth(RegisterWidth._64);
    private RegisterX86 StackPointer_64 => StackPointer.WithWidth(RegisterWidth._64);
    //private RegisterX86 StackPointer_16 => StackPointer.WithWidth(RegisterWidth._16);
    
    private RegisterX86 OperandAddress_64 => OperandAddress_8.WithWidth(RegisterWidth._64);
    private RegisterX86 OperandAddress_16 => OperandAddress_8.WithWidth(RegisterWidth._16);
    private RegisterX86 Temp64 => Temp8.WithWidth(RegisterWidth._64);
    private RegisterX86 Temp16 => Temp8.WithWidth(RegisterWidth._16);
    private RegisterX86 Operand_64 => Operand_8.WithWidth(RegisterWidth._64);
    private RegisterX86 Operand_16 => Operand_8.WithWidth(RegisterWidth._16);
    
    private RegisterX86 Temp64_2 => Temp8_2.WithWidth(RegisterWidth._64);
    //private RegisterX86 Temp16_2 => Temp8_2.WithWidth(RegisterWidth._16);

    public IEnumerable<X86Instruction> Convert(MosInstruction instruction, int offset, bool updateFlags)
    {
        if (options.UseArrayAsMem)
        {
            switch (instruction.AddressingMode)
            {
                case MosAddressingMode.Absolute:
                case MosAddressingMode.ZeroPage:
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, instruction.Operand, 1, "_memory"));
                    break;
                case MosAddressingMode.AbsoluteX:
                    yield return X86.Mov(Temp64, instruction.Operand);
                    yield return X86.Add(Temp16, X_16); // show do overflow for us
                    
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
                case MosAddressingMode.AbsoluteY:
                    yield return X86.Mov(Temp64, instruction.Operand);
                    yield return X86.Add(Temp16, Y_16); // show do overflow for us
                    
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
                case MosAddressingMode.Immediate:
                    // in naive, immediate is right in instruction.Operand
                    // but since the program can self change, it can change its immediate operand value
                    // so to be sure we need to load it from _memory array
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, (uint)offset + 1, 1, "_memory"));
                    break;
                case MosAddressingMode.Indirect:
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, instruction.Operand, 1, "_memory"));
                    if ((instruction.Operand & 0xFF) == 0xFF)
                        yield return X86.Lea(Temp64_2, X86.Indirect(0, (uint)(instruction.Operand & 0xFF00), 1, "_memory"));
                    break;
                case MosAddressingMode.IndirectX:
                    yield return X86.Mov(Temp64, instruction.Operand);
                    yield return X86.Add(Temp64, X_64);
                    yield return X86.And(Temp64, 0xFF);             // temp64 = (operand + X) & 0xFF;
                    yield return X86.Lea(Temp64_2, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(Temp64_2, Temp64); // Temp64_2 = &memory[(operand + X) & 0xFF];
                    yield return X86.Xor(OperandAddress_64, OperandAddress_64);
                    yield return X86.Mov(OperandAddress_8, X86.Indirect(Temp64_2)); // OperandAddress_8 = operandAddressLowByte = memory[(operand + X) & 0xFF]

                    yield return X86.Inc(Temp64);
                    yield return X86.And(Temp64, 0xFF);             // temp64 = (operand + X + 1) & 0xFF;
                    yield return X86.Lea(Temp64_2, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(Temp64_2, Temp64); // Temp64_2 = &memory[(operand + X + 1) & 0xFF];
                    yield return X86.Xor(Temp64, Temp64); // Temp64 = 0
                    yield return X86.Mov(Temp8, X86.Indirect(Temp64_2)); // Temp8 = operandAddressHighByte = memory[(operand + X + 1) & 0xFF]
                    yield return X86.Shl(Temp16, 8);
                    yield return X86.Or(OperandAddress_16, Temp16);
                    
                    yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
                case MosAddressingMode.IndirectY:
                    yield return X86.Xor(OperandAddress_64, OperandAddress_64);
                    
                    yield return X86.Lea(Temp64, X86.Indirect(0, (uint)((instruction.Operand + 1) & 0xFF), 1, "_memory")); // high
                    yield return X86.Mov(OperandAddress_8, X86.Indirect(Temp64));
                    yield return X86.Shl(OperandAddress_16, 8);
                    yield return X86.Lea(Temp64, X86.Indirect(0, instruction.Operand, 1, "_memory")); // low
                    yield return X86.Mov(OperandAddress_8, X86.Indirect(Temp64));
                    
                    yield return X86.Add(OperandAddress_64, Y_64);
                    yield return X86.And(OperandAddress_64, 0xFFFF);
                    yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
                case MosAddressingMode.ZeroPageX:
                    yield return X86.Mov(Temp64, instruction.Operand);
                    yield return X86.Add(Temp64, X_64);
                    yield return X86.And(Temp64, 0xFF);
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
                case MosAddressingMode.ZeroPageY:
                    yield return X86.Mov(Temp64, instruction.Operand);
                    yield return X86.Add(Temp64, Y_64);
                    yield return X86.And(Temp64, 0xFF);
                    yield return X86.Lea(OperandAddress_64, X86.Indirect(0, 0, 1, "_memory"));
                    yield return X86.Add(OperandAddress_64, Temp64);
                    break;
            }
        }
        else
        {
            switch (instruction.AddressingMode)
            {
                case MosAddressingMode.Absolute:
                case MosAddressingMode.ZeroPage:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    break;
                case MosAddressingMode.AbsoluteX:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DI, X_16);
                    break;
                case MosAddressingMode.AbsoluteY:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DI, Y_16);
                    break;
                case MosAddressingMode.Immediate:
                    // in naive, immediate is right in instruction.Operand
                    // but since the program can self change, it can change its immediate operand value
                    // so to be sure we need to load it from _memory array
                    yield return X86.Mov(RegisterX86.EDI, offset + 1);
                    break;
                case MosAddressingMode.Indirect:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Call("_getValue"); // RAX = memory[operand]
                    yield return X86.Push(RegisterX86.AX);
                    
                    if ((instruction.Operand & 0xFF) == 0xFF)
                    {
                        yield return X86.Mov(RegisterX86.EDI, instruction.Operand & 0xFF00);
                        yield return X86.Call("_getValue"); // RAX = memory[operand & 0xFF00]
                        yield return X86.Shl(RegisterX86.RAX, 8);// RAX = memory[operand & 0xFF00] << 8
                    }
                    else
                    {
                        yield return X86.Mov(RegisterX86.EDI, instruction.Operand + 1);
                        yield return X86.Call("_getValue"); // RAX = memory[operand + 1]
                        yield return X86.Shl(RegisterX86.RAX, 8); // RAX = memory[operand + 1] << 8
                    }
                    yield return X86.Pop(RegisterX86.DX); // DX = memory[operand]
                    yield return X86.Mov(RegisterX86.AL, RegisterX86.DL); // RAX = memory[operand + 1] << 8 | memory[operand]
                    yield return X86.Mov(Operand_16, RegisterX86.AX);
                    break;
                case MosAddressingMode.IndirectX:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DIL, X);
                    yield return X86.And(RegisterX86.EDI, 0xFF);             // EDI = (operand + X) & 0xFF;
                    yield return X86.Call("_getValue"); // RAX = operandAddressLowByte = memory[(operand + X) & 0xFF]

                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DIL, X);
                    yield return X86.Inc(RegisterX86.DIL);
                    yield return X86.And(RegisterX86.EDI, 0xFF);             // EDI = (operand + X + 1) & 0xFF;

                    yield return X86.Push(RegisterX86.AX);
                    yield return X86.Call("_getValue"); // RAX = operandAddressHighByte = memory[(operand + X + 1) & 0xFF]
                    yield return X86.Shl(RegisterX86.EAX, 8); // EAX = operandAddressHighByte << 8;
                    
                    yield return X86.Pop(RegisterX86.DI); //DI = lowbyte
                    yield return X86.Or(RegisterX86.DI, RegisterX86.AX); // DI = memory[(operand + X + 1) & 0xFF] << 8 | memory[(operand + X) & 0xFF]
                    break;
                case MosAddressingMode.IndirectY:
                    yield return X86.Mov(RegisterX86.EDI, ((instruction.Operand + 1) & 0xFF));
                    yield return X86.Call("_getValue"); // RAX = operandAddressHighByte = memory[(operand + 1) & 0xFF]

                    yield return X86.Shl(RegisterX86.EAX, 8); // RAX = highbyte << 8

                    yield return X86.Push(RegisterX86.AX);
                    yield return X86.Mov(RegisterX86.EDI, ((instruction.Operand) & 0xFF));
                    yield return X86.Call("_getValue"); // RAX = operandAddressLowByte = memory[(operand) & 0xFF]
                    
                    yield return X86.Pop(RegisterX86.DI); //DI = highByte << 8
                    yield return X86.Or(RegisterX86.DI, RegisterX86.AX); // DI = memory[(operand + 1) & 0xFF] << 8 | memory[(operand) & 0xFF]

                    yield return X86.Add(RegisterX86.DI, Y_16);
                    break;
                case MosAddressingMode.ZeroPageX:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DIL, X);
                    yield return X86.And(RegisterX86.EDI, 0xFF);
                    break;
                case MosAddressingMode.ZeroPageY:
                    yield return X86.Mov(RegisterX86.EDI, instruction.Operand);
                    yield return X86.Add(RegisterX86.DIL, Y);
                    yield return X86.And(RegisterX86.EDI, 0xFF);
                    break;
            }

            switch (instruction.AddressingMode)
            {
                case MosAddressingMode.Accumulator:
                    yield return X86.Mov(Operand_64, Accumulator_64);
                    break;
                case MosAddressingMode.Absolute:
                case MosAddressingMode.AbsoluteX:
                case MosAddressingMode.AbsoluteY:
                case MosAddressingMode.Immediate:
                case MosAddressingMode.IndirectX:
                case MosAddressingMode.IndirectY:
                case MosAddressingMode.ZeroPage:
                case MosAddressingMode.ZeroPageX:
                case MosAddressingMode.ZeroPageY:
                    // EDI = operandAddress
                    if (!(instruction.Opcode is MosOpcode.STX or MosOpcode.STA or MosOpcode.STY))
                    {
                        yield return X86.Push(RegisterX86.DI); // operand address
                        yield return X86.Call("_getValue"); // RAX = operandValue
                        yield return X86.Mov(Operand_64, RegisterX86.RAX);
                        yield return X86.Pop(OperandAddress_16);                        
                    }
                    else
                        yield return X86.Mov(OperandAddress_16, RegisterX86.DI); // operand address
                    break;
                case MosAddressingMode.Relative:
                case MosAddressingMode.Indirect:
                    break;
                default:
                    yield return X86.Xor(Operand_64, Operand_64);
                    break;
            }
        }

        // highly optimized implementations
        if (!updateFlags && options.UseArrayAsMem)
        {
            if (instruction.Opcode == MosOpcode.INC)
            {
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                {
                    yield return X86.Inc(Accumulator);
                    yield break;
                }
                else
                {
                    yield return X86.Inc(X86.Indirect(OperandAddress_64), RegisterWidth._8);
                    yield break;
                }
            }
        }

        if (options.UseArrayAsMem)
        {
            switch (instruction.AddressingMode)
            {
                case MosAddressingMode.Accumulator:
                    yield return X86.Mov(Operand_64, Accumulator_64);
                    break;
                case MosAddressingMode.Absolute:
                case MosAddressingMode.AbsoluteX:
                case MosAddressingMode.AbsoluteY:
                    yield return X86.Xor(Operand_64, Operand_64);
                    yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                    break;
                case MosAddressingMode.Immediate:
                    // native:
                    // yield return X86.Mov(Operand_8, instruction.Operand);
                    // self modyfing compliant:
                    yield return X86.Xor(Operand_64, Operand_64);
                    yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                    break;
                case MosAddressingMode.Indirect:
                    yield return X86.Xor(Operand_64, Operand_64);
                    if ((instruction.Operand & 0xFF) == 0xFF)
                    {
                        yield return X86.Xor(Temp64, Temp64);
                        yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                        yield return X86.Mov(Temp8, X86.Indirect(Temp64_2));
                        yield return X86.Shl(Temp16, 8);
                        yield return X86.Or(Operand_16, Temp16);
                    }
                    else
                        yield return X86.Mov(Operand_16, X86.Indirect(OperandAddress_64));
                    break;
                case MosAddressingMode.IndirectX:
                case MosAddressingMode.IndirectY:
                    yield return X86.Xor(Operand_64, Operand_64);
                    yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                    //yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                    break;
                case MosAddressingMode.Relative:
                    break;
                case MosAddressingMode.ZeroPage:
                case MosAddressingMode.ZeroPageX:
                case MosAddressingMode.ZeroPageY:
                    yield return X86.Xor(Operand_64, Operand_64);
                    yield return X86.Mov(Operand_8, X86.Indirect(OperandAddress_64));
                    break;
                default:
                    yield return X86.Xor(Operand_64, Operand_64);
                    break;
            }   
        }

        switch (instruction.Opcode)
        {
            case MosOpcode.ADC:
            {
                if (options.UseNativeFlags)
                {
                    foreach (var instr in TransferMosCarryFlag())
                        yield return instr;
                    
                    yield return X86.Adc(Accumulator, Operand_8);

                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z | Mos6502SR.C | Mos6502SR.N | Mos6502SR.V, updateFlags))
                        yield return instr;
                }
                else
                {

                    var isCarryLabel = GetNextLabel();
                    var noOverflowLabel = GetNextLabel();
                    var outLabel = GetNextLabel();
                    yield return X86.Mov(Temp64, Operand_64);
                    yield return X86.And(Temp64, 0xFF);
                    yield return X86.Add(Temp64, Accumulator_64);
                    yield return X86.Xor(Temp64_2, Temp64_2);
                    yield return X86.Test(Flags, (int)Mos6502SR.C); // is carry flag?
                    yield return X86.ConditionalSetNonZero(Temp8_2);
                    yield return X86.Add(Temp64, Temp64_2); // if there is, +1

                    if (updateFlags)
                    {
                        // CARRY FLAG
                        yield return X86.Cmp(Temp64, 0x100);
                        yield return X86.JumpGreaterEqual(isCarryLabel);
                        yield return X86.And(Flags, ~(byte)Mos6502SR.C & 0xFF);
                        yield return X86.Jump(outLabel);

                        yield return X86.Label(isCarryLabel);
                        yield return X86.Or(Flags, (byte)Mos6502SR.C);
                        yield return X86.Label(outLabel);
                        // END CARRY FLAG

                        // OVERFLOW FLAG
                        outLabel = GetNextLabel();
                        // ((A ^ m) & 0x80) == 0
                        yield return X86.Mov(Temp64_2, Accumulator_64);
                        yield return X86.Xor(Temp64_2, Operand_64);
                        yield return X86.Test(Temp64_2, 0x80);
                        yield return X86.JumpNonZero(noOverflowLabel);

                        // ((A ^ tmp) & 0x80) != 0
                        yield return X86.Mov(Temp64_2, Accumulator_64);
                        yield return X86.Xor(Temp64_2, Temp64);
                        yield return X86.Test(Temp64_2, 0x80);
                        yield return X86.JumpZero(noOverflowLabel);
                        yield return X86.Or(Flags, (byte)Mos6502SR.V);
                        yield return X86.Jump(outLabel);

                        yield return X86.Label(noOverflowLabel);
                        yield return X86.And(Flags, ~(byte)Mos6502SR.V & 0xFF);
                        yield return X86.Jump(outLabel);
                        yield return X86.Label(outLabel);
                        // END OVERFLOW FLAG   
                    }

                    yield return X86.Mov(Accumulator, Temp8); // save result to A

                    foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                        yield return instr;

                    yield return X86.Xor(Temp64, Temp64);
                    yield return X86.Xor(Temp64_2, Temp64_2);
                }

                break;
            }
            case MosOpcode.AND:
                yield return X86.And(Accumulator, Operand_8);
                foreach (var instr in TransferFlags(Accumulator, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                
                break;
            case MosOpcode.ASL:
            {
                var labelNoCarry = GetNextLabel();
                var outLabel = GetNextLabel();
                yield return X86.Shl(Operand_8, 1);
                if (options.UseNativeFlags)
                {
                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z | Mos6502SR.C | Mos6502SR.N, updateFlags))
                        yield return instr;
                }
                else
                {
                    yield return X86.JumpIfNoCarry(labelNoCarry);
                    yield return X86.Or(Flags, (byte)Mos6502SR.C);
                    yield return X86.Jump(outLabel);
                    yield return X86.Label(labelNoCarry);
                    yield return X86.And(Flags, (byte)((byte)~Mos6502SR.C & 0xFF));
                    yield return X86.Label(outLabel);
                
                    foreach (var instr in UpdateZeroNegative(Operand_8, updateFlags))
                        yield return instr;
                }
                
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                    yield return X86.Mov(Accumulator, Operand_8);
                else if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Operand_64);
                    yield return X86.Call("_setValue");
                }
                break;
            }
            case MosOpcode.BCC:
                yield return X86.Test(Flags, (int)Mos6502SR.C);
                foreach (var i in DoJump(X86.JumpZero, X86.JumpNonZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BCS:
                yield return X86.Test(Flags, (int)Mos6502SR.C);
                foreach (var i in DoJump(X86.JumpNonZero, X86.JumpZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BEQ:
                yield return X86.Test(Flags, (int)Mos6502SR.Z);
                foreach (var i in DoJump(X86.JumpNonZero, X86.JumpZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BIT:
                yield return X86.Mov(Temp8, Accumulator);
                yield return X86.And(Temp8, Operand_8);
                foreach (var instr in TransferFlags(Temp8, Mos6502SR.Z, updateFlags))
                    yield return instr;

                if (updateFlags)
                {
                    yield return X86.And(Flags, 0x3F);
                    yield return X86.And(Operand_8, 0xC0);
                    yield return X86.Or(Flags, Operand_8);
                    yield return X86.Or(Flags, (int)Mos6502SR.B | (int)Mos6502SR.Unused);   
                }
                break;
            case MosOpcode.BMI:
                yield return X86.Test(Flags, (int)Mos6502SR.N);
                foreach (var i in DoJump(X86.JumpNonZero, X86.JumpZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BNE:
                yield return X86.Test(Flags, (int)Mos6502SR.Z);
                foreach (var i in DoJump(X86.JumpZero, X86.JumpNonZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BPL:
                yield return X86.Test(Flags, (int)Mos6502SR.N);
                foreach (var i in DoJump(X86.JumpZero, X86.JumpNonZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BRK:
                // push IP
                if (options.UseArrayAsMem)
                {
                    yield return X86.Mov(Accumulator, (offset + 1) >> 8);
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Accumulator);
                    yield return X86.Dec(StackPointer);
                    yield return X86.Mov(Accumulator, (offset + 1) & 0xFF);
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Accumulator);
                    yield return X86.Dec(StackPointer);
                
                    // push flags | B | Unused
                    yield return X86.Mov(Temp8, Flags);
                    yield return X86.Or(Temp8, (byte)(Mos6502SR.B | Mos6502SR.Unused));
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Temp8);
                    yield return X86.Dec(StackPointer);   
                }
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, (offset + 1) >> 8);
                    yield return X86.Call("_setValue");
                    yield return X86.Dec(StackPointer);
                    
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, (offset + 1) & 0xFF);
                    yield return X86.Call("_setValue");
                    yield return X86.Dec(StackPointer);
                    
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, Flags_64);
                    yield return X86.Or(RegisterX86.RSI, (byte)(Mos6502SR.B | Mos6502SR.Unused));
                    yield return X86.Call("_setValue");
                    yield return X86.Dec(StackPointer);
                }

                // set I flag
                yield return X86.Or(Flags, (byte)Mos6502SR.I);
                
                // jump to instr from interrupt vector
                if (options.UseArrayAsMem)
                {
                    yield return X86.Xor(Operand_64, Operand_64);
                    yield return X86.Mov(Operand_16, X86.Indirect(0, options.BrkInterruptAddress, 1, "_memory"));
                    yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "addrMap"));
                    yield return X86.Jump($"[{Temp64} + 8 * {Operand_64}]");
                }
                else
                {
                    yield return X86.Mov(RegisterX86.EDI, options.BrkInterruptAddress);
                    yield return X86.Call("_getValue16"); // RAX = memory[BRK_INTERRUPT_ADDRESS]
                    if (options.CycleMethod)
                    {
                        foreach (var i in LeaveFromCycle(RegisterX86.AX, instruction))
                            yield return i;
                    }
                    else
                    {
                        yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "addrMap"));
                        yield return X86.Jump($"[{Temp64} + 8 * {RegisterX86.RAX}]");
                    }
                }
                break;
            case MosOpcode.BVC:
                yield return X86.Test(Flags, (int)Mos6502SR.V);
                foreach (var i in DoJump(X86.JumpZero, X86.JumpNonZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.BVS:
                yield return X86.Test(Flags, (int)Mos6502SR.V);
                foreach (var i in DoJump(X86.JumpNonZero, X86.JumpZero, instruction, offset))
                    yield return i;
                break;
            case MosOpcode.CLC:
                yield return X86.And(Flags, ~(byte)Mos6502SR.C & 0xFF);
                break;
            case MosOpcode.CLD:
                yield return X86.And(Flags, ~(byte)Mos6502SR.D & 0xFF);
                break;
            case MosOpcode.CLI:
                yield return X86.And(Flags, ~(byte)Mos6502SR.I & 0xFF);
                break;
            case MosOpcode.CLV:
                yield return X86.And(Flags, ~(byte)Mos6502SR.V & 0xFF);
                break;
            case MosOpcode.CMP:
            {
                foreach (var x86Instruction in ConvertCompare(Accumulator, updateFlags))
                    yield return x86Instruction;
                break;
            }
            case MosOpcode.CPX:
            {
                foreach (var x86Instruction in ConvertCompare(X, updateFlags))
                    yield return x86Instruction;
                break;
            }
            case MosOpcode.CPY:
            {
                foreach (var x86Instruction in ConvertCompare(Y, updateFlags))
                    yield return x86Instruction;
                break;
            }
            case MosOpcode.DEC:
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                {
                    yield return X86.Dec(Accumulator);
                    foreach (var instr in TransferFlags(Accumulator, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                        yield return instr;
                }
                else
                {
                    yield return X86.Dec(Operand_8);
                    foreach (var instr in TransferFlags(Operand_8, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                        yield return instr;
                    
                    if (options.UseArrayAsMem)
                        yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                    else
                    {
                        yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                        yield return X86.Mov(RegisterX86.RSI, Operand_64);
                        yield return X86.Call("_setValue");
                    }
                }
                break;
            case MosOpcode.DEX:
                yield return X86.Dec(X);
                foreach (var instr in TransferFlags(X, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.DEY:
                yield return X86.Dec(Y);
                foreach (var instr in TransferFlags(Y, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.EOR:
                yield return X86.Xor(Accumulator, Operand_8);
                foreach (var instr in TransferFlags(Accumulator, Mos6502SR.N | Mos6502SR.Z, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.INC:
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                {
                    yield return X86.Inc(Accumulator);
                    foreach (var instr in TransferFlags(Accumulator, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                        yield return instr;
                }
                else
                {
                    yield return X86.Inc(Operand_8);
                    foreach (var instr in TransferFlags(Operand_8, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                        yield return instr;
                    
                    if (options.UseArrayAsMem)
                        yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                    else
                    {
                        yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                        yield return X86.Mov(RegisterX86.RSI, Operand_64);
                        yield return X86.Call("_setValue");
                    }
                }
                break;
            case MosOpcode.INX:
                yield return X86.Inc(X);
                foreach (var instr in TransferFlags(X, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.INY:
                yield return X86.Inc(Y);
                foreach (var instr in TransferFlags(Y, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.JMP:
                if (instruction.AddressingMode == MosAddressingMode.Absolute)
                {
                    if (options.CycleMethod)
                    {
                        yield return (X86.Add(CycleCounter, instruction.CyclesCount()));
                        yield return (X86.Mov(Temp16, instruction.Operand));
                        yield return (X86.Ret());
                    }
                    else
                    {
                        yield return X86.Jump(GetLabelForJump(instruction.Operand)); // no self mod
                    }
                }
                else
                {
                    if (options.CycleMethod)
                    {
                        yield return (X86.Add(CycleCounter, instruction.CyclesCount()));
                        yield return (X86.Mov(Temp16, Operand_16));
                        yield return (X86.Ret());
                    }
                    else
                    {
                        yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "addrMap"));
                        yield return X86.And(Operand_64, 0xFFFF);
                        yield return X86.Jump($"[{Temp64} + 8 * {Operand_64}]");   
                    }
                }
                break;
            case MosOpcode.JSR:
                if (options.UseArrayAsMem)
                {
                    yield return X86.Mov(Temp8, (offset + 2) >> 8);
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Temp8);
                    yield return X86.Dec(StackPointer);
                    yield return X86.Mov(Temp8, (offset + 2) & 0xFF);
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Temp8);
                    yield return X86.Dec(StackPointer);
                }
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, (offset + 2) >> 8);
                    yield return X86.Call("_setValue");
                    yield return X86.Dec(StackPointer);
                    
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, (offset + 2) & 0xFF);
                    yield return X86.Call("_setValue");
                    yield return X86.Dec(StackPointer);
                }
                if (options.UseNativeCallAsJsr)
                    yield return X86.Call(GetLabelForJump(instruction.Operand));
                else
                {
                    if (options.CycleMethod)
                    {
                        foreach (var ins in LeaveFromCycle(instruction.Operand, instruction))
                            yield return (ins);
                    }
                    else
                        yield return X86.Jump(GetLabelForJump(instruction.Operand));
                }
                break;
            case MosOpcode.LDA:
                yield return X86.Mov(Accumulator, Operand_8);
                foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.LDX:
                yield return X86.Mov(X, Operand_8);
                foreach (var instr in UpdateZeroNegative(X, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.LDY:
                yield return X86.Mov(Y, Operand_8);
                foreach (var instr in UpdateZeroNegative(Y, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.LSR:
            {
                var labelNoCarry = GetNextLabel();
                var outLabel = GetNextLabel();
                yield return X86.Shr(Operand_8, 1);
                if (options.UseNativeFlags)
                {
                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z | Mos6502SR.C | Mos6502SR.N, updateFlags))
                        yield return instr;
                }
                else
                {
                    if (updateFlags)
                    {
                        yield return X86.JumpIfNoCarry(labelNoCarry);
                        yield return X86.Or(Flags, (byte)Mos6502SR.C);
                        yield return X86.Jump(outLabel);
                        yield return X86.Label(labelNoCarry);
                        yield return X86.And(Flags, (byte)((byte)~Mos6502SR.C & 0xFF));
                        yield return X86.Label(outLabel);
                        yield return X86.And(Flags, (byte)((byte)~Mos6502SR.N & 0xFF));   
                    }
                    foreach (var instr in UpdateZero(Operand_8, updateFlags))
                        yield return instr;   
                }
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                    yield return X86.Mov(Accumulator, Operand_8);
                else if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Operand_64);
                    yield return X86.Call("_setValue");
                }
                break;
            }
            case MosOpcode.NOP:
                yield return X86.Nop();
                break;
            case MosOpcode.ORA:
                yield return X86.Or(Accumulator, Operand_8);
                foreach (var instr in TransferFlags(Accumulator, Mos6502SR.Z | Mos6502SR.N, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.PHA:
                if (options.UseArrayAsMem)
                {
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Accumulator);
                }
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, Accumulator_64);
                    yield return X86.Call("_setValue");
                }
                yield return X86.Dec(StackPointer);
                break;
            case MosOpcode.PHP:
                yield return X86.Mov(Temp8, Flags);
                yield return X86.Or(Temp8, (byte)(Mos6502SR.B | Mos6502SR.Unused));
                if (options.UseArrayAsMem)
                {
                    yield return X86.Mov(X86.Indirect(StackPointer_64), Temp8);
                }
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Mov(RegisterX86.RSI, Temp64);
                    yield return X86.Call("_setValue");
                }
                yield return X86.Dec(StackPointer);
                break;
            case MosOpcode.PLA:
                yield return X86.Inc(StackPointer);
                if (options.UseArrayAsMem)
                    yield return X86.Mov(Accumulator, X86.Indirect(StackPointer_64));
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Call("_getValue");
                    yield return X86.Mov(Accumulator, RegisterX86.AL);
                }
                foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.PLP:
                yield return X86.Inc(StackPointer);
                if (options.UseArrayAsMem)
                    yield return X86.Mov(Flags, X86.Indirect(StackPointer_64));
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Call("_getValue");
                    yield return X86.Mov(Flags, RegisterX86.AL);
                }
                yield return X86.Or(Flags, (byte)(Mos6502SR.B | Mos6502SR.Unused));
                break;
            case MosOpcode.ROL:
            {
                if (options.UseNativeFlags)
                {
                    foreach (var instr in TransferMosCarryFlag())
                        yield return instr;
                    
                    yield return X86.Rcl(Operand_8, 1);
                    
                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.C, updateFlags))
                        yield return instr;
                }
                else
                {
                    yield return X86.Mov(Temp64, Flags_64);
                    yield return X86.And(Temp64, (int)Mos6502SR.C);
                    var labelNoCarry = GetNextLabel();
                    var outLabel = GetNextLabel();
                    yield return X86.Shl(Operand_8, 1);
                    if (updateFlags)
                    {
                        yield return X86.JumpIfNoCarry(labelNoCarry);
                        yield return X86.Or(Flags, (byte)Mos6502SR.C);
                        yield return X86.Jump(outLabel);
                        yield return X86.Label(labelNoCarry);
                        yield return X86.And(Flags, (byte)((byte)~Mos6502SR.C & 0xFF));
                        yield return X86.Label(outLabel);
                    }
                    yield return X86.Or(Operand_8, Temp8); // or with carry flag
                }
                
                foreach (var instr in UpdateZeroNegative(Operand_8, updateFlags))
                    yield return instr;
                
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                    yield return X86.Mov(Accumulator, Operand_8);
                else if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Operand_64);
                    yield return X86.Call("_setValue");
                }
                break;
            }
            case MosOpcode.ROR:
            {
                if (options.UseNativeFlags)
                {
                    foreach (var instr in TransferMosCarryFlag())
                        yield return instr;
                    
                    yield return X86.Rcr(Operand_8, 1);
                    
                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.C, updateFlags))
                        yield return instr;
                    
                }
                else
                {
                    yield return X86.Mov(Temp64, Flags_64);
                    yield return X86.And(Temp64, (int)Mos6502SR.C);
                    yield return X86.Shl(Temp64, 7); // calculate if carry will bring the most significant bit
                    yield return X86.Shr(Operand_8, 1);
                    if (updateFlags)
                    {
                        var labelNoCarry = GetNextLabel();
                        var outLabel = GetNextLabel();
                        yield return X86.JumpIfNoCarry(labelNoCarry);
                        yield return X86.Or(Flags, (byte)Mos6502SR.C);
                        yield return X86.Jump(outLabel);
                        yield return X86.Label(labelNoCarry);
                        yield return X86.And(Flags, (byte)((byte)~Mos6502SR.C & 0xFF));
                        yield return X86.Label(outLabel);
                    }

                    yield return X86.Or(Operand_8, Temp8); // or with carry flag
                }
                
                foreach (var instr in UpdateZeroNegative(Operand_8, updateFlags))
                    yield return instr;
                
                if (instruction.AddressingMode == MosAddressingMode.Accumulator)
                    yield return X86.Mov(Accumulator, Operand_8);
                else if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Operand_8);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Operand_64);
                    yield return X86.Call("_setValue");
                }

                break;
            }
            case MosOpcode.RTI:
                if (options.UseArrayAsMem)
                {
                    // pop status flags
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(Flags, X86.Indirect(StackPointer_64));
                    yield return X86.Or(Flags, (byte)(Mos6502SR.B | Mos6502SR.Unused));

                    yield return X86.Xor(Operand_64, Operand_64);
                    // pop IP low byte
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(Temp8, X86.Indirect(StackPointer_64));
                
                    // pop IP high byte
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(Operand_8, X86.Indirect(StackPointer_64));
                    yield return X86.Shl(Operand_16, 8);
                    yield return X86.Mov(Operand_8, Temp8);
                }
                else
                {
                    // pop status flags
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Call("_getValue");
                    yield return X86.Mov(Flags, RegisterX86.AL);
                    yield return X86.Or(Flags, (byte)(Mos6502SR.B | Mos6502SR.Unused));

                    
                    yield return X86.Xor(Operand_64, Operand_64);
                    // pop IP low byte
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Call("_getValue");
                    yield return X86.Mov(Temp8, RegisterX86.AL);
                    yield return X86.Push(Temp16);
                    
                
                    // pop IP high byte
                    yield return X86.Inc(StackPointer);
                    yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                    yield return X86.Call("_getValue");
                    yield return X86.Mov(Operand_8, RegisterX86.AL);
                    yield return X86.Shl(Operand_16, 8);
                    yield return X86.Pop(Temp16);
                    yield return X86.Mov(Operand_8, Temp8);
                }

                if (options.CycleMethod)
                {
                    foreach (var ins in LeaveFromCycle(Operand_16, instruction))
                        yield return (ins);
                }
                else
                {
                    yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "addrMap"));
                    yield return X86.Jump($"[{Temp64} + 8 * {Operand_64}]");
                }
                break;
            case MosOpcode.RTS:
                if (options.UseNativeCallAsJsr)
                {
                    yield return X86.Inc(StackPointer);
                    yield return X86.Inc(StackPointer);
                    yield return X86.Ret();
                }
                else
                {
                    yield return X86.Xor(Operand_64, Operand_64);
                    if (options.UseArrayAsMem)
                    {
                        // pop IP low byte
                        yield return X86.Inc(StackPointer);
                        yield return X86.Mov(Temp8, X86.Indirect(StackPointer_64));
                
                        // pop IP high byte
                        yield return X86.Inc(StackPointer);
                        yield return X86.Mov(Operand_8, X86.Indirect(StackPointer_64));
                        yield return X86.Shl(Operand_16, 8);
                        yield return X86.Mov(Operand_8, Temp8);                        
                    }
                    else
                    {
                        // pop IP low byte
                        yield return X86.Inc(StackPointer);
                        yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                        yield return X86.Call("_getValue");
                        yield return X86.Mov(Temp8, RegisterX86.AL);
                        yield return X86.Push(Temp16);
                        
                        // pop IP high byte
                        yield return X86.Inc(StackPointer);
                        yield return X86.Mov(RegisterX86.RDI, StackPointer_64);
                        yield return X86.Call("_getValue");
                        yield return X86.Mov(Operand_8, RegisterX86.AL);
                        
                        yield return X86.Shl(Operand_16, 8);
                        yield return X86.Pop(Temp16);

                        yield return X86.Mov(Operand_8, Temp8);    
                    }

                    
                    yield return X86.Inc(Operand_64); // IP++
                
                    if (options.CycleMethod)
                    {
                        yield return (X86.Add(CycleCounter, instruction.CyclesCount()));
                        yield return (X86.Mov(Temp16, Operand_16));
                        yield return (X86.Ret());
                    }
                    
                    yield return X86.Lea(Temp64, X86.Indirect(0, 0, 1, "addrMap"));
                    yield return X86.Jump($"[{Temp64} + 8 * {Operand_64}]");
                }
                break;
            case MosOpcode.SBC:
            {
                if (options.UseNativeFlags)
                {
                    yield return X86.Not(Operand_8);
                    
                    foreach (var instr in TransferMosCarryFlag())
                        yield return instr;

                    yield return X86.Adc(Accumulator, Operand_8);

                    foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z | Mos6502SR.C | Mos6502SR.N | Mos6502SR.V, updateFlags))
                        yield return instr;
                }
                else
                {
                    var isCarryLabel = GetNextLabel();
                    var noOverflowLabel = GetNextLabel();
                    var outLabel = GetNextLabel();
                    yield return X86.Mov(Temp64, Accumulator_64);
                    yield return X86.And(Temp64, 0xFF);
                    yield return X86.Sub(Temp64, Operand_64);
                    yield return X86.Xor(Temp64_2, Temp64_2);
                    yield return X86.Test(Flags, (int)Mos6502SR.C); // is carry flag?
                    yield return X86.ConditionalSetZero(Temp8_2);
                    yield return X86.Sub(Temp64, Temp64_2); // if there is, +1

                    if (updateFlags)
                    {
                        // CARRY FLAG
                        yield return X86.Cmp(Temp64, 0x100);
                        yield return X86.JumpLowerUnsigned(isCarryLabel);
                        yield return X86.And(Flags, ~(byte)Mos6502SR.C & 0xFF);
                        yield return X86.Jump(outLabel);

                        yield return X86.Label(isCarryLabel);
                        yield return X86.Or(Flags, (byte)Mos6502SR.C);
                        yield return X86.Label(outLabel);
                        // END CARRY FLAG
                    
                        // OVERFLOW FLAG
                        outLabel = GetNextLabel();
                        // ((A ^ m) & 0x80) != 0
                        yield return X86.Mov(Temp64_2, Accumulator_64);
                        yield return X86.Xor(Temp64_2, Operand_64);
                        yield return X86.Test(Temp64_2, 0x80);
                        yield return X86.JumpZero(noOverflowLabel);
                    
                        // ((A ^ tmp) & 0x80) != 0
                        yield return X86.Mov(Temp64_2, Accumulator_64);
                        yield return X86.Xor(Temp64_2, Temp64);
                        yield return X86.Test(Temp64_2, 0x80);
                        yield return X86.JumpZero(noOverflowLabel);
                        yield return X86.Or(Flags, (byte)Mos6502SR.V);
                        yield return X86.Jump(outLabel);
                    
                        yield return X86.Label(noOverflowLabel);
                        yield return X86.And(Flags, ~(byte)Mos6502SR.V & 0xFF);
                        yield return X86.Jump(outLabel);
                        yield return X86.Label(outLabel);
                        // END OVERFLOW FLAG
                    }
                    
                    yield return X86.Mov(Accumulator, Temp8); // save result to A
                    foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                        yield return instr;
                    
                    yield return X86.Xor(Temp64, Temp64);
                    yield return X86.Xor(Temp64_2, Temp64_2);   
                }
                break;
            }
            case MosOpcode.SEC:
                if (updateFlags)
                    yield return X86.Or(Flags, (int)Mos6502SR.C);
                break;
            case MosOpcode.SED:
                if (updateFlags)
                    yield return X86.Or(Flags, (int)Mos6502SR.D);
                break;
            case MosOpcode.SEI:
                if (updateFlags)
                    yield return X86.Or(Flags, (int)Mos6502SR.I);
                break;
            case MosOpcode.STA:
                if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Accumulator);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Accumulator_64);
                    yield return X86.Call("_setValue");
                }
                break;
            case MosOpcode.STX:
                if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), X);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, X_64);
                    yield return X86.Call("_setValue");
                }
                break;
            case MosOpcode.STY:
                if (options.UseArrayAsMem)
                    yield return X86.Mov(X86.Indirect(OperandAddress_64), Y);
                else
                {
                    yield return X86.Mov(RegisterX86.RDI, OperandAddress_64);
                    yield return X86.Mov(RegisterX86.RSI, Y_64);
                    yield return X86.Call("_setValue");
                }
                break;
            case MosOpcode.TAX:
                yield return X86.Mov(X, Accumulator);
                foreach (var instr in UpdateZeroNegative(X, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.TAY:
                yield return X86.Mov(Y, Accumulator);
                foreach (var instr in UpdateZeroNegative(Y, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.TSX:
                yield return X86.Mov(X, StackPointer);
                foreach (var instr in UpdateZeroNegative(X, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.TXA:
                yield return X86.Mov(Accumulator, X);
                foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.TXS:
                yield return X86.Mov(StackPointer, X);
                break;
            case MosOpcode.TYA:
                yield return X86.Mov(Accumulator, Y);
                foreach (var instr in UpdateZeroNegative(Accumulator, updateFlags))
                    yield return instr;
                break;
            case MosOpcode.HLT:
                yield return X86.Jump("nesend");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerable<X86Instruction> DoJump(Func<string, X86Instruction> jumpExpected, Func<string, X86Instruction> dontJump, MosInstruction instruction, int offset)
    {
        if (options.CycleMethod)
        {
            var doJump = GetNextLabel();
            var outLabel = GetNextLabel();
            yield return jumpExpected(doJump);
            yield return X86.Jump(outLabel);
            //yield return dontJump(outLabel);
            
            yield return X86.Label(doJump);
            foreach (var i in LeaveFromCycle(offset + 2 + (sbyte)instruction.Operand, instruction))
                yield return i;
                    
            yield return X86.Label(outLabel);
        }
        else
            yield return jumpExpected(GetLabelForJump(offset + 2 + (sbyte)instruction.Operand));
    }

    private IEnumerable<X86Instruction> ConvertCompare(RegisterX86 registerX86, bool updateFlags)
    {
        if (!updateFlags)
            yield break;
        
        var notNegative = GetNextLabel();
        var outLabel = GetNextLabel();
        yield return X86.Xor(Temp64, Temp64);
        yield return X86.Mov(Temp8, registerX86);
        yield return X86.Sub(Temp8, Operand_8);
        yield return X86.Test(Temp8, 0x80);
        yield return X86.JumpZero(notNegative);
        yield return X86.Or(Flags, (byte)Mos6502SR.N);
        yield return X86.Jump(outLabel);
        yield return X86.Label(notNegative);
        yield return X86.And(Flags, ~(byte)Mos6502SR.N & 0xFF);
        yield return X86.Label(outLabel);

        var isEqualLabel = GetNextLabel();
        outLabel = GetNextLabel();
        yield return X86.Cmp(registerX86, Operand_8);
        yield return X86.JumpEqual(isEqualLabel);
        yield return X86.And(Flags, ~(byte)Mos6502SR.Z & 0xFF);
        yield return X86.Jump(outLabel);
        yield return X86.Label(isEqualLabel);
        yield return X86.Or(Flags, (byte)Mos6502SR.Z);
        yield return X86.Label(outLabel);

        var isGreaterEqualLabel = GetNextLabel();
        outLabel = GetNextLabel();
        yield return X86.Cmp(registerX86, Operand_8);
        yield return X86.JumpGreaterEqualUnsigned(isGreaterEqualLabel);
        yield return X86.And(Flags, ~(byte)Mos6502SR.C & 0xFF);
        yield return X86.Jump(outLabel);
        yield return X86.Label(isGreaterEqualLabel);
        yield return X86.Or(Flags, (byte)Mos6502SR.C);
        yield return X86.Label(outLabel);
    }

    private IEnumerable<X86Instruction> TransferFlags(RegisterX86 reg, Mos6502SR flags, bool updateFlags)
    {
        if (!updateFlags)
            yield break;

        if (options.UseNativeFlags)
        {
            foreach (var instr in TransferNativeFlagsToMOS(flags, updateFlags))
                yield return instr;
        }
        else
        {
            if (flags == Mos6502SR.Z)
            {
                foreach (var instr in UpdateZero(reg, updateFlags))
                    yield return instr;
            }
            else if (flags == (Mos6502SR.Z | Mos6502SR.N))
            {
                foreach (var instr in UpdateZeroNegative(reg, updateFlags))
                    yield return instr;
            }
            else
            {
                throw new Exception("Invalid flags combination");
            }
        }
    }
    
    private IEnumerable<X86Instruction> UpdateZero(RegisterX86 reg, bool updateFlags)
    {
        if (!updateFlags)
            yield break;
        
        yield return X86.Test(reg, reg);
        foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z, updateFlags))
            yield return instr;
        
        // var ifNonZeroLabel = GetNextLabel();
        // var outLabel = GetNextLabel();
        // yield return X86.Test(reg, reg);
        // yield return X86.JumpNonZero(ifNonZeroLabel);
        // yield return X86.Or(Flags, (int)Mos6502SR.Z);
        // yield return X86.Jump(outLabel);
        // yield return X86.Label(ifNonZeroLabel);
        // yield return X86.And(Flags, ~(byte)Mos6502SR.Z & 0xFF);
        // yield return X86.Label(outLabel);
    }
    
    private IEnumerable<X86Instruction> UpdateZeroNegative(RegisterX86 reg, bool updateFlags)
    {
        if (!updateFlags)
            yield break;
        
        yield return X86.Test(reg, reg);
        foreach (var instr in TransferNativeFlagsToMOS(Mos6502SR.Z | Mos6502SR.N, updateFlags))
            yield return instr;
        // var ifNonZeroLabel = GetNextLabel();
        // var ifNegative = GetNextLabel();
        // var outLabel = GetNextLabel();
        // yield return X86.JumpNonZero(ifNonZeroLabel);
        // yield return X86.Or(Flags, (int)Mos6502SR.Z);
        // yield return X86.And(Flags, ~(byte)Mos6502SR.N & 0xFF);
        // yield return X86.Jump(outLabel);
        //
        // yield return X86.Label(ifNonZeroLabel);
        // yield return X86.JumpSign(ifNegative);
        //
        // yield return X86.And(Flags, ~(byte)(Mos6502SR.N | Mos6502SR.Z) & 0xFF);
        // yield return X86.Jump(outLabel);
        //
        // yield return X86.Label(ifNegative);
        // yield return X86.Or(Flags, (int)Mos6502SR.N);
        // yield return X86.And(Flags, ~(byte)Mos6502SR.Z & 0xFF);
        //
        // yield return X86.Label(outLabel);
    }
    
    private IEnumerable<X86Instruction> TransferMosCarryFlag()
    {
        var afterIsCarryLabel = GetNextLabel();
        
        yield return X86.ClearCarry();
        yield return X86.Test(Flags, (int)Mos6502SR.C); // is carry flag?
        yield return X86.JumpZero(afterIsCarryLabel);
        yield return X86.SetCarry();
                
        yield return X86.Label(afterIsCarryLabel);
    }

    private IEnumerable<X86Instruction> TransferNativeFlagsToMOS(Mos6502SR whatFlags, bool updateFlags)
    {
        if (!updateFlags)
            yield break;
        
        whatFlags = whatFlags & (Mos6502SR.Z | Mos6502SR.V | Mos6502SR.N | Mos6502SR.C);
        
        if (whatFlags.HasFlag(Mos6502SR.C))
            yield return X86.ConditionalSetCarry(Temp8);
        if (whatFlags.HasFlag(Mos6502SR.Z))
            yield return X86.ConditionalSetZero(Temp8_2);
        if (whatFlags.HasFlag(Mos6502SR.N))
            yield return X86.ConditionalSetSigned(Temp8_3);
        if (whatFlags.HasFlag(Mos6502SR.V))
            yield return X86.ConditionalSetOverflow(Temp8_4);

        yield return X86.And(Flags, ~(byte)whatFlags & 0xFF);
                    
        if (whatFlags.HasFlag(Mos6502SR.Z))
            yield return X86.Shl(Temp8_2, 1);
        if (whatFlags.HasFlag(Mos6502SR.N))
            yield return X86.Shl(Temp8_3, 7);
        if (whatFlags.HasFlag(Mos6502SR.V))
            yield return X86.Shl(Temp8_4, 6);
        
        if (whatFlags.HasFlag(Mos6502SR.C))
            yield return X86.Or(Flags, Temp8);
        
        if (whatFlags.HasFlag(Mos6502SR.Z))
            yield return X86.Or(Flags, Temp8_2);
        
        if (whatFlags.HasFlag(Mos6502SR.N))
            yield return X86.Or(Flags, Temp8_3);
        
        if (whatFlags.HasFlag(Mos6502SR.V))
            yield return X86.Or(Flags, Temp8_4);
    }
}