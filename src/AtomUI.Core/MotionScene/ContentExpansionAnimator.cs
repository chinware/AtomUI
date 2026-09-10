using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.MotionScene;

internal sealed class ContentExpansionAnimator : IMotionActorLayout
{
    // Store progress on the actor so layout and opacity share one animation clock.
    private static readonly AttachedProperty<double> ContentExpansionProgressProperty =
        AvaloniaProperty.RegisterAttached<ContentExpansionAnimator, BaseMotionActor, double>("ContentExpansionProgress", 1);

    private readonly BaseMotionActor _actor;
    private readonly Action? _preStart;
    private readonly Action? _completed;
    private Execution? _execution;
    private long _version;
    private Size _contentSize;
    private Size _arrangedContentSize;

    static ContentExpansionAnimator()
    {
        ContentExpansionProgressProperty.Changed
            .AddClassHandler<BaseMotionActor>(static (actor, _) => actor.InvalidateMeasure());
    }

    public ContentExpansionAnimator(BaseMotionActor actor, Action? preStart = null, Action? completed = null)
    {
        _actor = actor;
        _preStart = preStart;
        _completed = completed;
    }

    public async Task<bool> RunAsync(bool expanded, Direction direction, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero || !_actor.IsAttachedToVisualTree() || (!expanded && !_actor.IsVisible))
        {
            return ApplyState(expanded);
        }

        var horizontal = direction is Direction.Left or Direction.Right;
        var startExtent = _actor.IsVisible ? GetExtent(_actor.Bounds.Size, horizontal) : 0;
        var startDesiredExtent = _actor.IsVisible ? GetExtent(_actor.DesiredSize.Deflate(_actor.Margin), horizontal) : 0;
        var startOpacity = _actor.IsVisible ? _actor.Opacity : 0;
        var version = ++_version;
        Cancel();
        if (version != _version)
        {
            return false;
        }
        var execution = new Execution(expanded, direction, startExtent, startDesiredExtent);
        _execution = execution;
        _actor.MotionLayout = this;
        try
        {
            _actor.SetValue(ContentExpansionProgressProperty, 0);
            if (!ReferenceEquals(_execution, execution))
                return false;
            _actor.Opacity = startOpacity;
            if (!ReferenceEquals(_execution, execution))
                return false;
            _actor.IsVisible = true;
            if (!ReferenceEquals(_execution, execution))
                return false;
            _actor.InvalidateMeasure();
            _preStart?.Invoke();
            if (!ReferenceEquals(_execution, execution))
            {
                // NotifyMotionPreStart can set Animating after a handler has cancelled this run.
                if (_actor.MotionLayout is null)
                {
                    _actor.Animating = false;
                }
                return false;
            }

            var animation = new Animation
            {
                Duration = duration,
                Easing = new SplineEasing { X1 = 0.645, Y1 = 0.045, X2 = 0.355, Y2 = 1 },
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter(ContentExpansionProgressProperty, 0d),
                            new Setter(Visual.OpacityProperty, startOpacity)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter(ContentExpansionProgressProperty, 1d),
                            new Setter(Visual.OpacityProperty, expanded ? 1d : 0d)
                        }
                    }
                }
            };
            await animation.RunAsync(_actor, execution.Cancellation.Token);
            if (!ReferenceEquals(_execution, execution))
            {
                return false;
            }

            if (!ApplyState(expanded))
            {
                return false;
            }
            _completed?.Invoke();
            return true;
        }
        catch (OperationCanceledException) when (execution.Cancellation.IsCancellationRequested)
        {
            return false;
        }
        finally
        {
            if (ReferenceEquals(_execution, execution))
            {
                ApplyState(expanded);
            }
            execution.Cancellation.Dispose();
        }
    }

    public bool ApplyState(bool expanded)
    {
        var version = ++_version;
        Cancel();
        if (version != _version)
            return false;
        _actor.Opacity = expanded ? 1 : 0;
        if (version != _version)
            return false;
        _actor.IsVisible = expanded;
        if (version != _version)
            return false;
        _actor.ClearValue(ContentExpansionProgressProperty);
        if (version != _version)
            return false;
        _actor.Animating = false;
        _actor.InvalidateMeasure();
        return true;
    }

    private void Cancel()
    {
        var execution = _execution;
        _execution = null;
        _actor.MotionLayout = null;
        execution?.Cancellation.Cancel();
    }

    Size IMotionActorLayout.Measure(Size availableSize)
    {
        var execution = _execution!;
        var content = _actor.MotionTransformRoot;
        content?.Measure(availableSize);
        _contentSize = content?.DesiredSize ?? default;
        var target = execution.Expanded ? GetExtent(_contentSize, execution.Horizontal) : 0;
        var progress = _actor.GetValue(ContentExpansionProgressProperty);

        var presented = GetExtent(_actor.DesiredSize.Deflate(_actor.Margin), execution.Horizontal);
        var extent = execution.DesiredExtent.Interpolate(target, progress, presented);
        return execution.Horizontal ? _contentSize.WithWidth(extent) : _contentSize.WithHeight(extent);
    }

    Rect IMotionActorLayout.ConstrainArrangeRect(Rect rect)
    {
        var execution = _execution!;
        var available = rect.Size.Deflate(_actor.Margin);
        // A fixed-size parent can stretch the actor beyond DesiredSize. Keep that full content
        // arrangement, but constrain the actual actor bounds so clipping also constrains input.
        var fullExtent = Math.Max(GetExtent(_contentSize, execution.Horizontal), GetExtent(available, execution.Horizontal));
        _arrangedContentSize = execution.Horizontal ? available.WithWidth(fullExtent) : available.WithHeight(fullExtent);
        var target = execution.Expanded ? fullExtent : 0;
        var extent = execution.ViewportExtent.Interpolate(target,
            _actor.GetValue(ContentExpansionProgressProperty), GetExtent(_actor.Bounds.Size, execution.Horizontal));
        var margin = execution.Horizontal ? _actor.Margin.Left + _actor.Margin.Right : _actor.Margin.Top + _actor.Margin.Bottom;
        var length = Math.Min(GetExtent(rect.Size, execution.Horizontal), extent + margin);
        var size = execution.Horizontal ? rect.Size.WithWidth(length) : rect.Size.WithHeight(length);
        if (_actor.UseLayoutRounding)
        {
            // Compute the trailing anchor from the same pixel size ArrangeCore will use.
            // Anchoring the fractional extent first can move text on a no-progress reversal.
            size = LayoutHelper.RoundLayoutSizeUp(size, LayoutHelper.GetLayoutScale(_actor));
        }
        var origin = execution.Direction switch
        {
            Direction.Left => new Point(rect.Right - size.Width, rect.Y),
            Direction.Top => new Point(rect.X, rect.Bottom - size.Height),
            _ => rect.Position
        };
        return new Rect(origin, size);
    }

    Size IMotionActorLayout.Arrange(Size finalSize)
    {
        var execution = _execution!;
        var size = execution.Horizontal
            ? finalSize.WithWidth(_arrangedContentSize.Width)
            : finalSize.WithHeight(_arrangedContentSize.Height);
        var origin = execution.Direction switch
        {
            Direction.Left => new Point(finalSize.Width - size.Width, 0),
            Direction.Top => new Point(0, finalSize.Height - size.Height),
            _ => default
        };
        _actor.MotionTransformRoot?.Arrange(new Rect(origin, size));
        return finalSize;
    }

    private static double GetExtent(Size size, bool horizontal) => horizontal ? size.Width : size.Height;

    private sealed class Execution(bool expanded, Direction direction, double startExtent, double startDesiredExtent)
    {
        public bool Expanded { get; } = expanded;
        public Direction Direction { get; } = direction;
        public bool Horizontal { get; } = direction is Direction.Left or Direction.Right;
        public ExtentInterpolation DesiredExtent { get; } = new(startDesiredExtent);
        public ExtentInterpolation ViewportExtent { get; } = new(startExtent);
        public CancellationTokenSource Cancellation { get; } = new();
    }

    private sealed class ExtentInterpolation(double start)
    {
        private double _start = start;
        private double? _target;

        public double Interpolate(double target, double progress, double presented)
        {
            // Rebase a changing content/parent constraint without extending the original deadline.
            if (_target is { } previous && previous != target && progress < 1)
            {
                _start = (presented - target * progress) / (1 - progress);
            }
            _target = target;
            return Math.Max(0, _start + (target - _start) * progress);
        }
    }
}
