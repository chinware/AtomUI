using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class SegmentedViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Segmented";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public SegmentedViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}