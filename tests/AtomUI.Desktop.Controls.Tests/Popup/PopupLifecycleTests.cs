using AtomUI.MotionScene;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIPopup = AtomUI.Desktop.Controls.Popup;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Popups;

public class PopupLifecycleTests
{
    static PopupLifecycleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void PlacementTarget_Detach_ForceCloses_Animated_Popup()
    {
        var (window, panel, target, popup) = CreateAnimatedPopupWindow();
        var closedCount = 0;
        popup.Closed += (_, _) => closedCount++;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            panel.Children.Remove(target);
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeFalse();
            popup.IsPlayingCloseMotion.ShouldBeFalse();
            closedCount.ShouldBe(1);
            window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldBeEmpty();

            popup.MarginToAnchor     = 8;
            popup.RequestedPlacement = PlacementMode.Top;
            popup.Placement          = PlacementMode.Bottom;
            Dispatcher.UIThread.RunJobs();
        }
        finally
        {
            CloseWindow(window, popup);
        }
    }

    [Fact]
    public void LogicalOwner_Detach_ForceCloses_Animated_Popup()
    {
        var (window, panel, _, popup) = CreateAnimatedPopupWindow();
        var closedCount = 0;
        popup.Closed += (_, _) => closedCount++;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            panel.Children.Remove(popup);
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeFalse();
            popup.IsPlayingCloseMotion.ShouldBeFalse();
            closedCount.ShouldBe(1);
            window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldBeEmpty();
        }
        finally
        {
            CloseWindow(window, popup);
        }
    }

    [Fact]
    public void PlacementTarget_Change_To_Another_TopLevel_ForceCloses_Animated_Popup()
    {
        var (window, _, _, popup) = CreateAnimatedPopupWindow();
        var otherTarget = new Border
        {
            Width  = 80,
            Height = 24
        };
        var otherWindow = new AtomUIWindow
        {
            Width   = 240,
            Height  = 160,
            Content = otherTarget
        };

        try
        {
            window.Show();
            otherWindow.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            popup.PlacementTarget = otherTarget;
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeFalse();
            popup.IsPlayingCloseMotion.ShouldBeFalse();
            window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldBeEmpty();
        }
        finally
        {
            CloseWindow(window, popup);
            otherWindow.Close();
        }
    }

    [Fact]
    public void Normal_Close_Still_Uses_Close_Motion()
    {
        var (window, _, _, popup) = CreateAnimatedPopupWindow();
        var closedCount = 0;
        popup.Closed += (_, _) => closedCount++;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            popup.Close();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeTrue();
            popup.IsPlayingCloseMotion.ShouldBeTrue();
            closedCount.ShouldBe(0);
            window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldHaveSingleItem();
        }
        finally
        {
            CloseWindow(window, popup);
        }
    }

    [Fact]
    public void Direct_Popup_Without_Logical_Owner_Still_Uses_Close_Motion()
    {
        var target = new Border
        {
            Width  = 100,
            Height = 30
        };
        var popup = CreateAnimatedPopup(target);
        var window = new AtomUIWindow
        {
            Width   = 360,
            Height  = 320,
            Content = target
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            popup.Close();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeTrue();
            popup.IsPlayingCloseMotion.ShouldBeTrue();
            window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldHaveSingleItem();
        }
        finally
        {
            CloseWindow(window, popup);
        }
    }

    [Fact]
    public void Repeated_Close_Coalesces_Into_One_Close_Motion()
    {
        var (window, _, _, popup) = CreateAnimatedPopupWindow();
        var closeMotion = new CountingFadeOutMotion();
        popup.CloseMotion = closeMotion;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            OpenPopup(popup);

            popup.Close();
            popup.Close();
            Dispatcher.UIThread.RunJobs();

            popup.IsPlayingCloseMotion.ShouldBeTrue();
            closeMotion.ConfigurationCount.ShouldBe(1);
        }
        finally
        {
            CloseWindow(window, popup);
        }
    }

    private static (AtomUIWindow Window, Panel Panel, Control Target, AtomUIPopup Popup)
        CreateAnimatedPopupWindow()
    {
        var target = new Border
        {
            Width  = 100,
            Height = 30
        };
        var popup = CreateAnimatedPopup(target);

        var panel = new Canvas();
        panel.Children.Add(target);
        panel.Children.Add(popup);

        var window = new AtomUIWindow
        {
            Width   = 360,
            Height  = 320,
            Content = panel
        };
        return (window, panel, target, popup);
    }

    private static AtomUIPopup CreateAnimatedPopup(Control target)
    {
        var popup = new AtomUIPopup
        {
            PlacementTarget       = target,
            RequestedPlacement    = PlacementMode.Bottom,
            ShouldUseOverlayLayer = true,
            IsMotionEnabled       = true,
            CloseMotion           = new FadeOutMotion(),
            MotionDuration        = TimeSpan.FromSeconds(2),
            Child = new Border
            {
                Width  = 120,
                Height = 40
            }
        };
        popup.NotifyMotionActorReady(new PopupMotionActor());
        return popup;
    }

    private static void OpenPopup(AtomUIPopup popup)
    {
        popup.IsOpen = true;
        Dispatcher.UIThread.RunJobs();
        popup.IsOpen.ShouldBeTrue();
    }

    private static void CloseWindow(AtomUIWindow window, AtomUIPopup popup)
    {
        popup.CancelCloseAnimation();
        popup.IsMotionEnabled = false;
        popup.IsOpen          = false;
        window.Close();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class CountingFadeOutMotion : FadeOutMotion
    {
        public int ConfigurationCount { get; private set; }

        protected override void ConfigureTransitions()
        {
            ConfigurationCount++;
            base.ConfigureTransitions();
        }
    }
}
