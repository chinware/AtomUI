using System.Threading;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.FloatButton;

public class FloatButtonHostOverlayTests
{
    static FloatButtonHostOverlayTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void FloatButtonHost_Removes_Overlay_Button_When_Detached()
    {
        var host = new AtomUI.Desktop.Controls.FloatButtonHost();
        var window = CreateWindow(host, out var overlayPanel);

        try
        {
            var overlayLayer = ScopeAwareOverlayLayer.FindLayer(overlayPanel);

            overlayLayer.ShouldNotBeNull();
            overlayLayer.Children.Count.ShouldBe(1);

            overlayPanel.Children.Remove(host);
            Dispatcher.UIThread.RunJobs();

            overlayLayer.Children.Count.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FloatButtonGroupHost_Removes_Overlay_Group_When_Detached()
    {
        var host = new FloatButtonGroupHost();
        var window = CreateWindow(host, out var overlayPanel);

        try
        {
            var overlayLayer = ScopeAwareOverlayLayer.FindLayer(overlayPanel);

            overlayLayer.ShouldNotBeNull();
            overlayLayer.Children.Count.ShouldBe(1);

            overlayPanel.Children.Remove(host);
            Dispatcher.UIThread.RunJobs();

            overlayLayer.Children.Count.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FloatButtonGroupHost_Keeps_Initially_Closed_Menu_Hidden()
    {
        var host = new FloatButtonGroupHost
        {
            Trigger = FloatButtonGroupTrigger.Click
        };
        host.Children.Add(new AtomUI.Desktop.Controls.FloatButton());

        var window = CreateWindow(host, out var overlayPanel);

        try
        {
            var overlayLayer = ScopeAwareOverlayLayer.FindLayer(overlayPanel);
            overlayLayer.ShouldNotBeNull();

            var group = overlayLayer.GetVisualDescendants()
                                    .OfType<FloatButtonGroup>()
                                    .Single();
            var motionActor = group.GetVisualDescendants()
                                   .OfType<BaseMotionActor>()
                                   .Single();

            group.IsOpen.ShouldBeFalse();
            motionActor.IsVisible.ShouldBeFalse();

            Thread.Sleep(500);
            Dispatcher.UIThread.RunJobs();

            group.IsOpen.ShouldBeFalse();
            motionActor.IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static Avalonia.Controls.Window CreateWindow(Control host, out ScopeAwareOverlayLayerPanel overlayPanel)
    {
        overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 320,
            Height = 240
        };
        overlayPanel.Children.Add(host);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = overlayPanel
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
