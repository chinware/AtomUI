using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
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
    public void RibbonBadge_Adorner_Allows_Ribbon_To_Draw_Outside_Target_Bounds()
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
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 160,
            Content = new VisualLayerManager
            {
                EnableAdornerLayer = true,
                Child              = ribbonBadge
            }
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var adornerLayer = AdornerLayer.GetAdornerLayer(ribbonBadge);
            adornerLayer.ShouldNotBeNull();

            var adorner = adornerLayer.Children.SingleOrDefault(child =>
                ReferenceEquals(AdornerLayer.GetAdornedElement(child), ribbonBadge));
            adorner.ShouldNotBeNull();
            AdornerLayer.GetIsClipEnabled(adorner).ShouldBeFalse(
                "RibbonBadge intentionally draws its ribbon and folded corner outside the decorated target bounds.");
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
