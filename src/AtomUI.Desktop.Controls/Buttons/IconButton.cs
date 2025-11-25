using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using IconControl = Icon;

public class IconButton : AvaloniaButton,
                          IMotionAwareControl,
                          IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<IconButton, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<IconAnimation> LoadingAnimationProperty =
        IconControl.LoadingAnimationProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<TimeSpan> LoadingAnimationDurationProperty =
        IconControl.LoadingAnimationDurationProperty.AddOwner<IconButton>();
    
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
        IconControl.IconModeProperty.AddOwner<IconButton>();

    public static readonly StyledProperty<bool> IsEnableHoverEffectProperty = 
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsEnableHoverEffect));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconButton>();

    public PathIcon? Icon
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
    
    public IconMode IconMode
    {
        get => GetValue(IconModeProperty);
        set => SetValue(IconModeProperty, value);
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
    
    internal static readonly StyledProperty<bool> IsPassthroughMouseEventProperty = 
        AvaloniaProperty.Register<IconButton, bool>(nameof(IsPassthroughMouseEvent));
    
    public bool IsPassthroughMouseEvent
    {
        get => GetValue(IsPassthroughMouseEventProperty);
        set => SetValue(IsPassthroughMouseEventProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;

    #endregion
    
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaButton))]
    private static readonly Lazy<FieldInfo> IsFlyoutOpenFieldInfo = new Lazy<FieldInfo>(() => 
        typeof(AvaloniaButton).GetFieldInfoOrThrow("_isFlyoutOpen",
            BindingFlags.Instance | BindingFlags.NonPublic));
    #endregion

    static IconButton()
    {
        AffectsMeasure<IconButton>(IconProperty);
        AffectsRender<IconButton>(NormalIconBrushProperty, ActiveIconBrushProperty, DisabledIconBrushProperty, SelectedIconBrushProperty);
    }

    public IconButton()
    {
        this.RegisterResources();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (IsLoaded)
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty)
                ];
                NotifyConfigureTransitions(Transitions);
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected virtual void NotifyConfigureTransitions(Transitions transitions)
    {
    }

    protected bool IsFlyoutOpen()
    {
        return IsFlyoutOpenFieldInfo.Value.GetValue(this) as bool? ?? false;
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = !IsPassthroughMouseEvent;
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = !IsPassthroughMouseEvent;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        e.Handled = !IsPassthroughMouseEvent;
    }
}