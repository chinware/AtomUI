using System.Reactive.Disposables;
using AtomUI.Controls.CalendarView;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using PickerCalendar = AtomUI.Controls.CalendarView.Calendar;

namespace AtomUI.Controls;

public class ChoosingStatusEventArgs : EventArgs
{
    public bool IsChoosing { get; }

    public ChoosingStatusEventArgs(bool isChoosing)
    {
        IsChoosing = isChoosing;
    }
}

internal class DatePickerPresenter : PickerPresenterBase,
                                     IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        DatePicker.IsNeedConfirmProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<bool> IsShowNowProperty =
        DatePicker.IsShowNowProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        DatePicker.SelectedDateTimeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
        TimePicker.ClockIdentifierProperty.AddOwner<DatePickerPresenter>();

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

    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
    }

    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }

    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<DatePickerPresenter, bool> ButtonsPanelVisibleProperty =
        AvaloniaProperty.RegisterDirect<DatePickerPresenter, bool>(nameof(ButtonsPanelVisible),
            o => o.ButtonsPanelVisible,
            (o, v) => o.ButtonsPanelVisible = v);

    public static readonly StyledProperty<TimeSpan?> TempSelectedTimeProperty =
        AvaloniaProperty.Register<DatePickerPresenter, TimeSpan?>(nameof(TempSelectedTime));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DatePickerPresenter>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

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

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }

    #endregion

    #region 公共事件定义

    /// <summary>
    /// 当前 Pointer 选中的日期和时间的变化事件
    /// </summary>
    public event EventHandler<DateSelectedEventArgs>? HoverDateTimeChanged;

    /// <summary>
    /// 当前是否处于选择中状态
    /// </summary>
    public event EventHandler<ChoosingStatusEventArgs>? ChoosingStatueChanged;

    #endregion

    protected Button? NowButton;
    protected Button? TodayButton;
    protected Button? ConfirmButton;
    protected PickerCalendar? CalendarView;
    protected TimeView? TimeView;
    private CompositeDisposable? _pointerDisposables;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, thickness.Top, 0, 0))));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _pointerDisposables = new CompositeDisposable(2);
        _pointerDisposables.Add(PickerCalendar.IsPointerInMonthViewProperty.Changed.Subscribe(args =>
        {
            if (CalendarView is not null)
            {
                EmitChoosingStatueChanged(args.GetNewValue<bool>());
            }
        }));
        _pointerDisposables.Add(TimeView.IsPointerInSelectorProperty.Changed.Subscribe(args =>
        {
            if (TimeView is not null)
            {
                EmitChoosingStatueChanged(args.GetNewValue<bool>());
            }
        }));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerDisposables?.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsNeedConfirmProperty ||
            change.Property == IsShowNowProperty ||
            change.Property == IsShowTimeProperty)
        {
            SetupButtonStatus();
        }
        else if (change.Property == SelectedDateTimeProperty)
        {
            SetupConfirmButtonEnableStatus();
        }
    }

    protected virtual void SetupConfirmButtonEnableStatus()
    {
        if (ConfirmButton is not null)
        {
            ConfirmButton.IsEnabled = SelectedDateTime is not null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        NowButton     = e.NameScope.Get<Button>(DatePickerPresenterThemeConstants.NowButtonPart);
        TodayButton   = e.NameScope.Get<Button>(DatePickerPresenterThemeConstants.TodayButtonPart);
        ConfirmButton = e.NameScope.Get<Button>(DatePickerPresenterThemeConstants.ConfirmButtonPart);
        CalendarView  = e.NameScope.Get<PickerCalendar>(DatePickerPresenterThemeConstants.CalendarViewPart);
        TimeView      = e.NameScope.Find<TimeView>(DatePickerPresenterThemeConstants.TimeViewPart);
        SetupButtonStatus();
        if (CalendarView is not null)
        {
            CalendarView.HoverDateChanged += HandleCalendarViewDateHoverChanged;
            CalendarView.DateSelected     += HandleCalendarViewDateSelected;
        }

        if (TimeView is not null)
        {
            if (IsShowTime)
            {
                SyncTimeViewTimeValue();
            }

            TimeView.HoverTimeChanged += HandleTimeViewHoverChanged;
            TimeView.TimeSelected     += HandleTimeViewTimeSelected;
            TimeView.TempTimeSelected += HandleTimeViewTempTimeSelected;
        }

        if (TodayButton is not null)
        {
            TodayButton.Click += HandleTodayButtonClicked;
        }

        if (NowButton is not null)
        {
            NowButton.Click += HandleNowButtonClicked;
        }

        if (ConfirmButton is not null)
        {
            ConfirmButton.Click          += HandleConfirmButtonClicked;
            ConfirmButton.IsEnabled      =  SelectedDateTime is not null;
            ConfirmButton.PointerEntered += (sender, args) => { NotifyPointerEnterConfirmButton(); };
            ConfirmButton.PointerExited  += (sender, args) => { NotifyPointerExitConfirmButton(); };
        }

        SetupConfirmButtonEnableStatus();
    }

    protected virtual void NotifyPointerEnterConfirmButton()
    {
        if (CalendarView?.SelectedDate is not null)
        {
            var hoverDateTime =
                CollectDateTime(CalendarView?.SelectedDate, TempSelectedTime ?? TimeView?.SelectedTime);
            EmitHoverDateTimeChanged(hoverDateTime);
        }
    }

    protected virtual void NotifyPointerExitConfirmButton()
    {
        EmitChoosingStatueChanged(false);
    }

    private void HandleTodayButtonClicked(object? sender, RoutedEventArgs args)
    {
        SelectedDateTime = DateTime.Today;
        if (CalendarView is not null)
        {
            CalendarView.DisplayDate = DateTime.Today;
        }

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (CalendarView is not null)
        {
            CalendarView.SelectedDate = DateTime.Now;
        }

        if (IsShowTime && TimeView is not null)
        {
            TimeView.SelectedTime = DateTime.Now.TimeOfDay;
        }

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
    {
        NotifyConfirmButtonClicked();
    }

    protected virtual void NotifyConfirmButtonClicked()
    {
        if (SelectedDateTime is not null)
        {
            OnConfirmed();
        }
    }

    private void HandleCalendarViewDateHoverChanged(object? sender, DateSelectedEventArgs args)
    {
        NotifyCalendarViewDateHoverChanged(args.Date);
    }

    protected virtual void NotifyCalendarViewDateHoverChanged(DateTime? newDate)
    {
        // 需要组合日期和时间
        // 暂时没实现
        var hoverDateTime = CollectDateTime(newDate, TempSelectedTime);
        EmitHoverDateTimeChanged(hoverDateTime);
    }

    protected void EmitHoverDateTimeChanged(DateTime? newDate)
    {
        HoverDateTimeChanged?.Invoke(this, new DateSelectedEventArgs(newDate));
    }

    private void HandleCalendarViewDateSelected(object? sender, DateSelectedEventArgs args)
    {
        NotifyCalendarViewDateSelected();
    }

    protected virtual void NotifyCalendarViewDateSelected()
    {
        SelectedDateTime = CollectDateTime(CalendarView?.SelectedDate, TempSelectedTime ?? TimeView?.SelectedTime);
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    protected DateTime? CollectDateTime(DateTime? date, TimeSpan? timeSpan = null)
    {
        if (date is null)
        {
            return null;
        }

        date = date.Value.Date;
        if (IsShowTime && timeSpan is not null)
        {
            date = date.Value.Add(timeSpan.Value);
        }

        return date;
    }

    private void SetupButtonStatus()
    {
        if (NowButton is null ||
            TodayButton is null ||
            ConfirmButton is null)
        {
            return;
        }

        ConfirmButton.IsVisible = IsNeedConfirm;

        NowButton.IsVisible             = false;
        TodayButton.IsVisible           = false;
        NowButton.HorizontalAlignment   = HorizontalAlignment.Left;
        TodayButton.HorizontalAlignment = HorizontalAlignment.Left;

        if (IsShowNow)
        {
            NowButton.IsVisible   = false;
            TodayButton.IsVisible = false;
            if (IsShowTime)
            {
                NowButton.IsVisible = true;
            }
            else
            {
                TodayButton.IsVisible = true;
            }

            if (!IsNeedConfirm)
            {
                NowButton.HorizontalAlignment   = HorizontalAlignment.Center;
                TodayButton.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                NowButton.HorizontalAlignment   = HorizontalAlignment.Left;
                TodayButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        ButtonsPanelVisible = NowButton.IsVisible || TodayButton.IsVisible || ConfirmButton.IsVisible;
    }

    protected override void OnConfirmed()
    {
        if (CalendarView is not null)
        {
            CalendarView.SelectedDate = SelectedDateTime;
        }
        EmitChoosingStatueChanged(false);
        base.OnConfirmed();
    }

    internal void EmitConfirmed()
    {
        base.OnConfirmed();
    }

    protected void EmitChoosingStatueChanged(bool isChoosing)
    {
        ChoosingStatueChanged?.Invoke(this, new ChoosingStatusEventArgs(isChoosing));
    }

    protected override void OnDismiss()
    {
        base.OnDismiss();
        SelectedDateTime = null;
    }

    protected virtual void SyncTimeViewTimeValue()
    {
        if (TimeView is not null)
        {
            TimeView.SelectedTime = SelectedDateTime?.TimeOfDay ?? TimeSpan.Zero;
        }
    }

    private void HandleTimeViewHoverChanged(object? sender, TimeSelectedEventArgs args)
    {
        NotifyTimeViewHoverChanged(args.Time);
    }

    protected virtual void NotifyTimeViewHoverChanged(TimeSpan? newTime)
    {
        var hoverDateTime = CollectDateTime(SelectedDateTime, newTime);
        HoverDateTimeChanged?.Invoke(this, new DateSelectedEventArgs(hoverDateTime));
    }

    private void HandleTimeViewTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleTimeViewTempTimeSelected(object? sender, TimeSelectedEventArgs args)
    {
        TimeViewTempTimeSelected(args.Time);
    }

    protected virtual void TimeViewTempTimeSelected(TimeSpan? time)
    {
        TempSelectedTime = time;
    }
}