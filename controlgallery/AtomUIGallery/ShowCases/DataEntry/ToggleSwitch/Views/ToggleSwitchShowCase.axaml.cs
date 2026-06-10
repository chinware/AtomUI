using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using Button = AtomUI.Desktop.Controls.Button;
using ToggleSwitch = AtomUI.Desktop.Controls.ToggleSwitch;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public partial class ToggleSwitchShowCase : GalleryReactiveUserControl<ToggleSwitchViewModel>
{
    public const string LanguageId = nameof(ToggleSwitchShowCase);

    public ReactiveCommand<Unit, Unit> ToggleSwitchCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> ToggleLoadingStatus { get; private set; }

    public ToggleSwitchShowCase()
    {
        ToggleSwitchCommand = ReactiveCommand.Create(HandleToggleDisabledStatus);
        ToggleLoadingStatus = ReactiveCommand.Create(HandleToggleLoadingStatus);
        InitializeComponent();
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