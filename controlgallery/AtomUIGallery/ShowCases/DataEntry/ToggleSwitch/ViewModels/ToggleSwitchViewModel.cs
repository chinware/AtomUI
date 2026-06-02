using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public class ToggleSwitchViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ToggleSwitch";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public ToggleSwitchViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}