using System.Diagnostics;
using Mos6502Testing;

namespace NesConverter;

public class CMake
{
    public static void Configure(string src, string destFolder, params (string, string)[] defines)
    {
        src = Path.GetFullPath(src);
        destFolder = Path.GetFullPath(destFolder);
        if (!Directory.Exists(destFolder))
            throw new Exception("Directory " + destFolder + " doesn't exist");
        Console.WriteLine(destFolder + " " + string.Join(" ", defines.Select(pair => $"-D{pair.Item1}=\"{pair.Item2}\"")));
        var process = Process.Start(new ProcessStartInfo("cmake", "-S " + src + " -B " + destFolder + " " + string.Join(" ", defines.Select(pair => $"-D{pair.Item1}=\"{pair.Item2}\"")))
        {
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = true
        });
        process.WaitForExit();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
            throw new CompilerException(process.ExitCode, error);
    }
    
    public static void Build(string folder)
    {
        var process = Process.Start(new ProcessStartInfo("make")
        {
            RedirectStandardError = true,
            RedirectStandardOutput = false,
            WorkingDirectory = folder
        });
        process.WaitForExit();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
            throw new CompilerException(process.ExitCode, error);
    }
}