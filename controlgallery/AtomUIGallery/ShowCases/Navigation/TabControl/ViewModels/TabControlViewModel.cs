using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TabControl;

public class TabControlViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TabControl";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private Dock _positionTabControlPlacement = Dock.Top;

    public Dock PositionTabControlPlacement
    {
        get => _positionTabControlPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionTabControlPlacement, value);
    }

    private Dock _positionCardTabControlPlacement = Dock.Top;

    public Dock PositionCardTabControlPlacement
    {
        get => _positionCardTabControlPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionCardTabControlPlacement, value);
    }

    private SizeType _sizeTypeControl = SizeType.Middle;

    public SizeType SizeTypeTabControl
    {
        get => _sizeTypeControl;
        set => this.RaiseAndSetIfChanged(ref _sizeTypeControl, value);
    }

    public AvaloniaList<TabItemData> TabItemDataSource { get; set; } = new();

    public TabControlViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandleTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionTabControlPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleCardTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionCardTabControlPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleTabControlSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        SizeTypeTabControl = args.Index switch
        {
            0 => SizeType.Small,
            1 => SizeType.Middle,
            _ => SizeType.Large
        };
    }
}
