using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class CaptionButton : AvaloniaButton
{
    #region 公共属性定义

    public static readonly StyledProperty<PathIcon?> NormalIconProperty =
        AvaloniaProperty.Register<CaptionButton, PathIcon?>(nameof(NormalIcon));
    
    public static readonly StyledProperty<PathIcon?> CheckedIconProperty =
        AvaloniaProperty.Register<CaptionButton, PathIcon?>(nameof(CheckedIcon));
    
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = 
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconHeight));
    
    public static readonly StyledProperty<bool> IsCheckedProperty = 
        AvaloniaProperty.Register<CaptionButton, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay, defaultValue:false);
    
    public PathIcon? NormalIcon
    {
        get => GetValue(NormalIconProperty);
        set => SetValue(NormalIconProperty, value);
    }
    
    public PathIcon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
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
    
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CaptionButton>();
    
    internal static readonly StyledProperty<bool> IsWindowActiveProperty = 
        WindowTitleBar.IsWindowActiveProperty.AddOwner<CaptionButton>();
    
    internal static readonly DirectProperty<CaptionButton, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<CaptionButton, CornerRadius>(
            nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }

    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    #endregion

    static CaptionButton()
    {
        AffectsRender<CaptionButton>(EffectiveCornerRadiusProperty);
    }
    
    public CaptionButton()
    {
        this.RegisterTokenResourceScope(WindowTitleBarToken.ScopeProvider);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var size = Math.Max(e.NewSize.Height, e.NewSize.Width);
        EffectiveCornerRadius = new CornerRadius(size / 2);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        e.Handled = true;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        var minSize = Math.Min(size.Width, size.Height);
        return new Size(minSize, minSize);
    }
}
