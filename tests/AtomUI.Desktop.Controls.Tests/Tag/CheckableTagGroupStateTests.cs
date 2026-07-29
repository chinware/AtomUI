using System.Collections.ObjectModel;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagGroupStateTests
{
    static CheckableTagGroupStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Options_Reject_Null_And_Duplicate_Values()
    {
        var group = new CheckableTagGroup();

        Should.Throw<ArgumentException>(() => group.Options = new object?[] { "Movies", null });
        Should.Throw<ArgumentException>(() => group.Options = new[] { "Movies", "Movies" });
        Should.Throw<ArgumentException>(() => group.Options = new ICheckableTagOption[]
        {
            new CheckableTagOption { Value = "movies", Content = "Movies" },
            new CheckableTagOption { Value = "movies", Content = "Films" }
        });
    }

    [Fact]
    public void Explicit_Value_Wins_Over_Default_And_Waits_For_Delayed_Options()
    {
        var group = new CheckableTagGroup
        {
            CheckedItem       = "Music",
            DefaultCheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            group.Options = new[] { "Movies", "Books", "Music", "Sports" };
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Music");
            FindTag(group, "Music").IsChecked.ShouldBe(true);

            group.DefaultCheckedItem = "Sports";
            group.Options = Array.Empty<object>();
            group.Options = new[] { "Movies", "Books", "Music", "Sports" };
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Music");
            FindTag(group, "Music").IsChecked.ShouldBe(true);
        });
    }

    [Fact]
    public void Default_Value_Is_Applied_Only_Once()
    {
        var group = new CheckableTagGroup
        {
            Options            = new[] { "Movies", "Books", "Music" },
            DefaultCheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            group.CheckedItem.ShouldBe("Books");

            group.DefaultCheckedItem = "Music";
            group.CheckedItem = null;
            group.Options = new[] { "Books", "Music", "Sports" };
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBeNull();
            group.GetVisualDescendants()
                 .OfType<CheckableTag>()
                 .ShouldAllBe(tag => tag.IsChecked == false);
        });
    }

    [Fact]
    public void Explicit_CheckedItems_Win_Over_DefaultCheckedItems()
    {
        var explicitValues = new ObservableCollection<object> { "Music" };
        var group = new CheckableTagGroup
        {
            IsMultiple         = true,
            Options            = new[] { "Movies", "Books", "Music" },
            CheckedItems       = explicitValues,
            DefaultCheckedItems = new[] { "Books" }
        };

        ShowInWindow(group, () =>
        {
            group.CheckedItems.ShouldBeSameAs(explicitValues);
            FindTag(group, "Music").IsChecked.ShouldBe(true);
            FindTag(group, "Books").IsChecked.ShouldBe(false);
        });
    }

    [Fact]
    public void Dynamic_Options_Preserve_Public_Value_And_Release_Old_Collection()
    {
        var oldOptions = new ObservableCollection<object> { "Movies" };
        var newOptions = new ObservableCollection<object> { "Books" };
        var group = new CheckableTagGroup
        {
            Options     = oldOptions,
            CheckedItem = "Music"
        };

        ShowInWindow(group, () =>
        {
            oldOptions.Add("Music");
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Music").IsChecked.ShouldBe(true);

            oldOptions.Remove("Music");
            Dispatcher.UIThread.RunJobs();
            group.CheckedItem.ShouldBe("Music");

            group.Options = newOptions;
            oldOptions.Add("Music");
            Dispatcher.UIThread.RunJobs();
            group.GetVisualDescendants()
                 .OfType<CheckableTag>()
                 .ShouldNotContain(tag => Equals(tag.Content, "Music"));

            newOptions.Add("Music");
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Music").IsChecked.ShouldBe(true);
        });
    }

    [Fact]
    public void Options_Move_And_Reset_Rebuild_Containers_Without_Losing_Public_Selection()
    {
        var options = new ObservableCollection<object> { "Movies", "Books", "Music" };
        var group = new CheckableTagGroup
        {
            Options     = options,
            CheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            options.Move(1, 0);
            Dispatcher.UIThread.RunJobs();

            group.GetVisualDescendants()
                 .OfType<CheckableTag>()
                 .Select(tag => tag.Content)
                 .ShouldBe(new object?[] { "Books", "Movies", "Music" });
            FindTag(group, "Books").IsChecked.ShouldBe(true);

            options.Clear();
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Books");
            group.GetVisualDescendants().OfType<CheckableTag>().ShouldBeEmpty();

            options.Add("Books");
            Dispatcher.UIThread.RunJobs();

            FindTag(group, "Books").IsChecked.ShouldBe(true);
        });
    }

    [Fact]
    public void CheckedItems_Collection_Changes_Project_All_Collection_Actions()
    {
        var checkedItems = new ObservableCollection<object> { "Movies", "Music" };
        var group = new CheckableTagGroup
        {
            IsMultiple  = true,
            Options     = new[] { "Movies", "Books", "Music", "Sports" },
            CheckedItems = checkedItems
        };

        ShowInWindow(group, () =>
        {
            var checkedChangedCount = 0;
            var formValueChangedCount = 0;
            group.CheckedChanged += (_, _) => checkedChangedCount++;
            ((IFormItemAware)group).ValueChanged += (_, _) => formValueChangedCount++;

            checkedItems.Add("Books");
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Books").IsChecked.ShouldBe(true);

            checkedItems.Move(2, 0);
            Dispatcher.UIThread.RunJobs();
            group.CheckedItems.ShouldBe(new[] { "Books", "Movies", "Music" });

            checkedItems.Remove("Movies");
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Movies").IsChecked.ShouldBe(false);

            checkedItems.Clear();
            Dispatcher.UIThread.RunJobs();
            group.GetVisualDescendants()
                 .OfType<CheckableTag>()
                 .ShouldAllBe(tag => tag.IsChecked == false);

            checkedChangedCount.ShouldBe(4);
            formValueChangedCount.ShouldBe(4);
        });
    }

    [Fact]
    public void Replacing_CheckedItems_Releases_The_Old_Collection()
    {
        var oldCheckedItems = new ObservableCollection<object> { "Movies" };
        var newCheckedItems = new ObservableCollection<object> { "Books" };
        var group = new CheckableTagGroup
        {
            IsMultiple  = true,
            Options     = new[] { "Movies", "Books", "Music" },
            CheckedItems = oldCheckedItems
        };

        ShowInWindow(group, () =>
        {
            group.CheckedItems = newCheckedItems;
            Dispatcher.UIThread.RunJobs();

            var checkedChangedCount = 0;
            group.CheckedChanged += (_, _) => checkedChangedCount++;

            oldCheckedItems.Add("Music");
            Dispatcher.UIThread.RunJobs();
            checkedChangedCount.ShouldBe(0);
            FindTag(group, "Music").IsChecked.ShouldBe(false);

            newCheckedItems.Add("Music");
            Dispatcher.UIThread.RunJobs();
            checkedChangedCount.ShouldBe(1);
            FindTag(group, "Music").IsChecked.ShouldBe(true);
        });
    }

    [Fact]
    public void User_Interaction_Drops_Values_That_Are_Not_In_Current_Options()
    {
        var group = new CheckableTagGroup
        {
            IsMultiple  = true,
            Options     = new[] { "Movies", "Books", "Music" },
            CheckedItems = new ObservableCollection<object> { "missing", "Movies" }
        };

        ShowInWindow(group, () =>
        {
            FindTag(group, "Books").IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItems.ShouldBe(new[] { "Movies", "Books" });
        });
    }

    [Fact]
    public void Runtime_ItemTemplate_Changes_Are_Projected_To_The_Internal_Selection_Host()
    {
        var group = new CheckableTagGroup
        {
            Options = new[] { "Movies", "Books" }
        };

        ShowInWindow(group, () =>
        {
            var template = new FuncDataTemplate<object?>(
                (item, _) => new TextBlock { Text = $"Option:{item}" });

            group.ItemTemplate = template;
            Dispatcher.UIThread.RunJobs();

            var selectionHost = group.GetVisualDescendants()
                                     .OfType<SelectingItemsControl>()
                                     .Single();
            selectionHost.ItemTemplate.ShouldBeSameAs(template);
        });
    }

    [Fact]
    public void Detach_And_Reattach_Rebuilds_From_Current_Public_State()
    {
        var options = new ObservableCollection<object> { "Movies", "Books" };
        var checkedItems = new ObservableCollection<object> { "Movies" };
        var group = new CheckableTagGroup
        {
            IsMultiple  = true,
            Options     = options,
            CheckedItems = checkedItems
        };
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 240,
            Content = group
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var oldTag = FindTag(group, "Books");

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            options.Add("Music");
            checkedItems.Add("Music");
            oldTag.IsChecked = true;

            window.Content = group;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItems.ShouldBe(new[] { "Movies", "Music" });
            FindTag(group, "Music").IsChecked.ShouldBe(true);
            FindTag(group, "Books").IsChecked.ShouldBe(false);

            var eventArgs = new List<CheckableTagGroupCheckedChangedEventArgs>();
            group.CheckedChanged += (_, args) => eventArgs.Add(args);
            checkedItems.Add("Books");
            Dispatcher.UIThread.RunJobs();

            eventArgs.Count.ShouldBe(1);
            eventArgs[0].AddedItems.ShouldBe(new[] { "Books" });
            eventArgs[0].RemovedItems.Count.ShouldBe(0);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Template_Reapplication_Rebuilds_Containers_Without_Keeping_Old_Interaction()
    {
        var group = new CheckableTagGroup
        {
            Options     = new[] { "Movies", "Books", "Music" },
            CheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            var oldTag   = FindTag(group, "Movies");
            var template = group.Template;
            template.ShouldNotBeNull();

            group.Template = null;
            Dispatcher.UIThread.RunJobs();
            group.Template = template;
            Dispatcher.UIThread.RunJobs();

            var newTag = FindTag(group, "Movies");
            newTag.ShouldNotBeSameAs(oldTag);
            FindTag(group, "Books").IsChecked.ShouldBe(true);

            oldTag.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            group.CheckedItem.ShouldBe("Books");

            newTag.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            group.CheckedItem.ShouldBe("Movies");
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
