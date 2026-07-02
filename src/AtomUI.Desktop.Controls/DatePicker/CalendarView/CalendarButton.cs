using AtomUI.Animations;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Desktop.Controls.CalendarView;

[PseudoClasses(StdPseudoClass.Selected,
    StdPseudoClass.InActive,
    BtnFocusedPC,
    CalendarDayButtonPseudoClass.RangeStart,
    CalendarDayButtonPseudoClass.RangeEnd,
    CalendarDayButtonPseudoClass.RangeMiddle,
    CalendarDayButtonPseudoClass.RangePreviewStart,
    CalendarDayButtonPseudoClass.RangePreviewEnd,
    CalendarDayButtonPseudoClass.RangePreviewMiddle)]
internal sealed class CalendarButton : AvaloniaButton
{
    #region 公共事件定义

    /// <summary>
    /// Occurs when the left mouse button is pressed over this calendar button.
    /// </summary>
    public event EventHandler<PointerPressedEventArgs>? CalendarLeftMouseButtonDown;

    /// <summary>
    /// Occurs when the left mouse button is released over this calendar button.
    /// </summary>
    public event EventHandler<PointerReleasedEventArgs>? CalendarLeftMouseButtonUp;

    #endregion

    #region 内部属性定义

    internal const string BtnFocusedPC = ":btnfocused";
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CalendarButton>();

    internal static readonly DirectProperty<CalendarButton, bool> IsRangeStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangeStart),
            o => o.IsRangeStart,
            (o, v) => o.IsRangeStart = v);

    internal static readonly DirectProperty<CalendarButton, bool> IsRangeEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangeEnd),
            o => o.IsRangeEnd,
            (o, v) => o.IsRangeEnd = v);

    internal static readonly DirectProperty<CalendarButton, bool> IsRangeMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangeMiddle),
            o => o.IsRangeMiddle,
            (o, v) => o.IsRangeMiddle = v);

    internal static readonly DirectProperty<CalendarButton, bool> IsRangePreviewStartProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangePreviewStart),
            o => o.IsRangePreviewStart,
            (o, v) => o.IsRangePreviewStart = v);

    internal static readonly DirectProperty<CalendarButton, bool> IsRangePreviewEndProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangePreviewEnd),
            o => o.IsRangePreviewEnd,
            (o, v) => o.IsRangePreviewEnd = v);

    internal static readonly DirectProperty<CalendarButton, bool> IsRangePreviewMiddleProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, bool>(nameof(IsRangePreviewMiddle),
            o => o.IsRangePreviewMiddle,
            (o, v) => o.IsRangePreviewMiddle = v);

    internal static readonly DirectProperty<CalendarButton, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<CalendarButton, CornerRadius>(nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the Calendar associated with this button.
    /// </summary>
    internal Calendar? Owner { get; set; }

    #endregion

    /// <summary>
    /// A value indicating whether the button is focused.
    /// </summary>
    private bool _isCalendarButtonFocused;

    /// <summary>
    /// A value indicating whether the button is inactive.
    /// </summary>
    private bool _isInactive;

    /// <summary>
    /// A value indicating whether the button is selected.
    /// </summary>
    private bool _isSelected;
    private bool _isRangeStart;
    private bool _isRangeEnd;
    private bool _isRangeMiddle;
    private bool _isRangePreviewStart;
    private bool _isRangePreviewEnd;
    private bool _isRangePreviewMiddle;
    private CornerRadius _effectiveCornerRadius;

    public CalendarButton()
    {
        SetCurrentValue(ContentProperty, string.Empty);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the button is focused.
    /// </summary>
    internal bool IsCalendarButtonFocused
    {
        get => _isCalendarButtonFocused;

        set
        {
            if (_isCalendarButtonFocused != value)
            {
                _isCalendarButtonFocused = value;
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
            }
        }
    }

    internal bool IsRangeStart
    {
        get => _isRangeStart;
        set => SetAndRaise(IsRangeStartProperty, ref _isRangeStart, value);
    }

    internal bool IsRangeEnd
    {
        get => _isRangeEnd;
        set => SetAndRaise(IsRangeEndProperty, ref _isRangeEnd, value);
    }

    internal bool IsRangeMiddle
    {
        get => _isRangeMiddle;
        set => SetAndRaise(IsRangeMiddleProperty, ref _isRangeMiddle, value);
    }

    internal bool IsRangePreviewStart
    {
        get => _isRangePreviewStart;
        set => SetAndRaise(IsRangePreviewStartProperty, ref _isRangePreviewStart, value);
    }

    internal bool IsRangePreviewEnd
    {
        get => _isRangePreviewEnd;
        set => SetAndRaise(IsRangePreviewEndProperty, ref _isRangePreviewEnd, value);
    }

    internal bool IsRangePreviewMiddle
    {
        get => _isRangePreviewMiddle;
        set => SetAndRaise(IsRangePreviewMiddleProperty, ref _isRangePreviewMiddle, value);
    }

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        private set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UpdatePseudoClasses();
        ConfigureEffectiveCornerRadius();
    }
    
    /// <summary>
    /// Sets PseudoClasses based on current state.
    /// </summary>
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Selected, IsSelected);
        PseudoClasses.Set(StdPseudoClass.InActive, IsInactive);
        PseudoClasses.Set(BtnFocusedPC, IsCalendarButtonFocused && IsEnabled);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeStart, IsRangeStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeEnd, IsRangeEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangeMiddle, IsRangeMiddle);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewStart, IsRangePreviewStart);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewEnd, IsRangePreviewEnd);
        PseudoClasses.Set(CalendarDayButtonPseudoClass.RangePreviewMiddle, IsRangePreviewMiddle);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsRangeStartProperty ||
            change.Property == IsRangeEndProperty ||
            change.Property == IsRangeMiddleProperty ||
            change.Property == IsRangePreviewStartProperty ||
            change.Property == IsRangePreviewEndProperty ||
            change.Property == IsRangePreviewMiddleProperty)
        {
            UpdatePseudoClasses();
        }

        if (change.Property == IsRangeStartProperty ||
            change.Property == IsRangeEndProperty ||
            change.Property == IsRangeMiddleProperty ||
            change.Property == IsRangePreviewStartProperty ||
            change.Property == IsRangePreviewEndProperty ||
            change.Property == IsRangePreviewMiddleProperty ||
            change.Property == CornerRadiusProperty)
        {
            ConfigureEffectiveCornerRadius();
        }
    }

    private void ConfigureEffectiveCornerRadius()
    {
        var isVisualRangeStart = IsRangeStart || IsRangePreviewStart;
        var isVisualRangeEnd   = IsRangeEnd || IsRangePreviewEnd;
        var isVisualRangeMiddle = IsRangeMiddle || IsRangePreviewMiddle;
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

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            CalendarLeftMouseButtonDown?.Invoke(this, e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            CalendarLeftMouseButtonUp?.Invoke(this, e);
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
