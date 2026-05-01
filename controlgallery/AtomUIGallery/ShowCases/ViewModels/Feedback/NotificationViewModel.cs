using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class NotificationViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Notification";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    public NotificationViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }
}
