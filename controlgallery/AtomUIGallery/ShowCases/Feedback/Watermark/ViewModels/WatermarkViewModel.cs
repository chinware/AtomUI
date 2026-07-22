using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Watermark;

public class WatermarkViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Watermark";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public WatermarkViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
