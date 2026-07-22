using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Grid;

public class GridViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "GridShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public GridViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
