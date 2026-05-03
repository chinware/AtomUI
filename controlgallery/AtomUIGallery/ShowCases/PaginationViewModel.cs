using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class PaginationViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "Pagination";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public PaginationViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
