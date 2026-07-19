using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIPopup = AtomUI.Desktop.Controls.Popup;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tour;

public class TourPlacementTests
{
    static TourPlacementTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ShowTour_Configures_First_Target_Before_Opening_Popup()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour   = CreateTour(target, Desktop.Controls.TourPlacementMode.Bottom);
        var root   = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            var popup = GetPopup(tour);
            Control? placementTargetAtOpen = null;
            PlacementMode? placementAtOpen = null;
            ArrowPosition? arrowAtOpen = null;
            popup.Opened += (_, _) =>
            {
                placementTargetAtOpen = popup.PlacementTarget;
                placementAtOpen       = popup.RequestedPlacement;
                arrowAtOpen           = tour.ArrowPosition;
            };

            tour.ShowTour();
            RunLayout(window);

            placementTargetAtOpen.ShouldBeSameAs(target);
            placementAtOpen.ShouldBe(PlacementMode.Bottom);
            arrowAtOpen.ShouldBe(ArrowPosition.Top);
            popup.PlacementTarget.ShouldBeSameAs(target);
            popup.RequestedPlacement.ShouldBe(PlacementMode.Bottom);
            tour.CurrentArrowVisible.ShouldBeTrue();
            tour.ArrowPosition.ShouldBe(ArrowPosition.Top);
            tour.TargetClipBounds.ShouldBe(GetExpectedTargetClipBounds(target, window, 6, 6));
        });
    }

    [Fact]
    public void IsOpen_Positions_First_Popup_Below_Target()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour   = CreateTour(target, Desktop.Controls.TourPlacementMode.Bottom);
        var root   = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            var hostBounds   = GetBoundsInWindow(window.GetVisualDescendants().OfType<OverlayPopupHost>().Single(), window);
            var targetBounds = GetBoundsInWindow(target, window);

            hostBounds.Top.ShouldBeGreaterThan(targetBounds.Bottom);
            hostBounds.Center.X.ShouldBe(targetBounds.Center.X, 1);
            tour.ArrowPosition.ShouldBe(ArrowPosition.Top);
        });
    }

    [Fact]
    public void CurrentStepChange_Recomputes_Target_Placement_And_Arrow()
    {
        var upload = CreateTarget(240, 100, 72, 32);
        var save   = CreateTarget(420, 100, 64, 32);
        var tour = new Desktop.Controls.Tour
        {
            GapOffsetX = 6,
            GapOffsetY = 6
        };
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = upload,
            Placement = Desktop.Controls.TourPlacementMode.Bottom
        });
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = save,
            Placement = Desktop.Controls.TourPlacementMode.Right
        });
        var root = CreateRoot(upload, save, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            tour.SetValue(Desktop.Controls.Tour.CurrentIndexProperty, 1);
            RunLayout(window);

            var popup = GetPopup(tour);
            var hostBounds   = GetBoundsInWindow(window.GetVisualDescendants().OfType<OverlayPopupHost>().Single(), window);
            var targetBounds = GetBoundsInWindow(save, window);

            popup.PlacementTarget.ShouldBeSameAs(save);
            popup.RequestedPlacement.ShouldBe(PlacementMode.Right);
            tour.TargetClipBounds.ShouldBe(GetExpectedTargetClipBounds(save, window, 6, 6));
            tour.ArrowPosition.ShouldBe(ArrowPosition.Left);
            hostBounds.Left.ShouldBeGreaterThan(targetBounds.Right);
            hostBounds.Center.Y.ShouldBe(targetBounds.Center.Y, 1);
        });
    }

    [Fact]
    public void Preloaded_StepsSource_Opens_With_First_Step_Synchronized()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour   = new Desktop.Controls.Tour
        {
            StepsSource =
            [
                new Desktop.Controls.TourStep
                {
                    Target    = target,
                    Placement = Desktop.Controls.TourPlacementMode.Bottom
                }
            ],
            IsOpen = true
        };
        var root = CreateRoot(target, tour);
        Control? placementTargetAtOpen = null;
        PlacementMode? placementAtOpen = null;
        ArrowPosition? arrowAtOpen = null;

        ShowInWindow(
            root,
            window =>
            {
                RunLayout(window);

                placementTargetAtOpen.ShouldBeSameAs(target);
                placementAtOpen.ShouldBe(PlacementMode.Bottom);
                arrowAtOpen.ShouldBe(ArrowPosition.Top);
                tour.CurrentIndex.ShouldBe(0);
                GetPopup(tour).PlacementTarget.ShouldBeSameAs(target);
                tour.TargetClipBounds.ShouldBe(GetExpectedTargetClipBounds(target, window, 6, 6));
            },
            _ =>
            {
                tour.ApplyTemplate();
                var popup = GetPopup(tour);
                popup.Opened += (_, _) =>
                {
                    placementTargetAtOpen = popup.PlacementTarget;
                    placementAtOpen       = popup.RequestedPlacement;
                    arrowAtOpen           = tour.ArrowPosition;
                };
            });
    }

    [Fact]
    public void Same_Target_StepChange_Preserves_Flipped_Arrow_Direction()
    {
        var target = CreateTarget(320, 530, 72, 32);
        var tour   = new Desktop.Controls.Tour();
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = target,
            Placement = Desktop.Controls.TourPlacementMode.Bottom
        });
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = target,
            Placement = Desktop.Controls.TourPlacementMode.Bottom
        });
        var root = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            tour.IsPopupVerticalFlipped.ShouldBeTrue();
            tour.ArrowPosition.ShouldBe(ArrowPosition.Bottom);

            tour.SetValue(Desktop.Controls.Tour.CurrentIndexProperty, 1);
            RunLayout(window);

            tour.IsPopupVerticalFlipped.ShouldBeTrue();
            tour.ArrowPosition.ShouldBe(ArrowPosition.Bottom);
        });
    }

    [Fact]
    public void Step_Mask_Visibility_Falls_Back_To_Tour_Mask_Visibility()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour   = CreateTour(target, Desktop.Controls.TourPlacementMode.Bottom);
        tour.IsShowMask     = false;
        tour.IsArrowVisible = true;
        var root = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            var layer = window.GetVisualDescendants().OfType<Desktop.Controls.TourLayer>().Single();
            layer.IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void TourLayer_Motion_Follows_Tour_Motion_Setting()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour   = CreateTour(target, Desktop.Controls.TourPlacementMode.Bottom);
        tour.IsMotionEnabled = true;
        var root = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            var layer = window.GetVisualDescendants().OfType<Desktop.Controls.TourLayer>().Single();
            var transitionProperties = layer.Transitions.ShouldNotBeNull()
                                            .Select(transition => transition.Property)
                                            .ToArray();

            layer.IsMotionEnabled.ShouldBeTrue();
            transitionProperties.ShouldContain(Desktop.Controls.TourLayer.TargetRegionProperty);
            transitionProperties.ShouldContain(Desktop.Controls.TourLayer.TargetRegionCornerRadiusProperty);

            tour.IsMotionEnabled = false;
            RunLayout(window);

            layer.IsMotionEnabled.ShouldBeFalse();
            layer.Transitions.ShouldBeNull();
        });
    }

    [Fact]
    public void Explicit_Step_Placement_Is_Preserved_When_Container_Is_Prepared()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var step = new Desktop.Controls.TourStep
        {
            Target    = target,
            Placement = Desktop.Controls.TourPlacementMode.Right
        };
        var tour = new Desktop.Controls.Tour
        {
            Placement = Desktop.Controls.TourPlacementMode.Bottom
        };
        tour.Steps.Add(step);
        var root = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            step.Placement.ShouldBe(Desktop.Controls.TourPlacementMode.Right);
            GetPopup(tour).RequestedPlacement.ShouldBe(PlacementMode.Right);
            tour.ArrowPosition.ShouldBe(ArrowPosition.Left);
        });
    }

    [Fact]
    public void Step_Placement_Uses_Its_Own_Gap_Axis()
    {
        var target = CreateTarget(320, 100, 72, 32);
        var tour = new Desktop.Controls.Tour
        {
            Placement = Desktop.Controls.TourPlacementMode.Bottom,
            GapOffsetX = 24,
            GapOffsetY = 3
        };
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = target,
            Placement = Desktop.Controls.TourPlacementMode.Right
        });
        var root = CreateRoot(target, tour);

        ShowInWindow(root, window =>
        {
            tour.IsOpen = true;
            RunLayout(window);

            var popup = GetPopup(tour);
            var initialHostBounds = GetBoundsInWindow(
                window.GetVisualDescendants().OfType<OverlayPopupHost>().Single(), window);
            var defaultMargin =
                (TokenResourceUtils.FindTokenResource(tour, TourTokenKind.PopupMarginToAnchor) as double?) ?? 0;
            popup.MarginToAnchor.ShouldBe(defaultMargin + tour.GapOffsetX);

            tour.GapOffsetX = 30;
            RunLayout(window);

            var updatedHostBounds = GetBoundsInWindow(
                window.GetVisualDescendants().OfType<OverlayPopupHost>().Single(), window);
            popup.MarginToAnchor.ShouldBe(defaultMargin + tour.GapOffsetX);
            updatedHostBounds.Left.ShouldBe(initialHostBounds.Left + 6, 1);
        });
    }

    private static Border CreateTarget(double left, double top, double width, double height)
    {
        var target = new Border
        {
            Width  = width,
            Height = height
        };
        Canvas.SetLeft(target, left);
        Canvas.SetTop(target, top);
        return target;
    }

    private static Desktop.Controls.Tour CreateTour(Control target, Desktop.Controls.TourPlacementMode placement)
    {
        var tour = new Desktop.Controls.Tour
        {
            GapOffsetX = 6,
            GapOffsetY = 6
        };
        tour.Steps.Add(new Desktop.Controls.TourStep
        {
            Target    = target,
            Placement = placement
        });
        return tour;
    }

    private static TestCanvas CreateRoot(params Control[] children)
    {
        var root = new TestCanvas
        {
            Width  = 800,
            Height = 600
        };

        foreach (var child in children)
        {
            root.Children.Add(child);
        }

        return root;
    }

    private static AtomUIPopup GetPopup(Desktop.Controls.Tour tour)
    {
        return tour.GetTemplateDescendants().OfType<AtomUIPopup>().Single();
    }

    private static Rect GetExpectedTargetClipBounds(Control target, Visual relativeTo, double gapX, double gapY)
    {
        return GetBoundsInWindow(target, relativeTo).Inflate(new Thickness(gapX, gapY));
    }

    private static Rect GetBoundsInWindow(Visual visual, Visual relativeTo)
    {
        var transform = visual.TransformToVisual(relativeTo);
        transform.ShouldNotBeNull();
        return new Rect(visual.Bounds.Size).TransformToAABB(transform!.Value);
    }

    private static void RunLayout(AtomUIWindow window)
    {
        Dispatcher.UIThread.RunJobs();
        window.CaptureRenderedFrame();
        Dispatcher.UIThread.RunJobs();
    }

    private static void ShowInWindow(
        TestCanvas content,
        Action<AtomUIWindow> assertion,
        Action<AtomUIWindow>? beforeShow = null)
    {
        var window = new AtomUIWindow
        {
            Width   = 800,
            Height  = 600,
            Content = content
        };

        try
        {
            beforeShow?.Invoke(window);
            window.Show();
            RunLayout(window);
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class TestCanvas : Canvas;
}
