using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.ScrollViewers;

public class ScrollViewerLayoutTests
{
    static ScrollViewerLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(1.0, 0.99)]
    [InlineData(1.25, 0.79)]
    [InlineData(1.5, 0.66)]
    [InlineData(1.6666666666666667, 0.59)]
    [InlineData(2.0, 0.49)]
    public void Auto_ScrollBar_Hides_When_Range_Is_Less_Than_One_Physical_Pixel(
        double renderScaling,
        double maximum)
    {
        var scrollBar = CreateScrollBar(maximum);
        var window    = ShowInWindow(scrollBar, renderScaling);

        try
        {
            scrollBar.IsVisible.ShouldBeFalse();
            scrollBar.Maximum.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(1.0, 1.01)]
    [InlineData(1.25, 0.81)]
    [InlineData(1.5, 0.68)]
    [InlineData(1.6666666666666667, 0.61)]
    [InlineData(2.0, 0.51)]
    public void Auto_ScrollBar_Remains_Visible_When_Range_Exceeds_One_Physical_Pixel(
        double renderScaling,
        double maximum)
    {
        var scrollBar = CreateScrollBar(maximum);
        var window    = ShowInWindow(scrollBar, renderScaling);

        try
        {
            scrollBar.IsVisible.ShouldBeTrue();
            scrollBar.Maximum.ShouldBe(maximum, 0.000001);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ScrollViewer_Hides_One_Pixel_Wayland_Resize_Remainder()
    {
        var scrollViewer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            AllowAutoHide                  = false,
            Content                        = new Border { Width = 500, Height = 700 }
        };
        var window = new AvaloniaWindow
        {
            Width   = 600,
            Height  = 699.4,
            Content = scrollViewer
        };
        window.SetRenderScaling(1.6666666666666667);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var verticalScrollBar = scrollViewer.GetVisualDescendants()
                                                 .OfType<ScrollBar>()
                                                 .Single(item => item.Orientation == Orientation.Vertical);

            // The logical range is positive because the client size is
            // fractional, but it is only one physical pixel at this scale.
            scrollViewer.ScrollBarMaximum.Y.ShouldBeGreaterThan(0);
            verticalScrollBar.IsVisible.ShouldBeFalse();
            verticalScrollBar.Maximum.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ScrollViewer_Keeps_The_Bar_For_A_Real_Overflow()
    {
        var scrollViewer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            AllowAutoHide                  = false,
            Content                        = new Border { Width = 500, Height = 700 }
        };
        var window = new AvaloniaWindow
        {
            Width   = 600,
            Height  = 699,
            Content = scrollViewer
        };
        window.SetRenderScaling(1.6666666666666667);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var verticalScrollBar = scrollViewer.GetVisualDescendants()
                                                 .OfType<ScrollBar>()
                                                 .Single(item => item.Orientation == Orientation.Vertical);

            scrollViewer.ScrollBarMaximum.Y.ShouldBeGreaterThan(1);
            verticalScrollBar.IsVisible.ShouldBeTrue();
            verticalScrollBar.Maximum.ShouldBeGreaterThan(1);
        }
        finally
        {
            window.Close();
        }
    }

    private static ScrollBar CreateScrollBar(double maximum)
    {
        return new ScrollBar
        {
            Orientation  = Orientation.Vertical,
            Visibility   = ScrollBarVisibility.Auto,
            ViewportSize = 100,
            Maximum      = maximum,
            Width        = 20,
            Height       = 100
        };
    }

    private static AvaloniaWindow ShowInWindow(Control content, double renderScaling)
    {
        var window = new AvaloniaWindow
        {
            Width   = 20,
            Height  = 100,
            Content = content
        };
        window.SetRenderScaling(renderScaling);
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
