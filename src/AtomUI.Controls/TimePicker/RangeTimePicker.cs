using AtomUI.Controls.Internal;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class RangeTimePicker : RangeInfoPickerInput
{
    #region 公共属性定义

    public static readonly StyledProperty<TimeSpan?> RangeStartSelectedTimeProperty =
        AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeStartSelectedTime),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> RangeEndSelectedTimeProperty =
        AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeEndSelectedTime),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> RangeStartDefaultTimeProperty =
        AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeStartDefaultTime),
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> RangeEndDefaultTimeProperty =
        AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeEndDefaultTime),
            enableDataValidation: true);
    
    public static readonly StyledProperty<int> MinuteIncrementProperty =
        AvaloniaProperty.Register<RangeTimePicker, int>(nameof(MinuteIncrement), 1, coerce: CoerceMinuteIncrement);

    public static readonly StyledProperty<int> SecondIncrementProperty =
        AvaloniaProperty.Register<RangeTimePicker, int>(nameof(SecondIncrement), 1, coerce: CoerceSecondIncrement);
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        AvaloniaProperty.Register<RangeTimePicker, ClockIdentifierType>(nameof(ClockIdentifier));

    public TimeSpan? RangeStartSelectedTime
    {
        get => GetValue(RangeStartSelectedTimeProperty);
        set => SetValue(RangeStartSelectedTimeProperty, value);
    }

    public TimeSpan? RangeEndSelectedTime
    {
        get => GetValue(RangeEndSelectedTimeProperty);
        set => SetValue(RangeEndSelectedTimeProperty, value);
    }

    public TimeSpan? RangeStartDefaultTime
    {
        get => GetValue(RangeStartDefaultTimeProperty);
        set => SetValue(RangeStartDefaultTimeProperty, value);
    }

    public TimeSpan? RangeEndDefaultTime
    {
        get => GetValue(RangeEndDefaultTimeProperty);
        set => SetValue(RangeEndDefaultTimeProperty, value);
    }
    
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
    
    #endregion
    
    public override void Clear()
    {
        base.Clear();
        
        RangeStartSelectedTime = RangeStartDefaultTime;
        RangeEndSelectedTime = RangeEndDefaultTime;
    }
    
    protected override Flyout CreatePickerFlyout()
    {
        return new RangeTimePickerFlyout(this);
    }

    protected override void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
        base.NotifyFlyoutAboutToClose(selectedIsValid);
        if (!selectedIsValid)
        {
            if (RangeStartSelectedTime.HasValue)
            {
                Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                ResetRangeStartTimeValue();
            }
            
            if (RangeStartSelectedTime.HasValue)
            {
                Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                ResetRangeEndTimeValue();
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
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RangeActivatedPartProperty)
        {
            HandleRangeActivatedPartChanged();
        }

        if (VisualRoot is not null)
        {
            if (change.Property == RangeStartSelectedTimeProperty)
            {
                if (RangeStartSelectedTime.HasValue)
                {
                    _infoInputBox!.Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12);
                }
                else
                {
                    ResetRangeStartTimeValue();
                }
            }
            else if (change.Property == RangeEndSelectedTimeProperty)
            {
                if (RangeEndSelectedTime.HasValue)
                {
                    _secondaryInfoInputBox!.Text = DateTimeUtils.FormatTimeSpan(RangeEndSelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12);
                }
                else
                {
                    ResetRangeEndTimeValue();
                }
            }
        }
    }
    
    protected void ResetRangeStartTimeValue()
    {
        if (_infoInputBox is not null)
        {
            if (RangeStartDefaultTime is not null)
            {
                _infoInputBox.Text = DateTimeUtils.FormatTimeSpan(RangeStartDefaultTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                _infoInputBox.Clear();
            }
        }
    }
    
    protected void ResetRangeEndTimeValue()
    {
        if (_secondaryInfoInputBox is not null)
        {
            if (RangeEndDefaultTime is not null)
            {
                _secondaryInfoInputBox.Text = DateTimeUtils.FormatTimeSpan(RangeEndDefaultTime.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12);
            }
            else
            {
                _secondaryInfoInputBox.Clear();
            }
        }
    }
    
    protected override void HandleRangeActivatedPartChanged()
    {
        base.HandleRangeActivatedPartChanged();
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            if (RangeEndSelectedTime is null)
            {
                ResetRangeEndTimeValue();
            }
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            if (RangeStartSelectedTime is null)
            {
                ResetRangeStartTimeValue();
            }
        }
        else
        {
            if (RangeStartSelectedTime is null)
            {
                ResetRangeStartTimeValue();
            }
    
            if (RangeEndSelectedTime is null)
            {
                ResetRangeEndTimeValue();
            }
        }
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (RangeStartDefaultTime is not null)
        {
            RangeStartSelectedTime = RangeStartDefaultTime;
        }

        if (RangeEndDefaultTime is not null)
        {
            RangeEndDefaultTime = RangeEndDefaultTime;
        }
    }
    
    protected override bool ShowClearButtonPredicate()
    {
        return RangeStartSelectedTime is not null || RangeEndSelectedTime is not null;
    }
    
    internal void NotifyConfirmed(TimeSpan value)
    {
        _currentValidSelected = true;
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            RangeStartSelectedTime = value;
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            RangeEndSelectedTime = value;
        }
    }

    internal void NotifyTemporaryTimeSelected(TimeSpan value)
    {
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            Text = DateTimeUtils.FormatTimeSpan(value, ClockIdentifier == ClockIdentifierType.HourClock12);
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            SecondaryText = DateTimeUtils.FormatTimeSpan(value, ClockIdentifier == ClockIdentifierType.HourClock12);
        }
    }
}