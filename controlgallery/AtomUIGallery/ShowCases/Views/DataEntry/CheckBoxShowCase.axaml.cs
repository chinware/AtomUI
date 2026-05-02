using System.Collections.Generic;
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
                this.BindCommand(viewModel, vm => vm.CheckStatusCommand, v => v.CheckStatusBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.EnableStatusCommand, v => v.EnableStatusBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckBoxCommand, v => v.ControlledCheckbox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedAllStatusCommand, v => v.CheckAllCheckbox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand1, v => v.AppleCheckBox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand2, v => v.PearCheckBox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand3, v => v.OrangeCheckBox)
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