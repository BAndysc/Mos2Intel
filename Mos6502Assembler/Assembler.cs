using System.Globalization;
using System.Text.RegularExpressions;
using Mos6502;

namespace Mos6502Assembler;

public class Assembler
{
    private static string MosNumber16 = "(\\$[0-9a-zA-Z]{1,4}|[0-9]{1,5})";
    //private static string MosNumber8 = "(\\$[0-9a-zA-Z]{1,2}|[0-9]{1,3})";
    private static string StringLiteral = "([a-zA-Z][a-zA-Z0-9_]*)";
    
    private static string MosNumber16OrStringLiteral = "(\\$[0-9a-zA-Z]{1,4}|[0-9]{1,5}|[a-zA-Z][a-zA-Z0-9_]*)(?: *\\+ *(\\d+))?";
    private static string MosNumber8OrStringLiteral = "(\\$[0-9a-zA-Z]{1,2}|[0-9]{1,3}|[a-zA-Z][a-zA-Z0-9_]*)(?: *\\+ *(\\d+))?";

    private static string LabelStrRegex = "([a-zA-Z][a-zA-Z0-9_]*)";
    private static Regex LabelRegex = new Regex(LabelStrRegex);
    private static Regex LabelLineRegex = new Regex("^ *" + LabelStrRegex + ":");
    private static Regex OrgLineRegex = new Regex("^ *\\.?org *" + MosNumber16);
    private static Regex DefineLineRegex = new Regex("^ *\\.?define *" + StringLiteral + " *" + MosNumber16);
    private static Regex RawByteRegex = new Regex("^ *\\.?db *" + MosNumber8OrStringLiteral);
    private static Regex MacroArg2Regex = new Regex("^.macro *" + StringLiteral + " +" + StringLiteral + " +" + StringLiteral);
    private static Regex MacroArg1Regex = new Regex("^.macro *" + StringLiteral + " +" + StringLiteral);
    private static Regex EndMacroRegex = new Regex("^.endmacro");
    private static Regex MacroCallArg2Regex = new Regex("^" + StringLiteral + " +" + MosNumber16OrStringLiteral + " +" + MosNumber16OrStringLiteral);
    private static Regex MacroCallArg1Regex = new Regex("^" + StringLiteral + " +" + MosNumber16OrStringLiteral);
    
    private static (MosAddressingMode mode, int bytes, Regex regex)[] OperandRegexes = 
    {
        (MosAddressingMode.Accumulator, 0, new Regex($"^A$")),
        (MosAddressingMode.Immediate, 1, new Regex($"^#{MosNumber8OrStringLiteral}$")),
        (MosAddressingMode.Indirect, 2, new Regex($"^\\({MosNumber16OrStringLiteral}\\)$")),
        (MosAddressingMode.IndirectX, 1, new Regex($"^\\({MosNumber8OrStringLiteral}, *[xX]\\)$")),
        (MosAddressingMode.IndirectY, 1, new Regex($"^\\({MosNumber8OrStringLiteral}\\), *[yY]$")),
        (MosAddressingMode.ZeroPage, 1, new Regex($"^{MosNumber8OrStringLiteral}$")),
        (MosAddressingMode.ZeroPageX, 1, new Regex($"^{MosNumber8OrStringLiteral}, *[xX]$")),
        (MosAddressingMode.ZeroPageY, 1, new Regex($"^{MosNumber8OrStringLiteral}, *[yY]$")),
        (MosAddressingMode.Absolute, 2, new Regex($"^{MosNumber16OrStringLiteral}$")),
        (MosAddressingMode.AbsoluteX, 2, new Regex($"^{MosNumber16OrStringLiteral}, *[xX]$")),
        (MosAddressingMode.AbsoluteY, 2, new Regex($"^{MosNumber16OrStringLiteral}, *[yY]$")),
        (MosAddressingMode.Absolute, 2, new Regex($"(@)")),
    };

    private List<(int offset, string label, bool relative)> labelsToFill = new();
    private Dictionary<string, ushort> defines = new();

    public IList<byte> Assemble(string source, out int instructions, int baseAddress = 0)
    {
        var lines = source.Split("\n");
        return Assemble(lines, out instructions, baseAddress);
    }

    public IList<byte> Assemble(string[] lines, out int instructions, int baseAddress = 0, Dictionary<string, (List<string> argsName, List<string> instrs)>? macros = null)
    {
        labelsToFill.Clear();
        List<byte> bytes = new List<byte>();
        while (bytes.Count < ushort.MaxValue)
            bytes.Add(0);
        Dictionary<string, int> labels = new Dictionary<string, int>();
        int lineNumber = 1;
        instructions = 0;
        int IP = 0;
        int maxIP = 0;
        bool insideMacroDefinition = false;
        string? currentMacroName = null;
        List<string> currentMacroArgument = new();
        List<string> macroLines = new();
        macros ??= new();
        foreach (var sourceLine in lines)
        {
            var commentIndex = sourceLine.IndexOf(';');
            var line = commentIndex == -1 ? sourceLine.Trim() : sourceLine.AsSpan(0, commentIndex).Trim().ToString();

            if (string.IsNullOrWhiteSpace(line))
            {
                lineNumber++;
                continue;
            }
            var labelMatch = LabelLineRegex.Match(line);
            var orgMatch = OrgLineRegex.Match(line);
            var defineMatch = DefineLineRegex.Match(line);
            var rawByteMatch = RawByteRegex.Match(line);
            var macroMatch = MacroArg1Regex.Match(line);
            var macro2Match = MacroArg2Regex.Match(line);
            var endMacro = EndMacroRegex.Match(line);
            var macroCall = MacroCallArg1Regex.Match(line);
            var macro2Call = MacroCallArg2Regex.Match(line);
            if (macro2Match.Success)
            {
                insideMacroDefinition = true;
                currentMacroName = macro2Match.Groups[1].Value;
                macroLines.Clear();
                currentMacroArgument.Clear();
                currentMacroArgument.Add(macro2Match.Groups[2].Value);
                currentMacroArgument.Add(macro2Match.Groups[3].Value);
            }
            else if (macroMatch.Success)
            {
                insideMacroDefinition = true;
                currentMacroName = macroMatch.Groups[1].Value;
                currentMacroArgument.Clear();
                currentMacroArgument.Add(macroMatch.Groups[2].Value);
                macroLines.Clear();
            }
            else if (endMacro.Success)
            {
                insideMacroDefinition = false;
                macros.Add(currentMacroName!, (currentMacroArgument.ToList(), macroLines.ToList()));
                macroLines.Clear();
            }
            else if (insideMacroDefinition)
            {
                macroLines.Add(line);
            }
            else if (labelMatch.Success)
            {
                var label = labelMatch.Groups[1].Value;
                labels[label] = baseAddress + IP;
            }
            else if (orgMatch.Success)
            {
                IP = ParseNumber16(orgMatch.Groups[1].Value);
                maxIP = Math.Max(maxIP, IP);
            }
            else if (defineMatch.Success)
            {
                var name = defineMatch.Groups[1].Value;
                var value = ParseNumber16(defineMatch.Groups[2].Value);
                if (defines.ContainsKey(name))
                    throw new MosAssemblerException(lineNumber, sourceLine, $"Define {name} redefined here");
                defines[name] = value;
            }
            else if (rawByteMatch.Success)
            {
                var numStr = rawByteMatch.Groups[1].Value;
                if (char.IsNumber(numStr[0]) || numStr[0] == '$')
                {
                    bytes[IP++] = (byte)ParseNumber16(numStr);
                    maxIP = Math.Max(maxIP, IP);
                }
                else
                {
                    if (!defines.TryGetValue(numStr, out var operand))
                        throw new MosAssemblerException(lineNumber, line, "Undefined constant " + numStr);
                    bytes[IP++] = (byte)operand;
                    maxIP = Math.Max(maxIP, IP);
                }
            }
            else if (macro2Call.Success && macros.TryGetValue(macro2Call.Groups[1].Value, out var macro1))
            {
                var argValue = macro2Call.Groups[2].Value;
                var arg1Offset = int.TryParse( macro2Call.Groups[3].Value, out _) ? int.Parse( macro2Call.Groups[3].Value) : 0;
                var arg2Value = macro2Call.Groups[4].Value;
                var arg2Offset = int.TryParse( macro2Call.Groups[5].Value, out _) ? int.Parse( macro2Call.Groups[5].Value) : 0;
                var instrs = macro1.instrs.ToArray();
                instrs = instrs.Select(i => i.Replace(macro1.argsName[0], argValue)).ToArray();
                instrs = instrs.Select(i => i.Replace(macro1.argsName[1], arg2Value)).ToArray();

                var nested = new Assembler(){defines = defines.ToDictionary(x => x.Key, x => x.Value)};
                var producedBytes = nested.Assemble(instrs, out var instrNum, 0, macros);
                foreach (var b in producedBytes)
                {
                    bytes[IP++] = b;
                }
                maxIP = Math.Max(maxIP, IP);
                instructions += instrNum;
            }
            else if (macroCall.Success && macros.TryGetValue(macroCall.Groups[1].Value, out var macro2))
            {
                var argValue = macroCall.Groups[2].Value;
                var instrs = macro2.instrs.Select(i => i.Replace(macro2.argsName[0], argValue)).ToArray();
                var nested = new Assembler(){defines = defines.ToDictionary(x => x.Key, x => x.Value)};
                var producedBytes = nested.Assemble(instrs, out var instrNum, 0, macros);
                foreach (var b in producedBytes)
                {
                    bytes[IP++] = b;
                }
                maxIP = Math.Max(maxIP, IP);
                instructions += instrNum;
            }
            else
            {
                var instr = AssembleLine(sourceLine, line, lineNumber, IP);
                try
                {
                    instructions++;
                    var assembled = instr.Encode();
                    foreach (var t in assembled)
                        bytes[IP++] = t;
                    maxIP = Math.Max(maxIP, IP);
                }
                catch (IllegalInstructionException e)
                {
                    throw new MosAssemblerException(lineNumber, sourceLine, e.Message);
                }

                lineNumber++;
            }
        }

        foreach (var (offset, label, relative) in labelsToFill)
        {
            if (!labels.TryGetValue(label, out var labelOffset))
                throw new MosAssemblerException(-1, "?", "Unknown label " + label);

            if (!relative)
            {
                bytes[offset] = (byte)(labelOffset & 0xFF);
                bytes[offset + 1] = (byte)(labelOffset >> 8);
            }
            else
            {
                int diff = labelOffset - (offset + baseAddress);
                if (diff < -128 || diff > 127)
                    throw new MosAssemblerException(-1, "?", "Trying to do long relative jump to label " + label + ". This is illegal");
                bytes[offset] = diff > 0 ? (byte)(diff - 1) : (byte)(255 + diff);
            }
        }

        return bytes.Take(maxIP).ToList();
    }

    private MosInstruction AssembleLine(string sourceLine, string line, int lineNumber, int offset)
    {
        var spaceIndex = line.IndexOf(' ');
        var opcodeSpan = spaceIndex == -1 ? line : line.AsSpan(0, spaceIndex);

        if (!Enum.TryParse(opcodeSpan, true, out MosOpcode opcode))
            throw new MosAssemblerException(lineNumber, sourceLine, $"{opcode} is an unknown opcode");

        if (spaceIndex == -1)
        {
            return new MosInstruction(opcode, opcode.IsImplied() ? MosAddressingMode.Implied : MosAddressingMode.Accumulator);
        }

        var operandStr = line.AsSpan(spaceIndex + 1).Trim().ToString();
        foreach (var pair in OperandRegexes)
        {
            var mode = pair.mode;
            var match = pair.regex.Match(operandStr);
            if (match.Success)
            {
                if (match.Groups.Count == 1)
                {
                    return new MosInstruction(opcode, mode);
                }
                else
                {
                    if (opcode.IsBranchOpcode())
                        mode = MosAddressingMode.Relative;

                    if (opcode.IsOnlyTwoBytesOperandSupported() && pair.bytes != 2)
                        continue;
                    
                    var numStr = match.Groups[1].ValueSpan;
                    var numOffset = match.Groups.Count >= 3 && int.TryParse(match.Groups[2].Value, out _) ? int.Parse(match.Groups[2].Value) : 0;
                    ushort operand = 0;
                    if (numStr[0] == '@')
                        operand = (ushort)(offset);
                    else if (char.IsNumber(numStr[0]) || numStr[0] == '$')
                    {
                        operand = ParseNumber16(numStr);  
                    }
                    else
                    {
                        if (!defines.TryGetValue(numStr.ToString(), out operand))
                            continue;
                    }

                    operand = (ushort)(operand + numOffset);
                    if (pair.bytes == 1 && operand >= 256)
                        continue;
                    return new MosInstruction(opcode, mode, operand); 
                }
            }   
        }

        var m = LabelRegex.Match(operandStr);
        if (m.Success)
        {
            labelsToFill.Add((offset + 1, m.Groups[1].Value, opcode.IsBranchOpcode()));
            return new MosInstruction(opcode, opcode.IsBranchOpcode() ? MosAddressingMode.Relative : MosAddressingMode.Absolute);
        }

        throw new MosAssemblerException(lineNumber, sourceLine, "Can't assemble the line");
    }

    private static ushort ParseNumber16(ReadOnlySpan<char> numStr)
    {
        ushort operand;
        if (numStr.StartsWith("$"))
            operand = ushort.Parse(numStr.Slice(1), NumberStyles.HexNumber);
        else
            operand = ushort.Parse(numStr);
        return operand;
    }
}

public class MosAssemblerException : Exception 
{
    public MosAssemblerException(int lineNumber, string line, string message) : base(
        $"Error in line {lineNumber}:\n{line}\n\n{message}")
    {
        
    }
}