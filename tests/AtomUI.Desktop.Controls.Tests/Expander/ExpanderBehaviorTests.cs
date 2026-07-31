using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIExpander = AtomUI.Desktop.Controls.Expander;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.ExpanderControl;

public class ExpanderBehaviorTests
{
    private const double LayoutTolerance = 0.5;

    static ExpanderBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Custom_Header_Padding_Uses_Compact_Expand_Icon_Layout()
    {
        var expander = new AtomUIExpander
        {
            Header          = "This is panel header 1",
            Content         = "Content",
            HeaderPadding   = new Thickness(5),
            ContentPadding  = new Thickness(5),
            IsMotionEnabled = false
        };

        var window = ShowInWindow(expander);
        try
        {
            var headerDecorator = GetHeaderDecorator(expander);
            var expandButton    = GetExpandButton(expander);
            var headerPresenter = GetHeaderPresenter(expander);

            expandButton.Bounds.Width.ShouldBe(expandButton.IconWidth, LayoutTolerance);
            expandButton.Bounds.Height.ShouldBe(expandButton.IconHeight, LayoutTolerance);

            GetHorizontalGap(expandButton, headerPresenter, headerDecorator)
                .ShouldBe(5, LayoutTolerance);

            GetCenterY(expandButton, headerDecorator)
                .ShouldBe(GetCenterY(headerPresenter, headerDecorator), LayoutTolerance);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Ghost_And_Borderless_Mode_Update_Frame_And_Content_Separator_At_Runtime()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsExpanded      = true,
            IsMotionEnabled = false,
            BorderThickness = new Thickness(5)
        };

        var window = ShowInWindow(expander);
        try
        {
            var frame = FindVisualByName<PixelAlignedBorder>(expander, "PART_Frame");
            frame.ShouldNotBeNull();
            frame!.BorderThickness.ShouldBe(new Thickness(5));
            frame.BorderBrush.ShouldNotBeNull();

            var contentBorder = GetContentBorder(expander);
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 5, 0, 0));

            expander.IsBorderless = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(default(Thickness));
            contentBorder.BorderThickness.ShouldBe(default(Thickness));

            expander.IsBorderless = false;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(5));
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 5, 0, 0));

            expander.IsGhostStyle = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(default(Thickness));
            contentBorder.BorderThickness.ShouldBe(default(Thickness));

            expander.IsGhostStyle = false;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(5));
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 5, 0, 0));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Pixel_Aligned_Frame_And_Content_Separator_Apply_Theme_Brushes()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsExpanded      = true,
            IsMotionEnabled = false,
            BorderThickness = new Thickness(1)
        };

        var window = ShowInWindow(expander);
        try
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            var borderBrush = GetThemeResource<IBrush>(SharedTokenKind.ColorBorder);
            var frame = FindVisualByName<PixelAlignedBorder>(expander, "PART_Frame");
            frame.ShouldNotBeNull();
            frame!.BorderBrush.ShouldNotBeNull();
            BrushShouldHaveSameColor(frame.BorderBrush, borderBrush);
            DrawingGroupHasPenWithBrush(RenderToDrawingGroup(frame), borderBrush)
                .ShouldBeTrue("the expander frame must render a visible outer border");

            var headerDecorator = GetHeaderDecorator(expander);
            headerDecorator.Background.ShouldNotBeNull();
            headerDecorator.BorderThickness.ShouldBe(default(Thickness));

            var contentBorder = GetContentBorder(expander);
            contentBorder.BorderBrush.ShouldNotBeNull();
            BrushShouldHaveSameColor(contentBorder.BorderBrush, borderBrush);
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 1, 0, 0));
            DrawingGroupHasFillWithBrush(RenderToDrawingGroup(contentBorder), borderBrush)
                .ShouldBeTrue("the expanded expander content must render the separator line");
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(ExpandDirection.Down, 0, 1, 0, 0)]
    [InlineData(ExpandDirection.Up, 0, 0, 0, 1)]
    [InlineData(ExpandDirection.Left, 0, 0, 1, 0)]
    [InlineData(ExpandDirection.Right, 1, 0, 0, 0)]
    public void Content_Separator_Is_Owned_By_Content_And_Follows_Direction(
        ExpandDirection direction,
        double left,
        double top,
        double right,
        double bottom)
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            ExpandDirection = direction,
            IsExpanded      = true,
            IsMotionEnabled = false,
            BorderThickness = new Thickness(1)
        };

        var window = ShowInWindow(expander);
        try
        {
            GetHeaderDecorator(expander).BorderThickness.ShouldBe(default(Thickness));
            GetContentBorder(expander).BorderThickness.ShouldBe(
                new Thickness(left, top, right, bottom));
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(ExpandDirection.Down)]
    [InlineData(ExpandDirection.Up)]
    [InlineData(ExpandDirection.Left)]
    [InlineData(ExpandDirection.Right)]
    public void Content_Separator_Remains_Adjacent_To_Header_In_Every_Direction(
        ExpandDirection direction)
    {
        var expander = new AtomUIExpander
        {
            Header = new Border
            {
                Width  = 70,
                Height = 24
            },
            Content = new Border
            {
                Width  = 80,
                Height = 40
            },
            HeaderPadding    = default,
            ContentPadding   = default,
            ExpandDirection  = direction,
            IsExpanded       = true,
            IsShowExpandIcon = false,
            IsMotionEnabled  = false,
            BorderThickness  = new Thickness(1)
        };

        var window = ShowInWindow(expander);
        try
        {
            var mainLayout = FindVisualByName<DockPanel>(expander, "PART_MainLayout");
            mainLayout.ShouldNotBeNull();

            var headerLayoutTransform =
                FindVisualByName<Control>(expander, "PART_HeaderLayoutTransform");
            headerLayoutTransform.ShouldNotBeNull();

            var mainBounds = new Rect(mainLayout!.Bounds.Size);
            var headerBounds = GetPhysicalBounds(headerLayoutTransform!, mainLayout);
            var contentBounds = GetPhysicalBounds(GetContentBorder(expander), mainLayout);

            AssertContentAndHeaderArrangement(direction, mainBounds, headerBounds, contentBounds);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Content_Separator_Moves_To_Mapped_Edge_When_Direction_Changes()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            ExpandDirection = ExpandDirection.Down,
            IsExpanded      = true,
            IsMotionEnabled = false,
            BorderThickness = new Thickness(2)
        };

        var window = ShowInWindow(expander);
        try
        {
            var contentBorder = GetContentBorder(expander);
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 2, 0, 0));

            expander.ExpandDirection = ExpandDirection.Up;
            Dispatcher.UIThread.RunJobs();
            ReferenceEquals(GetContentBorder(expander), contentBorder).ShouldBeTrue();
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 0, 0, 2));

            expander.ExpandDirection = ExpandDirection.Left;
            Dispatcher.UIThread.RunJobs();
            ReferenceEquals(GetContentBorder(expander), contentBorder).ShouldBeTrue();
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 0, 2, 0));

            expander.ExpandDirection = ExpandDirection.Right;
            Dispatcher.UIThread.RunJobs();
            ReferenceEquals(GetContentBorder(expander), contentBorder).ShouldBeTrue();
            contentBorder.BorderThickness.ShouldBe(new Thickness(2, 0, 0, 0));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Runtime_Asymmetric_Border_Thickness_Uses_Bottom_For_Current_Direction_Edge()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            ExpandDirection = ExpandDirection.Left,
            IsExpanded      = true,
            IsMotionEnabled = false,
            BorderThickness = new Thickness(1, 2, 3, 4)
        };

        var window = ShowInWindow(expander);
        try
        {
            var contentBorder = GetContentBorder(expander);
            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 0, 4, 0));

            expander.BorderThickness = new Thickness(5, 6, 7, 8);
            Dispatcher.UIThread.RunJobs();

            contentBorder.BorderThickness.ShouldBe(new Thickness(0, 0, 8, 0));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Icon_Trigger_Does_Not_Toggle_From_Header_Click()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            TriggerType     = ExpanderTriggerType.Icon,
            IsMotionEnabled = false
        };

        var window = ShowInWindow(expander);
        try
        {
            Click(GetHeaderDecorator(expander), window);
            expander.IsExpanded.ShouldBeFalse();

            var expandButton = GetExpandButton(expander);
            expandButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, expandButton));
            Dispatcher.UIThread.RunJobs();

            expander.IsExpanded.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Content_Motion_Reversal_Does_Not_Change_Separator_And_Uses_Latest_Expanded_State()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsMotionEnabled = true,
            MotionDuration  = TimeSpan.FromMilliseconds(100),
            BorderThickness = new Thickness(2)
        };

        var window = ShowInWindow(expander);
        try
        {
            var motionActor = GetContentMotionActor(expander);
            var initialContentBorderThickness = expander.ContentBorderThickness;
            var contentBorderChangeCount = 0;
            using var contentBorderSubscription =
                expander.GetObservable(AtomUIExpander.ContentBorderThicknessProperty)
                        .Subscribe(_ => contentBorderChangeCount++);

            contentBorderChangeCount = 0;

            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            motionActor.MotionTransform.ShouldNotBeNull();

            expander.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            motionActor.MotionTransform.ShouldNotBeNull();

            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            WaitForLayoutCondition(() => motionActor.IsVisible &&
                                         motionActor.MotionTransform is null &&
                                         motionActor.Transitions is null);

            expander.IsExpanded.ShouldBeTrue();
            motionActor.IsVisible.ShouldBe(expander.IsExpanded);
            motionActor.MotionTransform.ShouldBeNull();
            motionActor.Transitions.ShouldBeNull();
            contentBorderChangeCount.ShouldBe(0);
            expander.ContentBorderThickness.ShouldBe(initialContentBorderThickness);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Template_Reapply_Unsubscribes_Old_Expand_Button()
    {
        var expander = new TestExpander { IsMotionEnabled = false };
        var oldButton = new IconButton();
        var newButton = new IconButton();

        expander.ApplyTemplateParts(oldButton, new LayoutAwareMotionActor());
        expander.ApplyTemplateParts(newButton, new LayoutAwareMotionActor());

        oldButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, oldButton));
        expander.IsExpanded.ShouldBeFalse();

        newButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, newButton));
        expander.IsExpanded.ShouldBeTrue();
    }

    [Fact]
    public void Template_Reapply_Clears_Old_Content_Motion_Actor_Values()
    {
        var expander = new TestExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsMotionEnabled = true,
            MotionDuration  = TimeSpan.FromMilliseconds(100)
        };

        var window = ShowInWindow(expander);
        try
        {
            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            var oldMotionActor = GetContentMotionActor(expander);
            oldMotionActor.MotionTransform.ShouldNotBeNull();
            oldMotionActor.Transitions.ShouldNotBeNull();
            oldMotionActor.MotionTransformOperations = TransformOperations.Identity;
            oldMotionActor.Height = 42;
            oldMotionActor.MotionTransformOperations.ShouldNotBeNull();
            oldMotionActor.Height.ShouldBe(42);
            var lateCompletionCount = 0;
            oldMotionActor.Completed += (_, _) => lateCompletionCount++;

            expander.ApplyTemplateParts(new IconButton(), new LayoutAwareMotionActor());

            oldMotionActor.MotionTransform.ShouldBeNull();
            oldMotionActor.MotionTransformOperations.ShouldBeNull();
            oldMotionActor.Transitions.ShouldBeNull();
            oldMotionActor.Height.ShouldBe(double.NaN);

            Thread.Sleep(expander.MotionDuration + TimeSpan.FromMilliseconds(50));
            Dispatcher.UIThread.RunJobs();

            oldMotionActor.MotionTransform.ShouldBeNull();
            oldMotionActor.MotionTransformOperations.ShouldBeNull();
            oldMotionActor.Transitions.ShouldBeNull();
            oldMotionActor.Height.ShouldBe(double.NaN);
            lateCompletionCount.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Detach_Clears_Active_Content_Motion_Actor_Values()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsMotionEnabled = true,
            MotionDuration  = TimeSpan.FromMilliseconds(100)
        };

        var window = ShowInWindow(expander);
        try
        {
            expander.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(expander);
            motionActor.MotionTransform.ShouldNotBeNull();
            motionActor.Transitions.ShouldNotBeNull();
            motionActor.MotionTransformOperations = TransformOperations.Identity;
            motionActor.Height = 42;
            motionActor.MotionTransformOperations.ShouldNotBeNull();
            motionActor.Height.ShouldBe(42);
            var lateCompletionCount = 0;
            motionActor.Completed += (_, _) => lateCompletionCount++;

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            motionActor.MotionTransform.ShouldBeNull();
            motionActor.MotionTransformOperations.ShouldBeNull();
            motionActor.Transitions.ShouldBeNull();
            motionActor.Height.ShouldBe(double.NaN);

            Thread.Sleep(expander.MotionDuration + TimeSpan.FromMilliseconds(50));
            Dispatcher.UIThread.RunJobs();

            motionActor.MotionTransform.ShouldBeNull();
            motionActor.MotionTransformOperations.ShouldBeNull();
            motionActor.Transitions.ShouldBeNull();
            motionActor.Height.ShouldBe(double.NaN);
            lateCompletionCount.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Collapse_Detach_And_Reattach_Applies_Current_Collapsed_Stable_State()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsExpanded      = true,
            IsMotionEnabled = true,
            MotionDuration  = TimeSpan.FromMilliseconds(100)
        };

        var window = ShowInWindow(expander);
        try
        {
            var motionActor = GetContentMotionActor(expander);
            motionActor.IsVisible.ShouldBeTrue();

            expander.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            motionActor.IsVisible.ShouldBeTrue();
            motionActor.MotionTransform.ShouldNotBeNull();

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            expander.IsExpanded.ShouldBeFalse();
            motionActor.IsVisible.ShouldBeFalse();
            motionActor.Opacity.ShouldBe(0);

            window.Content = expander;
            Dispatcher.UIThread.RunJobs();

            ReferenceEquals(GetContentMotionActor(expander), motionActor).ShouldBeTrue();
            motionActor.IsVisible.ShouldBe(expander.IsExpanded);
            motionActor.Opacity.ShouldBe(0);

            Thread.Sleep(expander.MotionDuration + TimeSpan.FromMilliseconds(50));
            Dispatcher.UIThread.RunJobs();

            motionActor.IsVisible.ShouldBe(expander.IsExpanded);
            motionActor.Opacity.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow ShowInWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 220,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void Click(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    private static PixelAlignedBorder GetHeaderDecorator(AtomUIExpander expander)
    {
        var header = FindVisualByName<PixelAlignedBorder>(expander, "PART_HeaderDecorator");
        header.ShouldNotBeNull();
        return header!;
    }

    private static IconButton GetExpandButton(AtomUIExpander expander)
    {
        var button = FindTemplatePart<IconButton>(expander, "PART_ExpandButton");
        button.ShouldNotBeNull();
        return button!;
    }

    private static ContentPresenter GetHeaderPresenter(AtomUIExpander expander)
    {
        var presenter = FindTemplatePart<ContentPresenter>(expander, "PART_HeaderPresenter");
        presenter.ShouldNotBeNull();
        return presenter!;
    }

    private static BaseMotionActor GetContentMotionActor(AtomUIExpander expander)
    {
        var motionActor = FindTemplatePart<BaseMotionActor>(expander, "PART_ContentMotionActor");
        motionActor.ShouldNotBeNull();
        return motionActor!;
    }

    private static PixelAlignedBorder GetContentBorder(AtomUIExpander expander)
    {
        var motionActor = GetContentMotionActor(expander);
        motionActor.Content.ShouldBeOfType<PixelAlignedBorder>();
        var contentBorder = (PixelAlignedBorder)motionActor.Content!;
        string.IsNullOrEmpty(contentBorder.Name).ShouldBeTrue();
        return contentBorder;
    }

    private static void AssertContentAndHeaderArrangement(ExpandDirection direction,
                                                          Rect mainBounds,
                                                          Rect headerBounds,
                                                          Rect contentBounds)
    {
        contentBounds.Width.ShouldBeGreaterThan(0);
        contentBounds.Height.ShouldBeGreaterThan(0);

        if (direction is ExpandDirection.Down or ExpandDirection.Up)
        {
            headerBounds.Left.ShouldBe(mainBounds.Left, LayoutTolerance);
            headerBounds.Right.ShouldBe(mainBounds.Right, LayoutTolerance);
            contentBounds.Left.ShouldBe(mainBounds.Left, LayoutTolerance);
            contentBounds.Right.ShouldBe(mainBounds.Right, LayoutTolerance);

            if (direction == ExpandDirection.Down)
            {
                headerBounds.Top.ShouldBe(mainBounds.Top, LayoutTolerance);
                contentBounds.Top.ShouldBe(headerBounds.Bottom, LayoutTolerance);
                contentBounds.Bottom.ShouldBe(mainBounds.Bottom, LayoutTolerance);
            }
            else
            {
                contentBounds.Top.ShouldBe(mainBounds.Top, LayoutTolerance);
                contentBounds.Bottom.ShouldBe(headerBounds.Top, LayoutTolerance);
                headerBounds.Bottom.ShouldBe(mainBounds.Bottom, LayoutTolerance);
            }

            return;
        }

        headerBounds.Top.ShouldBe(mainBounds.Top, LayoutTolerance);
        headerBounds.Bottom.ShouldBe(mainBounds.Bottom, LayoutTolerance);
        contentBounds.Top.ShouldBe(mainBounds.Top, LayoutTolerance);
        contentBounds.Bottom.ShouldBe(mainBounds.Bottom, LayoutTolerance);

        if (direction == ExpandDirection.Left)
        {
            contentBounds.Left.ShouldBe(mainBounds.Left, LayoutTolerance);
            contentBounds.Right.ShouldBe(headerBounds.Left, LayoutTolerance);
            headerBounds.Right.ShouldBe(mainBounds.Right, LayoutTolerance);
        }
        else
        {
            headerBounds.Left.ShouldBe(mainBounds.Left, LayoutTolerance);
            contentBounds.Left.ShouldBe(headerBounds.Right, LayoutTolerance);
            contentBounds.Right.ShouldBe(mainBounds.Right, LayoutTolerance);
        }
    }

    private static Rect GetPhysicalBounds(Control control, Visual relativeTo)
    {
        Point[] corners =
        [
            new Point(),
            new Point(control.Bounds.Width, 0),
            new Point(0, control.Bounds.Height),
            new Point(control.Bounds.Width, control.Bounds.Height)
        ];

        var translatedCorners = corners.Select(corner =>
        {
            var translated = control.TranslatePoint(corner, relativeTo);
            translated.ShouldNotBeNull();
            return translated.Value;
        }).ToArray();

        var left   = translatedCorners.Min(point => point.X);
        var top    = translatedCorners.Min(point => point.Y);
        var right  = translatedCorners.Max(point => point.X);
        var bottom = translatedCorners.Max(point => point.Y);
        return new Rect(left, top, right - left, bottom - top);
    }

    private static void WaitForLayoutCondition(Func<bool> condition, int timeoutMilliseconds = 500)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs();
            if (condition())
            {
                return;
            }

            Thread.Sleep(10);
        }

        Dispatcher.UIThread.RunJobs();
    }

    private static T? FindTemplatePart<T>(AtomUIExpander expander, string name)
        where T : Control
    {
        return expander.GetSelfAndVisualDescendants()
                       .OfType<T>()
                       .FirstOrDefault(control => control.Name == name &&
                                                  ReferenceEquals(control.TemplatedParent, expander));
    }

    private static T? FindVisualByName<T>(Control root, string name)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name);
    }

    private static double GetHorizontalGap(Control left, Control right, Visual relativeTo)
    {
        var leftOrigin  = GetRelativeOrigin(left, relativeTo);
        var rightOrigin = GetRelativeOrigin(right, relativeTo);
        return rightOrigin.X - (leftOrigin.X + left.Bounds.Width);
    }

    private static double GetCenterY(Control control, Visual relativeTo)
    {
        var origin = GetRelativeOrigin(control, relativeTo);
        return origin.Y + control.Bounds.Height / 2;
    }

    private static Point GetRelativeOrigin(Control control, Visual relativeTo)
    {
        var origin = control.TranslatePoint(new Point(), relativeTo);
        origin.ShouldNotBeNull();
        return origin.Value;
    }

    private static DrawingGroup RenderToDrawingGroup(Control control)
    {
        var drawingGroup = new DrawingGroup();
        using var context = drawingGroup.Open();
        control.Render(context);
        return drawingGroup;
    }

    private static bool DrawingGroupHasPenWithBrush(DrawingGroup drawingGroup, IBrush expectedBrush)
    {
        return EnumerateGeometryDrawings(drawingGroup)
            .Any(drawing => drawing.Pen is { Brush: { } brush } &&
                            BrushesHaveSameColor(brush, expectedBrush));
    }

    private static bool DrawingGroupHasFillWithBrush(DrawingGroup drawingGroup, IBrush expectedBrush)
    {
        return EnumerateGeometryDrawings(drawingGroup)
            .Any(drawing => drawing.Brush is { } brush &&
                            BrushesHaveSameColor(brush, expectedBrush));
    }

    private static IEnumerable<GeometryDrawing> EnumerateGeometryDrawings(Drawing drawing)
    {
        if (drawing is GeometryDrawing geometryDrawing)
        {
            yield return geometryDrawing;
        }
        else if (drawing is DrawingGroup drawingGroup)
        {
            foreach (var child in drawingGroup.Children.SelectMany(EnumerateGeometryDrawings))
            {
                yield return child;
            }
        }
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        BrushesHaveSameColor(actual, expected).ShouldBeTrue();
    }

    private static bool BrushesHaveSameColor(IBrush? actual, IBrush? expected)
    {
        return GetSolidBrushColor(actual) == GetSolidBrushColor(expected);
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private sealed class TestExpander : AtomUIExpander
    {
        protected override Type StyleKeyOverride => typeof(AtomUIExpander);

        public void ApplyTemplateParts(IconButton expandButton, BaseMotionActor motionActor)
        {
            var nameScope = new NameScope();
            nameScope.Register("PART_ExpandButton", expandButton);
            nameScope.Register("PART_ContentMotionActor", motionActor);

            OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }
    }
}
