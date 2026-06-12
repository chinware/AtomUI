using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Community;

public class CommunityViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Community";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public CommunityViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
