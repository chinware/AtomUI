using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class CustomizeThemeViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "CustomizeTheme";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public CustomizeThemeViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}