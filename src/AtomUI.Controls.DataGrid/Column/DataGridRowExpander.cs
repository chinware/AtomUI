using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class DataGridRowExpander : ToggleButton,
                                     IResourceBindingManager,
                                     ICustomHitTest
{
    internal static readonly DirectProperty<DataGridRowExpander, double> IndicatorThicknessProperty =
        AvaloniaProperty.RegisterDirect<DataGridRowExpander, double>(
            nameof(IndicatorThickness),
            o => o.IndicatorThickness,
            (o, v) => o.IndicatorThickness = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridRowExpander>();

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

    private CompositeDisposable? _resourceBindingsDisposable;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;

    private Rectangle? _horizontalIndicator;
    private Rectangle? _verticalIndicator;
    private Border? _frame;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size  = base.ArrangeOverride(finalSize);
        var delta = IndicatorThickness / 2;
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

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _verticalIndicator   = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.VerticalIndicatorPart);
        _horizontalIndicator = e.NameScope.Find<Rectangle>(DataGridRowExpanderThemeConstants.HorizontalIndicatorPart);
        _frame               = e.NameScope.Find<Border>(DataGridRowExpanderThemeConstants.FramePart);
        ConfigureTransitions();
    }

    public bool HitTest(Point point)
    {
        return true;
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_frame != null)
            {
                _frame.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty)
                };
            }

            if (_verticalIndicator != null)
            {
                _verticalIndicator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Rectangle.FillProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
                };
            }

            if (_horizontalIndicator != null)
            {
                _horizontalIndicator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Rectangle.FillProperty),
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(Visual.RenderTransformProperty)
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
            if (_verticalIndicator != null)
            {
                _verticalIndicator.Transitions?.Clear();
                _verticalIndicator.Transitions = null;
            }
            if (_horizontalIndicator != null)
            {
                _horizontalIndicator.Transitions?.Clear();
                _horizontalIndicator.Transitions = null;
            }
        }
    }
}