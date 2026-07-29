using System.Collections.ObjectModel;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagGroupEventAndFormTests
{
    static CheckableTagGroupEventAndFormTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Single_Selection_Raises_One_Event_With_Value_Delta()
    {
        var group = new CheckableTagGroup
        {
            Options            = new[] { "Movies", "Books", "Music" },
            DefaultCheckedItem = "Movies"
        };

        ShowInWindow(group, () =>
        {
            var events = new List<CheckableTagGroupCheckedChangedEventArgs>();
            group.CheckedChanged += (_, args) => events.Add(args);

            FindTag(group, "Books").IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            events.Count.ShouldBe(1);
            events[0].IsMultiple.ShouldBeFalse();
            events[0].OldCheckedItem.ShouldBe("Movies");
            events[0].NewCheckedItem.ShouldBe("Books");
            events[0].AddedItems.ShouldBe(new[] { "Books" });
            events[0].RemovedItems.ShouldBe(new[] { "Movies" });

            FindTag(group, "Books").IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            events.Count.ShouldBe(2);
            events[1].OldCheckedItem.ShouldBe("Books");
            events[1].NewCheckedItem.ShouldBeNull();
            events[1].AddedItems.Count.ShouldBe(0);
            events[1].RemovedItems.ShouldBe(new[] { "Books" });
        });
    }

    [Fact]
    public void Mode_Conversion_Uses_Current_Mode_Value_And_Raises_Once()
    {
        var group = new CheckableTagGroup
        {
            Options     = new[] { "Movies", "Books", "Music" },
            CheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            var events = new List<CheckableTagGroupCheckedChangedEventArgs>();
            group.CheckedChanged += (_, args) => events.Add(args);

            group.IsMultiple = true;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItems.ShouldBe(new[] { "Books" });
            events.Count.ShouldBe(1);
            events[0].IsMultiple.ShouldBeTrue();

            group.IsMultiple = false;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Books");
            events.Count.ShouldBe(2);
            events[1].IsMultiple.ShouldBeFalse();
        });
    }

    [Fact]
    public void Invalid_Values_Do_Not_Produce_Selection_Events_And_Mode_Conversion_Uses_First_Valid_Value()
    {
        var checkedItems = new ObservableCollection<object> { "missing", "Books" };
        var group = new CheckableTagGroup
        {
            IsMultiple  = true,
            Options     = new[] { "Movies", "Books", "Music" },
            CheckedItems = checkedItems
        };

        ShowInWindow(group, () =>
        {
            var checkedChangedCount = 0;
            var formValueChangedCount = 0;
            group.CheckedChanged += (_, _) => checkedChangedCount++;
            ((IFormItemAware)group).ValueChanged += (_, _) => formValueChangedCount++;

            checkedItems.Add("also-missing");
            checkedItems.Remove("missing");
            Dispatcher.UIThread.RunJobs();
            checkedChangedCount.ShouldBe(0);
            formValueChangedCount.ShouldBe(0);

            checkedItems.Insert(0, "missing-again");
            group.IsMultiple = false;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Books");
            checkedChangedCount.ShouldBe(1);
            formValueChangedCount.ShouldBe(1);
        });
    }

    [Fact]
    public void Form_Value_Follows_The_Active_Selection_Mode()
    {
        var group = new CheckableTagGroup
        {
            Options            = new[] { "Movies", "Books", "Music" },
            DefaultCheckedItem = "Movies"
        };

        ShowInWindow(group, () =>
        {
            var formItem = (IFormItemAware)group;
            var valueChangedCount = 0;
            formItem.ValueChanged += (_, _) => valueChangedCount++;

            formItem.GetFormValue().ShouldBe("Movies");
            formItem.SetFormValue("Books");
            formItem.GetFormValue().ShouldBe("Books");
            FindTag(group, "Books").IsChecked.ShouldBe(true);

            formItem.ClearFormValue();
            formItem.GetFormValue().ShouldBeNull();
            valueChangedCount.ShouldBe(2);

            group.IsMultiple = true;
            Dispatcher.UIThread.RunJobs();
            valueChangedCount = 0;

            var checkedItems = new ObservableCollection<object> { "Movies", "Music" };
            formItem.SetFormValue(checkedItems);
            formItem.GetFormValue().ShouldBeSameAs(checkedItems);
            FindTag(group, "Movies").IsChecked.ShouldBe(true);
            FindTag(group, "Music").IsChecked.ShouldBe(true);

            formItem.ClearFormValue();
            formItem.GetFormValue().ShouldBeNull();
            valueChangedCount.ShouldBe(2);
        });
    }

    [Fact]
    public void Ancestor_Disabled_State_Disables_Every_Generated_Tag()
    {
        var group = new CheckableTagGroup
        {
            IsEnabled = false,
            Options   = new[] { "Movies", "Books", "Music" }
        };

        ShowInWindow(group, () =>
        {
            group.GetVisualDescendants()
                 .OfType<CheckableTag>()
                 .ShouldAllBe(tag => !tag.IsEffectivelyEnabled);
        });
    }

    private static CheckableTag FindTag(Control root, object content)
    {
        return root.GetVisualDescendants()
                   .OfType<CheckableTag>()
                   .Single(tag => Equals(tag.Content, content));
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
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
            Dispatcher.UIThread.RunJobs();
        }
    }
}
