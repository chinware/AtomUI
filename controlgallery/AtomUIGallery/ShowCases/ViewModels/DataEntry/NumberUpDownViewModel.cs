using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class NumberUpDownViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "NumberUpDown";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public NumberUpDownViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}