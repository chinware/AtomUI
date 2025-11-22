using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class DrawerViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Drawer";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public DrawerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}