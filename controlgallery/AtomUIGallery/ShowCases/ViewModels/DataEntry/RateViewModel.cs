using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class RateViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Rate";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public RateViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}