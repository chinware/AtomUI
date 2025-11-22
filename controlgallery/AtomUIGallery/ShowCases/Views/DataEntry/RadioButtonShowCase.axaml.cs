using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class RadioButtonShowCase : ReactiveUserControl<RadioButtonViewModel>
{
    public RadioButtonShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
    
    public void ToggleDisabledStatus(object arg)
    {
        ToggleDisabledRadioUnChecked.IsEnabled = !ToggleDisabledRadioUnChecked.IsEnabled;
        ToggleDisabledRadioChecked.IsEnabled   = !ToggleDisabledRadioChecked.IsEnabled;
    }
}