using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.CalendarView;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using PickerCalendar = AtomUI.Desktop.Controls.CalendarView.Calendar;

namespace AtomUI.Desktop.Controls;

public class ChoosingStatusEventArgs : EventArgs
{
    public bool IsChoosing { get; }

    public ChoosingStatusEventArgs(bool isChoosing)
    {
        IsChoosing = isChoosing;
    }
}

internal class DatePickerPresenter : PickerPresenterBase
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        DatePicker.IsNeedConfirmProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<bool> IsShowNowProperty =
        DatePicker.IsShowNowProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DatePickerMode> PickerModeProperty =
        DatePicker.PickerModeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        DatePicker.SelectedDateTimeProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> PickerDisplayDateProperty =
        DatePicker.PickerDisplayDateProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        DatePicker.MinDateProperty.AddOwner<DatePickerPresenter>();

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        DatePicker.MaxDateProperty.AddOwner<DatePickerPresenter>();

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

    public DatePickerMode PickerMode
    {
        get => GetValue(PickerModeProperty);
        set => SetValue(PickerModeProperty, value);
    }

    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }

    public DateTime? PickerDisplayDate
    {
        get => GetValue(PickerDisplayDateProperty);
        set => SetValue(PickerDisplayDateProperty, value);
    }

    public DateTime? MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    public DateTime? MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    public ClockIdentifierType ClockIdentifier
    {
        get => GetValue(ClockIdentifierProperty);
        set => SetValue(ClockIdentifierProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<DatePickerPresenter, bool> IsButtonsPanelVisibleProperty =
        AvaloniaProperty.RegisterDirect<DatePickerPresenter, bool>(nameof(IsButtonsPanelVisible),
            o => o.IsButtonsPanelVisible,
            (o, v) => o.IsButtonsPanelVisible = v);

    internal static readonly DirectProperty<DatePickerPresenter, bool> IsTimeSelectionVisibleProperty =
        AvaloniaProperty.RegisterDirect<DatePickerPresenter, bool>(nameof(IsTimeSelectionVisible),
            o => o.IsTimeSelectionVisible,
            (o, v) => o.IsTimeSelectionVisible = v);

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
    private bool _isTimeSelectionVisible;

    internal bool IsButtonsPanelVisible
    {
        get => _buttonsPanelVisible;
        set => SetAndRaise(IsButtonsPanelVisibleProperty, ref _buttonsPanelVisible, value);
    }

    internal bool IsTimeSelectionVisible
    {
        get => _isTimeSelectionVisible;
        set => SetAndRaise(IsTimeSelectionVisibleProperty, ref _isTimeSelectionVisible, value);
    }

    public TimeSpan? TempSelectedTime
    {
        get => GetValue(TempSelectedTimeProperty);
        set => SetValue(TempSelectedTimeProperty, value);
    }

    #endregion

    #region 公共事件定义

    /// <summary>
    /// 当前 Pointer 选中的日期和时间的变化事件
    /// </summary>
    public event EventHandler<DateSelectedEventArgs>? HoverDateTimeChanged;

    /// <summary>
    /// 当前是否处于选择中状态
    /// </summary>
    public event EventHandler<ChoosingStatusEventArgs>? ChoosingStatusChanged;

    #endregion

    protected Button? NowButton;
    protected Button? TodayButton;
    protected Button? ConfirmButton;
    protected PickerCalendar? CalendarView;
    protected TimeView? TimeView;
    private CompositeDisposable? _pointerDisposables;
    private DateTime? _pendingOpenDisplayAnchor;
    private DatePickerDateRangeConstraint _effectiveDateRange;

    protected DatePickerDateRangeConstraint EffectiveDateRange => _effectiveDateRange;

    internal void ResetOpenPanelState()
    {
        _pendingOpenDisplayAnchor = ResolveOpenDisplayAnchor();
        ApplyPendingOpenPanelState();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RefreshPointerSubscriptions();
    }

    private void RefreshPointerSubscriptions()
    {
        _pointerDisposables?.Dispose();
        _pointerDisposables = null;
        if (CalendarView is not null)
        {
            _pointerDisposables ??= new CompositeDisposable(2);
            _pointerDisposables.Add(CalendarView.GetObservable(PickerCalendar.IsPointerInMonthViewProperty)
                .Subscribe(EmitChoosingStatusChanged));
        }
        if (TimeView is not null)
        {
            _pointerDisposables ??= new CompositeDisposable(2);
            _pointerDisposables.Add(TimeView.GetObservable(TimeView.IsPointerInSelectorProperty)
                .Subscribe(EmitChoosingStatusChanged));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerDisposables?.Dispose();
        _pointerDisposables = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsNeedConfirmProperty ||
            change.Property == IsShowNowProperty ||
            change.Property == IsShowTimeProperty ||
            change.Property == PickerModeProperty ||
            change.Property == MinDateProperty ||
            change.Property == MaxDateProperty)
        {
            SynchronizeCalendarState();
            SetupButtonStatus();
        }
        else if (change.Property == SelectedDateTimeProperty)
        {
            SynchronizeCalendarState();
        }
    }

    protected virtual void SetupConfirmButtonEnableStatus()
    {
        if (ConfirmButton is not null)
        {
            ConfirmButton.IsEnabled = EffectiveDateRange.Contains(SelectedDateTime);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachTemplateEventHandlers();
        base.OnApplyTemplate(e);
        ResolveTemplateParts(e);
        SynchronizeCalendarState();
        SetupButtonStatus();
        AttachTemplateEventHandlers();
        SetupConfirmButtonEnableStatus();
        RefreshPointerSubscriptionsIfAttached();
        ApplyPendingOpenPanelState();
    }

    private void ResolveTemplateParts(TemplateAppliedEventArgs e)
    {
        NowButton     = e.NameScope.Get<Button>("PART_NowButton");
        TodayButton   = e.NameScope.Get<Button>("PART_TodayButton");
        ConfirmButton = e.NameScope.Get<Button>("PART_ConfirmButton");
        CalendarView  = e.NameScope.Get<PickerCalendar>("PART_CalendarView");
        TimeView      = e.NameScope.Find<TimeView>("PART_TimeView");
    }

    private void AttachTemplateEventHandlers()
    {
        if (CalendarView is not null)
        {
            CalendarView.HoverDateChanged += HandleCalendarViewDateHoverChanged;
            CalendarView.DateSelected     += HandleCalendarViewDateSelected;
        }

        if (TimeView is not null)
        {
            if (IsTimeSelectionVisible)
            {
                SyncTimeViewTimeValue();
            }

            TimeView.HoverTimeChanged += HandleTimeViewHoverChanged;
            TimeView.TimeSelected     += HandleTimeViewTimeSelected;
            TimeView.TempTimeSelected += HandleTimeViewTempTimeSelected;
        }

        if (TodayButton is not null)
        {
            TodayButton.Click          += HandleTodayButtonClicked;
            TodayButton.PointerEntered += HandleTodayButtonPointerEntered;
            TodayButton.PointerExited  += HandleTodayButtonPointerExited;
        }

        if (NowButton is not null)
        {
            NowButton.Click          += HandleNowButtonClicked;
            NowButton.PointerEntered += HandleNowButtonPointerEntered;
            NowButton.PointerExited  += HandleNowButtonPointerExited;
        }

        if (ConfirmButton is not null)
        {
            ConfirmButton.Click          += HandleConfirmButtonClicked;
            ConfirmButton.IsEnabled      =  SelectedDateTime is not null;
            ConfirmButton.PointerEntered += HandleConfirmButtonPointerEntered;
            ConfirmButton.PointerExited  += HandleConfirmButtonPointerExited;
        }
    }

    private void RefreshPointerSubscriptionsIfAttached()
    {
        if (this.IsAttachedToVisualTree())
        {
            RefreshPointerSubscriptions();
        }
    }

    private void DetachTemplateEventHandlers()
    {
        if (CalendarView is not null)
        {
            CalendarView.HoverDateChanged -= HandleCalendarViewDateHoverChanged;
            CalendarView.DateSelected     -= HandleCalendarViewDateSelected;
        }

        if (TimeView is not null)
        {
            TimeView.HoverTimeChanged -= HandleTimeViewHoverChanged;
            TimeView.TimeSelected     -= HandleTimeViewTimeSelected;
            TimeView.TempTimeSelected -= HandleTimeViewTempTimeSelected;
        }

        if (TodayButton is not null)
        {
            TodayButton.Click          -= HandleTodayButtonClicked;
            TodayButton.PointerEntered -= HandleTodayButtonPointerEntered;
            TodayButton.PointerExited  -= HandleTodayButtonPointerExited;
        }

        if (NowButton is not null)
        {
            NowButton.Click          -= HandleNowButtonClicked;
            NowButton.PointerEntered -= HandleNowButtonPointerEntered;
            NowButton.PointerExited  -= HandleNowButtonPointerExited;
        }

        if (ConfirmButton is not null)
        {
            ConfirmButton.Click          -= HandleConfirmButtonClicked;
            ConfirmButton.PointerEntered -= HandleConfirmButtonPointerEntered;
            ConfirmButton.PointerExited  -= HandleConfirmButtonPointerExited;
        }
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
        EmitChoosingStatusChanged(false);
    }
    
    protected virtual void NotifyPointerEnterTodayButton()
    {
        var hoverDateTime =
            CollectDateTime(DateTime.Now, TimeSpan.Zero);
        EmitHoverDateTimeChanged(hoverDateTime);
    }

    protected virtual void NotifyPointerExitTodayButton()
    {
        EmitChoosingStatusChanged(false);
    }
    
    protected virtual void NotifyPointerEnterNowButton()
    {
        var hoverDateTime =
            CollectDateTime(DateTime.Now, DateTime.Now.TimeOfDay);
        EmitHoverDateTimeChanged(hoverDateTime);
    }

    protected virtual void NotifyPointerExitNowButton()
    {
        EmitChoosingStatusChanged(false);
    }

    protected virtual DateTime? ResolveOpenDisplayAnchor()
    {
        var anchor = SelectedDateTime ?? PickerDisplayDate;
        return anchor.HasValue
            ? DatePickerFormattingHelper.NormalizeDateTime(anchor.Value, PickerMode)
            : null;
    }

    protected void ApplyCalendarDisplayAnchor(PickerCalendar calendar, DateTime anchor)
    {
        anchor = EffectiveDateRange.Clamp(anchor);
        calendar.SetCurrentValue(PickerCalendar.DisplayDateProperty, anchor);
        calendar.SelectedMonth    = anchor;
        calendar.SelectedYear     = anchor;
        calendar.LastSelectedDate = anchor;
        calendar.UpdateHighlightDays();
    }

    private void ApplyPendingOpenPanelState()
    {
        if (_pendingOpenDisplayAnchor is null || CalendarView is null)
        {
            return;
        }

        ApplyCalendarDisplayAnchor(CalendarView, _pendingOpenDisplayAnchor.Value);
        _pendingOpenDisplayAnchor = null;
    }

    protected virtual void SynchronizeCalendarState()
    {
        _effectiveDateRange = DatePickerDateRangeConstraint.Create(MinDate, MaxDate, PickerMode);
        if (CalendarView is null)
        {
            SetupConfirmButtonEnableStatus();
            return;
        }

        CalendarView.SetCurrentValue(PickerCalendar.PickerModeProperty, PickerMode);
        CalendarView.SetCurrentValue(PickerCalendar.DisplayDateStartProperty, _effectiveDateRange.Start);
        CalendarView.SetCurrentValue(PickerCalendar.DisplayDateEndProperty, _effectiveDateRange.End);
        CalendarView.SetCurrentValue(
            PickerCalendar.SelectedDateProperty,
            GetValidCalendarDate(SelectedDateTime));
        SetupConfirmButtonEnableStatus();
    }

    protected DateTime? GetValidCalendarDate(DateTime? dateTime)
    {
        return dateTime.HasValue && EffectiveDateRange.Contains(dateTime)
            ? EffectiveDateRange.Normalize(dateTime.Value)
            : null;
    }

    private void HandleTodayButtonClicked(object? sender, RoutedEventArgs args)
    {
        NotifyTodayButtonClicked();
    }

    private void HandleTodayButtonPointerEntered(object? sender, PointerEventArgs args)
    {
        NotifyPointerEnterTodayButton();
    }

    private void HandleTodayButtonPointerExited(object? sender, PointerEventArgs args)
    {
        NotifyPointerExitTodayButton();
    }

    protected virtual void NotifyTodayButtonClicked()
    {
        if (!EffectiveDateRange.Contains(DateTime.Today))
        {
            return;
        }

        SetCurrentValue(SelectedDateTimeProperty, DateTime.Today);
        
        CalendarView?.SetCurrentValue(PickerCalendar.DisplayDateProperty, DateTime.Today);

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }

    private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
    {
        NotifyNowButtonClicked();
    }

    private void HandleNowButtonPointerEntered(object? sender, PointerEventArgs args)
    {
        NotifyPointerEnterNowButton();
    }

    private void HandleNowButtonPointerExited(object? sender, PointerEventArgs args)
    {
        NotifyPointerExitNowButton();
    }

    protected virtual void NotifyNowButtonClicked()
    {
        if (!EffectiveDateRange.Contains(DateTime.Now))
        {
            return;
        }

        if (CalendarView is not null)
        {
            CalendarView?.SetCurrentValue(PickerCalendar.SelectedDateProperty, DateTime.Now);
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

    private void HandleConfirmButtonPointerEntered(object? sender, PointerEventArgs args)
    {
        NotifyPointerEnterConfirmButton();
    }

    private void HandleConfirmButtonPointerExited(object? sender, PointerEventArgs args)
    {
        NotifyPointerExitConfirmButton();
    }

    protected virtual void NotifyConfirmButtonClicked()
    {
        if (EffectiveDateRange.Contains(SelectedDateTime))
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
        SetCurrentValue(SelectedDateTimeProperty, CollectDateTime(CalendarView?.SelectedDate, TempSelectedTime ?? TimeView?.SelectedTime));
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
        if (IsTimeSelectionVisible && timeSpan is not null)
        {
            date = date.Value.Add(timeSpan.Value);
        }

        return date;
    }

    private void SetupButtonStatus()
    {
        IsTimeSelectionVisible = IsShowTime && PickerMode == DatePickerMode.Date;

        if (NowButton is null ||
            TodayButton is null ||
            ConfirmButton is null)
        {
            return;
        }

        ConfirmButton.IsVisible = IsNeedConfirm;
        TodayButton.IsEnabled   = EffectiveDateRange.Contains(DateTime.Today);
        NowButton.IsEnabled     = EffectiveDateRange.Contains(DateTime.Now);

        NowButton.IsVisible             = false;
        TodayButton.IsVisible           = false;
        NowButton.HorizontalAlignment   = HorizontalAlignment.Left;
        TodayButton.HorizontalAlignment = HorizontalAlignment.Left;

        if (IsShowNow && PickerMode == DatePickerMode.Date)
        {
            NowButton.IsVisible   = false;
            TodayButton.IsVisible = false;
            if (IsTimeSelectionVisible)
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

        IsButtonsPanelVisible = NowButton.IsVisible || TodayButton.IsVisible || ConfirmButton.IsVisible;
    }

    protected override void OnConfirmed()
    {
        CalendarView?.SetCurrentValue(PickerCalendar.SelectedDateProperty, SelectedDateTime);
        EmitChoosingStatusChanged(false);
        base.OnConfirmed();
    }

    internal void EmitConfirmed()
    {
        base.OnConfirmed();
    }

    protected void EmitChoosingStatusChanged(bool isChoosing)
    {
        ChoosingStatusChanged?.Invoke(this, new ChoosingStatusEventArgs(isChoosing));
    }

    protected override void OnDismiss()
    {
        base.OnDismiss();
        SetCurrentValue(SelectedDateTimeProperty, null);
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
