using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls.Primitives;
using AtomUI.Icons.AntDesign;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

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
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<RangeTimePicker, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        AvaloniaProperty.Register<RangeTimePicker, bool>(nameof(IsShowNow), true);

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
    
    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<RangeTimePicker, double> PreferredWidthProperty =
        AvaloniaProperty.RegisterDirect<RangeTimePicker, double>(nameof(PreferredWidth),
            o => o.PreferredWidth,
            (o, v) => o.PreferredWidth = v);
    
    internal static readonly DirectProperty<RangeTimePicker, string?> AmTextProperty =
        AvaloniaProperty.RegisterDirect<RangeTimePicker, string?>(nameof(AmText),
            o => o.AmText,
            (o, v) => o.AmText = v);
    
    internal static readonly DirectProperty<RangeTimePicker, string?> PmTextProperty =
        AvaloniaProperty.RegisterDirect<RangeTimePicker, string?>(nameof(PmText),
            o => o.PmText,
            (o, v) => o.PmText = v);

    private double _preferredWidth;

    internal double PreferredWidth
    {
        get => _preferredWidth;
        set
        {
            if (MathUtils.AreClose(_preferredWidth, value))
            {
                return;
            }

            SetAndRaise(PreferredWidthProperty, ref _preferredWidth, value);
            InvalidateMeasure();
        }
    }
    
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
        set => SetAndRaise(PmTextProperty, ref _pmText, value);
    }

    #endregion

    private const string AntDesignDefaultRangeStartInputWidthReferenceText = "Start time";
    private const string AntDesignDefaultRangeEndInputWidthReferenceText   = "End time";

    private TimePickerPresenter? _pickerPresenter;
    
    static RangeTimePicker()
    {
        AffectsMeasure<RangeTimePicker>(PreferredWidthProperty);
        RangeStartSelectedTimeProperty.Changed.AddClassHandler<RangeTimePicker>((picker, args) => picker.HandleSelectedValueChanged(args));
        RangeEndSelectedTimeProperty.Changed.AddClassHandler<RangeTimePicker>((picker, args) => picker.HandleSelectedValueChanged(args));
    }

    public RangeTimePicker()
    {
    }
    
    /// <summary>
    /// 清除时间选择器的值，不考虑默认值
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        
        RangeStartSelectedTime = null;
        RangeEndSelectedTime   = null;
    }
    
    /// <summary>
    /// 重置时间选择器的值，当有默认值设置的时候，会将当前的值设置成默认值
    /// </summary>
    public void Reset()
    {
        RangeStartSelectedTime = RangeStartDefaultTime;
        RangeEndSelectedTime = RangeEndDefaultTime;
    }
    
    protected override Control CreatePickerPresenter()
    {
        var timePickerPresenter = new TimePickerPresenter();
        timePickerPresenter[!TimePickerPresenter.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        timePickerPresenter[!TimePickerPresenter.MinuteIncrementProperty] = this[!MinuteIncrementProperty];
        timePickerPresenter[!TimePickerPresenter.SecondIncrementProperty] = this[!SecondIncrementProperty];
        timePickerPresenter[!TimePickerPresenter.ClockIdentifierProperty] = this[!ClockIdentifierProperty];
        timePickerPresenter[!TimePickerPresenter.IsNeedConfirmProperty]   = this[!IsNeedConfirmProperty];
        timePickerPresenter[!TimePickerPresenter.IsShowNowProperty]       = this[!IsShowNowProperty];
        return timePickerPresenter;
    }

    protected override void NotifyPickerPresenterCreated(Control pickerPresenter)
    {
        base.NotifyPickerPresenterCreated(pickerPresenter);
        _pickerPresenter = pickerPresenter as TimePickerPresenter;
    }

    protected override void NotifyPickerPresenterCleared(Control pickerPresenter)
    {
        base.NotifyPickerPresenterCleared(pickerPresenter);
        _pickerPresenter = null;
    }

    protected override void NotifyPickerOpened()
    {
        base.NotifyPickerOpened();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatusChanged += HandleChoosingStatusChanged;
            _pickerPresenter.HoverTimeChanged      += HandleHoverTimeChanged;
            _pickerPresenter.Confirmed             += HandleConfirmed;
        }
    }

    protected override void NotifyPickerClosed()
    {
        base.NotifyPickerClosed();
        if (_pickerPresenter is not null)
        {
            _pickerPresenter.ChoosingStatusChanged -= HandleChoosingStatusChanged;
            _pickerPresenter.HoverTimeChanged      -= HandleHoverTimeChanged;
            _pickerPresenter.Confirmed             -= HandleConfirmed;
        }
    }
    
    private void HandleChoosingStatusChanged(object? sender, ChoosingStatusEventArgs args)
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
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime,
                ClockIdentifier == ClockIdentifierType.HourClock12);
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            SecondaryText = DateTimeUtils.FormatTimeSpan(RangeEndSelectedTime,
                ClockIdentifier == ClockIdentifierType.HourClock12);
        }
    }
    
    private void HandleHoverTimeChanged(object? sender, TimeSelectedEventArgs args)
    {
        if (args.Time.HasValue)
        {
            if (RangeActivatedPart == RangeActivatedPart.Start)
            {
                Text = DateTimeUtils.FormatTimeSpan(args.Time.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
            }
            else if (RangeActivatedPart == RangeActivatedPart.End)
            {
                SecondaryText = DateTimeUtils.FormatTimeSpan(args.Time.Value,
                    ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
            }
        }
        else
        {
            ClearHoverSelectedInfo();
        }
    }
    
    private void HandleConfirmed(object? sender, EventArgs args)
    {
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            RangeStartSelectedTime = _pickerPresenter?.SelectedTime;
            if (RangeEndSelectedTime is null)
            {
                RangeActivatedPart = RangeActivatedPart.End;
                return;
            }
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            RangeEndSelectedTime = _pickerPresenter?.SelectedTime;
            if (RangeStartSelectedTime is null)
            {
                RangeActivatedPart = RangeActivatedPart.Start;
                return;
            }
        }

        ClosePickerFlyout();
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
            NotifyRangeActivatedPartChanged();
        }
        else if (IsFormattedTextAffectingProperty(change.Property))
        {
            RefreshRangeTexts();
            CalculatePreferredWidth();
        }
        else if (IsPreferredWidthAffectingProperty(change.Property))
        {
            CalculatePreferredWidth();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == RangeStartSelectedTimeProperty)
            {
                if (RangeStartSelectedTime.HasValue)
                {
                    Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
                }
                else
                {
                    ResetRangeStartTimeValue();
                }
                CalculatePreferredWidth();
            }
            else if (change.Property == RangeEndSelectedTimeProperty)
            {
                if (RangeEndSelectedTime.HasValue)
                {
                    SecondaryText = DateTimeUtils.FormatTimeSpan(RangeEndSelectedTime.Value,
                        ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
                }
                else
                {
                    ResetRangeEndTimeValue();
                }
                CalculatePreferredWidth();
            }
        }
    }

    private static bool IsFormattedTextAffectingProperty(AvaloniaProperty property)
    {
        return property == ClockIdentifierProperty ||
               property == AmTextProperty ||
               property == PmTextProperty;
    }

    private static bool IsPreferredWidthAffectingProperty(AvaloniaProperty property)
    {
        return property == FontSizeProperty ||
               property == FontFamilyProperty ||
               property == FontStyleProperty ||
               property == FontWeightProperty ||
               property == SizeTypeProperty ||
               property == MinWidthProperty ||
               property == WidthProperty ||
               property == MaxWidthProperty ||
               property == HorizontalAlignmentProperty;
    }

    private void RefreshRangeTexts()
    {
        if (RangeStartSelectedTime.HasValue)
        {
            Text = DateTimeUtils.FormatTimeSpan(RangeStartSelectedTime.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
        }
        else
        {
            ResetRangeStartTimeValue();
        }

        if (RangeEndSelectedTime.HasValue)
        {
            SecondaryText = DateTimeUtils.FormatTimeSpan(RangeEndSelectedTime.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
        }
        else
        {
            ResetRangeEndTimeValue();
        }
    }
    
    private void CalculatePreferredWidth()
    {
        if (!double.IsNaN(Width) || HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            PreferredInputWidth = double.NaN;
            PreferredWidth      = 0;
        }
        else
        {
            var preferredInputWidth = CalculateContentPreferredWidth();

            if (!double.IsNaN(MinWidth))
            {
                preferredInputWidth = Math.Max(MinWidth, preferredInputWidth);
            }

            if (!double.IsNaN(MaxWidth))
            {
                preferredInputWidth = Math.Min(MaxWidth, preferredInputWidth);
            }
            PreferredInputWidth = preferredInputWidth;
            PreferredWidth      = preferredInputWidth;
        }
    }

    private double CalculateContentPreferredWidth()
    {
        var formatWidth = DateTimeUtils.CalculateWidestFormattedTimeSpanSize(
            ClockIdentifier == ClockIdentifierType.HourClock12,
            AmText, PmText,
            FontSize, FontFamily, FontStyle, FontWeight).Width;
        var defaultInputBaselineWidth = DatePickerFormattingHelper.CalculateAntDesignInputBaselineWidth(
            FontSize,
            FontFamily,
            FontStyle,
            FontWeight,
            AntDesignDefaultRangeStartInputWidthReferenceText,
            AntDesignDefaultRangeEndInputWidthReferenceText);

        return Math.Max(formatWidth, defaultInputBaselineWidth);
    }
    
    protected void ResetRangeStartTimeValue()
    {
        if (RangeStartDefaultTime is not null)
        {
            Text = DateTimeUtils.FormatTimeSpan(RangeStartDefaultTime.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
        }
        else
        {
            Text = null;
        }
    }
    
    protected void ResetRangeEndTimeValue()
    {
        if (RangeEndDefaultTime is not null)
        {
            SecondaryText = DateTimeUtils.FormatTimeSpan(RangeEndDefaultTime.Value,
                ClockIdentifier == ClockIdentifierType.HourClock12, AmText, PmText);
        }
        else
        {
            SecondaryText = null;
        }
    }
    
    protected override void NotifyRangeActivatedPartChanged()
    {
        base.NotifyRangeActivatedPartChanged();
        if (RangeActivatedPart == RangeActivatedPart.Start)
        {
            if (RangeEndSelectedTime is null)
            {
                ResetRangeStartTimeValue();
            }
            if (_pickerPresenter is not null)
            {
                _pickerPresenter.SelectedTime = RangeStartSelectedTime;
            }
        }
        else if (RangeActivatedPart == RangeActivatedPart.End)
        {
            if (RangeStartSelectedTime is null)
            {
                ResetRangeEndTimeValue();
            }
            if (_pickerPresenter is not null)
            {
                _pickerPresenter.SelectedTime = RangeEndSelectedTime;
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
            if (_pickerPresenter is not null)
            {
                _pickerPresenter.SelectedTime = null;
            }
        }
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var size   = base.MeasureOverride(availableSize);
        var width  = size.Width;
        var height = size.Height;
        if (PreferredWidth > 0 &&
            InfoInputBox is not null &&
            SecondaryInfoInputBox is not null)
        {
            var currentInputWidth = InfoInputBox.DesiredSize.Width + SecondaryInfoInputBox.DesiredSize.Width;
            var preferredWidth    = size.Width - currentInputWidth + PreferredWidth * 2;
            width = Math.Max(width, preferredWidth);
        }

        return new Size(width, height);
    }

    protected override bool ShowClearButtonPredicate()
    {
        return RangeStartSelectedTime is not null || RangeEndSelectedTime is not null;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (RangeStartDefaultTime is not null && RangeStartSelectedTime is null)
        {
            RangeStartSelectedTime = RangeStartDefaultTime;
        }
        
        if (RangeEndDefaultTime is not null && RangeEndSelectedTime is null)
        {
            RangeEndSelectedTime = RangeEndDefaultTime;
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (InfoIcon is null)
        {
            SetValue(InfoIconProperty, new ClockCircleOutlined(), BindingPriority.Template);
        }
        RefreshRangeTexts();
        CalculatePreferredWidth();
    }
    
    #region 实现 FormItem 接口
    protected override void NotifySetFormValue(object? value)
    {
        var rangeValue = value as (TimeSpan?, TimeSpan?)?;
        if (rangeValue != null)
        {
            RangeStartSelectedTime = rangeValue.Value.Item1;
            RangeEndSelectedTime   = rangeValue.Value.Item2;
        }
    }

    protected override object? NotifyGetFormValue()
    {
        if (RangeStartSelectedTime is null || RangeEndSelectedTime is null)
        {
            return null;
        }
        return (RangeStartSelectedTime, RangeEndSelectedTime);
    }

    protected override void NotifyClearFormValue()
    {
        RangeStartSelectedTime = null;
        RangeEndSelectedTime   = null;
    }

    private void HandleSelectedValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        NotifyFormValueChanged((RangeStartSelectedTime, RangeEndSelectedTime));
    }
    #endregion
}
