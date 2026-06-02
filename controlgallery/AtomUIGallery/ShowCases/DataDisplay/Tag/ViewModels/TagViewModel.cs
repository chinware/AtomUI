using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tag;

public class TagViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tag";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public TagViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
