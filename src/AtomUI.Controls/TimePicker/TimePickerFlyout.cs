using System.Reactive.Disposables;
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
    private CompositeDisposable? _presenterBindingDisposables;
    
    protected override Control CreatePresenter()
    {
        TimePickerPresenter = new TimePickerPresenter();
        _presenterBindingDisposables?.Dispose();
        _presenterBindingDisposables = new CompositeDisposable(8);
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, TimePickerPresenter, TimePickerPresenter.IsMotionEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, MinuteIncrementProperty, TimePickerPresenter, TimePickerPresenter.MinuteIncrementProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, SecondIncrementProperty, TimePickerPresenter, TimePickerPresenter.SecondIncrementProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ClockIdentifierProperty, TimePickerPresenter, TimePickerPresenter.ClockIdentifierProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, SelectedTimeProperty, TimePickerPresenter, TimePickerPresenter.SelectedTimeProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsNeedConfirmProperty, TimePickerPresenter, TimePickerPresenter.IsNeedConfirmProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowNowProperty, TimePickerPresenter, TimePickerPresenter.IsShowNowProperty));
        
        var flyoutPresenter = new FlyoutPresenter
        {
            Content = TimePickerPresenter
        };
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, flyoutPresenter, IsShowArrowProperty));
        
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, flyoutPresenter);
        return flyoutPresenter;
    }
}
