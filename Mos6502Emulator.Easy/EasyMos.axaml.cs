using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using VirtualMachine;

namespace Mos6502Emulator.Easy;

public partial class EasyMos : UserControl
{
    protected IMemory memory = null!;
    protected Mos6502Cpu cpu = null!;
    protected Machine machine = null!;

    private IBrush[] palette = {
        new SolidColorBrush(Color.Parse("#000000")), new SolidColorBrush(Color.Parse("#ffffff")), new SolidColorBrush(Color.Parse("#880000")), new SolidColorBrush(Color.Parse("#aaffee")),
        new SolidColorBrush(Color.Parse("#cc44cc")), new SolidColorBrush(Color.Parse("#00cc55")), new SolidColorBrush(Color.Parse("#0000aa")), new SolidColorBrush(Color.Parse("#eeee77")),
        new SolidColorBrush(Color.Parse("#dd8855")), new SolidColorBrush(Color.Parse("#664400")), new SolidColorBrush(Color.Parse("#ff7777")), new SolidColorBrush(Color.Parse("#333333")),
        new SolidColorBrush(Color.Parse("#777777")), new SolidColorBrush(Color.Parse("#aaff66")), new SolidColorBrush(Color.Parse("#0088ff")), new SolidColorBrush(Color.Parse("#bbbbbb"))
    };
    
    public EasyMos()
    {
        InitializeComponent();
        Focusable = true;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        var binFile = Program.CommandLineArguments[^2];
        var startLocation = Program.CommandLineArguments[^1].StartsWith("$")
            ? ushort.Parse(Program.CommandLineArguments[^1].Substring(1), NumberStyles.HexNumber)
            : ushort.Parse(Program.CommandLineArguments[^1]);
        
        memory = new RandomAccessMemory(0x10000);
        cpu = new Mos6502Cpu(startLocation);
        machine = new Machine(memory, cpu);
        memory.Fill(File.ReadAllBytes(binFile));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        memory[0xFF] = (byte)e.Key.ToString().ToLower()[0];
        e.Handled = true;
    }

    public override void Render(DrawingContext context)
    {
        memory[0xFE] = (byte)Random.Shared.Next(0, 255);
        base.Render(context);
        double w = Bounds.Width / 32;
        double h = Bounds.Height / 32;
        for (int y = 0; y < 32; ++y)
        {
            for (int x = 0; x < 32; ++x)
            {
                var color = palette[(memory[0x200 + x + y * 32] & 0xf)];
                context.FillRectangle(color, new Rect(x * w, y * h, w, h));
            }
        }
        for (int i = 0; i < 100; ++i)
            cpu.Step(memory);

        Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
    }
}