using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class MarqueeLabel : TextBlock
{
    #region 公共属性定义

    public static readonly StyledProperty<double> CycleSpaceProperty =
        AvaloniaProperty.Register<MarqueeLabel, double>(nameof(CycleSpace));

    public static readonly StyledProperty<double> MoveSpeedProperty =
        AvaloniaProperty.Register<MarqueeLabel, double>(nameof(MoveSpeed), 150);
    
    /// <summary>
    /// 默认的间隔
    /// </summary>
    public double CycleSpace
    {
        get => GetValue(CycleSpaceProperty);
        set => SetValue(CycleSpaceProperty, value);
    }

    /// <summary>
    /// 移动速度
    /// </summary>
    public double MoveSpeed
    {
        get => GetValue(MoveSpeedProperty);
        set => SetValue(MoveSpeedProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<double> PivotOffsetProperty =
        AvaloniaProperty.Register<MarqueeLabel, double>(nameof(PivotOffset));
    
    /// <summary>
    /// 内部动画使用，当前焦点，也就是文字最左侧
    /// </summary>
    internal double PivotOffset
    {
        get => GetValue(PivotOffsetProperty);
        set => SetValue(PivotOffsetProperty, value);
    }

    #endregion
    
    private ControlStyleState _styleState;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _initialized;
    private Animation? _animation;
    private bool _animationRunning;
    private double _lastDesiredWidth;
    private double _lastTextWidth;
    private double _pivotOffsetStartValue;

    static MarqueeLabel()
    {
        AffectsRender<MarqueeLabel>(PivotOffsetProperty, CycleSpaceProperty, MoveSpeedProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        HandleStartupMarqueeAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HandleCleanupMarqueeAnimation();
        _pivotOffsetStartValue = 0;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_initialized)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            TextWrapping        = TextWrapping.NoWrap;
            CollectStyleState();
            SetupTokenBindings();
            _initialized = true;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        HandleLayoutUpdated(size, availableSize);
        return size;
    }

    private double CalculateDuration(double distance)
    {
        // 计算持续时间，确保至少有一毫秒的持续时间以避免除以零的错误
        return 4 * Math.Max(1, distance / MoveSpeed * 1000);
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
    }

    private void SetupTokenBindings()
    {
        TokenResourceBinder.CreateTokenBinding(this, CycleSpaceProperty, MarqueeLabelTokenResourceKey.CycleSpace);
        TokenResourceBinder.CreateTokenBinding(this, MoveSpeedProperty, MarqueeLabelTokenResourceKey.DefaultSpeed);
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (_initialized)
        {
            if (e.Property == IsPointerOverProperty)
            {
                CollectStyleState();
                if (_styleState.HasFlag(ControlStyleState.MouseOver))
                {
                    _pivotOffsetStartValue = PivotOffset;
                    HandleCleanupMarqueeAnimation();
                }
                else
                {
                    if (DesiredSize.Width < _lastDesiredWidth ||
                        MathUtils.AreClose(DesiredSize.Width, _lastDesiredWidth))
                    {
                        ReConfigureAnimation();
                        HandleStartupMarqueeAnimation();
                    }
                }
                // 这里处理暂停事件
            }
            else if (e.Property == CycleSpaceProperty || e.Property == MoveSpeedProperty)
            {
                var originRunning = _animationRunning;
                ReConfigureAnimation();
                if (originRunning)
                {
                    HandleStartupMarqueeAnimation();
                }
            }
        }
    }

    private void HandleStartupMarqueeAnimation()
    {
        if (_animation is not null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var animationTask = _animation!.RunAsync(this, _cancellationTokenSource.Token);
            _animationRunning = true;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await animationTask;
                _animationRunning = false;
                _animation        = null;
            });
        }
    }

    private void HandleCleanupMarqueeAnimation()
    {
        _cancellationTokenSource?.Cancel();
    }

    private void HandleLayoutUpdated(Size size, Size availableSize)
    {
        if (availableSize.Width > size.Width || MathUtils.AreClose(availableSize.Width, size.Width))
        {
            HandleCleanupMarqueeAnimation();
            PivotOffset            = 0;
            _lastDesiredWidth      = 0;
            _pivotOffsetStartValue = 0;
        }
        else
        {
            if (!MathUtils.AreClose(_lastDesiredWidth, size.Width))
            {
                _lastDesiredWidth = size.Width;
                if (!MathUtils.AreClose(_lastTextWidth, TextLayout.Width))
                {
                    _lastTextWidth = TextLayout.Width;
                }
            }

            if (!_animationRunning)
            {
                ReConfigureAnimation();
                HandleStartupMarqueeAnimation();
            }
        }
    }

    private void ReConfigureAnimation()
    {
        if (_animation is not null && _animationRunning)
        {
            _cancellationTokenSource?.Cancel();
        }

        var cycleWidth = _lastTextWidth + CycleSpace;
        if (_pivotOffsetStartValue < -cycleWidth)
        {
            _pivotOffsetStartValue %= cycleWidth;
        }

        var delta = _pivotOffsetStartValue - cycleWidth - _pivotOffsetStartValue;
        _animation = new Animation
        {
            IterationCount = new IterationCount(long.MaxValue),
            Children =
            {
                new KeyFrame
                {
                    Cue     = new Cue(0.0d),
                    Setters = { new Setter(PivotOffsetProperty, _pivotOffsetStartValue) }
                },
                new KeyFrame
                {
                    Cue     = new Cue(1.0d),
                    Setters = { new Setter(PivotOffsetProperty, _pivotOffsetStartValue - cycleWidth) }
                }
            },
            FillMode = FillMode.Both,
            Duration = TimeSpan.FromMilliseconds(CalculateDuration(_lastTextWidth + CycleSpace))
        };
    }

    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        var offset = TextLayout.OverhangLeading + PivotOffset;
        TextLayout.Draw(context, origin + new Point(offset, 0));
        if (PivotOffset < 0)
        {
            TextLayout.Draw(context, origin + new Point(offset + _lastTextWidth + CycleSpace, 0));
        }
    }
}