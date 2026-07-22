using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public class ToggleSwitchViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ToggleSwitch";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private bool _isDisabledDemoEnabled = true;
    private bool _isLoadingDemoLoading  = true;

    public bool IsDisabledDemoEnabled
    {
        get => _isDisabledDemoEnabled;
        set => this.RaiseAndSetIfChanged(ref _isDisabledDemoEnabled, value);
    }

    public bool IsLoadingDemoLoading
    {
        get => _isLoadingDemoLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoadingDemoLoading, value);
    }

    public ToggleSwitchViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
