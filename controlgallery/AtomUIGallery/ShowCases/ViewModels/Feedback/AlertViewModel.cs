using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class AlertViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Alert";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public AlertViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}