using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

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
        
    public static readonly StyledProperty<IBrush?> NormalIconColorProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(NormalIconColor));
    
    public static readonly StyledProperty<IBrush?> ActiveIconColorProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(ActiveIconColor));
    
    public static readonly StyledProperty<IBrush?> SelectedIconColorProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(SelectedIconColor));
    
    public static readonly StyledProperty<IBrush?> DisabledIconColorProperty =
        AvaloniaProperty.Register<ToggleIconButton, IBrush?>(
            nameof(DisabledIconColor));

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

    public IBrush? NormalIconColor
    {
        get => GetValue(NormalIconColorProperty);
        set => SetValue(NormalIconColorProperty, value);
    }

    public IBrush? ActiveIconColor
    {
        get => GetValue(ActiveIconColorProperty);
        set => SetValue(ActiveIconColorProperty, value);
    }

    public IBrush? SelectedIconColor
    {
        get => GetValue(SelectedIconColorProperty);
        set => SetValue(SelectedIconColorProperty, value);
    }

    public IBrush? DisabledIconColor
    {
        get => GetValue(DisabledIconColorProperty);
        set => SetValue(DisabledIconColorProperty, value);
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

    protected virtual void ConfigureIcon(Icon icon)
    {
        BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty);
        BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty);
        
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            BindUtils.RelayBind(this, NormalIconColorProperty, icon, Icon.NormalFilledBrushProperty);
            BindUtils.RelayBind(this, ActiveIconColorProperty, icon, Icon.ActiveFilledBrushProperty);
            BindUtils.RelayBind(this, SelectedIconColorProperty, icon, Icon.SelectedFilledBrushProperty);
            BindUtils.RelayBind(this, DisabledIconColorProperty, icon, Icon.DisabledFilledBrushProperty);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CheckedIconProperty ||
            change.Property == UnCheckedIconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
                ConfigureIcon(newIcon);
            }
        }
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