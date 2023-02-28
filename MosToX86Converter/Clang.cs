using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Mos6502Testing;

public enum Language
{
    C,
    CPP
}

public class Compiler
{
    public static void AssembleX86(string src, string output, string? extra = null)
    {
        extra ??= "";
        var format = "elf64";
        // if macos
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            format = "macho64";
        }
        var process = Process.Start(new ProcessStartInfo("yasm", $"-f {format} {extra} -c {src} -o {output}")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true
        });
        process.WaitForExit();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
        {
            throw new CompilerException(process.ExitCode, error + "\n" + string.Join("\n", File.ReadAllLines(src).Select((x, i) => $"{i+1}: {x}")));
        }
    }
    
    public static void BuildCObjectFile(string src, string output, string? extra = null, Language lang = Language.C)
    {
        extra ??= "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            extra += " -arch x86_64";
        if (lang == Language.CPP)
            extra += " -std=c++17";
        var process = Process.Start(new ProcessStartInfo(lang == Language.CPP ? "clang++" : "clang", $" {extra} -c {src} -o {output}")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true
        });
        process.WaitForExit();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
            throw new CompilerException(process.ExitCode, error);
    }
    
    public static void LinkExecutable(string output, string? linkFlags, Language lang, params string[] objectFiles)
    {
        string ob = string.Join(" ", objectFiles);
        linkFlags ??= "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            linkFlags += " -arch x86_64";
        var process = Process.Start(new ProcessStartInfo(lang == Language.CPP ? "clang++" : "clang", $" {linkFlags} {ob} -o {output}")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true
        });
        process.WaitForExit();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
            throw new CompilerException(process.ExitCode, error);
    }
}

[ExcludeFromCodeCoverage]
public class CompilerException : Exception
{
    public CompilerException(int exitCode, string error) : base("Compiler exited with code " + exitCode + "\n\n" + error)
    {
        
    }
}