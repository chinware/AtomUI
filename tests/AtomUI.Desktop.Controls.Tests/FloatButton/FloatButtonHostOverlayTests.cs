using System.Threading;
using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
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

    [Fact]
    public void FloatButtonGroupHost_Click_Trigger_Reopens_After_Shared_Bound_State_Closes()
    {
        var viewModel = new OpenStateViewModel
        {
            IsOpen = true
        };
        var hoverHost = CreateBoundGroupHost(viewModel, FloatButtonGroupTrigger.Hover, FloatButtonShape.Square);
        hoverHost.FloatOffsetX = 80;
        var clickHost = CreateBoundGroupHost(viewModel, FloatButtonGroupTrigger.Click, FloatButtonShape.Circle);
        var panel = new Panel();
        panel.Children.Add(hoverHost);
        panel.Children.Add(clickHost);

        var window = CreateWindow(panel, out var overlayPanel);

        try
        {
            var overlayLayer = ScopeAwareOverlayLayer.FindLayer(overlayPanel);
            overlayLayer.ShouldNotBeNull();

            var clickGroup = overlayLayer.GetVisualDescendants()
                                         .OfType<FloatButtonGroup>()
                                         .Single(group => group.Trigger == FloatButtonGroupTrigger.Click);

            viewModel.IsOpen = false;
            Dispatcher.UIThread.RunJobs();

            clickGroup.IsOpen.ShouldBeFalse();

            var triggerButton = clickGroup.GetVisualDescendants()
                                          .OfType<AtomUI.Desktop.Controls.FloatButton>()
                                          .Single(button => button.GetVisualParent() is Canvas);

            Click(triggerButton, window);
            Dispatcher.UIThread.RunJobs();

            viewModel.IsOpen.ShouldBeTrue();
            clickHost.IsOpen.ShouldBeTrue();
            clickGroup.IsOpen.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FloatButtonGroupHost_Does_Not_Cancel_External_Switch_Open_When_Closed()
    {
        var viewModel = new OpenStateViewModel
        {
            IsOpen = false
        };
        var toggleSwitch = new AtomUI.Desktop.Controls.ToggleSwitch();
        toggleSwitch.Bind(
            Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty,
            new Binding(nameof(OpenStateViewModel.IsOpen))
            {
                Source = viewModel
            });
        var hoverHost = CreateBoundGroupHost(viewModel, FloatButtonGroupTrigger.Hover, FloatButtonShape.Square);
        hoverHost.FloatOffsetX = 80;
        var clickHost = CreateBoundGroupHost(viewModel, FloatButtonGroupTrigger.Click, FloatButtonShape.Circle);
        var panel = new Panel();
        panel.Children.Add(toggleSwitch);
        panel.Children.Add(hoverHost);
        panel.Children.Add(clickHost);

        var window = CreateWindow(panel, out _);

        try
        {
            Dispatcher.UIThread.RunJobs();

            clickHost.IsOpen.ShouldBeFalse();

            Click(toggleSwitch, window);
            Dispatcher.UIThread.RunJobs();

            viewModel.IsOpen.ShouldBeTrue();
            toggleSwitch.IsChecked.ShouldBe(true);
            hoverHost.IsOpen.ShouldBeTrue();
            clickHost.IsOpen.ShouldBeTrue();
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

    private static FloatButtonGroupHost CreateBoundGroupHost(
        OpenStateViewModel viewModel,
        FloatButtonGroupTrigger trigger,
        FloatButtonShape shape)
    {
        var host = new FloatButtonGroupHost
        {
            Trigger         = trigger,
            Shape           = shape,
            IsMotionEnabled = false
        };
        host.Bind(
            FloatButtonGroupHost.IsOpenProperty,
            new Binding(nameof(OpenStateViewModel.IsOpen))
            {
                Source = viewModel
            });
        host.Children.Add(new AtomUI.Desktop.Controls.FloatButton());
        host.Children.Add(new AtomUI.Desktop.Controls.FloatButton());
        return host;
    }

    private static void Click(Control control, Avalonia.Controls.Window window)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window);
        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private sealed class OpenStateViewModel : INotifyPropertyChanged
    {
        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen == value)
                {
                    return;
                }

                _isOpen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOpen)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
