using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class ColorPickerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ColorPicker";
    
    public IScreen HostScreen { get; }
    
    public string? UrlPathSegment => ID.ToString();

    public ColorPickerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
