using System.Reflection;
using AtomUI.Controls.Commons;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineStateTests
{
    static TimelineStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Reverse_Projects_Position_From_Visible_Visual_Order()
    {
        var first  = new Desktop.Controls.TimelineItem { Content = "A" };
        var hidden = new Desktop.Controls.TimelineItem { Content = "B", IsVisible = false };
        var third  = new Desktop.Controls.TimelineItem { Content = "C" };
        var last   = new Desktop.Controls.TimelineItem { Content = "D" };
        var timeline = new Desktop.Controls.Timeline
        {
            Width     = 640,
            IsReverse = true,
            Items     = { first, hidden, third, last }
        };

        ShowInWindow(timeline, () =>
        {
            GetState(last, "IsFirst").ShouldBeTrue();
            GetState(last, "IsLast").ShouldBeFalse();
            GetState(last, "IsOdd").ShouldBeFalse();

            GetState(third, "IsFirst").ShouldBeFalse();
            GetState(third, "IsLast").ShouldBeFalse();
            GetState(third, "IsOdd").ShouldBeTrue();

            GetState(first, "IsFirst").ShouldBeFalse();
            GetState(first, "IsLast").ShouldBeTrue();
            GetState(first, "IsOdd").ShouldBeFalse();

            GetState(hidden, "IsFirst").ShouldBeFalse();
            GetState(hidden, "IsLast").ShouldBeFalse();
            GetState(hidden, "IsOdd").ShouldBeFalse();
        });
    }

    [Fact]
    public void Visibility_Change_Reprojects_Visible_Items()
    {
        var first  = new Desktop.Controls.TimelineItem { Content = "A" };
        var second = new Desktop.Controls.TimelineItem { Content = "B" };
        var last   = new Desktop.Controls.TimelineItem { Content = "C" };
        var timeline = new Desktop.Controls.Timeline
        {
            Width = 640,
            Items = { first, second, last }
        };

        ShowInWindow(timeline, () =>
        {
            first.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            GetState(first, "IsFirst").ShouldBeFalse();
            GetState(first, "IsLast").ShouldBeFalse();
            GetState(first, "IsOdd").ShouldBeFalse();

            GetState(second, "IsFirst").ShouldBeTrue();
            GetState(second, "IsLast").ShouldBeFalse();
            GetState(second, "IsOdd").ShouldBeFalse();

            GetState(last, "IsFirst").ShouldBeFalse();
            GetState(last, "IsLast").ShouldBeTrue();
            GetState(last, "IsOdd").ShouldBeTrue();
        });
    }

    [Fact]
    public void Label_Change_Reprojects_Visible_Label_Layout()
    {
        var first  = new Desktop.Controls.TimelineItem { Content = "A" };
        var second = new Desktop.Controls.TimelineItem { Content = "B" };
        var hidden = new Desktop.Controls.TimelineItem
        {
            Content   = "C",
            Label     = "Hidden label",
            IsVisible = false
        };
        var timeline = new Desktop.Controls.Timeline
        {
            Width = 640,
            Items = { first, second, hidden }
        };

        ShowInWindow(timeline, () =>
        {
            GetState(first, "IsLabelLayout").ShouldBeFalse();
            GetState(second, "IsLabelLayout").ShouldBeFalse();
            GetState(hidden, "IsLabelLayout").ShouldBeFalse();

            second.Label = "Visible label";
            Dispatcher.UIThread.RunJobs();

            GetState(first, "IsLabelLayout").ShouldBeTrue();
            GetState(second, "IsLabelLayout").ShouldBeTrue();
            GetState(hidden, "IsLabelLayout").ShouldBeFalse();
        });
    }

    [Fact]
    public void Pending_Adjacency_Follows_Visible_Visual_Order()
    {
        var first  = new Desktop.Controls.TimelineItem { Content = "A" };
        var hidden = new Desktop.Controls.TimelineItem { Content = "B", IsVisible = false };
        var last   = new Desktop.Controls.TimelineItem { Content = "C" };
        var timeline = new Desktop.Controls.Timeline
        {
            Width   = 640,
            Pending = "Loading",
            Items   = { first, hidden, last }
        };

        ShowInWindow(timeline, () =>
        {
            GetState(first, "NextIsPending").ShouldBeFalse();
            GetState(hidden, "NextIsPending").ShouldBeFalse();
            GetState(last, "NextIsPending").ShouldBeTrue();

            timeline.IsReverse = true;
            Dispatcher.UIThread.RunJobs();

            GetState(first, "NextIsPending").ShouldBeFalse();
            GetState(hidden, "NextIsPending").ShouldBeFalse();
            GetState(last, "NextIsPending").ShouldBeFalse();
        });
    }

    [Fact]
    public void Orientation_Projects_To_Items_At_Runtime()
    {
        var item = new Desktop.Controls.TimelineItem { Content = "A" };
        var timeline = new Desktop.Controls.Timeline
        {
            Width = 640,
            Items = { item }
        };

        ShowInWindow(timeline, () =>
        {
            GetState<Orientation>(item, "Orientation").ShouldBe(Orientation.Vertical);

            timeline.Orientation = Orientation.Horizontal;
            Dispatcher.UIThread.RunJobs();

            GetState<Orientation>(item, "Orientation").ShouldBe(Orientation.Horizontal);
        });
    }

    private static bool GetState(AbstractTimelineItem item, string propertyName)
    {
        return GetState<bool>(item, propertyName);
    }

    private static T GetState<T>(AbstractTimelineItem item, string propertyName)
    {
        var property = typeof(AbstractTimelineItem).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic);

        return property.ShouldNotBeNull().GetValue(item).ShouldBeOfType<T>();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
