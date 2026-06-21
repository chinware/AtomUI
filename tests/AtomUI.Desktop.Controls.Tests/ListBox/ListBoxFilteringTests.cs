using System;
using System.Linq;
using System.Threading;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomHighlightStrategy = AtomUI.Desktop.Controls.TextBlockHighlightStrategy;
using AtomHighlightableTextBlock = AtomUI.Desktop.Controls.HighlightableTextBlock;
using AtomListBox = AtomUI.Desktop.Controls.ListBox;
using AtomListBoxItem = AtomUI.Desktop.Controls.ListBoxItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.ListBox;

public class ListBoxFilteringTests
{
    static ListBoxFilteringTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Filtering_Counts_All_Items_When_Some_Containers_Are_Unrealized()
    {
        var items = Enumerable.Range(0, 100)
                              .Select(i => i % 10 == 0 ? $"needle {i}" : $"content {i}")
                              .ToArray();
        var listBox = new AtomListBox
        {
            Width       = 240,
            Height      = 96,
            ItemsSource = items
        };
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 160,
            Content = listBox
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            listBox.ContainerFromIndex(90).ShouldBeNull(
                "the test must exercise virtualized items that do not have containers yet.");

            listBox.FilterValue = "needle";
            Dispatcher.UIThread.RunJobs();

            listBox.FilterResultCount.ShouldBe(
                10,
                "filtering should count matching data items, not only currently realized containers.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Filtering_State_Is_Applied_To_Container_Realized_After_Filtering()
    {
        var items = Enumerable.Range(0, 100)
                              .Select(i => i % 10 == 0 ? $"needle {i}" : $"content {i}")
                              .ToArray();
        var listBox = new AtomListBox
        {
            Width       = 240,
            Height      = 96,
            ItemsSource = items
        };
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 160,
            Content = listBox
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            listBox.ContainerFromIndex(90).ShouldBeNull(
                "the far matching item should not have a container before scrolling.");

            listBox.FilterValue = "needle";
            Dispatcher.UIThread.RunJobs();

            listBox.ScrollIntoView(90);
            var realizedItem = WaitFor(
                () => listBox.ContainerFromIndex(90) as AtomListBoxItem,
                "scrolling should realize the far matching item.");

            realizedItem.IsFiltering.ShouldBeTrue(
                "containers realized after filtering starts should still render filtering state.");
            realizedItem.FilterValue.ShouldBe("needle");
            realizedItem.IsVisible.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Changing_FilterValueSelector_Recalculates_Active_Filter()
    {
        var items = new[]
        {
            new FilterOption("alpha", "target"),
            new FilterOption("beta", "other")
        };
        var listBox = new AtomListBox
        {
            Width       = 240,
            Height      = 96,
            ItemsSource = items,
            FilterValue = "target"
        };
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 160,
            Content = listBox
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            listBox.FilterResultCount.ShouldBe(0);

            listBox.FilterValueSelector = item => ((FilterOption)item!).Keyword;
            Dispatcher.UIThread.RunJobs();

            listBox.FilterResultCount.ShouldBe(
                1,
                "changing the selector should re-run the active filter over the item data.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Removing_HideUnmatched_Strategy_Restores_Filtered_Item_Visibility()
    {
        var listBox = new AtomListBox
        {
            Width       = 240,
            Height      = 96,
            ItemsSource = new[] { "needle", "content" }
        };
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 160,
            Content = listBox
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var unmatchedItem = (AtomListBoxItem)listBox.ContainerFromIndex(1)!;
            listBox.FilterValue = "needle";
            Dispatcher.UIThread.RunJobs();
            unmatchedItem.IsVisible.ShouldBeFalse();

            listBox.FilterHighlightStrategy = AtomHighlightStrategy.HighlightedMatch;
            Dispatcher.UIThread.RunJobs();

            unmatchedItem.IsVisible.ShouldBeTrue(
                "turning off HideUnMatched should restore the item's original visibility.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Filtering_Highlight_Text_Uses_Item_Vertical_Content_Alignment()
    {
        var listBox = new AtomListBox
        {
            Width       = 360,
            Height      = 96,
            FilterValue = "car",
            ItemsSource = new[]
            {
                "Racing car sprays burning fuel into crowd.",
                "Japanese princess to wed commoner."
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 160,
            Content = listBox
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var matchedItem = (AtomListBoxItem)listBox.ContainerFromIndex(0)!;
            var highlightText = matchedItem.GetVisualDescendants()
                                           .OfType<AtomHighlightableTextBlock>()
                                           .Single();

            matchedItem.VerticalContentAlignment.ShouldBe(VerticalAlignment.Center);
            highlightText.VerticalAlignment.ShouldBe(
                matchedItem.VerticalContentAlignment,
                "filter result text should keep the same vertical alignment as normal item content.");
        }
        finally
        {
            window.Close();
        }
    }

    private static T WaitFor<T>(Func<T?> probe, string because)
        where T : class
    {
        for (var i = 0; i < 80; i++)
        {
            Dispatcher.UIThread.RunJobs();
            var value = probe();
            if (value != null)
            {
                return value;
            }
            Thread.Sleep(10);
        }

        throw new TimeoutException(because);
    }

    private sealed class FilterOption
    {
        public FilterOption(string label, string keyword)
        {
            Label   = label;
            Keyword = keyword;
        }

        public string Label { get; }
        public string Keyword { get; }

        public override string ToString()
        {
            return Label;
        }
    }
}
