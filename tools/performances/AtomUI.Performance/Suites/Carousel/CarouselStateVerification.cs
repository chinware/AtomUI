using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomCarousel = AtomUI.Desktop.Controls.Carousel;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCarouselStateVerification()
    {
        var failures = new List<string>();
        VerifyCarouselDefaultSlotsAreLazy(failures);
        VerifyCarouselNavButtonLifecycle(failures);
        VerifyCarouselPaginationLifecycle(failures);
        VerifyCarouselProgressLifecycle(failures);
        VerifyCarouselPageTransitionLifecycle(failures);
        VerifyCarouselTimerLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Carousel state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Carousel state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyCarouselDefaultSlotsAreLazy(ICollection<string> failures)
    {
        var carousel = CreateCarousel();
        using var _ = RealizeControl(carousel);

        Expect(CountVisualByTypeName(carousel, "CarouselNavButton") == 0,
            "Default Carousel should not create CarouselNavButton.",
            failures);
        Expect(CountVisualByTypeName(carousel, "CarouselPagination") == 1,
            "Default Carousel should create CarouselPagination.",
            failures);
        Expect(FindVisualByName<LayoutTransformControl>(carousel, "PaginationLayoutTransform") == null,
            "Bottom pagination should not create LayoutTransformControl.",
            failures);
        Expect(FindVisualByName<Border>(carousel, "Progress") == null,
            "Default Carousel should not create progress borders.",
            failures);
        Expect(carousel.PageTransition == null,
            "Default Carousel should not create PageTransition before the first real navigation.",
            failures);
        Expect(GetPrivateField(carousel, "AtomUI.Desktop.Controls.Carousel", "_autoPlayTimer") == null,
            "Default Carousel should not create an autoplay timer.",
            failures);
    }

    private static void VerifyCarouselNavButtonLifecycle(ICollection<string> failures)
    {
        var carousel = CreateCarousel();
        using var realized = RealizeControl(carousel);

        carousel.SetCurrentValue(AtomCarousel.IsShowNavButtonsProperty, true);
        RefreshLayout(realized.Window);
        var firstPrevious = FindVisualByName<Control>(carousel, "PART_PreviousButton");
        var firstNext     = FindVisualByName<Control>(carousel, "PART_NextButton");
        Expect(firstPrevious != null && firstNext != null,
            "Carousel should create both nav buttons when IsShowNavButtons becomes true.",
            failures);
        Expect(CountVisualByTypeName(carousel, "CarouselNavButton") == 2,
            "Carousel should create exactly two nav buttons.",
            failures);
        Expect(firstNext is CarouselNavButton { IconBrush: ISolidColorBrush nextIconBrush } &&
               nextIconBrush.Color == Avalonia.Media.Colors.White,
            "Dynamically created Carousel nav button should keep the Carousel white icon brush.",
            failures);
        Expect(firstNext is CarouselNavButton nextButton &&
               Math.Abs(nextButton.Opacity - 0.2) < 0.001,
            "Dynamically created Carousel nav button should keep the Carousel default opacity.",
            failures);

        if (firstNext is CarouselNavButton navButton)
        {
            navButton.SetCurrentValue(IconButton.IsMotionEnabledProperty, false);
            RefreshLayout(realized.Window);
            MoveMouseToControl(realized.Window, navButton);
            RefreshLayout(realized.Window);
            Expect(navButton.IsPointerOver,
                "Carousel nav button verification should exercise the real pointerover state.",
                failures);
            Expect(navButton.IconBrush is ISolidColorBrush hoverIconBrush &&
                   hoverIconBrush.Color == Avalonia.Media.Colors.White,
                "Carousel nav button should keep the Carousel white icon brush while pointerover.",
                failures);
            Expect(Math.Abs(navButton.Opacity - 1.0) < 0.001,
                "Carousel nav button should keep the Carousel hover opacity while pointerover.",
                failures);

            var navButtonPoint = GetControlCenterPoint(realized.Window, navButton);
            if (navButtonPoint.HasValue)
            {
                realized.Window.MouseDown(navButtonPoint.Value, MouseButton.Left);
                RefreshLayout(realized.Window);
                Expect(navButton.Classes.Contains(":pressed"),
                    "Carousel nav button verification should exercise the real pressed state.",
                    failures);
                Expect(navButton.IconBrush is ISolidColorBrush pressedIconBrush &&
                       pressedIconBrush.Color == Avalonia.Media.Colors.White,
                    "Carousel nav button should keep the Carousel white icon brush while pressed.",
                    failures);
                realized.Window.MouseUp(navButtonPoint.Value, MouseButton.Left);
                RefreshLayout(realized.Window);
            }
            else
            {
                failures.Add("Carousel nav button should translate to a window point for pressed verification.");
            }
        }

        carousel.SetCurrentValue(AtomCarousel.IsShowNavButtonsProperty, false);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(carousel, "CarouselNavButton") == 0,
            "Carousel should remove nav buttons when IsShowNavButtons becomes false.",
            failures);
        Expect(firstPrevious?.GetVisualParent() == null && firstNext?.GetVisualParent() == null,
            "Removed Carousel nav buttons should not keep visual parents.",
            failures);
        Expect(firstPrevious == null || firstPrevious.TemplatedParent == null,
            "Removed previous nav button should clear templated parent.",
            failures);
        Expect(firstNext == null || firstNext.TemplatedParent == null,
            "Removed next nav button should clear templated parent.",
            failures);
        Expect(GetPrivateField(carousel, "AtomUI.Desktop.Controls.Carousel", "_previousButton") == null &&
               GetPrivateField(carousel, "AtomUI.Desktop.Controls.Carousel", "_nextButton") == null,
            "Carousel should clear nav button fields after removal.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsShowNavButtonsProperty, true);
        RefreshLayout(realized.Window);
        var secondPrevious = FindVisualByName<Control>(carousel, "PART_PreviousButton");
        Expect(secondPrevious != null,
            "Carousel should recreate nav buttons when IsShowNavButtons becomes true again.",
            failures);
        Expect(!ReferenceEquals(firstPrevious, secondPrevious),
            "Carousel should not reuse removed nav button instances.",
            failures);
    }

    private static void VerifyCarouselPaginationLifecycle(ICollection<string> failures)
    {
        var carousel = CreateCarousel();
        using var realized = RealizeControl(carousel);
        var firstPagination = FindVisualByTypeName(carousel, "CarouselPagination");
        Expect(firstPagination != null,
            "Carousel should create pagination when IsShowPagination is true.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.PaginationPositionProperty, CarouselPaginationPosition.Left);
        RefreshLayout(realized.Window);
        var transform = FindVisualByName<LayoutTransformControl>(carousel, "PaginationLayoutTransform");
        Expect(transform != null,
            "Vertical pagination should create PaginationLayoutTransform.",
            failures);
        Expect(CountVisualByTypeName(carousel, "CarouselPagination") == 1,
            "Vertical pagination should keep a single CarouselPagination.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.PaginationPositionProperty, CarouselPaginationPosition.Bottom);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<LayoutTransformControl>(carousel, "PaginationLayoutTransform") == null,
            "Bottom pagination should remove PaginationLayoutTransform.",
            failures);
        Expect(transform?.GetVisualParent() == null,
            "Removed PaginationLayoutTransform should not keep a visual parent.",
            failures);
        Expect(transform == null || transform.TemplatedParent == null,
            "Removed PaginationLayoutTransform should clear templated parent.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsShowPaginationProperty, false);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(carousel, "CarouselPagination") == 0,
            "Carousel should remove pagination when IsShowPagination becomes false.",
            failures);
        Expect(firstPagination?.GetVisualParent() == null,
            "Removed CarouselPagination should not keep a visual parent.",
            failures);
        Expect(firstPagination == null || firstPagination.TemplatedParent == null,
            "Removed CarouselPagination should clear templated parent.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsShowPaginationProperty, true);
        RefreshLayout(realized.Window);
        var secondPagination = FindVisualByTypeName(carousel, "CarouselPagination");
        Expect(secondPagination != null,
            "Carousel should recreate pagination when IsShowPagination becomes true again.",
            failures);
        Expect(!ReferenceEquals(firstPagination, secondPagination),
            "Carousel should not reuse removed pagination instances.",
            failures);
    }

    private static void VerifyCarouselProgressLifecycle(ICollection<string> failures)
    {
        var carousel = CreateCarousel(isAutoPlay: true, isShowTransitionProgress: false);
        using var realized = RealizeControl(carousel);
        Expect(FindVisualByName<Border>(carousel, "Progress") == null,
            "Autoplay Carousel without progress should not create progress borders.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsShowTransitionProgressProperty, true);
        RefreshLayout(realized.Window);
        var progress = FindVisualByName<Border>(carousel, "Progress");
        Expect(CountProgressBorders(carousel) == 1,
            "Progress Carousel should create progress border only for the selected indicator.",
            failures);
        Expect(progress?.IsVisible == true,
            "Selected progress border should be visible.",
            failures);
        Expect(CountCarouselIndicatorFields(carousel, "_animation") == 1,
            "Progress Carousel should create animation only for the selected indicator.",
            failures);
        if (progress is not null)
        {
            var indicator = FindVisualAncestorByTypeName(progress, "CarouselPageIndicator");
            if (indicator is not null)
            {
                MoveMouseToControl(realized.Window, indicator);
                RefreshLayout(realized.Window);
                Expect(indicator.IsPointerOver,
                    "Carousel progress verification should exercise the real pointerover state.",
                    failures);
                Expect(!progress.IsVisible,
                    "Selected progress border should hide while its indicator is pointerover.",
                    failures);
                MoveMouseToControl(realized.Window, carousel);
                RefreshLayout(realized.Window);
                Expect(progress.IsVisible,
                    "Selected progress border should become visible again after pointer leaves the indicator.",
                    failures);
            }
            else
            {
                failures.Add("Selected progress border should have a CarouselPageIndicator visual ancestor.");
            }
        }

        carousel.SetCurrentValue(AtomCarousel.IsShowTransitionProgressProperty, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Border>(carousel, "Progress") == null,
            "Carousel should remove progress border when progress is disabled.",
            failures);
        Expect(progress?.GetVisualParent() == null,
            "Removed progress border should not keep a visual parent.",
            failures);
        Expect(progress == null || progress.TemplatedParent == null,
            "Removed progress border should clear templated parent.",
            failures);
        Expect(CountCarouselIndicatorFields(carousel, "_animation") == 0,
            "Carousel should clear progress animations when progress is disabled.",
            failures);
        Expect(CountCarouselIndicatorFields(carousel, "_cancellationTokenSource") == 0,
            "Carousel should clear progress cancellation sources when progress is disabled.",
            failures);
    }

    private static void VerifyCarouselPageTransitionLifecycle(ICollection<string> failures)
    {
        var carousel = CreateCarousel();
        using var realized = RealizeControl(carousel);
        Expect(carousel.PageTransition == null,
            "Carousel should not create PageTransition during initial realization.",
            failures);

        carousel.SetCurrentValue(SelectingItemsControl.SelectedIndexProperty, 1);
        RefreshLayout(realized.Window);
        Expect(carousel.PageTransition != null,
            "Carousel should create PageTransition on the first real selection change.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsMotionEnabledProperty, false);
        RefreshLayout(realized.Window);
        Expect(carousel.PageTransition == null,
            "Carousel should clear PageTransition when motion is disabled.",
            failures);
    }

    private static void VerifyCarouselTimerLifecycle(ICollection<string> failures)
    {
        var carousel = CreateCarousel();
        using var realized = RealizeControl(carousel);
        carousel.SetCurrentValue(AtomCarousel.IsAutoPlayProperty, true);
        RefreshLayout(realized.Window);
        var timer = GetPrivateField(carousel, "AtomUI.Desktop.Controls.Carousel", "_autoPlayTimer") as Avalonia.Threading.DispatcherTimer;
        Expect(timer?.IsEnabled == true,
            "Attached Carousel should start autoplay timer when IsAutoPlay becomes true.",
            failures);

        carousel.SetCurrentValue(AtomCarousel.IsAutoPlayProperty, false);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(carousel, "AtomUI.Desktop.Controls.Carousel", "_autoPlayTimer") == null,
            "Carousel should stop and clear autoplay timer when IsAutoPlay becomes false.",
            failures);
    }

    private static int CountProgressBorders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Border>()
                   .Count(border => border.Name == "Progress");
    }

    private static int CountCarouselIndicatorFields(Control root, string fieldName)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "CarouselPageIndicator")
                   .Count(control => GetPrivateField(control,
                       "AtomUI.Desktop.Controls.CarouselPageIndicator",
                       fieldName) is not null);
    }

    private static void MoveMouseToControl(Avalonia.Controls.Window window, Control control)
    {
        var point = GetControlCenterPoint(window, control);
        if (point.HasValue)
        {
            window.MouseMove(point.Value);
        }
    }

    private static Avalonia.Point? GetControlCenterPoint(Avalonia.Controls.Window window, Control control)
    {
        return control.TranslatePoint(
            new Avalonia.Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);
    }

    private static Control? FindVisualAncestorByTypeName(Control control, string typeName)
    {
        return control.GetVisualAncestors()
                      .OfType<Control>()
                      .FirstOrDefault(ancestor => ancestor.GetType().Name == typeName);
    }

}
