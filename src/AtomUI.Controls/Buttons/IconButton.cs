using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public class IconButton : AvaloniaButton,
                          ICustomHitTest,
                          IAnimationAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<IconButton, Icon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<double> IconWidthProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty
        = AvaloniaProperty.Register<IconButton, double>(nameof(IconHeight));

    public static readonly StyledProperty<bool> IsEnableHoverEffectProperty
        = AvaloniaProperty.Register<IconButton, bool>(nameof(IsEnableHoverEffect));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<IconButton, bool>(nameof(IsMotionEnabled), true);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<IconButton, bool>(nameof(IsWaveAnimationEnabled), true);

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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;

    #endregion

    static IconButton()
    {
        AffectsMeasure<IconButton>(IconProperty);
    }

    public IconButton()
    {
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
        Cursor = new Cursor(StandardCursorType.Hand);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupIcon();
        SetupIconMode();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (VisualRoot is not null)
        {
            if (e.Property == IconProperty)
            {
                var oldIcon = e.GetOldValue<Icon?>();
                if (oldIcon is not null)
                {
                    ((ISetLogicalParent)oldIcon).SetParent(null);
                }

                SetupIcon();
            }
            else if (e.Property == IsPressedProperty ||
                     e.Property == IsPointerOverProperty ||
                     e.Property == IsEnabledProperty)
            {
                SetupIconMode();
            }
        }
    }

    private void SetupIcon()
    {
        if (Icon is not null)
        {
            BindUtils.RelayBind(this, LoadingAnimationProperty, Icon, Icon.LoadingAnimationProperty);
            BindUtils.RelayBind(this, LoadingAnimationDurationProperty, Icon,
                Icon.LoadingAnimationDurationProperty);
            BindUtils.RelayBind(this, IconHeightProperty, Icon, HeightProperty);
            BindUtils.RelayBind(this, IconWidthProperty, Icon, WidthProperty);
        }
    }

    private void SetupIconMode()
    {
        if (Icon is not null)
        {
            if (!PseudoClasses.Contains(StdPseudoClass.Disabled))
            {
                Icon.IconMode = IconMode.Normal;
                if (PseudoClasses.Contains(StdPseudoClass.Pressed))
                {
                    Icon.IconMode = IconMode.Selected;
                }
                else if (PseudoClasses.Contains(StdPseudoClass.PointerOver))
                {
                    Icon.IconMode = IconMode.Active;
                }
            }
            else
            {
                Icon.IconMode = IconMode.Disabled;
            }
        }
    }
    
    public bool HitTest(Point point)
    {
        return true;
    }
}