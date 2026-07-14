using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIPopup = AtomUI.Desktop.Controls.Popup;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Popups;

public class PopupPlacementTests
{
    private const PopupPositionerConstraintAdjustment AllAdjustments =
        PopupPositionerConstraintAdjustment.FlipX |
        PopupPositionerConstraintAdjustment.FlipY |
        PopupPositionerConstraintAdjustment.SlideX |
        PopupPositionerConstraintAdjustment.SlideY |
        PopupPositionerConstraintAdjustment.ResizeX |
        PopupPositionerConstraintAdjustment.ResizeY;

    static PopupPlacementTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Overlay_Popup_Flips_Before_Entering_Csd_Bottom_Shadow()
    {
        var visibleFrame = new Rect(20, 20, 440, 300);
        var anchorRect   = new Rect(170, 240, 100, 30);

        var (geometry, flipX, flipY) = PopupUtils.CalculateConstrainedGeometry(
            anchorRect,
            new Size(180, 70),
            PopupAnchor.Bottom,
            PopupGravity.Bottom,
            AllAdjustments,
            default,
            visibleFrame);

        flipX.ShouldBeFalse();
        flipY.ShouldBeTrue();
        geometry.Y.ShouldBe(170);
        geometry.Bottom.ShouldBeLessThanOrEqualTo(visibleFrame.Bottom);
    }

    [Fact]
    public void Overlay_Popup_Slides_Inside_Csd_Visible_Frame()
    {
        var visibleFrame = new Rect(20, 20, 440, 300);
        var anchorRect   = new Rect(430, 100, 20, 30);

        var (geometry, _, _) = PopupUtils.CalculateConstrainedGeometry(
            anchorRect,
            new Size(120, 80),
            PopupAnchor.BottomLeft,
            PopupGravity.BottomRight,
            PopupPositionerConstraintAdjustment.SlideX,
            default,
            visibleFrame);

        geometry.Right.ShouldBe(visibleFrame.Right);
        geometry.Left.ShouldBeGreaterThanOrEqualTo(visibleFrame.Left);
    }

    [Fact]
    public void Custom_Placement_With_No_Adjustments_Remains_Unconstrained()
    {
        var visibleFrame = new Rect(20, 20, 440, 300);
        var anchorRect   = new Rect(450, 310, 1, 1);

        var (geometry, flipX, flipY) = PopupUtils.CalculateConstrainedGeometry(
            anchorRect,
            new Size(120, 80),
            PopupAnchor.TopLeft,
            PopupGravity.BottomRight,
            PopupPositionerConstraintAdjustment.None,
            default,
            visibleFrame);

        flipX.ShouldBeFalse();
        flipY.ShouldBeFalse();
        geometry.Position.ShouldBe(anchorRect.TopLeft);
        geometry.Right.ShouldBeGreaterThan(visibleFrame.Right);
        geometry.Bottom.ShouldBeGreaterThan(visibleFrame.Bottom);
    }

    [Fact]
    public void Csd_Overlay_Popup_Uses_Visible_Frame_Instead_Of_Shadow_Surface()
    {
        var target = new Border
        {
            Width  = 100,
            Height = 30
        };
        var popup = new Popup
        {
            PlacementTarget      = target,
            RequestedPlacement   = PlacementMode.Bottom,
            ShouldUseOverlayLayer = true,
            Child = new Border
            {
                Width  = 180,
                Height = 70
            }
        };
        var canvas = new Canvas();
        canvas.Children.Add(target);
        canvas.Children.Add(popup);

        var window = new AtomUIWindow
        {
            Width   = 480,
            Height  = 360,
            Content = canvas
        };

        try
        {
            window.Show();
            window.IsCsdEnabled         = true;
            window.FrameShadowThickness = new Thickness(20, 20, 20, 40);
            Dispatcher.UIThread.RunJobs();

            var canvasOrigin = canvas.TransformToVisual(window)!.Value.Transform(default(Point));
            var visibleFrame = new Rect(default, window.ClientSize).Deflate(window.FrameShadowThickness);
            Canvas.SetLeft(target, visibleFrame.Left + 120 - canvasOrigin.X);
            Canvas.SetTop(target, visibleFrame.Bottom - 30 - target.Height - canvasOrigin.Y);
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            var host = window.GetVisualDescendants().OfType<OverlayPopupHost>().Single();
            var hostTransform = host.TransformToVisual(window);
            hostTransform.ShouldNotBeNull();
            var hostBounds = new Rect(host.Bounds.Size).TransformToAABB(hostTransform!.Value);

            popup.IsVerticalFlipped.ShouldBeTrue();
            hostBounds.Top.ShouldBeGreaterThanOrEqualTo(visibleFrame.Top);
            hostBounds.Bottom.ShouldBeLessThanOrEqualTo(visibleFrame.Bottom);
        }
        finally
        {
            popup.IsOpen = false;
            window.Close();
        }
    }

    [Fact]
    public void Overlay_Popup_Repositions_When_PlacementTarget_Ancestor_Scrolls()
    {
        var target = new Border
        {
            Width  = 100,
            Height = 30
        };
        var popup = new AtomUIPopup
        {
            PlacementTarget      = target,
            RequestedPlacement   = PlacementMode.Bottom,
            ShouldUseOverlayLayer = true,
            Child = new Border
            {
                Width  = 120,
                Height = 40
            }
        };
        var canvas = new Canvas
        {
            Width  = 300,
            Height = 700
        };
        Canvas.SetLeft(target, 80);
        Canvas.SetTop(target, 180);
        canvas.Children.Add(target);
        canvas.Children.Add(popup);

        var scrollViewer = new ScrollViewer
        {
            Width  = 300,
            Height = 260,
            Content = canvas
        };
        var window = new AtomUIWindow
        {
            Width   = 360,
            Height  = 320,
            Content = scrollViewer
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            var host = window.GetVisualDescendants().OfType<OverlayPopupHost>().Single();
            var initialTargetTop = target.TransformToVisual(window)!.Value.Transform(default(Point)).Y;
            var initialHostTop   = host.TransformToVisual(window)!.Value.Transform(default(Point)).Y;

            scrollViewer.Offset = new Vector(0, 80);
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            var scrolledTargetTop = target.TransformToVisual(window)!.Value.Transform(default(Point)).Y;
            var scrolledHostTop   = host.TransformToVisual(window)!.Value.Transform(default(Point)).Y;
            var targetDelta       = scrolledTargetTop - initialTargetTop;
            var hostDelta         = scrolledHostTop - initialHostTop;

            targetDelta.ShouldBe(-80, 0.5);
            hostDelta.ShouldBe(targetDelta, 0.5);
        }
        finally
        {
            popup.IsOpen = false;
            window.Close();
        }
    }

    [Fact]
    public void Overlay_Popup_Closes_When_PlacementTarget_Scrolls_Above_Window()
    {
        var (window, scrollViewer, popup) = CreateScrollableOverlayPopupTestWindow(
            contentWidth: 300,
            contentHeight: 700,
            targetLeft: 80,
            targetTop: 180,
            viewportWidth: 300,
            viewportHeight: 260);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            popup.IsOpen.ShouldBeTrue();

            scrollViewer.Offset = new Vector(0, 260);
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeFalse();
        }
        finally
        {
            popup.IsOpen = false;
            window.Close();
        }
    }

    [Fact]
    public void Overlay_Popup_Closes_When_PlacementTarget_Scrolls_Left_Of_Window()
    {
        var (window, scrollViewer, popup) = CreateScrollableOverlayPopupTestWindow(
            contentWidth: 700,
            contentHeight: 260,
            targetLeft: 180,
            targetTop: 80,
            viewportWidth: 300,
            viewportHeight: 260);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            popup.IsOpen.ShouldBeTrue();

            scrollViewer.Offset = new Vector(320, 0);
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeFalse();
        }
        finally
        {
            popup.IsOpen = false;
            window.Close();
        }
    }

    [Fact]
    public void Overlay_Popup_Without_PlacementTarget_Stays_Open_When_Logical_Host_Has_Empty_Bounds()
    {
        var popup = new AtomUIPopup
        {
            ShouldUseOverlayLayer = true,
            Child = new Border
            {
                Width  = 120,
                Height = 40
            },
            IsOpen = true
        };
        var root = new Panel();
        root.Children.Add(popup);
        var window = new AtomUIWindow
        {
            Width   = 360,
            Height  = 320,
            Content = root
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.CaptureRenderedFrame();
            Dispatcher.UIThread.RunJobs();

            popup.IsOpen.ShouldBeTrue();
            window.GetVisualDescendants().OfType<OverlayPopupHost>().Single();
        }
        finally
        {
            popup.IsOpen = false;
            window.Close();
        }
    }

    private static (AtomUIWindow Window, ScrollViewer ScrollViewer, AtomUIPopup Popup)
        CreateScrollableOverlayPopupTestWindow(
            double contentWidth,
            double contentHeight,
            double targetLeft,
            double targetTop,
            double viewportWidth,
            double viewportHeight)
    {
        var target = new Border
        {
            Width  = 100,
            Height = 30
        };
        var popup = new AtomUIPopup
        {
            PlacementTarget      = target,
            RequestedPlacement   = PlacementMode.Bottom,
            ShouldUseOverlayLayer = true,
            Child = new Border
            {
                Width  = 120,
                Height = 40
            }
        };
        var canvas = new Canvas
        {
            Width  = contentWidth,
            Height = contentHeight
        };
        Canvas.SetLeft(target, targetLeft);
        Canvas.SetTop(target, targetTop);
        canvas.Children.Add(target);
        canvas.Children.Add(popup);

        var scrollViewer = new ScrollViewer
        {
            Width  = viewportWidth,
            Height = viewportHeight,
            Content = canvas
        };
        var window = new AtomUIWindow
        {
            Width   = viewportWidth + 60,
            Height  = viewportHeight + 60,
            Content = scrollViewer
        };

        return (window, scrollViewer, popup);
    }
}
