using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class ResultViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "Result";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public ResultViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
