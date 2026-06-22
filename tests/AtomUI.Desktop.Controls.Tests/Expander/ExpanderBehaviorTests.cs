using System;
using System.Linq;
using System.Threading;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
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
    public void Ghost_And_Borderless_Mode_Update_Frame_Border_At_Runtime()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsMotionEnabled = false,
            BorderThickness = new Thickness(5)
        };

        var window = ShowInWindow(expander);
        try
        {
            var frame = FindVisualByName<Border>(expander, "PART_Frame");
            frame.ShouldNotBeNull();
            frame!.BorderThickness.ShouldBe(new Thickness(5));

            expander.IsGhostStyle = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(0));

            expander.IsGhostStyle = false;
            expander.IsBorderless = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(0));

            expander.IsBorderless = false;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(5));
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
    public void Content_Motion_Reconciles_Final_Visibility_When_Expanded_Changes_During_Animation()
    {
        var expander = new AtomUIExpander
        {
            Header          = "Header",
            Content         = "Content",
            IsMotionEnabled = true,
            MotionDuration  = TimeSpan.FromMilliseconds(1)
        };

        var window = ShowInWindow(expander);
        try
        {
            expander.IsExpanded = true;
            expander.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(expander);
            expander.IsExpanded.ShouldBeFalse();
            motionActor.IsVisible.ShouldBeFalse();
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

    private static Border GetHeaderDecorator(AtomUIExpander expander)
    {
        var header = FindVisualByName<Border>(expander, "PART_HeaderDecorator");
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
}
