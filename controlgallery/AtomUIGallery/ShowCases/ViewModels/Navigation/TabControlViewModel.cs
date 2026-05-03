using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class TabControlViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TabControl";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    #region TabStrip

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

    #endregion

    #region TabControl

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
    #endregion

    public TabControlViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    #region TabStrip

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

    #endregion

    #region TabControl

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

    #endregion
}
