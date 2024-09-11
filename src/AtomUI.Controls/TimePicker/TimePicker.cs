using AtomUI.Controls.Internal;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Data;

namespace AtomUI.Controls;

public enum ClockIdentifierType
{
    HourClock12,
    HourClock24
}

public class TimePicker : InfoPickerInput
{
    #region 公共属性定义
    
    public static readonly StyledProperty<int> MinuteIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(MinuteIncrement), 1, coerce: CoerceMinuteIncrement);

    public static readonly StyledProperty<int> SecondIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(SecondIncrement), 1, coerce: CoerceSecondIncrement);
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        AvaloniaProperty.Register<TimePicker, ClockIdentifierType>(nameof(ClockIdentifier));

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(SelectedTime),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> DefaultTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(DefaultTime),
            enableDataValidation: true);
    
    public int MinuteIncrement
    {
        get => GetValue(MinuteIncrementProperty);
        set => SetValue(MinuteIncrementProperty, value);
    }

    public int SecondIncrement
    {
        get => GetValue(SecondIncrementProperty);
        set => SetValue(SecondIncrementProperty, value);
    }

    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    public TimeSpan? SelectedTime
    {
        get => GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    public TimeSpan? DefaultTime
    {
        get => GetValue(DefaultTimeProperty);
        set => SetValue(DefaultTimeProperty, value);
    }

    #endregion
    
    internal void NotifyTemporaryTimeSelected(TimeSpan value)
    {
        Text = DateTimeUtils.FormatTimeSpan(value, ClockIdentifier == ClockIdentifierType.HourClock12);
    }

    internal void NotifyConfirmed(TimeSpan value)
    {
        _currentValidSelected = true;
        SelectedTime          = value;
    }
    
    protected override Flyout CreatePickerFlyout()
    {
        return new TimePickerFlyout(this);
    }
    
    protected override void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
        if (!selectedIsValid)
        {
            if (SelectedTime.HasValue)
            {
                Text = DateTimeUtils.FormatTimeSpan(SelectedTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                ResetTimeValue();
            }
        }
    }
    
    protected void ResetTimeValue()
    {
        if (DefaultTime is not null)
        {
            Text = DateTimeUtils.FormatTimeSpan(DefaultTime.Value, ClockIdentifier == ClockIdentifierType.HourClock12);
            SelectedTime = DefaultTime;
        }
        else
        {
            Clear();
            SelectedTime = null;
        }
    }
    
    protected override void NotifyClearButtonClicked()
    {
        base.NotifyClearButtonClicked();
        ResetTimeValue();
    }
    
    protected override bool ShowClearButtonPredicate()
    {
        return SelectedTime is not null;
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == SelectedTimeProperty)
            {
                if (SelectedTime.HasValue)
                {
                    Text = DateTimeUtils.FormatTimeSpan(SelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12);
                }
                else
                {
                    ResetTimeValue();
                }
            }
        }
    }
    
    private static int CoerceMinuteIncrement(AvaloniaObject sender, int value)
    {
        if (value < 1 || value > 59)
        {
            throw new ArgumentOutOfRangeException(null, "1 >= MinuteIncrement <= 59");
        }

        return value;
    }

    private static int CoerceSecondIncrement(AvaloniaObject sender, int value)
    {
        if (value < 1 || value > 59)
        {
            throw new ArgumentOutOfRangeException(null, "1 >= SecondIncrement <= 59");
        }

        return value;
    }
}
