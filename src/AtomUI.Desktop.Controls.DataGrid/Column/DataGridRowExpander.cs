using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Data;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class DataGridRowExpander : ToggleButton
{
    internal static readonly DirectProperty<DataGridRowExpander, double> IndicatorThicknessProperty =
        AvaloniaProperty.RegisterDirect<DataGridRowExpander, double>(
            nameof(IndicatorThickness),
            o => o.IndicatorThickness,
            (o, v) => o.IndicatorThickness = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridRowExpander>();
    
    internal static readonly StyledProperty<ITransform?> VerticalIndicatorRenderTransformProperty = 
        AvaloniaProperty.Register<DataGridRowExpander, ITransform?>(nameof (VerticalIndicatorRenderTransform));
    
    internal static readonly StyledProperty<ITransform?> HorizontalIndicatorRenderTransformProperty = 
        AvaloniaProperty.Register<DataGridRowExpander, ITransform?>(nameof (HorizontalIndicatorRenderTransform));
    
    internal static readonly StyledProperty<IBrush?> IndicatorFillProperty = 
        AvaloniaProperty.Register<DataGridRowExpander, IBrush?>(nameof (IndicatorFill));

    internal double IndicatorThickness
    {
        get => _indicatorThickness;
        set => SetAndRaise(IndicatorThicknessProperty, ref _indicatorThickness, value);
    }

    private double _indicatorThickness;

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal ITransform? VerticalIndicatorRenderTransform
    {
        get => GetValue(VerticalIndicatorRenderTransformProperty);
        set => SetValue(VerticalIndicatorRenderTransformProperty, value);
    }
    
    internal ITransform? HorizontalIndicatorRenderTransform
    {
        get => GetValue(HorizontalIndicatorRenderTransformProperty);
        set => SetValue(HorizontalIndicatorRenderTransformProperty, value);
    }

    internal IBrush? IndicatorFill
    {
        get => GetValue(IndicatorFillProperty);
        set => SetValue(IndicatorFillProperty, value);
    }

    private Rectangle? _horizontalIndicator;
    private Rectangle? _verticalIndicator;
    private IDisposable? _disposable;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size  = base.ArrangeOverride(finalSize);
        var delta = IndicatorThickness;
        if (_verticalIndicator != null)
        {
            var indicatorSize = _verticalIndicator.DesiredSize;
            var x             = (finalSize.Width - indicatorSize.Width) / 2 - delta;
            var y             = (finalSize.Height - indicatorSize.Height) / 2 - delta;
            Canvas.SetLeft(_verticalIndicator, x);
            Canvas.SetTop(_verticalIndicator, y);
        }

        if (_horizontalIndicator != null)
        {
            var indicatorSize = _horizontalIndicator.DesiredSize;
            var x             = (finalSize.Width - indicatorSize.Width) / 2 - delta;
            var y             = (finalSize.Height - indicatorSize.Height) / 2 - delta;
            Canvas.SetLeft(_horizontalIndicator, x);
            Canvas.SetTop(_horizontalIndicator, y);
        }

        return size;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty)
        {
            IndicatorThickness = BorderThickness.Left;
        }

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _verticalIndicator   = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.VerticalIndicatorPart);
        _horizontalIndicator = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.HorizontalIndicatorPart);
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(HorizontalIndicatorRenderTransformProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(HorizontalIndicatorRenderTransformProperty)
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

    internal void NotifyLoadingRow(DataGridRow row)
    {
        _disposable = BindUtils.RelayBind(this, IsCheckedProperty, row, DataGridRow.IsDetailsVisibleProperty, BindingMode.TwoWay);
    }
    
    internal void NotifyUnLoadingRow(DataGridRow row)
    {
        _disposable?.Dispose();
    }
}