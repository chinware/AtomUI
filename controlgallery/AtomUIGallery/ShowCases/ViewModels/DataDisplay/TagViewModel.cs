using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class TagViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "Tag";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public TagViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
