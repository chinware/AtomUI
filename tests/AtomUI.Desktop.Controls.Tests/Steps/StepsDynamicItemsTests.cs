using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsDynamicItemsTests
{
    static StepsDynamicItemsTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Add_Reprojects_All_Affected_Items()
    {
        var source = CreateItems("A", "B", "C");
        var steps  = CreateSteps(source, current: 1);

        ShowInWindow(steps, () =>
        {
            source.Insert(0, new Desktop.Controls.StepsItem { Header = "X" });
            Dispatcher.UIThread.RunJobs();

            AssertProjection(steps, [0, 1, 2, 3], [false, true, false, false]);
        });
    }

    [Fact]
    public void Remove_Detaches_Removed_Item_And_Reprojects_Remaining_Items()
    {
        var source  = CreateItems("A", "B", "C");
        var removed = source[0];
        var steps   = CreateSteps(source, current: 1);

        ShowInWindow(steps, () =>
        {
            source.RemoveAt(0);
            Dispatcher.UIThread.RunJobs();

            removed.Owner.ShouldBeNull();
            removed.ItemIndex.ShouldBe(-1);
            AssertProjection(steps, [0, 1], [false, true]);
        });
    }

    [Fact]
    public void Replace_Detaches_Old_Item_And_Prepares_New_Item()
    {
        var source   = CreateItems("A", "B", "C");
        var replaced = source[1];
        var steps    = CreateSteps(source, current: 1);

        ShowInWindow(steps, () =>
        {
            source[1] = new Desktop.Controls.StepsItem { Header = "X" };
            Dispatcher.UIThread.RunJobs();

            replaced.Owner.ShouldBeNull();
            replaced.ItemIndex.ShouldBe(-1);
            AssertProjection(steps, [0, 1, 2], [false, true, false]);
        });
    }

    [Fact]
    public void Move_Reprojects_Step_Numbers_And_Statuses()
    {
        var source = CreateItems("A", "B", "C");
        var steps  = CreateSteps(source, current: 0);

        ShowInWindow(steps, () =>
        {
            source.Move(2, 0);
            Dispatcher.UIThread.RunJobs();

            AssertProjection(steps, [0, 1, 2], [true, false, false]);
            source.Select(item => item.Header).ShouldBe(["C", "A", "B"]);
        });
    }

    [Fact]
    public void Reset_Detaches_Old_Items_And_Prepares_New_Items()
    {
        var source   = CreateItems("A", "B", "C");
        var oldItems = source.ToArray();
        var steps    = CreateSteps(source, current: 0);

        ShowInWindow(steps, () =>
        {
            source.Clear();
            Dispatcher.UIThread.RunJobs();

            oldItems.Select(item => item.Owner).ShouldAllBe(owner => owner == null);
            oldItems.Select(item => item.ItemIndex).ShouldAllBe(index => index == -1);

            source.Add(new Desktop.Controls.StepsItem { Header = "X" });
            source.Add(new Desktop.Controls.StepsItem { Header = "Y" });
            Dispatcher.UIThread.RunJobs();

            AssertProjection(steps, [0, 1], [true, false]);
        });
    }

    [Fact]
    public void Generated_Container_Uses_Content_And_Releases_Owner_When_Removed()
    {
        var source = new ObservableCollection<string>(["A", "B"]);
        var steps = new Desktop.Controls.Steps
        {
            Width       = 640,
            ItemsSource = source
        };

        ShowInWindow(steps, () =>
        {
            var container = steps.ContainerFromIndex(0).ShouldBeOfType<Desktop.Controls.StepsItem>();
            container.Content.ShouldBe("A");
            container.Header.ShouldBeNull();
            container.Owner.ShouldBeSameAs(steps);

            source.RemoveAt(0);
            Dispatcher.UIThread.RunJobs();

            container.Owner.ShouldBeNull();
            container.ItemIndex.ShouldBe(-1);
        });
    }

    [Fact]
    public void Remove_Preserves_Direct_Item_ContentTemplate()
    {
        var template = new FuncDataTemplate<object?>((_, _) => new Border());
        var item = new Desktop.Controls.StepsItem
        {
            Header          = "A",
            ContentTemplate = template
        };
        var source = new ObservableCollection<Desktop.Controls.StepsItem>([item]);
        var steps = CreateSteps(source, current: 0);

        ShowInWindow(steps, () =>
        {
            source.Remove(item);
            Dispatcher.UIThread.RunJobs();

            item.ContentTemplate.ShouldBeSameAs(template);
        });
    }

    private static ObservableCollection<Desktop.Controls.StepsItem> CreateItems(params string[] headers)
    {
        return new ObservableCollection<Desktop.Controls.StepsItem>(
            headers.Select(header => new Desktop.Controls.StepsItem { Header = header }));
    }

    private static Desktop.Controls.Steps CreateSteps(
        ObservableCollection<Desktop.Controls.StepsItem> source,
        int current)
    {
        return new Desktop.Controls.Steps
        {
            Width       = 640,
            Current     = current,
            ItemsSource = source
        };
    }

    private static void AssertProjection(
        Desktop.Controls.Steps steps,
        int[] stepNumbers,
        bool[] isCurrent)
    {
        var items = steps.ItemsSource!.Cast<Desktop.Controls.StepsItem>().ToArray();
        items.Select(item => item.StepNumber).ShouldBe(stepNumbers);
        items.Select(item => item.ItemIndex).ShouldBe(Enumerable.Range(0, items.Length));
        items.Select(item => item.IsFirst).ShouldBe(
            Enumerable.Range(0, items.Length).Select(index => index == 0));
        items.Select(item => item.IsLast).ShouldBe(
            Enumerable.Range(0, items.Length).Select(index => index == items.Length - 1));
        items.Select(item => item.IsCurrent).ShouldBe(isCurrent);

        var effectiveStatuses = items.Select(item => item.EffectiveStatus).ToArray();
        items.Take(Math.Max(0, items.Length - 1))
             .Select(item => item.ConnectorStatus)
             .ShouldBe(effectiveStatuses.Skip(1));
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
