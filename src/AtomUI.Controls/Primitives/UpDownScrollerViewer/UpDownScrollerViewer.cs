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
    
    public static readonly StyledProperty<bool> ScrollUpButtonVisibleProperty = 
        AvaloniaProperty.Register<UpDownScrollerViewer, bool>(nameof(ScrollUpButtonVisible));
    
    public static readonly StyledProperty<bool> ScrollDownButtonVisibleProperty = 
        AvaloniaProperty.Register<UpDownScrollerViewer, bool>(nameof(ScrollDownButtonVisible));
    
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
    
    public bool ScrollUpButtonVisible
    {
        get => GetValue(ScrollUpButtonVisibleProperty);
        set => SetValue(ScrollUpButtonVisibleProperty, value);
    }
    
    public bool ScrollDownButtonVisible
    {
        get => GetValue(ScrollDownButtonVisibleProperty);
        set => SetValue(ScrollDownButtonVisibleProperty, value);
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
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture) ?? false;
        var scrollDownVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture) ?? false;
        if (scrollUpVisibility != AvaloniaProperty.UnsetValue)
        {
            var scrollUpButtonVisible = (bool)scrollUpVisibility && IsPointerOver;
            ScrollUpButtonOpacity = scrollUpButtonVisible ? 1.0 : 0.0;
            ScrollUpButtonVisible = scrollUpButtonVisible;
        }

        if (scrollDownVisibility != AvaloniaProperty.UnsetValue)
        {
            var scrollDownButtonVisible   = (bool)scrollDownVisibility && IsPointerOver;
            ScrollDownButtonOpacity = scrollDownButtonVisible ? 1.0 : 0.0;
            ScrollDownButtonVisible = scrollDownButtonVisible;
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