using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class RangeDatePickerFlyout : DatePickerFlyout
{
    public static readonly StyledProperty<DateTime?> SecondarySelectedDateTimeProperty =
        RangeDatePickerPresenter.SecondarySelectedDateTimeProperty.AddOwner<RangeDatePickerFlyout>();
    
    public DateTime? SecondarySelectedDateTime
    {
        get => GetValue(SecondarySelectedDateTimeProperty);
        set => SetValue(SecondarySelectedDateTimeProperty, value);
    }
    
    internal FlyoutPresenter? _flyoutPresenter;
    
    protected override Control CreatePresenter()
    {
        _flyoutPresenter = new FlyoutPresenter();
        ConfigureDatePickerPresenter();
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, _flyoutPresenter, IsShowArrowProperty);
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, _flyoutPresenter);
        return _flyoutPresenter;
    }
    
    private void ConfigureDatePickerPresenter()
    {
        if (_flyoutPresenter != null)
        {
            DatePickerPresenter? presenter;
            if (IsShowTime)
            {
                presenter = new TimedRangeDatePickerPresenter()
                {
                    IsShowTime = true
                };
            }
            else
            {
                presenter = new DualMonthRangeDatePickerPresenter();
            }
        
            BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, RangeDatePickerPresenter.IsMotionEnabledProperty);
            BindUtils.RelayBind(this, SelectedDateTimeProperty, presenter, RangeDatePickerPresenter.SelectedDateTimeProperty);
            BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, RangeDatePickerPresenter.IsNeedConfirmProperty);
            BindUtils.RelayBind(this, IsShowNowProperty, presenter, RangeDatePickerPresenter.IsShowNowProperty);
            BindUtils.RelayBind(this, IsShowTimeProperty, presenter, RangeDatePickerPresenter.IsShowTimeProperty);
            BindUtils.RelayBind(this, ClockIdentifierProperty, presenter, RangeDatePickerPresenter.ClockIdentifierProperty);
            BindUtils.RelayBind(this, SecondarySelectedDateTimeProperty, presenter, RangeDatePickerPresenter.SecondarySelectedDateTimeProperty);

            _flyoutPresenter.Content = presenter;
            DatePickerPresenter      = presenter;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsShowTimeProperty)
        {
            ConfigureDatePickerPresenter();
        }
    }
}