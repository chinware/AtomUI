using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TabItem = AtomUI.Controls.TabItem;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class TabControlShowCase : UserControl
{
    public TabControlShowCase()
    {
        InitializeComponent();
        DataContext                                          =  this;
        PositionTabStripOptionGroup.OptionCheckedChanged     += HandleTabStripPlacementOptionCheckedChanged;
        PositionCardTabStripOptionGroup.OptionCheckedChanged += HandleCardTabStripPlacementOptionCheckedChanged;
        SizeTypeTabStripOptionGroup.OptionCheckedChanged     += HandleTabStripSizeTypeOptionCheckedChanged;
        AddTabDemoStrip.AddTabRequest                        += HandleTabStripAddTabRequest;

        PositionTabControlOptionGroup.OptionCheckedChanged     += HandleTabControlPlacementOptionCheckedChanged;
        PositionCardTabControlOptionGroup.OptionCheckedChanged += HandleCardTabControlPlacementOptionCheckedChanged;
        SizeTypeTabControlOptionGroup.OptionCheckedChanged     += HandleTabControlSizeTypeOptionCheckedChanged;
        AddTabDemoTabControl.AddTabRequest                     += HandleTabControlAddTabRequest;
    }



    #region TabStrip

    public static readonly StyledProperty<Dock> PositionTabStripPlacementProperty =
        AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionTabStripPlacement), Dock.Top);

    public static readonly StyledProperty<Dock> PositionCardTabStripPlacementProperty =
        AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionCardTabStripPlacement), Dock.Top);

    public static readonly StyledProperty<SizeType> SizeTypeTabStripProperty =
        AvaloniaProperty.Register<TabControlShowCase, SizeType>(nameof(SizeTypeTabStrip), SizeType.Middle);

    public Dock PositionTabStripPlacement
    {
        get => GetValue(PositionTabStripPlacementProperty);
        set => SetValue(PositionTabStripPlacementProperty, value);
    }

    public Dock PositionCardTabStripPlacement
    {
        get => GetValue(PositionCardTabStripPlacementProperty);
        set => SetValue(PositionCardTabStripPlacementProperty, value);
    }

    public SizeType SizeTypeTabStrip
    {
        get => GetValue(SizeTypeTabStripProperty);
        set => SetValue(SizeTypeTabStripProperty, value);
    }

    #endregion



    #region TabControl

    public static readonly StyledProperty<Dock> PositionTabControlPlacementProperty =
        AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionTabControlPlacement), Dock.Top);

    public static readonly StyledProperty<Dock> PositionCardTabControlPlacementProperty =
        AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionCardTabControlPlacement), Dock.Top);

    public static readonly StyledProperty<SizeType> SizeTypeTabControlProperty =
        AvaloniaProperty.Register<TabControlShowCase, SizeType>(nameof(SizeTypeTabControl), SizeType.Middle);

    public Dock PositionTabControlPlacement
    {
        get => GetValue(PositionTabControlPlacementProperty);
        set => SetValue(PositionTabControlPlacementProperty, value);
    }

    public Dock PositionCardTabControlPlacement
    {
        get => GetValue(PositionCardTabControlPlacementProperty);
        set => SetValue(PositionCardTabControlPlacementProperty, value);
    }

    public SizeType SizeTypeTabControl
    {
        get => GetValue(SizeTypeTabControlProperty);
        set => SetValue(SizeTypeTabControlProperty, value);
    }

    #endregion



    #region TabStrip

    private void HandleTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            PositionTabStripPlacement = Dock.Top;
        else if (args.Index == 1)
            PositionTabStripPlacement = Dock.Bottom;
        else if (args.Index == 2)
            PositionTabStripPlacement = Dock.Left;
        else
            PositionTabStripPlacement = Dock.Right;
    }

    private void HandleCardTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            PositionCardTabStripPlacement = Dock.Top;
        else if (args.Index == 1)
            PositionCardTabStripPlacement = Dock.Bottom;
        else if (args.Index == 2)
            PositionCardTabStripPlacement = Dock.Left;
        else
            PositionCardTabStripPlacement = Dock.Right;
    }

    private void HandleTabStripSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            SizeTypeTabStrip = SizeType.Small;
        else if (args.Index == 1)
            SizeTypeTabStrip = SizeType.Middle;
        else
            SizeTypeTabStrip = SizeType.Large;
    }

    private void HandleTabStripAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoStrip.ItemCount;
        AddTabDemoStrip.Items.Add(new TabStripItem
        {
            Content    = $"new tab {index}",
            IsClosable = true
        });
    }

    #endregion



    #region TabControl

    private void HandleTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            PositionTabControlPlacement = Dock.Top;
        else if (args.Index == 1)
            PositionTabControlPlacement = Dock.Bottom;
        else if (args.Index == 2)
            PositionTabControlPlacement = Dock.Left;
        else
            PositionTabControlPlacement = Dock.Right;
    }

    private void HandleCardTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            PositionCardTabControlPlacement = Dock.Top;
        else if (args.Index == 1)
            PositionCardTabControlPlacement = Dock.Bottom;
        else if (args.Index == 2)
            PositionCardTabControlPlacement = Dock.Left;
        else
            PositionCardTabControlPlacement = Dock.Right;
    }

    private void HandleTabControlSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            SizeTypeTabControl = SizeType.Small;
        else if (args.Index == 1)
            SizeTypeTabControl = SizeType.Middle;
        else
            SizeTypeTabControl = SizeType.Large;
    }

    private void HandleTabControlAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoTabControl.ItemCount;
        AddTabDemoTabControl.Items.Add(new TabItem
        {
            Header     = $"new tab {index}",
            Content    = $"new tab content {index}",
            IsClosable = true
        });
    }

    #endregion
}