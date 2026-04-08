using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class RadioButtonShowCase : ReactiveUserControl<RadioButtonViewModel>
{
    public RadioButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is RadioButtonViewModel viewModel)
            {
                ConfigureRadioOptions(viewModel);
                
                this.OneWayBind(ViewModel, vm => vm.RadioOptions, v => v.RadioButtonGroup.ItemsSource)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.RadioOptions = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void ConfigureRadioOptions(RadioButtonViewModel viewModel)
    {
        viewModel.RadioOptions = new List<RadioButtonOption>
        {
            new () { Content = "Option A"},
            new () { Content = "Option B", IsChecked = true},
            new () { Content = "Option C"},
            new () { Content = "Option D", IsEnabled = false},
        };
    }
    
    public void ToggleDisabledStatus(object arg)
    {
        ToggleDisabledRadioUnChecked.IsEnabled = !ToggleDisabledRadioUnChecked.IsEnabled;
        ToggleDisabledRadioChecked.IsEnabled   = !ToggleDisabledRadioChecked.IsEnabled;
    }
}