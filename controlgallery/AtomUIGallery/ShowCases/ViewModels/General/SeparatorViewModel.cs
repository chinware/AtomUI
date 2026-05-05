using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class SeparatorViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Separator";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public SeparatorViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
