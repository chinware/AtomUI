using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagGroupBindingTests
{
    static CheckableTagGroupBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CheckedItem_Default_TwoWay_Binding_Synchronizes_In_Both_Directions()
    {
        var viewModel = new BindingViewModel { CheckedItem = "Movies" };
        var group = new CheckableTagGroup
        {
            Options = new[] { "Movies", "Books", "Music" }
        };
        group.Bind(
            CheckableTagGroup.CheckedItemProperty,
            new Binding(nameof(BindingViewModel.CheckedItem)) { Source = viewModel });

        ShowInWindow(group, () =>
        {
            FindTag(group, "Movies").IsChecked.ShouldBe(true);

            FindTag(group, "Books").IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            viewModel.CheckedItem.ShouldBe("Books");

            viewModel.CheckedItem = "Music";
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Music").IsChecked.ShouldBe(true);
            FindTag(group, "Books").IsChecked.ShouldBe(false);
        });
    }

    [Fact]
    public void CheckedItems_Default_TwoWay_Binding_Replaces_Only_The_Public_Value()
    {
        var initialValues = new ObservableCollection<object> { "Movies" };
        var viewModel = new BindingViewModel { CheckedItems = initialValues };
        var group = new CheckableTagGroup
        {
            IsMultiple = true,
            Options    = new[] { "Movies", "Books", "Music" }
        };
        group.Bind(
            CheckableTagGroup.CheckedItemsProperty,
            new Binding(nameof(BindingViewModel.CheckedItems)) { Source = viewModel });

        ShowInWindow(group, () =>
        {
            FindTag(group, "Books").IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            viewModel.CheckedItems.ShouldBe(new[] { "Movies", "Books" });
            viewModel.CheckedItems.ShouldNotBeSameAs(initialValues);

            viewModel.CheckedItems = new ObservableCollection<object> { "Music" };
            Dispatcher.UIThread.RunJobs();
            FindTag(group, "Movies").IsChecked.ShouldBe(false);
            FindTag(group, "Books").IsChecked.ShouldBe(false);
            FindTag(group, "Music").IsChecked.ShouldBe(true);
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

    private sealed class BindingViewModel : INotifyPropertyChanged
    {
        private object? _checkedItem;

        public object? CheckedItem
        {
            get => _checkedItem;
            set
            {
                if (Equals(_checkedItem, value))
                {
                    return;
                }
                _checkedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckedItem)));
            }
        }

        private IList? _checkedItems;

        public IList? CheckedItems
        {
            get => _checkedItems;
            set
            {
                if (ReferenceEquals(_checkedItems, value))
                {
                    return;
                }
                _checkedItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckedItems)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
