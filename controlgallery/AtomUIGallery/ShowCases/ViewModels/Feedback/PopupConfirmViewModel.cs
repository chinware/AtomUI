using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class PopupConfirmViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "PopupConfirm";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    public PopupConfirmViewModel(IScreen screen)
    {
        Activator = new ViewModelActivator();
        HostScreen = screen;
    }
}