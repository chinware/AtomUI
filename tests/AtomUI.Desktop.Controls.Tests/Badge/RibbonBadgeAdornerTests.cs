using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Badge;

public class RibbonBadgeAdornerTests
{
    static RibbonBadgeAdornerTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CountBadge_Adorner_Uses_Native_Adorner_Layer()
    {
        var countBadge = new AtomUI.Desktop.Controls.CountBadge
        {
            Count           = 5,
            DecoratedTarget = new Border
            {
                Width  = 48,
                Height = 48
            }
        };
        using var context = ShowInAdornerHost(countBadge);

        var adorner = FindNativeAdorner(countBadge);

        adorner.ShouldNotBeNull();
        AdornerLayer.GetIsClipEnabled(adorner).ShouldBeTrue();
    }

    [Fact]
    public void DotBadge_Adorner_Uses_Native_Adorner_Layer()
    {
        var dotBadge = new AtomUI.Desktop.Controls.DotBadge
        {
            Status          = AtomUI.Controls.Commons.DotBadgeStatus.Success,
            DecoratedTarget = new Border
            {
                Width  = 48,
                Height = 48
            }
        };
        using var context = ShowInAdornerHost(dotBadge);

        var adorner = FindNativeAdorner(dotBadge);

        adorner.ShouldNotBeNull();
        AdornerLayer.GetIsClipEnabled(adorner).ShouldBeTrue();
    }

    [Fact]
    public void RibbonBadge_Target_Mode_Uses_Inline_Visual_Tree()
    {
        var ribbonBadge = new AtomUI.Desktop.Controls.RibbonBadge
        {
            Text            = "v6.0.5",
            DecoratedTarget = new Border
            {
                Width  = 160,
                Height = 80
            }
        };
        using var context = ShowInAdornerHost(ribbonBadge, width: 240, height: 160);

        var nativeAdorner = FindNativeAdorner(ribbonBadge);
        var inlineAdornerCount = ribbonBadge.GetVisualDescendants()
                                            .Count(visual => visual.GetType().Name == "RibbonBadgeAdorner");

        nativeAdorner.ShouldBeNull(
            "RibbonBadge should behave like Ant Design Badge.Ribbon: the ribbon belongs to the decorated content tree, not a window-level adorner.");
        inlineAdornerCount.ShouldBe(1);
    }

    [Fact]
    public void RibbonBadge_Target_Mode_Keeps_Target_Visible_When_Badge_Is_Hidden()
    {
        var target = new Border
        {
            Width  = 160,
            Height = 80
        };
        var ribbonBadge = new AtomUI.Desktop.Controls.RibbonBadge
        {
            Text            = "v6.0.5",
            BadgeIsVisible  = false,
            DecoratedTarget = target
        };
        using var context = ShowInAdornerHost(ribbonBadge, width: 240, height: 160);

        var hasTarget = ribbonBadge.GetVisualDescendants().Any(visual => ReferenceEquals(visual, target));
        var inlineAdornerCount = ribbonBadge.GetVisualDescendants()
                                            .Count(visual => visual.GetType().Name == "RibbonBadgeAdorner");

        hasTarget.ShouldBeTrue("hiding a RibbonBadge should hide only the ribbon overlay, not the decorated content.");
        inlineAdornerCount.ShouldBe(0);
    }

    private static Control? FindNativeAdorner(Control badge)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(badge);
        adornerLayer.ShouldNotBeNull();

        return adornerLayer.Children.SingleOrDefault(child =>
            ReferenceEquals(AdornerLayer.GetAdornedElement(child), badge));
    }

    private static WindowContext ShowInAdornerHost(Control control, double width = 120, double height = 100)
    {
        var window = new AvaloniaWindow
        {
            Width   = width,
            Height  = height,
            Content = new VisualLayerManager
            {
                EnableAdornerLayer = true,
                Child              = control
            }
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return new WindowContext(window);
    }

    private sealed class WindowContext : IDisposable
    {
        private readonly AvaloniaWindow _window;

        public WindowContext(AvaloniaWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
