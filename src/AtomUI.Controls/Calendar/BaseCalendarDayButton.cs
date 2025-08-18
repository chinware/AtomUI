using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed,
    StdPseudoClass.Disabled,
    StdPseudoClass.Selected,
    StdPseudoClass.InActive,
    TodayPC,
    BlackoutPC,
    DayfocusedPC)]
internal class BaseCalendarDayButton : AvaloniaButton,
                                       IResourceBindingManager
{
    internal const string TodayPC = ":today";
    internal const string BlackoutPC = ":blackout";
    internal const string DayfocusedPC = ":dayfocused";

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseCalendarDayButton>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    private CompositeDisposable? _resourceBindingsDisposable;

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

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:Avalonia.Controls.Primitives.CalendarDayButton" />
    /// class.
    /// </summary>
    public BaseCalendarDayButton()
    {
        //Focusable = false;
        SetCurrentValue(ContentProperty, DefaultContent.ToString(CultureInfo.CurrentCulture));
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
        ConfigureTransitions();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty,
                    SharedTokenKey.MotionDurationFast)
            };
        }
        else
        {
            Transitions = null;
        }
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
        PseudoClasses.Set(TodayPC, IsToday);
        PseudoClasses.Set(BlackoutPC, IsBlackout);
        PseudoClasses.Set(DayfocusedPC, IsCurrent && IsEnabled);
    }

    /// <summary>
    /// Occurs when the left mouse button is pressed (or when the tip of the
    /// stylus touches the tablet PC) while the mouse pointer is over a
    /// UIElement.
    /// </summary>
    public event EventHandler<PointerPressedEventArgs>? CalendarDayButtonMouseDown;

    /// <summary>
    /// Occurs when the left mouse button is released (or the tip of the
    /// stylus is removed from the tablet PC) while the mouse (or the
    /// stylus) is over a UIElement (or while a UIElement holds mouse
    /// capture).
    /// </summary>
    public event EventHandler<PointerReleasedEventArgs>? CalendarDayButtonMouseUp;

    /// <summary>
    /// Provides class handling for the MouseLeftButtonDown event that
    /// occurs when the left mouse button is pressed while the mouse pointer
    /// is over this control.
    /// </summary>
    /// <param name="e">The event data. </param>
    /// <exception cref="System.ArgumentNullException">
    /// e is a null reference (Nothing in Visual Basic).
    /// </exception>
    /// <remarks>
    /// This method marks the MouseLeftButtonDown event as handled by
    /// setting the MouseButtonEventArgs.Handled property of the event data
    /// to true when the button is enabled and its ClickMode is not set to
    /// Hover.  Since this method marks the MouseLeftButtonDown event as
    /// handled in some situations, you should use the Click event instead
    /// to detect a button click.
    /// </remarks>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            CalendarDayButtonMouseDown?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Provides handling for the MouseLeftButtonUp event that occurs when
    /// the left mouse button is released while the mouse pointer is over
    /// this control.
    /// </summary>
    /// <param name="e">The event data.</param>
    /// <exception cref="System.ArgumentNullException">
    /// e is a null reference (Nothing in Visual Basic).
    /// </exception>
    /// <remarks>
    /// This method marks the MouseLeftButtonUp event as handled by setting
    /// the MouseButtonEventArgs.Handled property of the event data to true
    /// when the button is enabled and its ClickMode is not set to Hover.
    /// Since this method marks the MouseLeftButtonUp event as handled in
    /// some situations, you should use the Click event instead to detect a
    /// button click.
    /// </remarks>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            CalendarDayButtonMouseUp?.Invoke(this, e);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }
}