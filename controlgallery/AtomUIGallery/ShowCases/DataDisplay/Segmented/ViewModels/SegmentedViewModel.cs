using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Segmented;

public class SegmentedViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Segmented";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public SegmentedViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
