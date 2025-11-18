using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class ToggleIconButton : ToggleButton,
                                IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> CheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, Icon?>(nameof(CheckedIcon));

    public static readonly StyledProperty<Icon?> UnCheckedIconProperty
        = AvaloniaProperty.Register<ToggleIconButton, Icon?>(nameof(UnCheckedIcon));

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<ToggleIconButton, double>(nameof(IconHeight));
        
    public static readonly StyledProperty<IBrush?> NormalIconBrushProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(NormalIconBrush));
    
    public static readonly StyledProperty<IBrush?> ActiveIconBrushProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(ActiveIconBrush));
    
    public static readonly StyledProperty<IBrush?> SelectedIconBrushProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(SelectedIconBrush));
    
    public static readonly StyledProperty<IBrush?> DisabledIconBrushProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(DisabledIconBrush));

    public Icon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public Icon? UnCheckedIcon
    {
        get => GetValue(UnCheckedIconProperty);
        set => SetValue(UnCheckedIconProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public IBrush? NormalIconBrush
    {
        get => GetValue(NormalIconBrushProperty);
        set => SetValue(NormalIconBrushProperty, value);
    }

    public IBrush? ActiveIconBrush
    {
        get => GetValue(ActiveIconBrushProperty);
        set => SetValue(ActiveIconBrushProperty, value);
    }

    public IBrush? SelectedIconBrush
    {
        get => GetValue(SelectedIconBrushProperty);
        set => SetValue(SelectedIconBrushProperty, value);
    }

    public IBrush? DisabledIconBrush
    {
        get => GetValue(DisabledIconBrushProperty);
        set => SetValue(DisabledIconBrushProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;
    
    #endregion

    static ToggleIconButton()
    {
        AffectsMeasure<ToggleIconButton>(CheckedIconProperty);
        AffectsMeasure<ToggleIconButton>(UnCheckedIconProperty);
        AffectsMeasure<ToggleIconButton>(IsCheckedProperty);
    }

    public ToggleIconButton()
    {
        this.RegisterResources();
    }
    
    public bool HitTest(Point point)
    {
        return NotifyHistTest(point);
    }

    protected virtual bool NotifyHistTest(Point point)
    {
        return true;
    }
}