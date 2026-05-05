using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Primitives;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Icons.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

public enum ClockIdentifierType
{
    HourClock12,
    HourClock24
}

public class TimePicker : InfoPickerInput
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
        AvaloniaProperty.Register<TimePicker, ClockIdentifierType>(nameof(ClockIdentifier));

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

    internal static readonly DirectProperty<TimePicker, string?> AmTextProperty =
        AvaloniaProperty.RegisterDirect<TimePicker, string?>(nameof(AmText),
            o => o.AmText,
            (o, v) => o.AmText = v);
    
    internal static readonly DirectProperty<TimePicker, string?> PmTextProperty =
        AvaloniaProperty.RegisterDirect<TimePicker, string?>(nameof(PmText),
            o => o.PmText,
            (o, v) => o.PmText = v);

    private string? _amText;

    internal string? AmText
    {
        get => _amText;
        set => SetAndRaise(AmTextProperty, ref _amText, value);
    }
    
    private string? _pmText;

    internal string? PmText
    {
        get => _pmText;
        set => SetAndRaise(AmTextProperty, ref _pmText, value);
    }
    
    #endregion
    
    private TimePickerPresenter? _pickerPresenter;
    private CompositeDisposable? _flyoutBindingDisposables;
    
    public TimePicker()
    {
        this.RegisterTokenResourceScope(TimePickerToken.ScopeProvider);
    }
    
    static TimePicker()
    {
        SelectedTimeProperty.Changed.AddClassHandler<TimePicker>((timePicker, args) => timePicker.NotifyFormValueChanged(args.NewValue));
    }

    protected override Flyout CreatePickerFlyout()
    {
        var timePickerFlyout = new TimePickerFlyout();
        _flyoutBindingDisposables?.Dispose();
        _flyoutBindingDisposables = new CompositeDisposable(7);
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, timePickerFlyout, TimePickerFlyout.IsMotionEnabledProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, MinuteIncrementProperty, timePickerFlyout, TimePickerFlyout.MinuteIncrementProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, SecondIncrementProperty, timePickerFlyout, TimePickerFlyout.SecondIncrementProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, ClockIdentifierProperty, timePickerFlyout, TimePickerFlyout.ClockIdentifierProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, SelectedTimeProperty, timePickerFlyout, TimePickerFlyout.SelectedTimeProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsNeedConfirmProperty, timePickerFlyout, TimePickerFlyout.IsNeedConfirmProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsShowNowProperty, timePickerFlyout, TimePickerFlyout.IsShowNowProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, ShouldUseOverlayPopupProperty, timePickerFlyout, TimePickerFlyout.ShouldUseOverlayPopupProperty));
        
        return timePickerFlyout;
    }

    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (PickerFlyout is TimePickerFlyout timePickerFlyout)
        {
            _pickerPresenter = timePickerFlyout.TimePickerPresenter;
        }
    }

    protected override void NotifyFlyoutOpened()
    {
        base.NotifyFlyoutOpened();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatueChanged += HandleChoosingStatueChanged;
            _pickerPresenter.HoverTimeChanged      += HandleHoverTimeChanged;
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
            if (!IsNeedConfirm)
            {
                SelectedTime = _pickerPresenter?.SelectedTime;
            }
        }
    }

    private void HandleChoosingStatueChanged(object? sender, ChoosingStatusEventArgs args)
    {
        IsChoosing = args.IsChoosing;
        UpdatePseudoClasses();
        if (!args.IsChoosing)
        {
            ClearHoverSelectedInfo();
        }
    }

    private void ClearHoverSelectedInfo()
    {
        Text = DateTimeUtils.FormatTimeSpan(SelectedTime,
            ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
    }

    private void HandleHoverTimeChanged(object? sender, TimeSelectedEventArgs args)
    {
        if (args.Time.HasValue)
        {
            Text = DateTimeUtils.FormatTimeSpan(args.Time.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
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
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
        }
        else if (change.Property == FontSizeProperty ||
                 change.Property == FontFamilyProperty ||
                 change.Property == FontFamilyProperty ||
                 change.Property == FontStyleProperty ||
                 change.Property == ClockIdentifierProperty ||
                 change.Property == MinWidthProperty ||
                 change.Property == WidthProperty ||
                 change.Property == MaxWidthProperty)
        {
            CalculatePreferredWidth();
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

    private void CalculatePreferredWidth()
    {
        if (!double.IsNaN(Width) || HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            PreferredInputWidth = double.NaN;
        }
        else
        {
            var text = DateTimeUtils.FormatTimeSpan(TimeSpan.Zero,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
            var preferredInputWidth = TextUtils.CalculateTextSize(text, FontSize, FontFamily, FontStyle, FontWeight).Width;
            if (PlaceholderText != null)
            {
                preferredInputWidth = Math.Max(preferredInputWidth, TextUtils.CalculateTextSize(PlaceholderText, FontSize, FontFamily, FontStyle, FontWeight).Width);
            }

            preferredInputWidth *= 1.1;
            if (!double.IsNaN(MinWidth))
            {
                preferredInputWidth = Math.Max(MinWidth, preferredInputWidth);
            }

            if (!double.IsNaN(MaxWidth))
            {
                preferredInputWidth = Math.Min(MaxWidth, preferredInputWidth);
            }
            PreferredInputWidth = preferredInputWidth;
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (DefaultTime is not null && SelectedTime is null)
        {
            SelectedTime = DefaultTime;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (InfoIcon is null)
        {
            SetValue(InfoIconProperty, new ClockCircleOutlined(), BindingPriority.Template);
        }
    }
    
    #region 实现 FormItem 接口
    protected override void NotifySetFormValue(object? value)
    {
        SelectedTime = value as TimeSpan?;
    }

    protected override object? NotifyGetFormValue()
    {
        return SelectedTime;
    }

    protected override void NotifyClearFormValue()
    {
        SelectedTime = null;
    }
    #endregion
}