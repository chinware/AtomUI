using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public class PopupConfirmViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "PopupConfirm";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    public PopupConfirmViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }

}
