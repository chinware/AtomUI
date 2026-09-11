using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Window;

public class WindowViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Window";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public WindowViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
