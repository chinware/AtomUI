using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CheckBoxShowCase : ReactiveUserControl<CheckBoxViewModel>
{
    public CheckBoxShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is CheckBoxViewModel viewModel)
            {
                ConfigureCheckBoxOptions(viewModel);
                
                this.OneWayBind(viewModel, vm => vm.CheckBoxOptions, v => v.BasicCheckBoxGroup.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DefaultCheckBoxOptions, v => v.BasicCheckBoxGroup.CheckedItems)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.CheckBoxOptions        = null;
                    viewModel.DefaultCheckBoxOptions = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
    
    private void ConfigureCheckBoxOptions(CheckBoxViewModel viewModel)
    {
        var apple = new CheckBoxOption() { Content = "Apple" };
        var pear = new CheckBoxOption() { Content = "Pear" };
        viewModel.CheckBoxOptions = new List<CheckBoxOption>
        {
            apple,
            pear,
            new () { Content = "Orange", IsEnabled = false},
        };
        viewModel.DefaultCheckBoxOptions = new List<CheckBoxOption>
        {
            pear,
        };
    }
}