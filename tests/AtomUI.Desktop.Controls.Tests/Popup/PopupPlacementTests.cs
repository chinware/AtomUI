using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
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
}
