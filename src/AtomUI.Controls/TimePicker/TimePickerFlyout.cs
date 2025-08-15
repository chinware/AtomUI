using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class TimePickerFlyout : Flyout
{
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        TimePicker.IsNeedConfirmProperty.AddOwner<TimePickerFlyout>();
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        TimePicker.IsShowNowProperty.AddOwner<TimePickerFlyout>();
    
    public static readonly StyledProperty<int> MinuteIncrementProperty =
        TimePicker.MinuteIncrementProperty.AddOwner<TimePickerFlyout>();

    public static readonly StyledProperty<int> SecondIncrementProperty =
        TimePicker.SecondIncrementProperty.AddOwner<TimePickerFlyout>();
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<TimePickerFlyout>();

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        TimePicker.SelectedTimeProperty.AddOwner<TimePickerFlyout>();
    
    public bool IsNeedConfirm
    {
        get => GetValue(IsNeedConfirmProperty);
        set => SetValue(IsNeedConfirmProperty, value);
    }
    
    public bool IsShowNow
    {
        get => GetValue(IsShowNowProperty);
        set => SetValue(IsShowNowProperty, value);
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

    public TimeSpan? SelectedTime
    {
        get => GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }
    
    internal TimePickerPresenter? TimePickerPresenter;
    
    protected override Control CreatePresenter()
    {
        TimePickerPresenter = new TimePickerPresenter();
        
        BindUtils.RelayBind(this, IsMotionEnabledProperty, TimePickerPresenter, TimePickerPresenter.IsMotionEnabledProperty);
        BindUtils.RelayBind(this, MinuteIncrementProperty, TimePickerPresenter, TimePickerPresenter.MinuteIncrementProperty);
        BindUtils.RelayBind(this, SecondIncrementProperty, TimePickerPresenter, TimePickerPresenter.SecondIncrementProperty);
        BindUtils.RelayBind(this, ClockIdentifierProperty, TimePickerPresenter, TimePickerPresenter.ClockIdentifierProperty);
        BindUtils.RelayBind(this, SelectedTimeProperty, TimePickerPresenter, TimePickerPresenter.SelectedTimeProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, TimePickerPresenter, TimePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, TimePickerPresenter, TimePickerPresenter.IsShowNowProperty);
        
        var flyoutPresenter = new FlyoutPresenter
        {
            Content = TimePickerPresenter
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, flyoutPresenter, IsShowArrowProperty);
        
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, flyoutPresenter);
        return flyoutPresenter;
    }
}
