using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.DropdownButton;

public class DropdownButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "DropdownButton";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public DropdownButtonViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
