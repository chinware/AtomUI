using System.Reactive;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Button = AtomUI.Desktop.Controls.Button;
using ToggleSwitch = AtomUI.Desktop.Controls.ToggleSwitch;

namespace AtomUIGallery.ShowCases.Views;

public partial class ToggleSwitchShowCase : ReactiveUserControl<ToggleSwitchViewModel>
{
    public ReactiveCommand<Unit, Unit> ToggleSwitchCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ToggleLoadingStatus { get; private set; }

    public ToggleSwitchShowCase()
    {
        ToggleSwitchCommand = ReactiveCommand.Create(HandleToggleDisabledStatus);
        ToggleLoadingStatus = ReactiveCommand.Create(HandleToggleLoadingStatus);
        InitializeComponent();
        this.WhenActivated(disposables => { });
    }

    private void HandleToggleDisabledStatus()
    {
        if (ToggleDisabledSwitch != null)
        {
            ToggleDisabledSwitch.IsEnabled = !ToggleDisabledSwitch.IsEnabled;
        }
    }

    private void HandleToggleLoadingStatus()
    {
        if (ToggleSwitchDefault != null)
        {
            ToggleSwitchDefault.IsLoading = !ToggleSwitchDefault.IsLoading;
        }

        if (ToggleSwitchSmall != null)
        {
            ToggleSwitchSmall.IsLoading = !ToggleSwitchSmall.IsLoading;
        }
    }
}