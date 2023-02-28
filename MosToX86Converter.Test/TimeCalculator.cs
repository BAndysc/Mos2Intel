using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MosToX86Converter.Test;

public class TimeCalculator
{
    private Stopwatch sw;
    private List<TimeSpan> scores = new();
    
    public TimeCalculator()
    {
        sw = new();
    }

    public System.IDisposable StartRun()
    {
        if (sw.IsRunning)
            throw new Exception("Cannot run a timer while it is already running");
        
        sw.Start();
        return new Stopper(this);
    }
    
    public int Runs => scores.Count;

    public TimeSpan Average => scores.Average();
    
    public IEnumerable<TimeSpan> Scores => scores;
    
    public TimeSpan Best => scores.Min();
    
    public TimeSpan Worst => scores.Max();
    
    public TimeSpan StdDev
    {
        get
        {
            var avg = Average;
            var sum = scores.Sum(x => (x - avg).TotalMilliseconds * (x - avg).TotalMilliseconds);
            return TimeSpan.FromMilliseconds(Math.Sqrt(sum / scores.Count));
        }
    }

    private void Stop()
    {
        sw.Stop();
        var elapsed = sw.Elapsed;
        sw.Reset();
        scores.Add(elapsed);
    }
    
    private class Stopper : System.IDisposable
    {
        private readonly TimeCalculator stopwatch;

        public Stopper(TimeCalculator stopwatch)
        {
            this.stopwatch = stopwatch;
        }
        
        public void Dispose()
        {
            stopwatch.Stop();
        }
    }
}

internal static class Extensions
{
    public static TimeSpan Average(this ICollection<TimeSpan> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        long mean = 0L;
        long remainder = 0L;
        int n = source.Count;
        foreach (var item in source)
        {
            long ticks = item.Ticks;
            mean += ticks / n;
            remainder += ticks % n;
            mean += remainder / n;
            remainder %= n;
        }

        return TimeSpan.FromTicks(mean);
    }
}