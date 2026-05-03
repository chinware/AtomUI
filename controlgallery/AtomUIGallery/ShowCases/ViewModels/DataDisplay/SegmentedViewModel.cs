using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class SegmentedViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "Segmented";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public SegmentedViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
