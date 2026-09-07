using System.Diagnostics;
using System.Globalization;
using System.Text;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.Workspace.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.GalleryPerformance;

internal sealed record GalleryResizeProbeOptions(
    double FromWidth,
    double ToWidth,
    double Step,
    int Warmup,
    int Iterations,
    bool ForceMaterialized,
    string Label);

internal sealed record GalleryResizeSample(
    int Iteration,
    string Phase,
    int UpdateCount,
    TimeSpan Total,
    double MedianUpdateMs,
    double P95UpdateMs,
    double MaxUpdateMs,
    long AllocatedBytes,
    int Gen0Collections,
    int ColumnChanges,
    int MaterializedBefore,
    int MaterializedAfter);

internal sealed record GalleryResizeResult(
    string Label,
    GalleryResizeProbeOptions Options,
    IReadOnlyList<GalleryResizeSample> Samples);

internal static class GalleryResizeProbe
{
    public static GalleryResizeResult Run(
        WorkspaceWindow window,
        Control route,
        GalleryResizeProbeOptions options)
    {
        if (options.ToWidth <= options.FromWidth || options.Step <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options));
        }

        var panel = route.GetSelfAndVisualDescendants()
                         .OfType<ShowCaseMasonryPanel>()
                         .Single();
        if (options.ForceMaterialized)
        {
            foreach (var item in route.GetSelfAndVisualDescendants().OfType<ShowCaseItem>())
            {
                item.MaterializeDeferredContent();
            }
        }

        var height = Math.Max(1, window.Height);
        var widths = BuildWidths(options.FromWidth, options.ToWidth, options.Step);
        Resize(window, options.FromWidth, height);

        var samples = new List<GalleryResizeSample>(options.Warmup + options.Iterations);
        for (var i = 0; i < options.Warmup; i++)
        {
            samples.Add(MeasureIteration(window, route, panel, widths, height, i + 1, "Warmup"));
        }
        for (var i = 0; i < options.Iterations; i++)
        {
            samples.Add(MeasureIteration(window, route, panel, widths, height, i + 1, "Measured"));
        }

        return new GalleryResizeResult(options.Label, options, samples);
    }

    public static string RenderMarkdown(GalleryResizeResult result)
    {
        var measured = result.Samples.Where(sample => sample.Phase == "Measured").ToArray();
        if (measured.Length == 0)
        {
            throw new InvalidOperationException("Resize result contains no measured samples.");
        }

        static string Number(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);

        var builder = new StringBuilder();
        builder.AppendLine($"# Gallery resize trace - {result.Label}");
        builder.AppendLine();
        builder.AppendLine($"- Widths: {Number(result.Options.FromWidth)} -> {Number(result.Options.ToWidth)} -> {Number(result.Options.FromWidth)} DIP");
        builder.AppendLine($"- Step: {Number(result.Options.Step)} DIP");
        builder.AppendLine($"- Force materialized: {result.Options.ForceMaterialized}");
        builder.AppendLine($"- Warmup: {result.Options.Warmup}; measured: {result.Options.Iterations}");
        builder.AppendLine();
        builder.AppendLine("| Phase | Iteration | Updates | Total ms | Update median ms | Update p95 ms | Max update ms | Alloc KB | Gen0 | Column changes | Materialized before | Materialized after |");
        builder.AppendLine("|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var sample in result.Samples)
        {
            builder.AppendLine($"| {sample.Phase} | {sample.Iteration} | {sample.UpdateCount} | {Number(sample.Total.TotalMilliseconds)} | {Number(sample.MedianUpdateMs)} | {Number(sample.P95UpdateMs)} | {Number(sample.MaxUpdateMs)} | {Number(sample.AllocatedBytes / 1024d)} | {sample.Gen0Collections} | {sample.ColumnChanges} | {sample.MaterializedBefore} | {sample.MaterializedAfter} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Measured summary");
        builder.AppendLine();
        builder.AppendLine("| Metric | Value |");
        builder.AppendLine("|---|---:|");
        builder.AppendLine($"| Total mean ms / round trip | {Number(measured.Average(sample => sample.Total.TotalMilliseconds))} |");
        builder.AppendLine($"| Update median mean ms / DIP update | {Number(measured.Average(sample => sample.MedianUpdateMs))} |");
        builder.AppendLine($"| Update p95 mean ms / DIP update | {Number(measured.Average(sample => sample.P95UpdateMs))} |");
        builder.AppendLine($"| Max update ms | {Number(measured.Max(sample => sample.MaxUpdateMs))} |");
        builder.AppendLine($"| Allocation mean KB / round trip | {Number(measured.Average(sample => sample.AllocatedBytes) / 1024d)} |");
        builder.AppendLine($"| Gen0 collections / all measured round trips | {measured.Sum(sample => sample.Gen0Collections)} |");
        builder.AppendLine($"| Column changes / all measured round trips | {measured.Sum(sample => sample.ColumnChanges)} |");
        return builder.ToString();
    }

    private static GalleryResizeSample MeasureIteration(
        WorkspaceWindow window,
        Control route,
        ShowCaseMasonryPanel panel,
        IReadOnlyList<double> widths,
        double height,
        int iteration,
        string phase)
    {
        Resize(window, widths[^1], height);
        var previousColumnCount = ResolveColumnCount(panel);
        var previous = new Dictionary<Control, int>(ReferenceEqualityComparer.Instance);
        var current = new Dictionary<Control, int>(ReferenceEqualityComparer.Instance);
        SnapshotColumns(panel, previous, previousColumnCount);
        var materializedBefore = CountMaterialized(route);
        var elapsedUpdates = new double[widths.Count];
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var gen0Before = GC.CollectionCount(0);
        var total = Stopwatch.StartNew();
        var columnChanges = 0;

        for (var i = 0; i < widths.Count; i++)
        {
            var started = Stopwatch.GetTimestamp();
            Resize(window, widths[i], height);
            elapsedUpdates[i] = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            var currentColumnCount = ResolveColumnCount(panel);
            SnapshotColumns(panel, current, currentColumnCount);
            if (currentColumnCount == previousColumnCount)
            {
                foreach (var (child, previousColumn) in previous)
                {
                    if (current.TryGetValue(child, out var currentColumn) && currentColumn != previousColumn)
                    {
                        columnChanges++;
                    }
                }
            }

            (previous, current) = (current, previous);
            current.Clear();
            previousColumnCount = currentColumnCount;
        }

        total.Stop();
        var materializedAfter = CountMaterialized(route);
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var gen0Collections = GC.CollectionCount(0) - gen0Before;
        var ordered = elapsedUpdates.Order().ToArray();
        return new GalleryResizeSample(
            iteration,
            phase,
            widths.Count,
            total.Elapsed,
            Percentile(ordered, 0.50),
            Percentile(ordered, 0.95),
            ordered[^1],
            allocatedBytes,
            gen0Collections,
            columnChanges,
            materializedBefore,
            materializedAfter);
    }

    private static double[] BuildWidths(double fromWidth, double toWidth, double step)
    {
        var widths = new List<double>();
        for (var width = fromWidth + step; width < toWidth; width += step)
        {
            widths.Add(width);
        }
        widths.Add(toWidth);
        for (var width = toWidth - step; width > fromWidth; width -= step)
        {
            widths.Add(width);
        }
        widths.Add(fromWidth);
        return widths.ToArray();
    }

    private static void Resize(WorkspaceWindow window, double width, double height)
    {
        window.Width = width;
        var size = new Size(width, height);
        window.Measure(size);
        window.Arrange(new Rect(size));
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    private static void SnapshotColumns(
        ShowCaseMasonryPanel panel,
        Dictionary<Control, int> target,
        int columnCount)
    {
        target.Clear();
        foreach (var child in panel.Children)
        {
            var column = ResolveColumn(panel, child, columnCount);
            if (column >= 0)
            {
                target[child] = column;
            }
        }
    }

    private static int ResolveColumnCount(ShowCaseMasonryPanel panel)
    {
        var width = panel.Bounds.Width;
        var columnGap = Math.Max(0, panel.ColumnGap);
        var minItemWidth = Math.Max(1, panel.MinItemWidth);
        var maxColumns = Math.Max(1, panel.MaxColumns);
        var count = (int)Math.Floor((width + columnGap) / (minItemWidth + columnGap));
        return Math.Clamp(count, 1, maxColumns);
    }

    private static int ResolveColumn(ShowCaseMasonryPanel panel, Control child, int columnCount)
    {
        if (!child.IsVisible || child is ShowCaseItem { Span: ShowCaseItemSpan.Full } or
            ShowCaseItem { IsOccupyEntireRow: true })
        {
            return -1;
        }

        var gap = Math.Max(0, panel.ColumnGap);
        var columnWidth = columnCount == 1
            ? panel.Bounds.Width
            : Math.Max(0, (panel.Bounds.Width - gap * (columnCount - 1)) / columnCount);
        return Math.Clamp((int)Math.Round(child.Bounds.X / (columnWidth + gap)), 0, columnCount - 1);
    }

    private static int CountMaterialized(Control route)
    {
        return route.GetSelfAndVisualDescendants()
                    .OfType<ShowCaseItem>()
                    .Count(item => item.IsDeferredContentMaterialized);
    }

    private static double Percentile(IReadOnlyList<double> ordered, double percentile)
    {
        if (ordered.Count == 0)
        {
            return 0;
        }
        var index = (ordered.Count - 1) * percentile;
        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);
        var weight = index - lower;
        return ordered[lower] * (1 - weight) + ordered[upper] * weight;
    }
}
