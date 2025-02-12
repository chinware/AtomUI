using AtomUI.Controls.Internal;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public enum ClockIdentifierType
{
    HourClock12,
    HourClock24
}

public class TimePicker : InfoPickerInput,
                          IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<TimePicker, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        AvaloniaProperty.Register<TimePicker, bool>(nameof(IsShowNow), true);
    
    public static readonly StyledProperty<int> MinuteIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(MinuteIncrement), 1, coerce: CoerceMinuteIncrement);

    public static readonly StyledProperty<int> SecondIncrementProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(SecondIncrement), 1, coerce: CoerceSecondIncrement);
    
    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        AvaloniaProperty.Register<TimePicker, ClockIdentifierType>(nameof(ClockIdentifier), ClockIdentifierType.HourClock12);

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(SelectedTime),
            enableDataValidation: true);

    public static readonly StyledProperty<TimeSpan?> DefaultTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(DefaultTime),
            enableDataValidation: true);
    
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

    public TimeSpan? DefaultTime
    {
        get => GetValue(DefaultTimeProperty);
        set => SetValue(DefaultTimeProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    
    string IControlSharedTokenResourcesHost.TokenId => TimePickerToken.ID;

    #endregion
    
    private TimePickerPresenter? _pickerPresenter;

    public TimePicker()
    {
        this.RegisterResources();
    }
    
    protected override Flyout CreatePickerFlyout()
    {
        return new TimePickerFlyout();
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (flyoutPresenter is TimePickerFlyoutPresenter timePickerFlyoutPresenter)
        {
            timePickerFlyoutPresenter.AttachedToVisualTree += (sender, args) =>
            {
                _pickerPresenter = timePickerFlyoutPresenter.TimePickerPresenter;
                ConfigurePickerPresenter(_pickerPresenter);
            };
        }
    }
    
    private void ConfigurePickerPresenter(TimePickerPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, TimePickerPresenter.IsMotionEnabledProperty);
        BindUtils.RelayBind(this, MinuteIncrementProperty, presenter, TimePickerPresenter.MinuteIncrementProperty);
        BindUtils.RelayBind(this, SecondIncrementProperty, presenter, TimePickerPresenter.SecondIncrementProperty);
        BindUtils.RelayBind(this, ClockIdentifierProperty, presenter, TimePickerPresenter.ClockIdentifierProperty);
        BindUtils.RelayBind(this, SelectedTimeProperty, presenter, TimePickerPresenter.SelectedTimeProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, TimePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, presenter, TimePickerPresenter.IsShowNowProperty);
    }
    
    protected override void NotifyFlyoutOpened()
    {
        base.NotifyFlyoutOpened();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged += HandleChoosingStatueChanged;
            _pickerPresenter.HoverTimeChanged  += HandleHoverTimeChanged;
            _pickerPresenter.Confirmed             += HandleConfirmed;
        }
    }
    
    protected override void NotifyFlyoutAboutToClose(bool selectedIsValid)
    {
        base.NotifyFlyoutAboutToClose(selectedIsValid);
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged -= HandleChoosingStatueChanged;
            _pickerPresenter.HoverTimeChanged      -= HandleHoverTimeChanged;
            _pickerPresenter.Confirmed             -= HandleConfirmed;
        }
    }
    
    private void HandleChoosingStatueChanged(object? sender, ChoosingStatusEventArgs args)
    {
        _isChoosing = args.IsChoosing;
        UpdatePseudoClasses();
        if (!args.IsChoosing)
        {
            ClearHoverSelectedInfo();
        }
    }
    
    private void ClearHoverSelectedInfo()
    {
        Text = DateTimeUtils.FormatTimeSpan(SelectedTime,
            ClockIdentifier == ClockIdentifierType.HourClock12);
    }
    
    private void HandleHoverTimeChanged(object? sender, TimeSelectedEventArgs args)
    {
        if (args.Time.HasValue)
        {
            Text = DateTimeUtils.FormatTimeSpan(args.Time.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12);
        }
        else
        {
            Text = null;
        }
    }
    
    private void HandleConfirmed(object? sender, EventArgs args)
    {
        SelectedTime = _pickerPresenter?.SelectedTime;
        ClosePickerFlyout();
    }

    /// <summary>
    /// 清除时间选择器的值，不考虑默认值
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        SelectedTime = null;
    }

    /// <summary>
    /// 重置时间选择器的值，当有默认值设置的时候，会将当前的值设置成默认值
    /// </summary>
    public void Reset()
    {
        SelectedTime = DefaultTime;
    }
    
    protected override bool ShowClearButtonPredicate()
    {
        return SelectedTime is not null;
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedTimeProperty)
        {
            Text = DateTimeUtils.FormatTimeSpan(SelectedTime,
                ClockIdentifier == ClockIdentifierType.HourClock12);
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
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (DefaultTime is not null && SelectedTime is null)
        {
            SelectedTime = DefaultTime;
        }
    }
    
}
