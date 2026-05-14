using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal sealed record PerfScenario(string Name, Func<int, Control> Create);

internal sealed record PerfResult(
    string Name,
    int Count,
    TimeSpan Elapsed,
    long AllocatedBytes,
    TreeStats TreeStats,
    AddOnDecoratedBoxPerfSnapshot ProbeSnapshot)
{
    public double MillisecondsPerItem => Elapsed.TotalMilliseconds / Count;
    public double KilobytesPerItem    => AllocatedBytes / 1024.0 / Count;
}

internal sealed class RealizedScenario : IDisposable
{
    public RealizedScenario(Avalonia.Controls.Window window, IReadOnlyList<Control> rootControls)
    {
        Window       = window;
        RootControls = rootControls;
    }

    public Avalonia.Controls.Window Window { get; }
    public IReadOnlyList<Control> RootControls { get; }

    public void Dispose()
    {
        Window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}

internal sealed record TreeStats(
    double VisualPerRoot,
    double LogicalPerRoot,
    double ContentPresenterPerRoot,
    double ButtonPerRoot,
    double TextBlockPerRoot,
    double IconPerRoot,
    double IconPresenterPerRoot,
    double PathIconPerRoot,
    double StackPanelPerRoot,
    double AddOnDecoratedBoxPerRoot)
{
    public static TreeStats Collect(IReadOnlyList<Control> roots)
    {
        var visualCount              = 0;
        var logicalCount             = 0;
        var contentPresenterCount    = 0;
        var buttonCount              = 0;
        var textBlockCount           = 0;
        var iconCount                = 0;
        var iconPresenterCount       = 0;
        var pathIconCount            = 0;
        var stackPanelCount          = 0;
        var addOnDecoratedBoxCount   = 0;

        foreach (var root in roots)
        {
            var visuals = root.GetSelfAndVisualDescendants().ToList();
            visualCount += visuals.Count;

            foreach (var visual in visuals)
            {
                var type = visual.GetType();
                if (type.Name == "ContentPresenter")
                {
                    contentPresenterCount++;
                }
                if (visual is Avalonia.Controls.Button)
                {
                    buttonCount++;
                }
                if (type.Name == "TextBlock")
                {
                    textBlockCount++;
                }
                if (type.Name.EndsWith("Icon", StringComparison.Ordinal) || IsAtomIcon(type))
                {
                    iconCount++;
                }
                if (visual is IconPresenter)
                {
                    iconPresenterCount++;
                }
                if (visual is PathIcon)
                {
                    pathIconCount++;
                }
                if (visual is StackPanel)
                {
                    stackPanelCount++;
                }
                if (IsAddOnDecoratedBox(type))
                {
                    addOnDecoratedBoxCount++;
                }
            }

            logicalCount += root.GetSelfAndLogicalDescendants().Count();
        }

        var rootCount = Math.Max(1, roots.Count);
        return new TreeStats(
            visualCount / (double)rootCount,
            logicalCount / (double)rootCount,
            contentPresenterCount / (double)rootCount,
            buttonCount / (double)rootCount,
            textBlockCount / (double)rootCount,
            iconCount / (double)rootCount,
            iconPresenterCount / (double)rootCount,
            pathIconCount / (double)rootCount,
            stackPanelCount / (double)rootCount,
            addOnDecoratedBoxCount / (double)rootCount);
    }

    private static bool IsAtomIcon(Type type)
    {
        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == "AtomUI.Controls.Icon")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool IsAddOnDecoratedBox(Type type)
    {
        if (type.FullName == "AtomUI.Desktop.Controls.AddOnDecoratedBox")
        {
            return true;
        }

        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == "AtomUI.Desktop.Controls.AddOnDecoratedBox")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }
}
