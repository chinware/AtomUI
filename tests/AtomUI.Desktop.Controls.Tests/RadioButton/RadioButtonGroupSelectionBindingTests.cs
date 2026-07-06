using System.ComponentModel;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomRadioButton = AtomUI.Desktop.Controls.RadioButton;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.RadioButton;

public class RadioButtonGroupSelectionBindingTests
{
    static RadioButtonGroupSelectionBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CheckedItem_Is_TwoWay_And_DataValidation_Enabled()
    {
        var metadata = Desktop.Controls.RadioButtonGroup.CheckedItemProperty.GetMetadata(
            typeof(Desktop.Controls.RadioButtonGroup));

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void CheckedItem_DefaultBindingMode_Updates_ViewModel()
    {
        var apple = CreateOption("Apple");
        var pear  = CreateOption("Pear");
        var viewModel = new RadioButtonGroupBindingViewModel
        {
            CheckedItem = apple
        };
        var group = new Desktop.Controls.RadioButtonGroup
        {
            ItemsSource = new[] { apple, pear }
        };
        group.Bind(
            Desktop.Controls.RadioButtonGroup.CheckedItemProperty,
            new Binding(nameof(RadioButtonGroupBindingViewModel.CheckedItem))
            {
                Source = viewModel
            });

        ShowInWindow(group, () =>
        {
            group.CheckedItem.ShouldBeSameAs(apple);

            group.CheckedItem = pear;
            Dispatcher.UIThread.RunJobs();

            viewModel.CheckedItem.ShouldBeSameAs(pear);
        });
    }

    [Fact]
    public void CheckedItem_User_Selection_Updates_ViewModel()
    {
        var apple = CreateOption("Apple");
        var pear  = CreateOption("Pear");
        var viewModel = new RadioButtonGroupBindingViewModel
        {
            CheckedItem = apple
        };
        var group = new Desktop.Controls.RadioButtonGroup
        {
            ItemsSource = new[] { apple, pear }
        };
        group.Bind(
            Desktop.Controls.RadioButtonGroup.CheckedItemProperty,
            new Binding(nameof(RadioButtonGroupBindingViewModel.CheckedItem))
            {
                Source = viewModel
            });

        ShowInWindow(group, () =>
        {
            FindRadioButton(group, apple).IsChecked.ShouldBe(true);

            FindRadioButton(group, pear).IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBeSameAs(pear);
            viewModel.CheckedItem.ShouldBeSameAs(pear);
        });
    }

    private static RadioButtonOption CreateOption(string content)
    {
        return new RadioButtonOption
        {
            Content = content
        };
    }

    private static AtomRadioButton FindRadioButton(Control root, RadioButtonOption option)
    {
        return root.GetVisualDescendants()
                   .OfType<AtomRadioButton>()
                   .Single(radioButton => ReferenceEquals(radioButton.Content, option));
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

    private sealed class RadioButtonGroupBindingViewModel : INotifyPropertyChanged
    {
        private object? _checkedItem;

        public object? CheckedItem
        {
            get => _checkedItem;
            set
            {
                if (ReferenceEquals(_checkedItem, value))
                {
                    return;
                }

                _checkedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CheckedItem)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
