using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomSplitView = AtomUI.Desktop.Controls.SplitView;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSplitViewScenarios()
    {
        return
        [
            new PerfScenario("SplitView.Closed.Overlay.Left", _ => CreateSplitView()),
            new PerfScenario("SplitView.Open.Overlay.Left", _ => CreateSplitView(isPaneOpen: true)),
            new PerfScenario("SplitView.Closed.CompactInline.Left", _ => CreateSplitView(SplitViewDisplayMode.CompactInline)),
            new PerfScenario("SplitView.Open.Inline.Left", _ => CreateSplitView(SplitViewDisplayMode.Inline, true)),
            new PerfScenario("SplitView.Open.CompactOverlay.Right", _ => CreateSplitView(
                SplitViewDisplayMode.CompactOverlay,
                true,
                SplitViewPanePlacement.Right)),
            new PerfScenario("SplitView.Open.Overlay.Top", _ => CreateSplitView(
                SplitViewDisplayMode.Overlay,
                true,
                SplitViewPanePlacement.Top)),
            new PerfScenario("SplitView.Open.Overlay.Bottom", _ => CreateSplitView(
                SplitViewDisplayMode.Overlay,
                true,
                SplitViewPanePlacement.Bottom)),
            new PerfScenario("SplitView.MotionDisabled.Open", _ => CreateSplitView(isPaneOpen: true, isMotionEnabled: false)),
            new PerfScenario("SplitView.Batch.ClosedOverlay8", _ => CreateSplitViewBatch())
        ];
    }

    private static AtomSplitView CreateSplitView(
        SplitViewDisplayMode displayMode = SplitViewDisplayMode.Overlay,
        bool isPaneOpen = false,
        SplitViewPanePlacement panePlacement = SplitViewPanePlacement.Left,
        bool isMotionEnabled = true)
    {
        return new AtomSplitView
        {
            Width                      = 420,
            Height                     = panePlacement is SplitViewPanePlacement.Top or SplitViewPanePlacement.Bottom ? 260 : 220,
            DisplayMode                = displayMode,
            IsPaneOpen                 = isPaneOpen,
            PanePlacement              = panePlacement,
            UseLightDismissOverlayMode = true,
            IsMotionEnabled            = isMotionEnabled,
            Pane                       = CreateSplitViewPane(panePlacement),
            Content                    = CreateSplitViewContent()
        };
    }

    private static Control CreateSplitViewBatch()
    {
        var panel = new StackPanel
        {
            Spacing = 8
        };

        for (var i = 0; i < 8; i++)
        {
            panel.Children.Add(CreateSplitView(
                displayMode: i % 2 == 0 ? SplitViewDisplayMode.Overlay : SplitViewDisplayMode.CompactInline,
                panePlacement: (i % 4) switch
                {
                    1 => SplitViewPanePlacement.Right,
                    2 => SplitViewPanePlacement.Top,
                    3 => SplitViewPanePlacement.Bottom,
                    _ => SplitViewPanePlacement.Left
                }));
        }

        return panel;
    }

    private static Border CreateSplitViewPane(SplitViewPanePlacement placement)
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(245, 247, 250)),
            Child      = new StackPanel
            {
                Margin   = new Avalonia.Thickness(12),
                Spacing  = 8,
                Children =
                {
                    new AtomTextBlock { Text = placement.ToString() },
                    new AtomTextBlock { Text = "Pane item A" },
                    new AtomTextBlock { Text = "Pane item B" }
                }
            }
        };
    }

    private static Border CreateSplitViewContent()
    {
        return new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            Child      = new AtomTextBlock
            {
                Text                = "Content",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            }
        };
    }
}
