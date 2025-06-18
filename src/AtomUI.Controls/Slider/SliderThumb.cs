using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed)]
public class SliderThumb : TemplatedControl
{
    #region 公共属性定义

    public static readonly RoutedEvent<VectorEventArgs> DragStartedEvent =
        RoutedEvent.Register<SliderThumb, VectorEventArgs>(nameof(DragStarted), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<VectorEventArgs> DragDeltaEvent =
        RoutedEvent.Register<SliderThumb, VectorEventArgs>(nameof(DragDelta), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<VectorEventArgs> DragCompletedEvent =
        RoutedEvent.Register<SliderThumb, VectorEventArgs>(nameof(DragCompleted), RoutingStrategies.Bubble);

    public static readonly StyledProperty<IBrush?> OutlineBrushProperty =
        AvaloniaProperty.Register<SliderThumb, IBrush?>(nameof(OutlineBrush));

    public static readonly StyledProperty<Thickness> OutlineThicknessProperty =
        AvaloniaProperty.Register<SliderThumb, Thickness>(nameof(OutlineThickness));

    public static readonly StyledProperty<double> ThumbCircleSizeProperty =
        AvaloniaProperty.Register<SliderThumb, double>(nameof(ThumbCircleSize));

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SliderThumb>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    private Point? _lastPoint;

    static SliderThumb()
    {
        DragStartedEvent.AddClassHandler<SliderThumb>((x, e) => x.OnDragStarted(e), RoutingStrategies.Bubble);
        DragDeltaEvent.AddClassHandler<SliderThumb>((x, e) => x.OnDragDelta(e), RoutingStrategies.Bubble);
        DragCompletedEvent.AddClassHandler<SliderThumb>((x, e) => x.OnDragCompleted(e), RoutingStrategies.Bubble);
        AffectsRender<SliderThumb>(ThumbCircleSizeProperty,
            OutlineThicknessProperty,
            BorderThicknessProperty,
            OutlineBrushProperty,
            BorderBrushProperty);
    }

    public event EventHandler<VectorEventArgs>? DragStarted
    {
        add => AddHandler(DragStartedEvent, value);
        remove => RemoveHandler(DragStartedEvent, value);
    }

    public event EventHandler<VectorEventArgs>? DragDelta
    {
        add => AddHandler(DragDeltaEvent, value);
        remove => RemoveHandler(DragDeltaEvent, value);
    }

    public event EventHandler<VectorEventArgs>? DragCompleted
    {
        add => AddHandler(DragCompletedEvent, value);
        remove => RemoveHandler(DragCompletedEvent, value);
    }

    internal IBrush? OutlineBrush
    {
        get => GetValue(OutlineBrushProperty);
        set => SetValue(OutlineBrushProperty, value);
    }

    internal Thickness OutlineThickness
    {
        get => GetValue(OutlineThicknessProperty);
        set => SetValue(OutlineThicknessProperty, value);
    }

    internal double ThumbCircleSize
    {
        get => GetValue(ThumbCircleSizeProperty);
        set => SetValue(ThumbCircleSizeProperty, value);
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions()
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(OutlineBrushProperty),
                TransitionUtils.CreateTransition<ThicknessTransition>(OutlineThicknessProperty,
                    SharedTokenKey.MotionDurationFast),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    internal void AdjustDrag(Vector v)
    {
        if (_lastPoint.HasValue)
        {
            _lastPoint = _lastPoint.Value + v;
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SliderThumbAutomationPeer(this);
    }

    protected virtual void OnDragStarted(VectorEventArgs e)
    {
    }

    protected virtual void OnDragDelta(VectorEventArgs e)
    {
    }

    protected virtual void OnDragCompleted(VectorEventArgs e)
    {
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            var ev = new VectorEventArgs
            {
                RoutedEvent = DragCompletedEvent,
                Vector      = _lastPoint.Value
            };

            _lastPoint = null;

            RaiseEvent(ev);
        }

        PseudoClasses.Remove(StdPseudoClass.Pressed);

        base.OnPointerCaptureLost(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            var ev = new VectorEventArgs
            {
                RoutedEvent = DragDeltaEvent,
                Vector      = e.GetPosition(this) - _lastPoint.Value
            };

            RaiseEvent(ev);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        e.Handled  = true;
        _lastPoint = e.GetPosition(this);

        var ev = new VectorEventArgs
        {
            RoutedEvent = DragStartedEvent,
            Vector      = (Vector)_lastPoint
        };

        PseudoClasses.Add(StdPseudoClass.Pressed);

        e.PreventGestureRecognition();

        RaiseEvent(ev);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (_lastPoint.HasValue)
        {
            e.Handled  = true;
            _lastPoint = null;

            var ev = new VectorEventArgs
            {
                RoutedEvent = DragCompletedEvent,
                Vector      = e.GetPosition(this)
            };

            RaiseEvent(ev);
        }

        PseudoClasses.Remove(StdPseudoClass.Pressed);
    }

    public override void Render(DrawingContext context)
    {
        // 绘制圆
        var centerPos         = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var thumbCircleRadius = ThumbCircleSize / 2 + BorderThickness.Left / 2;
        var circlePen         = new Pen(BorderBrush, BorderThickness.Left);
        context.DrawEllipse(Background, circlePen, centerPos, thumbCircleRadius, thumbCircleRadius);
        // 绘制 outline
        var outlinePen       = new Pen(OutlineBrush, OutlineThickness.Left);
        var outlinePenRadius = ThumbCircleSize / 2 + BorderThickness.Left / 2 + OutlineThickness.Left / 2;
        context.DrawEllipse(null, outlinePen, centerPos, outlinePenRadius, outlinePenRadius);
    }
}