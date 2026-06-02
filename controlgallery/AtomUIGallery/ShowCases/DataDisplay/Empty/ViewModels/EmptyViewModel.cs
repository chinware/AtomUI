using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Empty;

public class EmptyViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Empty";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public EmptyViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
