using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TabStrip;

public class TabStripViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TabStrip";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private Dock _positionTabStripPlacement = Dock.Top;

    public Dock PositionTabStripPlacement
    {
        get => _positionTabStripPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionTabStripPlacement, value);
    }

    private Dock _positionCardTabStripPlacement = Dock.Top;

    public Dock PositionCardTabStripPlacement
    {
        get => _positionCardTabStripPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionCardTabStripPlacement, value);
    }

    private SizeType _sizeTypeTabStrip = SizeType.Middle;

    public SizeType SizeTypeTabStrip
    {
        get => _sizeTypeTabStrip;
        set => this.RaiseAndSetIfChanged(ref _sizeTypeTabStrip, value);
    }

    public AvaloniaList<TabItemData> TabStripItemDataSource { get; set; } = new();

    public TabStripViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandleTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionTabStripPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleCardTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionCardTabStripPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleTabStripSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        SizeTypeTabStrip = args.Index switch
        {
            0 => SizeType.Small,
            1 => SizeType.Middle,
            _ => SizeType.Large
        };
    }
}
