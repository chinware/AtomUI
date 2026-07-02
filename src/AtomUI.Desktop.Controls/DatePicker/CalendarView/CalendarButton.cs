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

[PseudoClasses(StdPseudoClass.Selected, StdPseudoClass.InActive, BtnFocusedPC)]
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UpdatePseudoClasses();
    }
    
    /// <summary>
    /// Sets PseudoClasses based on current state.
    /// </summary>
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Selected, IsSelected);
        PseudoClasses.Set(StdPseudoClass.InActive, IsInactive);
        PseudoClasses.Set(BtnFocusedPC, IsCalendarButtonFocused && IsEnabled);
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
