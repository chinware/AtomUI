using Avalonia;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public class TimerStatistic : AbstractStatistic
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<AbstractStatistic, DateTime>(nameof(Value));
    
    public static readonly StyledProperty<string?> FormatProperty =
        AvaloniaProperty.Register<AbstractStatistic, string?>(nameof(Format));
    
    public static readonly StyledProperty<TimeSpan> RefreshDurationProperty =
        AvaloniaProperty.Register<AbstractStatistic, TimeSpan>(nameof(RefreshDuration), TimeSpan.FromMilliseconds(10));
    
    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public TimeSpan RefreshDuration
    {
        get => GetValue(RefreshDurationProperty);
        set => SetValue(RefreshDurationProperty, value);
    }


    public Func<TimeSpan, string>? Formatter;
    #endregion

    #region 公共事件定义

    public event EventHandler? CountdownFinished;

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TimerStatistic, TimeSpan> RemainingTimeProperty =
        AvaloniaProperty.RegisterDirect<TimerStatistic, TimeSpan>(nameof(RemainingTime),
            o => o.RemainingTime,
            (o, v) => o.RemainingTime = v);
    
    internal static readonly DirectProperty<TimerStatistic, string?> RemainingTimeTextProperty =
        AvaloniaProperty.RegisterDirect<TimerStatistic, string?>(nameof(RemainingTimeText),
            o => o.RemainingTimeText,
            (o, v) => o.RemainingTimeText = v);
    
    private TimeSpan _remainingTime;

    internal TimeSpan RemainingTime
    {
        get => _remainingTime;
        set => SetAndRaise(RemainingTimeProperty, ref _remainingTime, value);
    }
    
    private string? _remainingTimeText;

    internal string? RemainingTimeText
    {
        get => _remainingTimeText;
        set => SetAndRaise(RemainingTimeTextProperty, ref _remainingTimeText, value);
    }

    #endregion
    
    private DispatcherTimer? _timer;

    static TimerStatistic()
    {
        AffectsMeasure<TimerStatistic>(ValueProperty,  FormatProperty, RemainingTimeTextProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer?.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= HandleTickElapsed;
            _timer = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RemainingTimeProperty ||
            change.Property == FormatProperty)
        {
            GenerateRemainingTimeText();
        }

        if (change.Property == ValueProperty ||
            change.Property == RefreshDurationProperty)
        {
            BuildTimer(true);
        }
    }

    private void GenerateRemainingTimeText()
    {
        string? formattedText;
        if (Formatter != null)
        {
            formattedText = Formatter(RemainingTime);
        }
        else
        {
            if (Format != null)
            {
                formattedText = RemainingTime.ToString(Format);
            }
            else if (RemainingTime.TotalHours >= 1)
            {
                formattedText = $"{(int)RemainingTime.TotalHours:00}:{RemainingTime.Minutes:00}:{RemainingTime.Seconds:00}";
            }
            else
            {
                formattedText = $"{(int)RemainingTime.TotalMinutes:00}:{RemainingTime.Seconds:00}";
            }
        }
        SetCurrentValue(RemainingTimeTextProperty, formattedText);
    }
    
    private void BuildTimer(bool start)
    {
        // 先清理旧定时器
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= HandleTickElapsed;
        }
        
        _timer          =  new DispatcherTimer();
        _timer.Interval =  RefreshDuration;
        _timer.Tick     += HandleTickElapsed;
        if (start)
        {
            _timer.Start();
        }
    }
    
    private void HandleTickElapsed(object? sender, EventArgs args)
    {
        if (Value > DateTime.Now)
        {
            RemainingTime = Value - DateTime.Now;
        }
        else
        {
            RemainingTime = DateTime.Now - Value;
        }
        if (RemainingTime <= TimeSpan.Zero)
        {
            RemainingTime = TimeSpan.Zero;
            _timer?.Stop();
            CountdownFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}