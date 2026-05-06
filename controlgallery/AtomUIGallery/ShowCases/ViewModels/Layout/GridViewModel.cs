using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class GridViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "GridShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public GridViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
