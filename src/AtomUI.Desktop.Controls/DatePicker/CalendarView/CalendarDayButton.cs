using System.Globalization;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Generated.AtomUIDesktopControls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Desktop.Controls.CalendarView;

[PseudoClasses(StdPseudoClass.Pressed,
    StdPseudoClass.Disabled,
    StdPseudoClass.Selected,
    StdPseudoClass.InActive,
    CalendarDayButtonPseudoClass.Today,
    CalendarDayButtonPseudoClass.Blackout,
    CalendarDayButtonPseudoClass.DayFocused,
    CalendarDayButtonPseudoClass.RangeStart,
    CalendarDayButtonPseudoClass.RangeEnd,
    CalendarDayButtonPseudoClass.RangeMiddle,
    CalendarDayButtonPseudoClass.RangePreviewStart,
    CalendarDayButtonPseudoClass.RangePreviewEnd,
    CalendarDayButtonPseudoClass.RangePreviewMiddle,
    CalendarDayButtonPseudoClass.WeekNumber,
    CalendarDayButtonPseudoClass.WeekSelectionStart,
    CalendarDayButtonPseudoClass.WeekSelectionMiddle,
    CalendarDayButtonPseudoClass.WeekSelectionEnd,
    CalendarDayButtonPseudoClass.WeekRangeStart,
    CalendarDayButtonPseudoClass.WeekRangeMiddle,
    CalendarDayButtonPseudoClass.WeekRangeEnd,
    CalendarDayButtonPseudoClass.WeekHoverStart,
    CalendarDayButtonPseudoClass.WeekHoverMiddle,
    CalendarDayButtonPseudoClass.WeekHoverEnd)]
internal sealed class CalendarDayButton : AvaloniaButton
{
    #region 公共事件定义

    /// <summary>
    /// Occurs when the left pointer button is pressed over this day button.
    /// </summary>
    public event EventHandler<PointerPressedEventArgs>? CalendarDayButtonMouseDown;

    /// <summary>
    /// Occurs when the left pointer button is released over this day button.
    /// </summary>
    public event EventHandler<PointerReleasedEventArgs>? CalendarDayButtonMouseUp;

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CalendarDayButton>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangeStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangeStart),
            o => o.IsRangeStart,
            (o, v) => o.IsRangeStart = v);
    
    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangeEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangeEnd),
            o => o.IsRangeEnd,
            (o, v) => o.IsRangeEnd = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangeMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangeMiddle),
            o => o.IsRangeMiddle,
            (o, v) => o.IsRangeMiddle = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangePreviewStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangePreviewStart),
            o => o.IsRangePreviewStart,
            (o, v) => o.IsRangePreviewStart = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangePreviewEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangePreviewEnd),
            o => o.IsRangePreviewEnd,
            (o, v) => o.IsRangePreviewEnd = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsRangePreviewMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsRangePreviewMiddle),
            o => o.IsRangePreviewMiddle,
            (o, v) => o.IsRangePreviewMiddle = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekNumberProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekNumber),
            o => o.IsWeekNumber,
            (o, v) => o.IsWeekNumber = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekSelectionStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekSelectionStart),
            o => o.IsWeekSelectionStart,
            (o, v) => o.IsWeekSelectionStart = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekSelectionMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekSelectionMiddle),
            o => o.IsWeekSelectionMiddle,
            (o, v) => o.IsWeekSelectionMiddle = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekSelectionEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekSelectionEnd),
            o => o.IsWeekSelectionEnd,
            (o, v) => o.IsWeekSelectionEnd = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekRangeStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekRangeStart),
            o => o.IsWeekRangeStart,
            (o, v) => o.IsWeekRangeStart = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekRangeMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekRangeMiddle),
            o => o.IsWeekRangeMiddle,
            (o, v) => o.IsWeekRangeMiddle = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekRangeEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekRangeEnd),
            o => o.IsWeekRangeEnd,
            (o, v) => o.IsWeekRangeEnd = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekHoverStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekHoverStart),
            o => o.IsWeekHoverStart,
            (o, v) => o.IsWeekHoverStart = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekHoverMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekHoverMiddle),
            o => o.IsWeekHoverMiddle,
            (o, v) => o.IsWeekHoverMiddle = v);

    internal static readonly DirectProperty<CalendarDayButton, bool> IsWeekHoverEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, bool>(nameof(IsWeekHoverEnd),
            o => o.IsWeekHoverEnd,
            (o, v) => o.IsWeekHoverEnd = v);

    internal static readonly DirectProperty<CalendarDayButton, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<CalendarDayButton, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);

    private bool _isRangeStart;
    internal bool IsRangeStart
    {
        get => _isRangeStart;
        set => SetAndRaise(IsRangeStartProperty, ref _isRangeStart, value);
    }

    private bool _isRangeEnd;
    internal bool IsRangeEnd
    {
        get => _isRangeEnd;
        set => SetAndRaise(IsRangeEndProperty, ref _isRangeEnd, value);
    }

    private bool _isRangeMiddle;
    internal bool IsRangeMiddle
    {
        get => _isRangeMiddle;
        set => SetAndRaise(IsRangeMiddleProperty, ref _isRangeMiddle, value);
    }

    private bool _isRangePreviewStart;
    internal bool IsRangePreviewStart
    {
        get => _isRangePreviewStart;
        set => SetAndRaise(IsRangePreviewStartProperty, ref _isRangePreviewStart, value);
    }

    private bool _isRangePreviewEnd;
    internal bool IsRangePreviewEnd
    {
        get => _isRangePreviewEnd;
        set => SetAndRaise(IsRangePreviewEndProperty, ref _isRangePreviewEnd, value);
    }

    private bool _isRangePreviewMiddle;
    internal bool IsRangePreviewMiddle
    {
        get => _isRangePreviewMiddle;
        set => SetAndRaise(IsRangePreviewMiddleProperty, ref _isRangePreviewMiddle, value);
    }

    private bool _isWeekNumber;
    internal bool IsWeekNumber
    {
        get => _isWeekNumber;
        set => SetAndRaise(IsWeekNumberProperty, ref _isWeekNumber, value);
    }

    private bool _isWeekSelectionStart;
    internal bool IsWeekSelectionStart
    {
        get => _isWeekSelectionStart;
        set => SetAndRaise(IsWeekSelectionStartProperty, ref _isWeekSelectionStart, value);
    }

    private bool _isWeekSelectionMiddle;
    internal bool IsWeekSelectionMiddle
    {
        get => _isWeekSelectionMiddle;
        set => SetAndRaise(IsWeekSelectionMiddleProperty, ref _isWeekSelectionMiddle, value);
    }

    private bool _isWeekSelectionEnd;
    internal bool IsWeekSelectionEnd
    {
        get => _isWeekSelectionEnd;
        set => SetAndRaise(IsWeekSelectionEndProperty, ref _isWeekSelectionEnd, value);
    }

    private bool _isWeekRangeStart;
    internal bool IsWeekRangeStart
    {
        get => _isWeekRangeStart;
        set => SetAndRaise(IsWeekRangeStartProperty, ref _isWeekRangeStart, value);
    }

    private bool _isWeekRangeMiddle;
    internal bool IsWeekRangeMiddle
    {
        get => _isWeekRangeMiddle;
        set => SetAndRaise(IsWeekRangeMiddleProperty, ref _isWeekRangeMiddle, value);
    }

    private bool _isWeekRangeEnd;
    internal bool IsWeekRangeEnd
    {
        get => _isWeekRangeEnd;
        set => SetAndRaise(IsWeekRangeEndProperty, ref _isWeekRangeEnd, value);
    }

    private bool _isWeekHoverStart;
    internal bool IsWeekHoverStart
    {
        get => _isWeekHoverStart;
        set => SetAndRaise(IsWeekHoverStartProperty, ref _isWeekHoverStart, value);
    }

    private bool _isWeekHoverMiddle;
    internal bool IsWeekHoverMiddle
    {
        get => _isWeekHoverMiddle;
        set => SetAndRaise(IsWeekHoverMiddleProperty, ref _isWeekHoverMiddle, value);
    }

    private bool _isWeekHoverEnd;
    internal bool IsWeekHoverEnd
    {
        get => _isWeekHoverEnd;
        set => SetAndRaise(IsWeekHoverEndProperty, ref _isWeekHoverEnd, value);
    }
    
    private CornerRadius _effectiveCornerRadius;
    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }

    #endregion
    
    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal Calendar? Owner { get; set; }
    
    /// <summary>
    /// Default content for the CalendarDayButton.
    /// </summary>
    private const int DefaultContent = 1;

    private bool _ignoringMouseOverState;
    private bool _isBlackout;

    private bool _isCurrent;
    private bool _isInactive;
    private bool _isSelected;
    private bool _isToday;
    
    public CalendarDayButton()
    {
        //Focusable = false;
        // CalendarDayButton 由 DatePicker 与 RangeDatePicker 两个 owner 的月网格共享;
        // 两个 owner 的 popup.cell 生成常量值一致,构造期一次性注入 marker,
        // 月网格 rebuild 与容器回收复用新实例时保持不变。
        Classes.Add(DatePickerSemanticParts.PopupCellClass);
        SetCurrentValue(ContentProperty, DefaultContent.ToString(CultureInfo.InvariantCulture));
    }

    internal int Index { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the button is the focused
    /// element on the Calendar control.
    /// </summary>
    internal bool IsCurrent
    {
        get => _isCurrent;

        set
        {
            if (_isCurrent != value)
            {
                _isCurrent = value;
                UpdatePseudoClasses();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this is a blackout date.
    /// </summary>
    internal bool IsBlackout
    {
        get => _isBlackout;

        set
        {
            if (_isBlackout != value)
            {
                _isBlackout = value;
                UpdatePseudoClasses();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this button represents
    /// today.
    /// </summary>
    internal bool IsToday
    {
        get => _isToday;

        set
        {
            if (_isToday != value)
            {
                _isToday = value;
                UpdatePseudoClasses();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the button is inactive.
    /// </summary>
    internal bool IsInactive
    {
        get => _isInactive;

        set
        {
            if (_isInactive != value)
            {
                _isInactive = value;
                UpdatePseudoClasses();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the button is selected.
    /// </summary>
    internal bool IsSelected
    {
        get => _isSelected;

        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                UpdatePseudoClasses();
                ConfigureEffectiveCornerRadius();
            }
        }
    }

    /// <summary>
    /// Ensure the button is not in the MouseOver state.
    /// </summary>
    /// <remarks>
    /// If a button is in the MouseOver state when a Popup is closed (as is
    /// the case when you select a date in the DatePicker control), it will
    /// continue to think it's in the mouse over state even when the Popup
    /// opens again and it's not.  This method is used to forcibly clear the
    /// state by changing the CommonStates state group.
    /// </remarks>
    internal void IgnoreMouseOverState()
    {
        // TODO: Investigate whether this needs to be done by changing the
        // state everytime we change any state, or if it can be done once
        // to properly reset the control.

        _ignoringMouseOverState = false;

        // If the button thinks it's in the MouseOver state (which can
        // happen when a Popup is closed before the button can change state)
        // we will override the state so it shows up as normal.
        if (IsPointerOver)
        {
            _ignoringMouseOverState = true;
            UpdatePseudoClasses();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UpdatePseudoClasses();
        ConfigureEffectiveCornerRadius();
    }

    private void UpdatePseudoClasses()
    {
        if (_ignoringMouseOverState)
        {
            PseudoClasses.Set(StdPseudoClass.Pressed, IsPressed);
            PseudoClasses.Set(StdPseudoClass.Disabled, !IsEnabled);
        }

        PseudoClasses.Set(StdPseudoClass.Selected, IsSelected);
        PseudoClasses.Set(StdPseudoClass.InActive, IsInactive);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.Today, IsToday);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.Blackout, IsBlackout);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.DayFocused, IsCurrent && IsEnabled);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeStart, IsRangeStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeEnd, IsRangeEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeMiddle, IsRangeMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewStart, IsRangePreviewStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewEnd, IsRangePreviewEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewMiddle, IsRangePreviewMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekNumber, IsWeekNumber);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekSelectionStart, IsWeekSelectionStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekSelectionMiddle, IsWeekSelectionMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekSelectionEnd, IsWeekSelectionEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekRangeStart, IsWeekRangeStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekRangeMiddle, IsWeekRangeMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekRangeEnd, IsWeekRangeEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekHoverStart, IsWeekHoverStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekHoverMiddle, IsWeekHoverMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.WeekHoverEnd, IsWeekHoverEnd);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            CalendarDayButtonMouseDown?.Invoke(this, e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            CalendarDayButtonMouseUp?.Invoke(this, e);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsRangeStartProperty ||
            change.Property == IsRangeEndProperty ||
            change.Property == IsRangeMiddleProperty ||
            change.Property == IsRangePreviewStartProperty ||
            change.Property == IsRangePreviewEndProperty ||
            change.Property == IsRangePreviewMiddleProperty ||
            change.Property == IsWeekNumberProperty ||
            change.Property == IsWeekSelectionStartProperty ||
            change.Property == IsWeekSelectionMiddleProperty ||
            change.Property == IsWeekSelectionEndProperty ||
            change.Property == IsWeekRangeStartProperty ||
            change.Property == IsWeekRangeMiddleProperty ||
            change.Property == IsWeekRangeEndProperty ||
            change.Property == IsWeekHoverStartProperty ||
            change.Property == IsWeekHoverMiddleProperty ||
            change.Property == IsWeekHoverEndProperty)
        {
            UpdatePseudoClasses();
        }
        if (change.Property == IsRangeStartProperty ||
            change.Property == IsRangeEndProperty ||
            change.Property == IsRangeMiddleProperty ||
            change.Property == IsRangePreviewStartProperty ||
            change.Property == IsRangePreviewEndProperty ||
            change.Property == IsRangePreviewMiddleProperty ||
            change.Property == IsWeekSelectionStartProperty ||
            change.Property == IsWeekSelectionMiddleProperty ||
            change.Property == IsWeekSelectionEndProperty ||
            change.Property == IsWeekRangeStartProperty ||
            change.Property == IsWeekRangeMiddleProperty ||
            change.Property == IsWeekRangeEndProperty ||
            change.Property == IsWeekHoverStartProperty ||
            change.Property == IsWeekHoverMiddleProperty ||
            change.Property == IsWeekHoverEndProperty ||
            change.Property == CornerRadiusProperty)
        {
            ConfigureEffectiveCornerRadius();
        }
    }

    private void ConfigureEffectiveCornerRadius()
    {
        var isVisualRangeStart = IsRangeStart ||
                                 IsRangePreviewStart ||
                                 IsWeekSelectionStart ||
                                 IsWeekRangeStart ||
                                 IsWeekHoverStart;
        var isVisualRangeMiddle = IsWeekSelectionMiddle || IsWeekRangeMiddle || IsWeekHoverMiddle;
        var isVisualRangeEnd = IsRangeEnd ||
                               IsRangePreviewEnd ||
                               IsWeekSelectionEnd ||
                               IsWeekRangeEnd ||
                               IsWeekHoverEnd;
        if (isVisualRangeStart && !isVisualRangeEnd)
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, new CornerRadius(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft));
        }
        else if (isVisualRangeEnd && !isVisualRangeStart)
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, new CornerRadius(0, CornerRadius.TopRight, CornerRadius.BottomRight, 0));
        }
        else if (isVisualRangeMiddle)
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, new CornerRadius(0));
        }
        else
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, CornerRadius);
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
