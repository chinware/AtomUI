using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class SeparatorViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "Separator";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public SeparatorViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
