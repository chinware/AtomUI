using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class DatePickerFlyout : Flyout
{
    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        DatePicker.SelectedDateTimeProperty.AddOwner<DatePickerFlyout>();
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        DatePicker.IsNeedConfirmProperty.AddOwner<DatePickerFlyout>();
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        DatePicker.IsShowNowProperty.AddOwner<DatePickerFlyout>();
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<DatePickerFlyout>();
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<DatePickerFlyout>();
    
    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }
    
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

    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
    }
    
    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }
    
    internal DatePickerPresenter? DatePickerPresenter { get; set; }
    
    protected override Control CreatePresenter()
    {
        DatePickerPresenter = new DatePickerPresenter();
        
        BindUtils.RelayBind(this, IsMotionEnabledProperty, DatePickerPresenter, DatePickerPresenter.IsMotionEnabledProperty);
        BindUtils.RelayBind(this, SelectedDateTimeProperty, DatePickerPresenter, DatePickerPresenter.SelectedDateTimeProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, DatePickerPresenter, DatePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, DatePickerPresenter, DatePickerPresenter.IsShowNowProperty);
        BindUtils.RelayBind(this, IsShowTimeProperty, DatePickerPresenter, DatePickerPresenter.IsShowTimeProperty);
        BindUtils.RelayBind(this, ClockIdentifierProperty, DatePickerPresenter, DatePickerPresenter.ClockIdentifierProperty);
        
        var flyoutPresenter = new FlyoutPresenter
        {
            Content = DatePickerPresenter
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, flyoutPresenter, IsShowArrowProperty);
        
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, flyoutPresenter);
        return flyoutPresenter;
    }
}