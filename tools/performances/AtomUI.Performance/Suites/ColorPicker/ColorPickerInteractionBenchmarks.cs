using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static int RunColorPickerInteractionBenchmarks(int count, string? markdownOutputPath)
    {
        var updateCount = Math.Max(1, count);
        var results = new[]
        {
            MeasureColorSpectrumUpdateBitmapSources(updateCount),
            MeasureColorSpectrumHsvUpdates(updateCount),
            MeasureTransparentBgBrushBuildSameToken(updateCount)
        };
        var text = RenderColorPickerInteractionTable(results);

        Console.WriteLine(text);

        if (!string.IsNullOrWhiteSpace(markdownOutputPath))
        {
            var fullPath = System.IO.Path.GetFullPath(markdownOutputPath);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, RenderColorPickerInteractionMarkdown(results),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine($"Wrote markdown result: {fullPath}");
        }

        return 0;
    }

    private static ColorPickerInteractionResult MeasureColorSpectrumUpdateBitmapSources(int updateCount)
    {
        var view = CreateColorPickerView();

        using var realized = RealizeControl(view);
        var spectrum = FindVisualByName<Control>(view, "ColorSpectrum") ??
                       throw new InvalidOperationException("ColorSpectrum was not realized.");
        var spectrumRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumRectangle") ??
                                throw new InvalidOperationException("PART_SpectrumRectangle was not realized.");
        var spectrumOverlayRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumOverlayRectangle") ??
                                       throw new InvalidOperationException("PART_SpectrumOverlayRectangle was not realized.");

        if (!WaitForColorSpectrumBrushes(realized.Window, spectrumRectangle, spectrumOverlayRectangle))
        {
            throw new InvalidOperationException("ColorSpectrum brushes were not created.");
        }

        var updateBitmapSources = GetUpdateBitmapSourcesDelegate(spectrum);
        var baseBrushes = new HashSet<IBrush>(ReferenceEqualityComparer.Instance);
        var overlayBrushes = new HashSet<IBrush>(ReferenceEqualityComparer.Instance);

        for (var i = 0; i < 20; i++)
        {
            updateBitmapSources();
        }
        Dispatcher.UIThread.RunJobs();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < updateCount; i++)
        {
            updateBitmapSources();
            if (spectrumRectangle.Fill is { } baseBrush)
            {
                baseBrushes.Add(baseBrush);
            }
            if (spectrumOverlayRectangle.Fill is { } overlayBrush)
            {
                overlayBrushes.Add(overlayBrush);
            }
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new ColorPickerInteractionResult(
            "ColorSpectrum.UpdateBitmapSources",
            updateCount,
            stopwatch.Elapsed,
            allocatedBytes,
            baseBrushes.Count,
            overlayBrushes.Count,
            TreeStats.Collect(realized.RootControls));
    }

    private static ColorPickerInteractionResult MeasureColorSpectrumHsvUpdates(int updateCount)
    {
        var view = CreateColorPickerView();

        using var realized = RealizeControl(view);
        var spectrum = FindVisualByName<Control>(view, "ColorSpectrum") ??
                       throw new InvalidOperationException("ColorSpectrum was not realized.");
        var spectrumRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumRectangle") ??
                                throw new InvalidOperationException("PART_SpectrumRectangle was not realized.");
        var spectrumOverlayRectangle = FindVisualByName<Rectangle>(view, "PART_SpectrumOverlayRectangle") ??
                                       throw new InvalidOperationException("PART_SpectrumOverlayRectangle was not realized.");

        if (!WaitForColorSpectrumBrushes(realized.Window, spectrumRectangle, spectrumOverlayRectangle))
        {
            throw new InvalidOperationException("ColorSpectrum brushes were not created.");
        }

        var hsvProperty = GetColorSpectrumHsvColorProperty(spectrum);
        var baseBrushes = new HashSet<IBrush>(ReferenceEqualityComparer.Instance);
        var overlayBrushes = new HashSet<IBrush>(ReferenceEqualityComparer.Instance);

        for (var i = 0; i < 20; i++)
        {
            spectrum.SetValue(hsvProperty, CreateBenchmarkHsv(i));
        }
        Dispatcher.UIThread.RunJobs();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < updateCount; i++)
        {
            spectrum.SetValue(hsvProperty, CreateBenchmarkHsv(i));
            if (spectrumRectangle.Fill is { } baseBrush)
            {
                baseBrushes.Add(baseBrush);
            }
            if (spectrumOverlayRectangle.Fill is { } overlayBrush)
            {
                overlayBrushes.Add(overlayBrush);
            }
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new ColorPickerInteractionResult(
            "ColorSpectrum.HsvColorUpdate",
            updateCount,
            stopwatch.Elapsed,
            allocatedBytes,
            baseBrushes.Count,
            overlayBrushes.Count,
            TreeStats.Collect(realized.RootControls));
    }

    private static ColorPickerInteractionResult MeasureTransparentBgBrushBuildSameToken(int updateCount)
    {
        var buildTransparentBgBrush = GetTransparentBgBrushBuildDelegate();
        var fillColor = Color.FromArgb(0x26, 0x00, 0x00, 0x00);
        var distinctBrushes = new HashSet<IBrush>(ReferenceEqualityComparer.Instance);

        for (var i = 0; i < 20; i++)
        {
            buildTransparentBgBrush(4, fillColor);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < updateCount; i++)
        {
            distinctBrushes.Add(buildTransparentBgBrush(4, fillColor));
        }

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new ColorPickerInteractionResult(
            "TransparentBgBrush.BuildSameToken",
            updateCount,
            stopwatch.Elapsed,
            allocatedBytes,
            distinctBrushes.Count,
            0,
            TreeStats.Collect([]));
    }

    private static StyledProperty<HsvColor> GetColorSpectrumHsvColorProperty(Control spectrum)
    {
        var field = spectrum.GetType().GetField("HsvColorProperty",
            BindingFlags.Static | BindingFlags.Public);
        return field?.GetValue(null) as StyledProperty<HsvColor> ??
               throw new InvalidOperationException("ColorSpectrum.HsvColorProperty was not found.");
    }

    private static Action GetUpdateBitmapSourcesDelegate(Control spectrum)
    {
        var method = spectrum.GetType().GetMethod("UpdateBitmapSources",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.CreateDelegate<Action>(spectrum) ??
               throw new InvalidOperationException("ColorSpectrum.UpdateBitmapSources was not found.");
    }

    private static Func<double, Color, IBrush> GetTransparentBgBrushBuildDelegate()
    {
        var type = Type.GetType(
            "AtomUI.Desktop.Controls.Primitives.TransparentBgBrushUtils, AtomUI.Desktop.Controls.ColorPicker");
        var method = type?.GetMethod(
            "Build",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(double), typeof(Color)],
            modifiers: null);
        return method?.CreateDelegate<Func<double, Color, IBrush>>() ??
               throw new InvalidOperationException("TransparentBgBrushUtils.Build was not found.");
    }

    private static HsvColor CreateBenchmarkHsv(int index)
    {
        var hue = index * 37 % 360;
        var saturation = 0.35 + index % 7 * 0.08;
        var value = 0.45 + index % 5 * 0.09;
        return new HsvColor(1, hue, Math.Min(saturation, 1), Math.Min(value, 1));
    }

    private static string RenderColorPickerInteractionTable(IReadOnlyList<ColorPickerInteractionResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Scenario                         Updates  Total ms  us/update  KB total  bytes/update  Base brushes  Overlay brushes  Visual  Logical");
        builder.AppendLine("----------------------------------------------------------------------------------------------------------------------------------");
        foreach (var result in results)
        {
            builder.AppendLine(CultureInfo.InvariantCulture,
                $"{result.Name,-34}{result.UpdateCount,7}{result.Elapsed.TotalMilliseconds,10:0.00}{result.MicrosecondsPerUpdate,11:0.00}{result.AllocatedBytes / 1024.0,10:0.0}{result.BytesPerUpdate,14:0.0}{result.DistinctBaseBrushes,14}{result.DistinctOverlayBrushes,17}{result.TreeStats.VisualPerRoot,8:0.0}{result.TreeStats.LogicalPerRoot,9:0.0}");
        }
        return builder.ToString();
    }

    private static string RenderColorPickerInteractionMarkdown(IReadOnlyList<ColorPickerInteractionResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# ColorPicker Interaction Benchmark");
        builder.AppendLine();
        builder.AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine("- Configuration: Release");
        builder.AppendLine("- Runner: `tools/performances/AtomUI.Performance --measure-colorpicker-interactions`");
        builder.AppendLine("- Operation: realized `ColorPickerView` -> ready `ColorSpectrum` bitmap brushes -> repeated `HsvColor` updates");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Updates | Total ms | us/update | KB total | bytes/update | Distinct base brushes | Distinct overlay brushes | Visual/root | Logical/root |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var result in results)
        {
            builder.AppendLine(CultureInfo.InvariantCulture,
                $"| {result.Name} | {result.UpdateCount} | {result.Elapsed.TotalMilliseconds:0.00} | {result.MicrosecondsPerUpdate:0.00} | {result.AllocatedBytes / 1024.0:0.0} | {result.BytesPerUpdate:0.0} | {result.DistinctBaseBrushes} | {result.DistinctOverlayBrushes} | {result.TreeStats.VisualPerRoot:0.0} | {result.TreeStats.LogicalPerRoot:0.0} |");
        }
        return builder.ToString();
    }

    private sealed record ColorPickerInteractionResult(
        string Name,
        int UpdateCount,
        TimeSpan Elapsed,
        long AllocatedBytes,
        int DistinctBaseBrushes,
        int DistinctOverlayBrushes,
        TreeStats TreeStats)
    {
        public double MicrosecondsPerUpdate => Elapsed.TotalMilliseconds * 1000 / UpdateCount;
        public double BytesPerUpdate => AllocatedBytes / (double)UpdateCount;
    }
}
