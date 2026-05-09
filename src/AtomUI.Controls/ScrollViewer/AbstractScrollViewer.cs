using AtomUI.Animations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls.Commons;

using AvaloniaScrollViewer = Avalonia.Controls.ScrollViewer;

public abstract class AbstractScrollViewer : AvaloniaScrollViewer, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractScrollViewer>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ScrollBarsSeparatorOpacityProperty =
        AvaloniaProperty.Register<AbstractScrollViewer, double>(nameof(ScrollBarsSeparatorOpacity));

    internal static readonly StyledProperty<double> ScrollBarOpacityProperty =
        AvaloniaProperty.Register<AbstractScrollViewer, double>(nameof(ScrollBarOpacity));

    internal double ScrollBarsSeparatorOpacity
    {
        get => GetValue(ScrollBarsSeparatorOpacityProperty);
        set => SetValue(ScrollBarsSeparatorOpacityProperty, value);
    }

    internal double ScrollBarOpacity
    {
        get => GetValue(ScrollBarOpacityProperty);
        set => SetValue(ScrollBarOpacityProperty, value);
    }

    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.Dispatcher.Post(this.EnableTransitions);
    }
}
