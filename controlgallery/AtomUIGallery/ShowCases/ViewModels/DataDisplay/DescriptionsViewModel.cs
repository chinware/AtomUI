using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class DescriptionsViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Descriptions";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();
    
    public DescriptionsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}