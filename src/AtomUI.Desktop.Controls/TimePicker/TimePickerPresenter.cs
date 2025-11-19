using AtomUI.Controls;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

internal class TimePickerPresenter : PickerPresenterBase
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        TimePicker.IsNeedConfirmProperty.AddOwner<TimePickerPresenter>();

    public static readonly StyledProperty<bool> IsShowNowProperty =
        TimePicker.IsShowNowProperty.AddOwner<TimePickerPresenter>();

    public static readonly StyledProperty<int> MinuteIncrementProperty =
        TimePicker.MinuteIncrementProperty.AddOwner<TimePickerPresenter>();

    public static readonly StyledProperty<int> SecondIncrementProperty =
        TimePicker.SecondIncrementProperty.AddOwner<TimePickerPresenter>();

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<TimePickerPresenter>();

    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        TimePicker.SelectedTimeProperty.AddOwner<TimePickerPresenter>();

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

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TimePickerPresenter, bool> ButtonsPanelVisibleProperty =
        AvaloniaProperty.RegisterDirect<TimePickerPresenter, bool>(nameof(ButtonsPanelVisible),
            o => o.ButtonsPanelVisible,
            (o, v) => o.ButtonsPanelVisible = v);

    internal static readonly StyledProperty<TimeSpan?> TempSelectedTimeProperty =
        AvaloniaProperty.Register<TimePickerPresenter, TimeSpan?>(nameof(TempSelectedTime));
    
    internal static readonly DirectProperty<TimePickerPresenter, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<TimePickerPresenter, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TimePickerPresenter>();

    private bool _buttonsPanelVisible = true;

    internal bool ButtonsPanelVisible
    {
        get => _buttonsPanelVisible;
        set => SetAndRaise(ButtonsPanelVisibleProperty, ref _buttonsPanelVisible, value);
    }

    public TimeSpan? TempSelectedTime
    {
        get => GetValue(TempSelectedTimeProperty);
        set => SetValue(TempSelectedTimeProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }
    
    #endregion

    #region 公共事件定义

    /// <summary>
    /// 当前 Pointer 选中的日期和时间的变化事件
    /// </summary>
    public event EventHandler<TimeSelectedEventArgs>? HoverTimeChanged;

    /// <summary>
    /// 当前是否处于选择中状态
    /// </summary>
    public event EventHandler<ChoosingStatusEventArgs>? ChoosingStatueChanged;

    #endregion
    
    private IDisposable? _choosingStateDisposable;
    private Button? _nowButton;
    private Button? _confirmButton;
    private TimeView? _timeView;

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                OnDismiss();
                e.Handled = true;
                break;
            case Key.Tab:
                if (FocusUtils.GetFocusManager(this)?.GetFocusedElement() is { } focus)
                {
                    var nextFocus = KeyboardNavigationHandler.GetNext(focus, NavigationDirection.Next);
                    nextFocus?.Focus(NavigationMethod.Tab);
                    e.Handled = true;
                }

                break;
            case Key.Enter:
                OnConfirmed();
                e.Handled = true;
                break;
        }

        base.OnKeyDown(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsNeedConfirmProperty ||
            change.Property == IsShowNowProperty)
        {
            SetupButtonStatus();
        }
        else if (change.Property == SelectedTimeProperty || change.Property == TempSelectedTimeProperty)
        {
            if (_confirmButton is not null)
            {
                _confirmButton.IsEnabled = (SelectedTime is not null || TempSelectedTime is not null);
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _nowButton     = e.NameScope.Get<Button>(TimePickerPresenterThemeConstants.NowButtonPart);
        _confirmButton = e.NameScope.Get<Button>(TimePickerPresenterThemeConstants.ConfirmButtonPart);
        _timeView      = e.NameScope.Get<TimeView>(TimePickerPresenterThemeConstants.TimeViewPart);
        SetupButtonStatus();
        if (_timeView is not null)
        {
            _timeView.HoverTimeChanged += HandleTimeViewHoverChanged;
            _timeView.TimeSelected     += HandleTimeViewTimeSelected;
            _timeView.TempTimeSelected += HandleTimeViewTempTimeSelected;
        }

        if (_nowButton is not null)
        {
            _nowButton.Click += HandleNowButtonClicked;
        }

        if (_confirmButton is not null)
        {
            _confirmButton.Click     += HandleConfirmButtonClicked;
            _confirmButton.IsEnabled =  (SelectedTime is not null || TempSelectedTime is not null);
            _confirmButton.PointerEntered += (sender, args) =>
            {
                if (TempSelectedTime is not null)
                {
                    HoverTimeChanged?.Invoke(this, new TimeSelectedEventArgs(TempSelectedTime));
                }
            };
            _confirmButton.PointerExited += (sender, args) =>
            {
                ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(false));
            };
        }
    }

    private void SetupButtonStatus()
    {
        if (_nowButton is null || _confirmButton is null)
        {
            return;
        }

        _confirmButton.IsVisible = IsNeedConfirm;

        if (IsShowNow)
        {
            if (!IsNeedConfirm)
            {
                _nowButton.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                _nowButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
        else
        {
            _nowButton.IsVisible           = false;
            _nowButton.HorizontalAlignment = HorizontalAlignment.Left;
        }

        ButtonsPanelVisible = _nowButton.IsVisible || _confirmButton.IsVisible;
    }

    private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
    {
        SelectedTime = DateTime.Now.TimeOfDay;
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (TempSelectedTime is not null)
        {
            SelectedTime = TempSelectedTime;
            OnConfirmed();
        }
    }

    private void HandleTimeViewHoverChanged(object? sender, TimeSelectedEventArgs args)
    {
        HoverTimeChanged?.Invoke(this, new TimeSelectedEventArgs(args.Time));
    }

    private void HandleTimeViewTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        SelectedTime = args.Time;
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleTimeViewTempTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        TempSelectedTime = args.Time;
        if (!IsNeedConfirm)
        {
            SelectedTime = TempSelectedTime;
        }
    }

    protected override void OnConfirmed()
    {
        ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(false));
        base.OnConfirmed();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_timeView is not null)
        {
            _choosingStateDisposable = TimeView.IsPointerInSelectorProperty.Changed.Subscribe(args =>
            {
                ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(args.GetNewValue<bool>()));
            });
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _choosingStateDisposable?.Dispose();
        _choosingStateDisposable = null;
    }
}