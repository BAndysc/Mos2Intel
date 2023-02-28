using Avalonia;
using System;
using System.Globalization;
using System.IO;

namespace Mos6502Emulator.Easy
{
    class Program
    {
        public static string[] CommandLineArguments = {};
        
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length < 2 || !File.Exists(args[^2]) || args[^1].StartsWith("$") && !ushort.TryParse(args[^1].Substring(1), NumberStyles.HexNumber, null, out _) || !args[^1].StartsWith("$") && !ushort.TryParse(args[^1], out _))
            {
                Console.WriteLine("Usage: ./easy [easy bin file] [start ip]");
                return;
            }
            CommandLineArguments = args;
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}