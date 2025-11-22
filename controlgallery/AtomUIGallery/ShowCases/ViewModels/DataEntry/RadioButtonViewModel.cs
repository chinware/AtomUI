using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class RadioButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "RadioButton";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public RadioButtonViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}