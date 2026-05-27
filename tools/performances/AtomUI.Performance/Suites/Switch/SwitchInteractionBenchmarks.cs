using System.Diagnostics;
using System.Globalization;
using System.Text;
using AtomUI.Utils;
using Avalonia;
using AtomSwitch = AtomUI.Desktop.Controls.ToggleSwitch;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static int RunSwitchInteractionBenchmarks(int count, string? markdownOutputPath)
    {
        var updateCount = Math.Max(1, count);
        var result      = MeasureSwitchCheckedToggle(updateCount);
        var text        = RenderSwitchInteractionTable(result);

        Console.WriteLine(text);

        if (!string.IsNullOrWhiteSpace(markdownOutputPath))
        {
            var fullPath = Path.GetFullPath(markdownOutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, RenderSwitchInteractionMarkdown(result),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine($"Wrote markdown result: {fullPath}");
        }

        return 0;
    }

    private static SwitchInteractionResult MeasureSwitchCheckedToggle(int updateCount)
    {
        var toggleSwitch = new AtomSwitch
        {
            OnContent  = "Enabled",
            OffContent = "Disabled"
        };

        using var realized = RealizeControl(toggleSwitch);
        RefreshLayout(realized.Window);
        var desiredSize = toggleSwitch.DesiredSize;

        for (var i = 0; i < 20; i++)
        {
            toggleSwitch.IsChecked = i % 2 == 0;
            RefreshLayout(realized.Window);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore     = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch           = Stopwatch.StartNew();
        var measureInvalidations = 0;
        var arrangeInvalidations = 0;
        var desiredSizeChanges   = 0;

        for (var i = 0; i < updateCount; i++)
        {
            toggleSwitch.IsChecked = i % 2 == 0;
            if (!toggleSwitch.IsMeasureValid)
            {
                measureInvalidations++;
            }
            if (!toggleSwitch.IsArrangeValid)
            {
                arrangeInvalidations++;
            }

            RefreshLayout(realized.Window);

            if (!MathUtils.AreClose(toggleSwitch.DesiredSize.Width, desiredSize.Width) ||
                !MathUtils.AreClose(toggleSwitch.DesiredSize.Height, desiredSize.Height))
            {
                desiredSizeChanges++;
            }
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new SwitchInteractionResult(
            "Switch.IsCheckedToggle",
            updateCount,
            stopwatch.Elapsed,
            allocatedBytes,
            measureInvalidations,
            arrangeInvalidations,
            desiredSizeChanges,
            TreeStats.Collect(realized.RootControls));
    }

    private static string RenderSwitchInteractionTable(SwitchInteractionResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Scenario                  Updates  Total ms  us/update  KB total  bytes/update  Measure invalidations  Arrange invalidations  DesiredSize changes  Visual  Logical");
        builder.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");
        builder.AppendLine(CultureInfo.InvariantCulture,
            $"{result.Name,-30}{result.UpdateCount,7}{result.Elapsed.TotalMilliseconds,10:0.00}{result.MicrosecondsPerUpdate,11:0.00}{result.AllocatedBytes / 1024.0,10:0.0}{result.BytesPerUpdate,14:0.0}{result.MeasureInvalidations,23}{result.ArrangeInvalidations,23}{result.DesiredSizeChanges,21}{result.TreeStats.VisualPerRoot,8:0.0}{result.TreeStats.LogicalPerRoot,9:0.0}");
        return builder.ToString();
    }

    private static string RenderSwitchInteractionMarkdown(SwitchInteractionResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Switch Interaction Benchmark");
        builder.AppendLine();
        builder.AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine("- Configuration: Release");
        builder.AppendLine("- Runner: `tools/performances/AtomUI.Performance --measure-switch-interactions`");
        builder.AppendLine("- Operation: realized `ToggleSwitch` with on/off text -> repeated `IsChecked` changes -> refresh layout after every change");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Updates | Total ms | us/update | KB total | bytes/update | Measure invalidations | Arrange invalidations | DesiredSize changes | Visual/root | Logical/root |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        builder.AppendLine(CultureInfo.InvariantCulture,
            $"| {result.Name} | {result.UpdateCount} | {result.Elapsed.TotalMilliseconds:0.00} | {result.MicrosecondsPerUpdate:0.00} | {result.AllocatedBytes / 1024.0:0.0} | {result.BytesPerUpdate:0.0} | {result.MeasureInvalidations} | {result.ArrangeInvalidations} | {result.DesiredSizeChanges} | {result.TreeStats.VisualPerRoot:0.0} | {result.TreeStats.LogicalPerRoot:0.0} |");
        return builder.ToString();
    }

    private sealed record SwitchInteractionResult(
        string Name,
        int UpdateCount,
        TimeSpan Elapsed,
        long AllocatedBytes,
        int MeasureInvalidations,
        int ArrangeInvalidations,
        int DesiredSizeChanges,
        TreeStats TreeStats)
    {
        public double MicrosecondsPerUpdate => Elapsed.TotalMilliseconds * 1000 / UpdateCount;
        public double BytesPerUpdate => AllocatedBytes / (double)UpdateCount;
    }
}
