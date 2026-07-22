using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tag;

public class TagViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tag";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public TagViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
