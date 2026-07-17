using System;
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

public class StepsNavigationLayoutTests
{
    static StepsNavigationLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(Orientation.Horizontal, AtomUISizeType.Middle)]
    [InlineData(Orientation.Horizontal, AtomUISizeType.Small)]
    [InlineData(Orientation.Vertical, AtomUISizeType.Middle)]
    [InlineData(Orientation.Vertical, AtomUISizeType.Small)]
    public void Navigation_Arrow_Is_Reserved_Inside_Each_NonLast_Item(
        Orientation orientation,
        AtomUISizeType sizeType)
    {
        var steps = CreateNavigationSteps(orientation, sizeType);

        ShowInWindow(steps, () =>
        {
            var items = steps.Items.Cast<Desktop.Controls.StepsItem>().ToArray();
            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];
                var arrow = FindControl(item, "NavigationArrow");

                arrow.IsVisible.ShouldBe(index < items.Length - 1);
                if (arrow.IsVisible)
                {
                    arrow.Bounds.Width.ShouldBeGreaterThan(0);
                    arrow.Bounds.Height.ShouldBeGreaterThan(0);
                    new Rect(item.Bounds.Size).Contains(
                            arrow.TranslatePoint(new Point(0, 0), item).ShouldNotBeNull())
                        .ShouldBeTrue();
                    new Rect(item.Bounds.Size).Contains(
                        arrow.TranslatePoint(
                            new Point(arrow.Bounds.Width - 0.1, arrow.Bounds.Height - 0.1),
                            item).ShouldNotBeNull())
                        .ShouldBeTrue();
                }
            }
        });
    }

    [Theory]
    [InlineData(Orientation.Horizontal)]
    [InlineData(Orientation.Vertical)]
    public void Navigation_Current_Item_Has_One_Direction_Aware_Active_Indicator(Orientation orientation)
    {
        var steps = CreateNavigationSteps(orientation, AtomUISizeType.Middle);
        steps.Current = 1;

        ShowInWindow(steps, () =>
        {
            var items = steps.Items.Cast<Desktop.Controls.StepsItem>().ToArray();
            for (var index = 0; index < items.Length; index++)
            {
                var active = items[index].GetVisualDescendants()
                                         .OfType<PixelAlignedBorder>()
                                         .Single(control => control.Name == "NavigationActiveIndicator");
                active.IsVisible.ShouldBe(index == 1);

                if (active.IsVisible && orientation == Orientation.Horizontal)
                {
                    active.Bounds.Width.ShouldBeGreaterThan(active.Bounds.Height);
                    active.Bounds.Bottom.ShouldBe(items[index].Bounds.Height, 1);
                }
                else if (active.IsVisible)
                {
                    active.Bounds.Height.ShouldBeGreaterThan(active.Bounds.Width);
                    active.Bounds.Right.ShouldBe(items[index].Bounds.Width, 1);
                }
            }
        });
    }

    private static Desktop.Controls.Steps CreateNavigationSteps(Orientation orientation, AtomUISizeType sizeType)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width           = orientation == Orientation.Horizontal ? 720 : 320,
            Type            = Desktop.Controls.StepsType.Navigation,
            Orientation     = orientation,
            SizeType        = sizeType,
            IsItemClickable = true
        };
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "First", SubHeader = "00:01", Content = "Content" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Second", SubHeader = "00:02", Content = "Content" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Last", SubHeader = "00:03", Content = "Content" });
        return steps;
    }

    private static Control FindControl(Desktop.Controls.StepsItem item, string name)
    {
        return item.GetVisualDescendants().OfType<Control>().Single(control => control.Name == name);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 480,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
