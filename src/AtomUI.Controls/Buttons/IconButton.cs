using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<IconButton, Icon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        Icon.LoadingAnimationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        Icon.LoadingAnimationDurationProperty.AddOwner<IconButton>();
    
    public static readonly StyledProperty<IBrush?> NormalIconBrushProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(NormalIconBrush));

    public static readonly StyledProperty<IBrush?> ActiveIconBrushProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(ActiveIconBrush));

    public static readonly StyledProperty<IBrush?> SelectedIconBrushProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(SelectedIconBrush));

    public static readonly StyledProperty<IBrush?> DisabledIconBrushProperty =
        AvaloniaProperty.Register<IconButton, IBrush?>(
            nameof(DisabledIconBrush));

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<IconButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = 
        AvaloniaProperty.Register<IconButton, double>(nameof(IconHeight));
    
    public static readonly StyledProperty<IconMode> IconModeProperty =
        Icon.IconModeProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<bool> IsEnableHoverEffectProperty = 
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsEnableHoverEffect));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconButton>();

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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;

    #endregion
    
    private Border? _frame;

    static IconButton()
    {
        AffectsMeasure<IconButton>(IconProperty);
    }

    public IconButton()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frame = e.NameScope.Find<Border>(IconButtonThemeConstants.FramePart);
        ConfigureTransitions();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_frame != null)
            {
                _frame.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
                };
            }
        }
        else
        {
            if (_frame != null)
            {
                _frame.Transitions?.Clear();
                _frame.Transitions = null;
            }
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}