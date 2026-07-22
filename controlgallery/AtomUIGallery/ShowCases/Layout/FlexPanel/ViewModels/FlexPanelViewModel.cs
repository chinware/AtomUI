using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.FlexPanel;

public class FlexPanelViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "FlexPanelShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public FlexPanelViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
