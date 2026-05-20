using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomSplitter = AtomUI.Desktop.Controls.Splitter;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSplitterScenarios()
    {
        return
        [
            new PerfScenario("Splitter.Basic.Vertical2", _ => CreateBasicSplitter(Orientation.Vertical)),
            new PerfScenario("Splitter.Basic.Horizontal2", _ => CreateBasicSplitter(Orientation.Horizontal)),
            new PerfScenario("Splitter.Multi.Vertical4", _ => CreateMultiPanelSplitter()),
            new PerfScenario("Splitter.Collapsible.Always3", _ => CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode.Always)),
            new PerfScenario("Splitter.Collapsible.Hidden3", _ => CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode.Hidden)),
            new PerfScenario("Splitter.Lazy.Pair", _ => CreateLazySplitterPair()),
            new PerfScenario("Splitter.GalleryShape", _ => CreateSplitterGalleryShape())
        ];
    }

    private static AtomSplitter CreateBasicSplitter(Orientation orientation)
    {
        var splitter = new AtomSplitter
        {
            Orientation = orientation,
            Width       = orientation == Orientation.Vertical ? 420 : double.NaN,
            Height      = orientation == Orientation.Vertical ? 180 : 220
        };
        splitter.Children.Add(CreateSplitterPanel("First", size: "40%"));
        splitter.Children.Add(CreateSplitterPanel("Second"));
        return splitter;
    }

    private static AtomSplitter CreateMultiPanelSplitter()
    {
        var splitter = new AtomSplitter
        {
            Orientation = Orientation.Vertical,
            Width       = 520,
            Height      = 220
        };
        splitter.Children.Add(CreateSplitterPanel("A", size: "20%"));
        splitter.Children.Add(CreateSplitterPanel("B", size: "20%", isAlternate: true));
        splitter.Children.Add(CreateSplitterPanel("C"));
        splitter.Children.Add(CreateSplitterPanel("D", isAlternate: true));
        return splitter;
    }

    private static AtomSplitter CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode mode)
    {
        var splitter = new AtomSplitter
        {
            Orientation = Orientation.Vertical,
            Width       = 520,
            Height      = 220
        };
        var first  = CreateSplitterPanel("First", defaultSize: "33%");
        var second = CreateSplitterPanel("Second", defaultSize: "34%", isAlternate: true);
        var third  = CreateSplitterPanel("Third");

        var collapsible = new SplitterPanelCollapsible
        {
            IsEnabled           = true,
            ShowCollapsibleIcon = mode
        };
        AtomSplitter.SetCollapsible(first, collapsible);
        AtomSplitter.SetCollapsible(second, collapsible);
        AtomSplitter.SetCollapsible(third, collapsible);

        splitter.Children.Add(first);
        splitter.Children.Add(second);
        splitter.Children.Add(third);
        return splitter;
    }

    private static StackPanel CreateLazySplitterPair()
    {
        return new StackPanel
        {
            Spacing  = 12,
            Children =
            {
                CreateLazySplitter(Orientation.Vertical),
                CreateLazySplitter(Orientation.Horizontal)
            }
        };
    }

    private static AtomSplitter CreateLazySplitter(Orientation orientation)
    {
        var splitter = new AtomSplitter
        {
            Orientation = orientation,
            IsLazy      = true,
            Width       = orientation == Orientation.Vertical ? 420 : double.NaN,
            Height      = orientation == Orientation.Vertical ? 140 : 180
        };
        splitter.Children.Add(CreateSplitterPanel("First", size: "50%"));
        splitter.Children.Add(CreateSplitterPanel("Second", isAlternate: true));
        return splitter;
    }

    private static Control CreateSplitterGalleryShape()
    {
        return new StackPanel
        {
            Spacing  = 12,
            Children =
            {
                CreateBasicSplitter(Orientation.Vertical),
                CreateBasicSplitter(Orientation.Horizontal),
                CreateCompositeSplitter(),
                CreateResizableDisabledSplitter(),
                CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode.Always),
                CreateMultiPanelSplitter(),
                CreateLazySplitterPair()
            }
        };
    }

    private static AtomSplitter CreateCompositeSplitter()
    {
        var splitter = new AtomSplitter
        {
            Orientation = Orientation.Vertical,
            Width       = 520,
            Height      = 260
        };
        splitter.Children.Add(CreateSplitterPanel("Left", size: "40%"));
        splitter.Children.Add(CreateBasicSplitter(Orientation.Horizontal));
        return splitter;
    }

    private static AtomSplitter CreateResizableDisabledSplitter()
    {
        var splitter = new AtomSplitter
        {
            Orientation = Orientation.Vertical,
            Width       = 520,
            Height      = 220
        };
        splitter.Children.Add(CreateSplitterPanel("Resizable", size: "35%"));
        var disabled = CreateSplitterPanel("Not resizable", defaultSize: "120", isAlternate: true);
        AtomSplitter.SetIsResizable(disabled, false);
        splitter.Children.Add(disabled);
        splitter.Children.Add(CreateSplitterPanel("Resizable", defaultSize: "120"));
        return splitter;
    }

    private static Border CreateSplitterPanel(
        string text,
        string? size = null,
        string? defaultSize = null,
        string? minSize = null,
        bool isAlternate = false)
    {
        var panel = new Border
        {
            Background = new SolidColorBrush(isAlternate
                ? Color.FromArgb(20, 0, 0, 0)
                : Color.FromArgb(10, 0, 0, 0)),
            Child = new AtomTextBlock
            {
                Text                = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            }
        };
        if (size != null)
        {
            AtomSplitter.SetSize(panel, Dimension.Parse(size));
        }
        if (defaultSize != null)
        {
            AtomSplitter.SetDefaultSize(panel, Dimension.Parse(defaultSize));
        }
        if (minSize != null)
        {
            AtomSplitter.SetMinSize(panel, Dimension.Parse(minSize));
        }
        return panel;
    }
}
