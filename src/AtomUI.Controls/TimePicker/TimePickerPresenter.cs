using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class TimePickerPresenter : PickerPresenterBase,
                                     IResourceBindingManager
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

    public static readonly StyledProperty<TimeSpan?> TempSelectedTimeProperty =
        AvaloniaProperty.Register<TimePickerPresenter, TimeSpan?>(nameof(TempSelectedTime));

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
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TimePickerPresenter>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    
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

    private CompositeDisposable? _resourceBindingsDisposable;
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
    }

    protected override void OnConfirmed()
    {
        ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(false));
        base.OnConfirmed();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty, SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, thickness.Top, 0, 0))));
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
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}