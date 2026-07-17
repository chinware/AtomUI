using System;
using System.Collections.Generic;
using System.Linq;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUISizeType = AtomUI.SizeType;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsLayoutMatrixTests
{
    static StepsLayoutMatrixTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    public static IEnumerable<object[]> LayoutCases
    {
        get
        {
            foreach (var type in Enum.GetValues<Desktop.Controls.StepsType>())
            foreach (var orientation in Enum.GetValues<Orientation>())
            foreach (var titlePlacement in Enum.GetValues<Orientation>())
            foreach (var sizeType in new[] { AtomUISizeType.Middle, AtomUISizeType.Small })
            {
                yield return [type, orientation, titlePlacement, sizeType];
            }
        }
    }

    [Theory]
    [MemberData(nameof(LayoutCases))]
    public void Every_Layout_Combination_Uses_One_Finite_Semantic_Tree(
        Desktop.Controls.StepsType type,
        Orientation orientation,
        Orientation titlePlacement,
        AtomUISizeType sizeType)
    {
        var steps = CreateSteps(type, orientation, titlePlacement, sizeType);

        ShowInWindow(steps, () =>
        {
            foreach (var item in steps.Items.Cast<Desktop.Controls.StepsItem>())
            {
                var indicator = item.GetVisualDescendants()
                                    .OfType<Desktop.Controls.StepsItemIndicator>()
                                    .Single();
                var header = FindControl(item, "HeaderPresenter");
                var subHeader = FindControl(item, "SubHeaderPresenter");
                var content = FindControl(item, "ContentPresenter");
                var connector = FindControl(item, "Connector");
                var arrow = FindControl(item, "NavigationArrow");

                AssertFinite(indicator);
                AssertFinite(header);
                AssertFinite(subHeader);
                AssertFinite(content);
                AssertFinite(connector);
                AssertFinite(arrow);

                indicator.Bounds.Width.ShouldBeGreaterThan(0);
                indicator.Bounds.Height.ShouldBeGreaterThan(0);
                AssertNoInteriorOverlap(indicator, header);
                AssertNoInteriorOverlap(indicator, subHeader);
                AssertNoInteriorOverlap(indicator, content);
                AssertNoInteriorOverlap(header, content);
                AssertNoInteriorOverlap(subHeader, content);

                content.IsVisible.ShouldBe(type != Desktop.Controls.StepsType.Inline);
                connector.IsVisible.ShouldBe(type != Desktop.Controls.StepsType.Navigation && !item.IsLast);
                arrow.IsVisible.ShouldBe(type == Desktop.Controls.StepsType.Navigation && !item.IsLast);
            }
        });
    }

    [Fact]
    public void Runtime_Type_Orientation_TitlePlacement_And_Size_Changes_Rearrange_The_Same_Nodes()
    {
        var steps = CreateSteps(
            Desktop.Controls.StepsType.Default,
            Orientation.Horizontal,
            Orientation.Horizontal,
            AtomUISizeType.Middle);

        ShowInWindow(steps, () =>
        {
            var item = steps.Items[0].ShouldBeOfType<Desktop.Controls.StepsItem>();
            var indicator = item.GetVisualDescendants()
                                .OfType<Desktop.Controls.StepsItemIndicator>()
                                .Single();
            var header = FindControl(item, "HeaderPresenter");
            var connector = FindControl(item, "Connector");
            var arrow = FindControl(item, "NavigationArrow");
            var middleWidth = indicator.Bounds.Width;

            header.Bounds.X.ShouldBeGreaterThanOrEqualTo(indicator.Bounds.Right);

            steps.Type = Desktop.Controls.StepsType.Dot;
            Relayout();
            header.Bounds.Y.ShouldBeGreaterThanOrEqualTo(indicator.Bounds.Bottom);

            steps.Type = Desktop.Controls.StepsType.Navigation;
            steps.Orientation = Orientation.Vertical;
            steps.TitlePlacement = Orientation.Vertical;
            Relayout();
            connector.IsVisible.ShouldBeFalse();
            arrow.IsVisible.ShouldBeTrue();
            arrow.Bounds.Y.ShouldBeGreaterThanOrEqualTo(header.Bounds.Bottom);

            steps.Type = Desktop.Controls.StepsType.Default;
            steps.Orientation = Orientation.Horizontal;
            steps.TitlePlacement = Orientation.Horizontal;
            steps.SizeType = AtomUISizeType.Small;
            Relayout();
            indicator.Bounds.Width.ShouldBeLessThan(middleWidth);
        });
    }

    [Fact]
    public void Dot_Current_Item_Is_Larger_And_Inline_Content_Is_Removed_From_Layout()
    {
        var steps = CreateSteps(
            Desktop.Controls.StepsType.Dot,
            Orientation.Horizontal,
            Orientation.Horizontal,
            AtomUISizeType.Middle);

        ShowInWindow(steps, () =>
        {
            var first = steps.Items[0].ShouldBeOfType<Desktop.Controls.StepsItem>();
            var second = steps.Items[1].ShouldBeOfType<Desktop.Controls.StepsItem>();
            var currentIndicator = GetIndicator(first);
            var waitingIndicator = GetIndicator(second);

            currentIndicator.Bounds.Width.ShouldBeGreaterThan(waitingIndicator.Bounds.Width);
            currentIndicator.Bounds.Height.ShouldBeGreaterThan(waitingIndicator.Bounds.Height);

            steps.Type = Desktop.Controls.StepsType.Inline;
            Relayout();
            FindControl(first, "ContentPresenter").IsVisible.ShouldBeFalse();
            GetIndicator(first).Bounds.Width.ShouldBe(GetIndicator(second).Bounds.Width);
        });
    }

    [Fact]
    public void Default_Indicator_Displays_The_OneBased_Step_Number()
    {
        var steps = CreateSteps(
            Desktop.Controls.StepsType.Default,
            Orientation.Horizontal,
            Orientation.Horizontal,
            AtomUISizeType.Middle);
        steps.Initial = 4;
        steps.Current = 4;

        ShowInWindow(steps, () =>
        {
            var first = steps.Items[0].ShouldBeOfType<Desktop.Controls.StepsItem>();
            var number = GetIndicator(first).GetVisualDescendants()
                                            .OfType<Avalonia.Controls.TextBlock>()
                                            .Single(control => control.Name == "StepNumberText");

            number.Text.ShouldBe("5");
        });
    }

    private static Desktop.Controls.Steps CreateSteps(
        Desktop.Controls.StepsType type,
        Orientation orientation,
        Orientation titlePlacement,
        AtomUISizeType sizeType)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width           = 720,
            Current         = 0,
            Type            = type,
            Orientation     = orientation,
            TitlePlacement  = titlePlacement,
            SizeType        = sizeType,
            IsItemClickable = true
        };
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "First step",
            SubHeader = "00:01",
            Content = "First step content"
        });
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "Second step",
            SubHeader = "00:02",
            Content = "Second step content"
        });
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "Last step",
            SubHeader = "00:03",
            Content = "Last step content"
        });
        return steps;
    }

    private static Desktop.Controls.StepsItemIndicator GetIndicator(Desktop.Controls.StepsItem item)
    {
        return item.GetVisualDescendants().OfType<Desktop.Controls.StepsItemIndicator>().Single();
    }

    private static Control FindControl(Desktop.Controls.StepsItem item, string name)
    {
        return item.GetVisualDescendants().OfType<Control>().Single(control => control.Name == name);
    }

    private static void AssertFinite(Control control)
    {
        double.IsFinite(control.Bounds.X).ShouldBeTrue();
        double.IsFinite(control.Bounds.Y).ShouldBeTrue();
        double.IsFinite(control.Bounds.Width).ShouldBeTrue();
        double.IsFinite(control.Bounds.Height).ShouldBeTrue();
        control.Bounds.Width.ShouldBeGreaterThanOrEqualTo(0);
        control.Bounds.Height.ShouldBeGreaterThanOrEqualTo(0);
    }

    private static void AssertNoInteriorOverlap(Control first, Control second)
    {
        if (!first.IsVisible || !second.IsVisible)
        {
            return;
        }

        var intersection = first.Bounds.Intersect(second.Bounds);
        (intersection.Width > 0 && intersection.Height > 0).ShouldBeFalse(
            $"{first.Name ?? first.GetType().Name} overlaps {second.Name ?? second.GetType().Name}.");
    }

    private static void Relayout()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
        Dispatcher.UIThread.RunJobs();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 420,
            Content = content
        };

        try
        {
            window.Show();
            Relayout();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
