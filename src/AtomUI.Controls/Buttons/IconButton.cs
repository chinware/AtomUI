using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public class IconButton : AvaloniaButton,
                          ICustomHitTest,
                          IMotionAwareControl,
                          IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<IconButton, Icon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<IconButton>();
    
    public static readonly StyledProperty<IBrush?> NormalIconColorProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(NormalIconColor));
    
    public static readonly StyledProperty<IBrush?> ActiveIconColorProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(ActiveIconColor));
    
    public static readonly StyledProperty<IBrush?> SelectedIconColorProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(SelectedIconColor));
    
    public static readonly StyledProperty<IBrush?> DisabledIconColorProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(DisabledIconColor));

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconHeight));

    public static readonly StyledProperty<bool> IsEnableHoverEffectProperty
        = AvaloniaProperty.Register<IconButton, bool>(nameof(IsEnableHoverEffect));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconButton>();

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IconAnimation LoadingAnimation
    {
        get => GetValue(LoadingAnimationProperty);
        set => SetValue(LoadingAnimationProperty, value);
    }

    public TimeSpan LoadingAnimationDuration
    {
        get => GetValue(LoadingAnimationDurationProperty);
        set => SetValue(LoadingAnimationDurationProperty, value);
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

    public bool IsEnableHoverEffect
    {
        get => GetValue(IsEnableHoverEffectProperty);
        set => SetValue(IsEnableHoverEffectProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;

    #endregion

    static IconButton()
    {
        AffectsMeasure<IconButton>(IconProperty);
    }

    public IconButton()
    {
        this.RegisterResources();
        this.BindMotionProperties();
        Classes.CollectionChanged += HandleClassesCollectionChanged;
    }

    private void HandleClassesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Icon is not null)
        {
            if (Classes.Contains(StdPseudoClass.Disabled))
            {
                Icon.IconMode = IconMode.Disabled;
            }
            else if (Classes.Contains(StdPseudoClass.Selected))
            {
                Icon.IconMode = IconMode.Selected;
            }
            else if (Classes.Contains(StdPseudoClass.Pressed))
            {
                Icon.IconMode = IconMode.Active;
            }
            else
            {
                Icon.IconMode = IconMode.Normal;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IconProperty)
            {
                if (e.OldValue is Icon oldIcon)
                {
                    oldIcon.SetTemplatedParent(null);
                }

                if (e.NewValue is Icon newIcon)
                {
                    newIcon.SetTemplatedParent(this);
                    ConfigureIcon(newIcon);
                }
            }
        }
    }

    private void ConfigureIcon(Icon icon)
    {
        BindUtils.RelayBind(this, LoadingAnimationProperty, icon, Icon.LoadingAnimationProperty);
        BindUtils.RelayBind(this, LoadingAnimationDurationProperty, icon,
            Icon.LoadingAnimationDurationProperty);
        BindUtils.RelayBind(this, IconHeightProperty, icon, HeightProperty);
        BindUtils.RelayBind(this, IconWidthProperty, icon, WidthProperty);
        if (icon.ThemeType != IconThemeType.TwoTone)
        {
            BindUtils.RelayBind(this, NormalIconColorProperty, icon, Icon.NormalFilledBrushProperty);
            BindUtils.RelayBind(this, ActiveIconColorProperty, icon, Icon.ActiveFilledBrushProperty);
            BindUtils.RelayBind(this, SelectedIconColorProperty, icon, Icon.SelectedFilledBrushProperty);
            BindUtils.RelayBind(this, DisabledIconColorProperty, icon, Icon.DisabledFilledBrushProperty);
        }
        icon.SetTemplatedParent(this);
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}