using System.Reactive.Disposables;
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
    private CompositeDisposable? _flyoutPresenterBindingDisposables;
    private CompositeDisposable? _presenterBindingDisposables;
    protected override Control CreatePresenter()
    {
        _flyoutPresenter = new FlyoutPresenter();
        _flyoutPresenterBindingDisposables?.Dispose();
        _flyoutPresenterBindingDisposables = new CompositeDisposable(1);
        ConfigureDatePickerPresenter(_flyoutPresenter);
        _flyoutPresenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, _flyoutPresenter, IsShowArrowProperty));
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, _flyoutPresenter);
        return _flyoutPresenter;
    }
    
    private void ConfigureDatePickerPresenter(FlyoutPresenter flyoutPresenter)
    {
        _presenterBindingDisposables?.Dispose();
        _presenterBindingDisposables = new CompositeDisposable(7);
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
        
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, RangeDatePickerPresenter.IsMotionEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, SelectedDateTimeProperty, presenter, RangeDatePickerPresenter.SelectedDateTimeProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, RangeDatePickerPresenter.IsNeedConfirmProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowNowProperty, presenter, RangeDatePickerPresenter.IsShowNowProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowTimeProperty, presenter, RangeDatePickerPresenter.IsShowTimeProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ClockIdentifierProperty, presenter, RangeDatePickerPresenter.ClockIdentifierProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, SecondarySelectedDateTimeProperty, presenter, RangeDatePickerPresenter.SecondarySelectedDateTimeProperty));

        flyoutPresenter.Content = presenter;
        DatePickerPresenter      = presenter;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsShowTimeProperty)
        {
            if (_flyoutPresenter != null)
            {
                ConfigureDatePickerPresenter(_flyoutPresenter);
            }
        }
    }
}