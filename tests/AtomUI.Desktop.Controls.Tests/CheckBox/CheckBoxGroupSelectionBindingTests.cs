using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomCheckBox = AtomUI.Desktop.Controls.CheckBox;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.CheckBox;

public class CheckBoxGroupSelectionBindingTests
{
    static CheckBoxGroupSelectionBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CheckedItems_Is_TwoWay_And_DataValidation_Enabled()
    {
        var metadata = Desktop.Controls.CheckBoxGroup.CheckedItemsProperty.GetMetadata(
            typeof(Desktop.Controls.CheckBoxGroup));

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void CheckedItems_DefaultBindingMode_Updates_ViewModel()
    {
        var apple = CreateOption("Apple");
        var pear  = CreateOption("Pear");
        var viewModel = new CheckBoxGroupBindingViewModel
        {
            CheckedItems = new ObservableCollection<CheckBoxOption> { apple }
        };
        var group = new Desktop.Controls.CheckBoxGroup
        {
            ItemsSource = new[] { apple, pear }
        };
        group.Bind(
            Desktop.Controls.CheckBoxGroup.CheckedItemsProperty,
            new Binding(nameof(CheckBoxGroupBindingViewModel.CheckedItems))
            {
                Source = viewModel
            });

        ShowInWindow(group, () =>
        {
            group.CheckedItems.ShouldBeSameAs(viewModel.CheckedItems);

            group.CheckedItems = new ObservableCollection<CheckBoxOption> { apple, pear };
            Dispatcher.UIThread.RunJobs();

            viewModel.CheckedItems.ShouldNotBeNull();
            viewModel.CheckedItems.ShouldBe(new[] { apple, pear });
        });
    }

    [Fact]
    public void CheckedItems_ObservableCollection_Mutation_Refreshes_Checked_States_And_Form_Value()
    {
        var apple = CreateOption("Apple");
        var pear  = CreateOption("Pear");
        var viewModel = new CheckBoxGroupBindingViewModel
        {
            CheckedItems = new ObservableCollection<CheckBoxOption> { apple }
        };
        var group = new Desktop.Controls.CheckBoxGroup
        {
            ItemsSource = new[] { apple, pear }
        };
        group.Bind(
            Desktop.Controls.CheckBoxGroup.CheckedItemsProperty,
            new Binding(nameof(CheckBoxGroupBindingViewModel.CheckedItems))
            {
                Source = viewModel
            });

        ShowInWindow(group, () =>
        {
            FindCheckBox(group, apple).IsChecked.ShouldBe(true);
            FindCheckBox(group, pear).IsChecked.ShouldBe(false);

            var formValueChangedCount = 0;
            ((IFormItemAware)group).ValueChanged += (_, _) => formValueChangedCount++;

            viewModel.CheckedItems!.Add(pear);
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(1);
            FindCheckBox(group, pear).IsChecked.ShouldBe(true);

            viewModel.CheckedItems.Remove(apple);
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(2);
            FindCheckBox(group, apple).IsChecked.ShouldBe(false);

            viewModel.CheckedItems.Clear();
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(3);
            FindCheckBox(group, pear).IsChecked.ShouldBe(false);
        });
    }

    [Fact]
    public void Direct_Checked_Disabled_Item_Remains_Checked_When_Another_Item_Is_Checked()
    {
        var apple = new AtomCheckBox
        {
            Content = "Apple"
        };
        var pear = new AtomCheckBox
        {
            Content   = "Pear",
            IsChecked = true,
            IsEnabled = false
        };
        var orange = new AtomCheckBox
        {
            Content = "Orange"
        };
        var group = new Desktop.Controls.CheckBoxGroup();
        group.Items.Add(apple);
        group.Items.Add(pear);
        group.Items.Add(orange);

        ShowInWindow(group, () =>
        {
            pear.IsChecked.ShouldBe(true);
            var initialFormValue = ((IFormItemAware)group).GetFormValue().ShouldBeAssignableTo<IList>();
            initialFormValue.ShouldNotBeNull();
            initialFormValue.Contains(pear).ShouldBeTrue();

            orange.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            pear.IsChecked.ShouldBe(true);
            orange.IsChecked.ShouldBe(true);
            group.CheckedItems.ShouldNotBeNull();
            group.CheckedItems!.Contains(pear).ShouldBeTrue();
            group.CheckedItems.Contains(orange).ShouldBeTrue();
            var updatedFormValue = ((IFormItemAware)group).GetFormValue().ShouldBeAssignableTo<IList>();
            updatedFormValue.ShouldNotBeNull();
            updatedFormValue.Contains(pear).ShouldBeTrue();
            updatedFormValue.Contains(orange).ShouldBeTrue();
        });
    }

    private static CheckBoxOption CreateOption(string content)
    {
        return new CheckBoxOption
        {
            Content = content
        };
    }

    private static AtomCheckBox FindCheckBox(Control root, CheckBoxOption option)
    {
        return root.GetVisualDescendants()
                   .OfType<AtomCheckBox>()
                   .Single(checkBox => ReferenceEquals(checkBox.Content, option));
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
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

    private sealed class CheckBoxGroupBindingViewModel : INotifyPropertyChanged
    {
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
