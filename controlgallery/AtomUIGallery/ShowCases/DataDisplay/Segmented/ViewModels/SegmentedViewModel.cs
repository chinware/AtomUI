using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Segmented;

public class SegmentedViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Segmented";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private CustomizableSizeType _roundShapeSizeType = CustomizableSizeType.Middle;

    public CustomizableSizeType RoundShapeSizeType
    {
        get => _roundShapeSizeType;
        set => this.RaiseAndSetIfChanged(ref _roundShapeSizeType, value);
    }

    public SegmentedViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
