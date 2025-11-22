using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class PaginationViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Pagination";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();
    
    public PaginationViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}