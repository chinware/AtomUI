using System.Globalization;
using System.Text;

namespace AtomUI.Performance;

internal static partial class Program
{
        private static string RenderTable(IReadOnlyList<PerfResult> results)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Scenario                                Count  Total ms  ms/item  KB/item  Visual  Logical  CP  Button  TB  Icon  IconP  PathI  Stack  AODB  IconUpdates  BrushCalls  Scanned");
            builder.AppendLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (var result in results)
            {
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.Name,-39}{result.Count,5}{result.Elapsed.TotalMilliseconds,10:0.00}{result.MillisecondsPerItem,9:0.000}{result.KilobytesPerItem,9:0.0}");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.VisualPerRoot,8:0.0}{result.TreeStats.LogicalPerRoot,9:0.0}");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.ContentPresenterPerRoot,4:0.0}{result.TreeStats.ButtonPerRoot,8:0.0}{result.TreeStats.TextBlockPerRoot,5:0.0}{result.TreeStats.IconPerRoot,6:0.0}");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.IconPresenterPerRoot,7:0.0}{result.TreeStats.PathIconPerRoot,7:0.0}{result.TreeStats.StackPanelPerRoot,7:0.0}{result.TreeStats.AddOnDecoratedBoxPerRoot,6:0.0}");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.ProbeSnapshot.UpdateIconStatusColorsCalls,13}{result.ProbeSnapshot.ApplyIconBrushCalls,12}{result.ProbeSnapshot.ApplyIconBrushScannedVisuals,9}");
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string RenderMarkdown(IReadOnlyList<PerfResult> results, PerfOptions options)
        {
            var builder = new StringBuilder();
            builder.AppendLine(options.Suite.Equals("icon", StringComparison.OrdinalIgnoreCase)
                ? "# Icon Baseline"
                : "# AddOnDecoratedBox / LineEdit Baseline");
            builder.AppendLine();
            builder.AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
            builder.AppendLine($"- Configuration: Debug");
            builder.AppendLine($"- Suite: `{options.Suite}`");
            builder.AppendLine($"- Count per scenario: {options.Count}");
            builder.AppendLine($"- Runner: `tools/performances/AtomUI.Performance`");
            builder.AppendLine();
            builder.AppendLine("| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | ContentPresenter/root | Button/root | TextBlock/root | Icon/root | IconPresenter/root | PathIcon/root | StackPanel/root | AddOnDecoratedBox/root | Icon status calls | Icon brush calls | Icon scan visuals | Icon matches |");
            builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

            foreach (var result in results)
            {
                builder.Append(CultureInfo.InvariantCulture,
                    $"| {result.Name} | {result.Count} | {result.Elapsed.TotalMilliseconds:0.00} | {result.MillisecondsPerItem:0.000} | {result.KilobytesPerItem:0.0} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.VisualPerRoot:0.0} | {result.TreeStats.LogicalPerRoot:0.0} | {result.TreeStats.ContentPresenterPerRoot:0.0} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.ButtonPerRoot:0.0} | {result.TreeStats.TextBlockPerRoot:0.0} | {result.TreeStats.IconPerRoot:0.0} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.IconPresenterPerRoot:0.0} | {result.TreeStats.PathIconPerRoot:0.0} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.TreeStats.StackPanelPerRoot:0.0} | {result.TreeStats.AddOnDecoratedBoxPerRoot:0.0} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.ProbeSnapshot.UpdateIconStatusColorsCalls} | {result.ProbeSnapshot.ApplyIconBrushCalls} | ");
                builder.Append(CultureInfo.InvariantCulture,
                    $"{result.ProbeSnapshot.ApplyIconBrushScannedVisuals} | {result.ProbeSnapshot.ApplyIconBrushMatchedIcons} |");
                builder.AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine("Notes:");
            builder.AppendLine();
            builder.AppendLine("- `Visual/root` and `Logical/root` include the scenario root control itself.");
            builder.AppendLine("- CompactSpace scenarios use three `LineEdit` children per root.");
            builder.AppendLine("- Icon suite measures materialization, template application and layout in headless mode; it does not isolate GPU/platform render cost.");
            builder.AppendLine("- Icon probe data is Debug-only and records `AddOnDecoratedBox.UpdateIconStatusColors()` plus `ApplyIconBrush()` scans.");
            builder.AppendLine("- Binding expression count is not directly measured yet; current baseline uses node counts, allocation, timing, and AddOnDecoratedBox probe counters.");
            builder.AppendLine("- This measures control-level template/style/materialization cost, not Gallery navigation.");
            return builder.ToString();
        }
}
