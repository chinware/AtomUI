using System.Globalization;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls.Primitives;

using AvaloniaScrollViewer = ScrollViewer;

internal class UpDownScrollerViewer : AvaloniaScrollViewer
{
    #region 内部属性定义
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<UpDownScrollerViewer>();
    
    public static readonly StyledProperty<double> ScrollUpButtonOpacityProperty = 
        AvaloniaProperty.Register<UpDownScrollerViewer, double>(nameof(ScrollUpButtonOpacity));
    
    public static readonly StyledProperty<double> ScrollDownButtonOpacityProperty = 
        AvaloniaProperty.Register<UpDownScrollerViewer, double>(nameof(ScrollDownButtonOpacity));
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public double ScrollUpButtonOpacity
    {
        get => GetValue(ScrollUpButtonOpacityProperty);
        set => SetValue(ScrollUpButtonOpacityProperty, value);
    }
    
    public double ScrollDownButtonOpacity
    {
        get => GetValue(ScrollDownButtonOpacityProperty);
        set => SetValue(ScrollDownButtonOpacityProperty, value);
    }
    
    #endregion
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupScrollButtonVisibility();
    }
    
    private void SetupScrollButtonVisibility()
    {
        var args = new List<object?>();
        args.Add(VerticalScrollBarVisibility);
        args.Add(Offset.Y);
        args.Add(Extent.Height);
        args.Add(Viewport.Height);
        var scrollUpVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
        var scrollDownVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
        if (scrollUpVisibility is not null &&
            scrollUpVisibility != AvaloniaProperty.UnsetValue)
        {
            ScrollUpButtonOpacity = (bool)scrollUpVisibility && IsPointerOver ? 1.0 : 0.0;
        }

        if (scrollDownVisibility is not null &&
            scrollDownVisibility != AvaloniaProperty.UnsetValue)
        {
            ScrollDownButtonOpacity = (bool)scrollDownVisibility && IsPointerOver ? 1.0 : 0.0;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == VerticalScrollBarVisibilityProperty ||
            change.Property == OffsetProperty ||
            change.Property == ExtentProperty ||
            change.Property == ViewportProperty ||
            change.Property == IsPointerOverProperty)
        {
            SetupScrollButtonVisibility();
        }
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<DoubleTransition>(ScrollDownButtonOpacityProperty),
                    TransitionUtils.CreateTransition<DoubleTransition>(ScrollUpButtonOpacityProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
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
}