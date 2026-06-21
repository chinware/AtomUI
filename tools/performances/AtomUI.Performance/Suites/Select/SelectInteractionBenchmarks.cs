using System.Diagnostics;
using System.Globalization;
using System.Text;
using AtomUI.Desktop.Controls;
using AtomUI.Utils;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static int RunSelectInteractionBenchmarks(int count, string? markdownOutputPath)
    {
        var updateCount = Math.Max(1, count);
        var results = new[]
        {
            MeasureSelectHandleInputState(
                "Select.HandleState.Empty",
                updateCount,
                static () => new Select
                {
                    OptionsSource = CreateSelectOptions()
                }),
            MeasureSelectHandleInputState(
                "Select.HandleState.ClearableSelected",
                updateCount,
                static () =>
                {
                    var options = CreateSelectOptions();
                    return new Select
                    {
                        IsAllowClear  = true,
                        OptionsSource = options,
                        SelectedOption = options[0]
                    };
                })
        };
        var text = RenderSelectInteractionTable(results);

        Console.WriteLine(text);

        if (!string.IsNullOrWhiteSpace(markdownOutputPath))
        {
            var fullPath = Path.GetFullPath(markdownOutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, RenderSelectInteractionMarkdown(results),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine($"Wrote markdown result: {fullPath}");
        }

        return results.Any(static result => result.PropagationMismatches > 0) ? 1 : 0;
    }

    private static SelectInteractionResult MeasureSelectHandleInputState(
        string name,
        int updateCount,
        Func<Select> createSelect)
    {
        var select = createSelect();
        using var realized = RealizeControl(select);
        RefreshLayout(realized.Window);

        var addOnBox = FindVisualByName<AddOnDecoratedBox>(select, AddOnDecoratedBox.AddOnDecoratedBoxPart) ??
                       throw new InvalidOperationException("PART_AddOnDecoratedBox was not realized.");
        var handle = FindVisualByName<SelectHandle>(select, "PART_SelectHandle") ??
                     throw new InvalidOperationException("PART_SelectHandle was not realized.");
        var desiredSize = select.DesiredSize;

        for (var i = 0; i < 20; i++)
        {
            ApplySelectHandleInputState(addOnBox, i);
            RefreshLayout(realized.Window);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        var propagationMismatches = 0;
        var measureInvalidations = 0;
        var arrangeInvalidations = 0;
        var desiredSizeChanges = 0;

        for (var i = 0; i < updateCount; i++)
        {
            var state = ApplySelectHandleInputState(addOnBox, i);
            if (!IsSelectHandleInputStateMatched(handle, state))
            {
                propagationMismatches++;
            }
            if (!select.IsMeasureValid)
            {
                measureInvalidations++;
            }
            if (!select.IsArrangeValid)
            {
                arrangeInvalidations++;
            }

            RefreshLayout(realized.Window);

            if (!MathUtils.AreClose(select.DesiredSize.Width, desiredSize.Width) ||
                !MathUtils.AreClose(select.DesiredSize.Height, desiredSize.Height))
            {
                desiredSizeChanges++;
            }
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new SelectInteractionResult(
            name,
            updateCount,
            stopwatch.Elapsed,
            allocatedBytes,
            propagationMismatches,
            measureInvalidations,
            arrangeInvalidations,
            desiredSizeChanges,
            TreeStats.Collect(realized.RootControls));
    }

    private static SelectHandleInputState ApplySelectHandleInputState(AddOnDecoratedBox addOnBox, int index)
    {
        var isHover = (index & 1) == 0;
        var isPressed = (index & 2) == 0;
        addOnBox.IsInnerBoxHover = isHover;
        addOnBox.IsInnerBoxPressed = isPressed;
        return new SelectHandleInputState(isHover, isPressed);
    }

    private static bool IsSelectHandleInputStateMatched(SelectHandle handle, SelectHandleInputState state)
    {
        return handle.IsInputHover == state.IsHover &&
               handle.IsInputPressed == state.IsPressed;
    }

    private static string RenderSelectInteractionTable(IReadOnlyList<SelectInteractionResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Scenario                              Updates  Total ms  us/update  KB total  bytes/update  Mismatches  Measure invalidations  Arrange invalidations  DesiredSize changes  Visual  Logical");
        builder.AppendLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        foreach (var result in results)
        {
            builder.AppendLine(CultureInfo.InvariantCulture,
                $"{result.Name,-38}{result.UpdateCount,7}{result.Elapsed.TotalMilliseconds,10:0.00}{result.MicrosecondsPerUpdate,11:0.00}{result.AllocatedBytes / 1024.0,10:0.0}{result.BytesPerUpdate,14:0.0}{result.PropagationMismatches,12}{result.MeasureInvalidations,23}{result.ArrangeInvalidations,23}{result.DesiredSizeChanges,21}{result.TreeStats.VisualPerRoot,8:0.0}{result.TreeStats.LogicalPerRoot,9:0.0}");
        }
        return builder.ToString();
    }

    private static string RenderSelectInteractionMarkdown(IReadOnlyList<SelectInteractionResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Select Interaction Benchmark");
        builder.AppendLine();
        builder.AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine("- Configuration: Debug");
        builder.AppendLine("- Runner: `tools/performances/AtomUI.Performance --measure-select-interactions`");
        builder.AppendLine("- Operation: realized `Select` -> repeated `AddOnDecoratedBox.IsInnerBoxHover/IsInnerBoxPressed` changes -> verify `SelectHandle.IsInputHover/IsInputPressed` -> refresh layout after every change");
        builder.AppendLine("- Scenarios: empty Select isolates handle state propagation; clearable selected Select also exercises the clear/open-indicator selector path.");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Updates | Total ms | us/update | KB total | bytes/update | Propagation mismatches | Measure invalidations | Arrange invalidations | DesiredSize changes | Visual/root | Logical/root |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var result in results)
        {
            builder.AppendLine(CultureInfo.InvariantCulture,
                $"| {result.Name} | {result.UpdateCount} | {result.Elapsed.TotalMilliseconds:0.00} | {result.MicrosecondsPerUpdate:0.00} | {result.AllocatedBytes / 1024.0:0.0} | {result.BytesPerUpdate:0.0} | {result.PropagationMismatches} | {result.MeasureInvalidations} | {result.ArrangeInvalidations} | {result.DesiredSizeChanges} | {result.TreeStats.VisualPerRoot:0.0} | {result.TreeStats.LogicalPerRoot:0.0} |");
        }
        return builder.ToString();
    }

    private readonly record struct SelectHandleInputState(bool IsHover, bool IsPressed);

    private sealed record SelectInteractionResult(
        string Name,
        int UpdateCount,
        TimeSpan Elapsed,
        long AllocatedBytes,
        int PropagationMismatches,
        int MeasureInvalidations,
        int ArrangeInvalidations,
        int DesiredSizeChanges,
        TreeStats TreeStats)
    {
        public double MicrosecondsPerUpdate => Elapsed.TotalMilliseconds * 1000 / UpdateCount;
        public double BytesPerUpdate => AllocatedBytes / (double)UpdateCount;
    }
}
