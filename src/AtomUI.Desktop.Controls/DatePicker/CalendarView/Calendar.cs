using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.CalendarView.Infrastructure;
using AtomUI.Desktop.Controls.CalendarView.State;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.CalendarView;

public class DateSelectedEventArgs : EventArgs
{
    public DateTime? Date { get; }
    public DateSelectedEventArgs(DateTime? value)
    {
        Date = value;
    }
}

[TemplatePart("PART_CalendarItem", typeof(CalendarItem))]
internal class Calendar : TemplatedControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<Calendar, DayOfWeek>(
            nameof(FirstDayOfWeek),
            DayOfWeek.Sunday);
    
    public static readonly StyledProperty<bool> IsTodayHighlightedProperty =
        AvaloniaProperty.Register<Calendar, bool>(
            nameof(IsTodayHighlighted),
            true);
    
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<Calendar, IBrush?>(nameof(HeaderBackground));
    
    public static readonly StyledProperty<CalendarMode> DisplayModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarMode>(
            nameof(DisplayMode),
            validate: IsValidDisplayMode);
    
    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(nameof(DisplayDate),
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<DateTime?> DisplayDateStartProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(DisplayDateStart),
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<DateTime?> DisplayDateEndProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(DisplayDateEnd),
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(SelectedDate));

    public static readonly StyledProperty<DatePickerMode> PickerModeProperty =
        DatePicker.PickerModeProperty.AddOwner<Calendar>();
    
    /// <summary>
    /// Gets or sets the day that is considered the beginning of the week.
    /// </summary>
    /// <value>
    /// A <see cref="T:System.DayOfWeek" /> representing the beginning of
    /// the week. The default is <see cref="F:System.DayOfWeek.Sunday" />.
    /// </value>
    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the current date is
    /// highlighted.
    /// </summary>
    /// <value>
    /// True if the current date is highlighted; otherwise, false. The
    /// default is true.
    /// </value>
    public bool IsTodayHighlighted
    {
        get => GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }
    
    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the calendar is displayed in
    /// months, years, or decades.
    /// </summary>
    /// <value>
    /// A value indicating what length of time the
    /// calendar should display.
    /// </value>
    public CalendarMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the date to display.
    /// </summary>
    /// <value>The date to display.</value>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The given date is not in the range specified by
    /// <see cref="DisplayDateStart" />
    /// and
    /// <see cref="DisplayDateEnd" />.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This property allows the developer to specify a date to display. If
    /// this property is unset, <see cref="SelectedDate" /> is displayed. If
    /// <see cref="SelectedDate" /> is also unset, today is displayed.
    /// </para>
    /// <para>
    /// To set this property in XAML, use a date specified in the format
    /// yyyy/mm/dd.  The mm and dd components must always consist of two
    /// characters, with a leading zero if necessary.  For instance, the
    /// month of May should be specified as 05.
    /// </para>
    /// </remarks>
    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the first date to be displayed.
    /// </summary>
    /// <value>The first date to display.</value>
    /// <remarks>
    /// To set this property in XAML, use a date specified in the format
    /// yyyy/mm/dd.  The mm and dd components must always consist of two
    /// characters, with a leading zero if necessary.  For instance, the
    /// month of May should be specified as 05.
    /// </remarks>
    public DateTime? DisplayDateStart
    {
        get => GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    /// <summary>
    /// Gets or sets the last date to be displayed.
    /// </summary>
    /// <value>The last date to display.</value>
    /// <remarks>
    /// To set this property in XAML, use a date specified in the format
    /// yyyy/mm/dd.  The mm and dd components must always consist of two
    /// characters, with a leading zero if necessary.  For instance, the
    /// month of May should be specified as 05.
    /// </remarks>
    public DateTime? DisplayDateEnd
    {
        get => GetValue(DisplayDateEndProperty);
        set => SetValue(DisplayDateEndProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the currently selected date.
    /// </summary>
    /// <value>The date currently selected. The default is null.</value>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The given date is outside the range specified by
    /// <see cref="DisplayDateStart" />
    /// and <see cref="DisplayDateEnd" />
    /// -or-
    /// The given date is in the
    /// <see cref="BlackoutDates" />
    /// collection.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// If set to anything other than null when
    /// <see cref="P:SelectionMode" /> is
    /// set to
    /// <see cref="F:Controls.CalendarSelectionMode.None" />.
    /// </exception>
    /// <remarks>
    /// Use this property when SelectionMode is set to SingleDate.  In other
    /// modes, this property will always be the first date in SelectedDates.
    /// </remarks>
    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public DatePickerMode PickerMode
    {
        get => GetValue(PickerModeProperty);
        set => SetValue(PickerModeProperty, value);
    }
    
    /// <summary>
    /// Gets a collection of dates that are marked as not selectable.
    /// </summary>
    /// <value>
    /// A collection of dates that cannot be selected. The default value is
    /// an empty collection.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Adding a date to this collection when it is already selected or
    /// adding a date outside the range specified by DisplayDateStart and
    /// DisplayDateEnd.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Dates in this collection will appear as disabled on the calendar.
    /// </para>
    /// <para>
    /// To make all past dates not selectable, you can use the
    /// AddDatesInPast method provided by the collection returned by this
    /// property.
    /// </para>
    /// </remarks>
    public CalendarBlackoutDatesCollection BlackoutDates { get; }
    
    #endregion

    #region 公共事件定义

    /// <summary>
    /// Occurs when the
    /// <see cref="DisplayDate" />
    /// property is changed.
    /// </summary>
    /// <remarks>
    /// This event occurs after DisplayDate is assigned its new value.
    /// </remarks>
    public event EventHandler<CalendarDateChangedEventArgs>? DisplayDateChanged;

    /// <summary>
    /// Occurs when the
    /// <see cref="DisplayMode" />
    /// property is changed.
    /// </summary>
    public event EventHandler<CalendarModeChangedEventArgs>? DisplayModeChanged;
    
    /// <summary>
    /// 日期选中事件
    /// </summary>
    public event EventHandler<DateSelectedEventArgs>? DateSelected;
    
    /// <summary>
    /// 当前 Pointer 选中的日期变化事件
    /// </summary>
    public event EventHandler<DateSelectedEventArgs>? HoverDateChanged;
    
    #endregion

    #region 内部事件定义

    internal event EventHandler<PointerReleasedEventArgs>? DayButtonMouseUp;

    #endregion

    #region 内部协作 API

    internal const int RowsPerMonth = 7;
    internal const int ColumnsPerMonth = 7;
    internal const int ColumnsPerWeekPanel = ColumnsPerMonth + 1;
    internal const int RowsPerYear = 3;
    internal const int ColumnsPerYear = 4;
    internal const int RowsPerMonthSelectionPanel = 4;
    internal const int ColumnsPerMonthSelectionPanel = 3;

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Calendar>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal CalendarDayButton? FocusButton { get; set; }
    internal CalendarButton? FocusCalendarButton { get; set; }
    internal CalendarItem? CalendarItem { get; private set;}
    
    internal DateTime? LastSelectedDateInternal { get; set; }

    internal DateTime? LastSelectedDate
    {
        get => LastSelectedDateInternal;

        set
        {
            LastSelectedDateInternal = value;

            if (FocusButton != null)
            {
                FocusButton.IsCurrent = false;
                FocusButton = null;
            }

            if (value.HasValue)
            {
                FocusButton = FindDayButtonFromDay(value.Value);
                if (FocusButton != null)
                {
                    FocusButton.IsCurrent = HasFocusInternal;
                }
            }
        }
    }
    
    private DateTime _selectedMonth;
    internal DateTime SelectedMonth
    {
        get => _selectedMonth;

        set
        {
            var monthDifferenceStart = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeStart);
            var monthDifferenceEnd   = DateTimeHelper.CompareYearMonth(value, DisplayDateRangeEnd);

            if (monthDifferenceStart >= 0 && monthDifferenceEnd <= 0)
            {
                _selectedMonth = DateTimeHelper.DiscardDayTime(value);
            }
            else
            {
                if (monthDifferenceStart < 0)
                {
                    _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
                }
                else
                {
                    Debug.Assert(monthDifferenceEnd > 0, "monthDifferenceEnd should be greater than 0!");
                    _selectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
                }
            }
        }
    }

    private DateTime _selectedYear;
    internal DateTime SelectedYear
    {
        get => _selectedYear;

        set
        {
            if (value.Year < DisplayDateRangeStart.Year)
            {
                _selectedYear = DisplayDateRangeStart;
            }
            else
            {
                if (value.Year > DisplayDateRangeEnd.Year)
                {
                    _selectedYear = DisplayDateRangeEnd;
                }
                else
                {
                    _selectedYear = value;
                }
            }
        }
    }

    internal DateTime DisplayDateInternal { get; set; }
    internal DateTime DisplayDateRangeStart => DisplayDateStart.GetValueOrDefault(DateTime.MinValue);
    internal DateTime DisplayDateRangeEnd => DisplayDateEnd.GetValueOrDefault(DateTime.MaxValue);
    internal bool HasFocusInternal { get; set; }
    internal DateTime? HoverDate { get; private set; }

    internal static readonly StyledProperty<bool> IsPointerInMonthViewProperty =
        AvaloniaProperty.Register<Calendar, bool>(nameof(IsPointerInMonthView), false);

    internal bool IsPointerInMonthView
    {
        get => GetValue(IsPointerInMonthViewProperty);
        set => SetValue(IsPointerInMonthViewProperty, value);
    }

    #endregion
    
    private readonly CalendarCultureContext _cultureContext = new();
    private CalendarViewStateController? _stateController;
    
    static Calendar()
    {
        IsEnabledProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnIsEnabledChanged(e));
        FirstDayOfWeekProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnFirstDayOfWeekChanged(e));
        IsTodayHighlightedProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnIsTodayHighlightedChanged(e));
        DisplayModeProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnDisplayModePropertyChanged(e));
        DisplayDateProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnDisplayDateChanged(e));
        DisplayDateStartProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnDisplayDateStartChanged(e));
        DisplayDateEndProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnDisplayDateEndChanged(e));
        SelectedDateProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnSelectedDateChanged(e));
        PickerModeProperty.Changed.AddClassHandler<Calendar>((x, e) => x.OnPickerModeChanged(e));
        KeyDownEvent.AddClassHandler<Calendar>((x, e) => x.HandleCalendarKeyDown(e));
        HorizontalAlignmentProperty.OverrideDefaultValue<Calendar>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<Calendar>(VerticalAlignment.Top);
    }

    public Calendar()
    {
        BlackoutDates = new CalendarBlackoutDatesCollection(this);
        _stateController = new CalendarViewStateController(
            CalendarViewState.CreateDefault(DateTime.Today, _cultureContext.CurrentFormat));
        SetCurrentValue(DisplayDateProperty, DateTime.Today);
        UpdateDisplayDate(this, DisplayDate, DateTime.MinValue);
    }

    internal CalendarViewState SyncAndGetCurrentViewState()
    {
        SyncViewStateFromCurrentProperties();
        return _stateController?.State
               ?? throw new InvalidOperationException("Calendar view state controller is not initialized.");
    }

    protected void ApplyViewStateAction(CalendarViewAction action)
    {
        _stateController?.Apply(action);
    }

    internal void RefreshCultureFromThemeManager()
    {
        _cultureContext.RefreshFromThemeManager();
        ApplyViewStateAction(CalendarViewAction.SetCulture(_cultureContext.CurrentFormat));
    }

    protected virtual void SyncViewStateFromCurrentProperties()
    {
        ApplyViewStateAction(CalendarViewAction.SetDisplayRange(DisplayDateStart, DisplayDateEnd));
        ApplyViewStateAction(CalendarViewAction.SetDisplayDate(DisplayDate));
        ApplyViewStateAction(CalendarViewAction.SetSelectedDate(SelectedDate));
        ApplyViewStateAction(CalendarViewAction.SetSelectedMonth(SelectedMonth));
        ApplyViewStateAction(CalendarViewAction.SetSelectedYear(SelectedYear));
        ApplyViewStateAction(CalendarViewAction.SetFocusedDate(LastSelectedDate));
        ApplyViewStateAction(CalendarViewAction.SetHoverDate(HoverDate));
        ApplyViewStateAction(CalendarViewAction.SetBlackoutDates(BlackoutDates));
        ApplyViewStateAction(CalendarViewAction.SetFirstDayOfWeek(FirstDayOfWeek));
        ApplyViewStateAction(CalendarViewAction.SetTodayHighlighted(IsTodayHighlighted));
        ApplyViewStateAction(CalendarViewAction.SetDisplayMode(DisplayMode));
        ApplyViewStateAction(CalendarViewAction.SetPickerMode(PickerMode));
        ApplyViewStateAction(CalendarViewAction.SetCulture(_cultureContext.CurrentFormat));
    }
    
    /// <summary>
    /// FirstDayOfWeekProperty property changed handler.
    /// </summary>
    /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
    private void OnFirstDayOfWeekChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (IsValidFirstDayOfWeek(change.NewValue!))
        {
            SyncViewStateFromCurrentProperties();
            UpdateMonths();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(change), "Invalid DayOfWeek");
        }
    }
    
    private static bool IsValidFirstDayOfWeek(object value)
    {
        var day = (DayOfWeek)value;

        return day == DayOfWeek.Sunday
               || day == DayOfWeek.Monday
               || day == DayOfWeek.Tuesday
               || day == DayOfWeek.Wednesday
               || day == DayOfWeek.Thursday
               || day == DayOfWeek.Friday
               || day == DayOfWeek.Saturday;
    }
    
    /// <summary>
    /// IsTodayHighlightedProperty property changed handler.
    /// </summary>
    /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
    private void OnIsTodayHighlightedChanged(AvaloniaPropertyChangedEventArgs change)
    {
        SyncViewStateFromCurrentProperties();
        var i = DateTimeHelper.CompareYearMonth(DisplayDateInternal, DateTime.Today);

        if (i > -2 && i < 2)
        {
            UpdateMonths();
        }
    }
    
    /// <summary>
    /// DisplayModeProperty property changed handler.
    /// </summary>
    /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
    private void OnDisplayModePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var mode    = (CalendarMode)change.NewValue!;
        var oldMode = (CalendarMode)change.OldValue!;

        if (CalendarItem != null)
        {
            switch (oldMode)
            {
                case CalendarMode.Month:
                {
                    SelectedYear  = DisplayDateInternal;
                    SelectedMonth = DisplayDateInternal;
                    break;
                }
                case CalendarMode.Year:
                {
                    SetCurrentValue(DisplayDateProperty, SelectedMonth);
                    SelectedYear = SelectedMonth;
                    break;
                }
                case CalendarMode.Decade:
                {
                    SetCurrentValue(DisplayDateProperty, SelectedYear);
                    SelectedMonth = SelectedYear;
                    break;
                }
            }

            switch (mode)
            {
                case CalendarMode.Month:
                {
                    OnMonthClick();
                    break;
                }
                case CalendarMode.Year:
                case CalendarMode.Decade:
                {
                    OnHeaderClick();
                    break;
                }
            }
        }

        OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)change.OldValue, mode));
        SyncViewStateFromCurrentProperties();
    }

    private void OnPickerModeChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var targetDisplayMode = GetTargetDisplayMode((DatePickerMode)change.NewValue!);
        if (DisplayMode != targetDisplayMode)
        {
            SetCurrentValue(DisplayModeProperty, targetDisplayMode);
        }

        if (PickerMode == DatePickerMode.Week && FirstDayOfWeek != DayOfWeek.Monday)
        {
            SetCurrentValue(FirstDayOfWeekProperty, DayOfWeek.Monday);
        }

        SyncViewStateFromCurrentProperties();
        UpdateMonths();
    }

    internal static CalendarMode GetTargetDisplayMode(DatePickerMode pickerMode)
    {
        return pickerMode switch
        {
            DatePickerMode.Month or DatePickerMode.Quarter => CalendarMode.Year,
            DatePickerMode.Year                            => CalendarMode.Decade,
            _                                              => CalendarMode.Month
        };
    }
    
    private static bool IsValidDisplayMode(CalendarMode mode)
    {
        return mode == CalendarMode.Month
               || mode == CalendarMode.Year
               || mode == CalendarMode.Decade;
    }
    
    private void OnDisplayModeChanged(CalendarModeChangedEventArgs args)
    {
        DisplayModeChanged?.Invoke(this, args);
    }

    protected virtual void OnDisplayDateChanged(AvaloniaPropertyChangedEventArgs change)
    {
        UpdateDisplayDate(this, (DateTime)change.NewValue!, (DateTime)change.OldValue!);
        SyncViewStateFromCurrentProperties();
    }

    private static void UpdateDisplayDate(Calendar c, DateTime addedDate, DateTime removedDate)
    {
        _ = c ?? throw new ArgumentNullException(nameof(c));

        // If DisplayDate < DisplayDateStart, DisplayDate = DisplayDateStart
        if (DateTime.Compare(addedDate, c.DisplayDateRangeStart) < 0)
        {
            c.DisplayDate = c.DisplayDateRangeStart;
            return;
        }

        // If DisplayDate > DisplayDateEnd, DisplayDate = DisplayDateEnd
        if (DateTime.Compare(addedDate, c.DisplayDateRangeEnd) > 0)
        {
            c.DisplayDate = c.DisplayDateRangeEnd;
            return;
        }

        c.SetupDisplayDateInternal(addedDate);
        c.UpdateMonths();
        c.OnDisplayDate(new CalendarDateChangedEventArgs(removedDate, addedDate));
    }

    protected virtual void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var selectedDate = change.NewValue as DateTime?;
        if (selectedDate.HasValue)
        {
            var normalizedDate = NormalizePickerDate(selectedDate.Value);
            if (DateTimeHelper.CompareDays(normalizedDate, selectedDate.Value) != 0)
            {
                SetCurrentValue(SelectedDateProperty, normalizedDate);
                return;
            }
        }

        if (!IsValidDateSelection(this, selectedDate))
        {
            SetCurrentValue(SelectedDateProperty, change.OldValue as DateTime?);
            SyncViewStateFromCurrentProperties();
            throw new ArgumentOutOfRangeException(nameof(change), "SelectedDate value is not valid.");
        }

        LastSelectedDate = selectedDate;

        if (selectedDate.HasValue &&
            ShouldSelectedDateUpdateDisplayDate(selectedDate.Value) &&
            DateTimeHelper.CompareYearMonth(selectedDate.Value, DisplayDateInternal) != 0)
        {
            SetCurrentValue(DisplayDateProperty, selectedDate.Value);
        }
        else
        {
            UpdateMonths();
        }
        SyncViewStateFromCurrentProperties();
    }

    protected virtual bool ShouldSelectedDateUpdateDisplayDate(DateTime selectedDate)
    {
        return true;
    }

    protected virtual void SetupDisplayDateInternal(DateTime displayDate)
    {
        DisplayDateInternal = DateTimeHelper.DiscardDayTime(displayDate);
    }
    
    protected void OnDisplayDate(CalendarDateChangedEventArgs e)
    {
        DisplayDateChanged?.Invoke(this, e);
    }

    private void OnDisplayDateStartChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var newValue = change.NewValue as DateTime?;

        if (newValue.HasValue)
        {
            if (SelectedDate.HasValue && DateTime.Compare(SelectedDate.Value, newValue.Value) < 0)
            {
                SetCurrentValue(SelectedDateProperty, newValue.Value);
                return;
            }

            // if DisplayDateStart > DisplayDateEnd,
            // DisplayDateEnd = DisplayDateStart
            if (DateTime.Compare(newValue.Value, DisplayDateRangeEnd) > 0)
            {
                SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
            }

            // If DisplayDate < DisplayDateStart,
            // DisplayDate = DisplayDateStart
            if (DateTimeHelper.CompareYearMonth(newValue.Value, DisplayDateInternal) > 0)
            {
                SetCurrentValue(DisplayDateProperty, newValue.Value);
            }
        }

        UpdateMonths();
        SyncViewStateFromCurrentProperties();
    }

    private void OnDisplayDateEndChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var newValue = change.NewValue as DateTime?;

        if (newValue.HasValue)
        {
            if (SelectedDate.HasValue && DateTime.Compare(SelectedDate.Value, newValue.Value) > 0)
            {
                SetCurrentValue(SelectedDateProperty, newValue.Value);
                return;
            }

            // if DisplayDateEnd < DisplayDateStart,
            // DisplayDateEnd = DisplayDateStart
            if (DateTime.Compare(newValue.Value, DisplayDateRangeStart) < 0)
            {
                SetCurrentValue(DisplayDateEndProperty, DisplayDateStart);
                return;
            }

            // If DisplayDate > DisplayDateEnd,
            // DisplayDate = DisplayDateEnd
            if (DateTimeHelper.CompareYearMonth(newValue.Value, DisplayDateInternal) < 0)
            {
                SetCurrentValue(DisplayDateProperty, newValue.Value);
            }
        }

        UpdateMonths();
        SyncViewStateFromCurrentProperties();
    }

    internal CalendarDayButton? FindDayButtonFromDay(DateTime day)
    {
        if (CalendarItem?.MonthView != null)
        {
            foreach (var b in CalendarItem.MonthView.Children.OfType<CalendarDayButton>())
            {
                if (b.IsWeekNumber)
                {
                    continue;
                }

                var d = b.DataContext as DateTime?;
                if (d.HasValue && DateTimeHelper.CompareDays(d.Value, day) == 0)
                {
                    return b;
                }
            }
        }

        return null;
    }

    private void OnSelectedMonthChanged(DateTime? selectedMonth)
    {
        if (selectedMonth.HasValue)
        {
            Debug.Assert(DisplayMode == CalendarMode.Year, "DisplayMode should be Year!");
            SelectedMonth = selectedMonth.Value;
            UpdateMonths();
        }
    }

    private void OnSelectedYearChanged(DateTime? selectedYear)
    {
        if (selectedYear.HasValue)
        {
            Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");
            SelectedYear = selectedYear.Value;
            UpdateMonths();
        }
    }

    internal void OnHeaderClick()
    {
        Debug.Assert(DisplayMode == CalendarMode.Year || DisplayMode == CalendarMode.Decade,
            "The DisplayMode should be Year or Decade");
        if (CalendarItem != null && CalendarItem.MonthView != null && CalendarItem.YearView != null)
        {
            CalendarItem.MonthView.IsVisible = false;
            CalendarItem.YearView.IsVisible  = true;
            UpdateMonths();
        }
    }

    internal virtual void ResetStates()
    {
        if (CalendarItem?.MonthView != null)
        {
            foreach (var d in CalendarItem.MonthView.Children.OfType<CalendarDayButton>())
            {
                d.IgnoreMouseOverState();
            }
        }
    }

    protected internal virtual void UpdateMonths()
    {
        if (CalendarItem != null)
        {
            UpdateCalendarMonths(CalendarItem);
        }
    }

    internal void UpdateCalendarMonths(CalendarItem calendarItem)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                calendarItem.UpdateMonthMode();
                break;
            }
            case CalendarMode.Year:
            {
                calendarItem.UpdateYearMode();
                break;
            }
            case CalendarMode.Decade:
            {
                calendarItem.UpdateDecadeMode();
                break;
            }
        }
    }

    internal static bool IsValidDateSelection(Calendar cal, DateTime? value)
    {
        if (!value.HasValue)
        {
            return true;
        }

        if (cal.BlackoutDates.Contains(value.Value))
        {
            return false;
        }

        return DateTime.Compare(value.Value, cal.DisplayDateRangeStart) >= 0 &&
               DateTime.Compare(value.Value, cal.DisplayDateRangeEnd) <= 0;
    }

    internal void OnPreviousMonthClick()
    {
        if (DisplayMode == CalendarMode.Month)
        {
            var d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
            if (d.HasValue)
            {
                if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                {
                    LastSelectedDate = d.Value;
                }

                SetCurrentValue(DisplayDateProperty, d.Value);
            }
        }
    }

    internal void OnPreviousClick()
    {
        if (DisplayMode == CalendarMode.Month)
        {
            var d = DateTimeHelper.AddYears(DateTimeHelper.DiscardDayTime(DisplayDate), -1);
            if (d.HasValue)
            {
                if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                {
                    LastSelectedDate = d.Value;
                }

                SetCurrentValue(DisplayDateProperty, d.Value);
            }
        }
        else if (DisplayMode == CalendarMode.Year)
        {
            var d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), -1);

            if (d.HasValue)
            {
                SelectedMonth = d.Value;
            }
            else
            {
                SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
            }
        }
        else if (DisplayMode == CalendarMode.Decade)
        {
            Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade!");

            var d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), -10);

            if (d.HasValue)
            {
                var decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
                SelectedYear = new DateTime(decade, 1, 1);
            }
            else
            {
                SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeStart);
            }
        }

        UpdateMonths();
    }

    internal void OnNextMonthClick()
    {
        if (DisplayMode == CalendarMode.Month)
        {
            var d = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
            if (d.HasValue)
            {
                if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                {
                    LastSelectedDate = d.Value;
                }

                SetCurrentValue(DisplayDateProperty, d.Value);
            }
        }
    }

    internal void OnNextClick()
    {
        if (DisplayMode == CalendarMode.Month)
        {
            var d = DateTimeHelper.AddYears(DateTimeHelper.DiscardDayTime(DisplayDate), 1);
            if (d.HasValue)
            {
                if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, d.Value) != 0)
                {
                    LastSelectedDate = d.Value;
                }

                SetCurrentValue(DisplayDateProperty, d.Value);
            }
        }
        else if (DisplayMode == CalendarMode.Year)
        {
            var d = DateTimeHelper.AddYears(new DateTime(SelectedMonth.Year, 1, 1), 1);

            if (d.HasValue)
            {
                SelectedMonth = d.Value;
            }
            else
            {
                SelectedMonth = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
            }
        }
        else if (DisplayMode == CalendarMode.Decade)
        {
            Debug.Assert(DisplayMode == CalendarMode.Decade, "DisplayMode should be Decade");

            var d = DateTimeHelper.AddYears(new DateTime(SelectedYear.Year, 1, 1), 10);

            if (d.HasValue)
            {
                var decade = Math.Max(1, DateTimeHelper.DecadeOfDate(d.Value));
                SelectedYear = new DateTime(decade, 1, 1);
            }
            else
            {
                SelectedYear = DateTimeHelper.DiscardDayTime(DisplayDateRangeEnd);
            }
        }

        UpdateMonths();
    }

    /// <summary>
    /// If the day is a trailing day, Update the DisplayDate.
    /// </summary>
    /// <param name="selectedDate">The selected day.</param>
    internal virtual void NotifyDayClick(DateTime selectedDate)
    {
        Debug.Assert(DisplayMode == CalendarMode.Month, "DisplayMode should be Month!");
        var i = DateTimeHelper.CompareYearMonth(selectedDate, DisplayDateInternal);
   
        if (i > 0)
        {
            OnNextMonthClick();
        }
        else if (i < 0)
        {
            OnPreviousMonthClick();
        }
    }
    
    internal void NotifyDateSelected()
    {
        DateSelected?.Invoke(this, new DateSelectedEventArgs(SelectedDate));
    }
    
    internal void NotifyDateSelected(DateTime? selected)
    {
        DateSelected?.Invoke(this, new DateSelectedEventArgs(selected));
    }

    internal DateTime NormalizePickerDate(DateTime date)
    {
        return DatePickerFormattingHelper.NormalizeDateTime(date, PickerMode, DayOfWeek.Monday);
    }

    internal void SelectPickerDate(DateTime date)
    {
        var normalizedDate = NormalizePickerDate(date);
        SetCurrentValue(SelectedDateProperty, normalizedDate);
        NotifyDateSelected(normalizedDate);
        UpdateHighlightDays();
    }
    
    internal virtual void NotifyHoverDateChanged(DateTime? hoverDate)
    {
        HoverDate = hoverDate.HasValue ? NormalizePickerDate(hoverDate.Value) : null;
        HoverDateChanged?.Invoke(this, new DateSelectedEventArgs(HoverDate));
    }

    private void OnMonthClick()
    {
        Debug.Assert(CalendarItem is not null);
        
        if (CalendarItem != null && CalendarItem.YearView != null && CalendarItem.MonthView != null)
        {
            CalendarItem.YearView.IsVisible  = false;
            CalendarItem.MonthView.IsVisible = true;

            if (!LastSelectedDate.HasValue || DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDate) != 0)
            {
                LastSelectedDate = DisplayDate;
            }

            UpdateMonths();
        }
    }

    public override string ToString()
    {
        if (SelectedDate != null)
        {
            return SelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat());
        }
        return string.Empty;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!HasFocusInternal && e.InitialPressMouseButton == MouseButton.Left)
        {
            Focus();
        }
    }

    internal void OnDayButtonMouseUp(PointerReleasedEventArgs e)
    {
        DayButtonMouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Default mouse wheel handler for the calendar control.
    /// </summary>
    /// <param name="e">Mouse wheel event args.</param>
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (!e.Handled)
        {
            CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out bool ctrl, out bool shift);

            if (!ctrl)
            {
                if (e.Delta.Y > 0)
                {
                    ProcessPageUpKey(false);
                }
                else
                {
                    ProcessPageDownKey(false);
                }
            }
            else
            {
                if (e.Delta.Y > 0)
                {
                    ProcessDownKey(ctrl, shift);
                }
                else
                {
                    ProcessUpKey(ctrl, shift);
                }
            }
            e.Handled = true;
        }
    }

    internal void HandleCalendarKeyDown(KeyEventArgs e)
    {
        if (!e.Handled && IsEnabled)
        {
            e.Handled = ProcessCalendarKey(e);
        }
    }

    internal bool ProcessCalendarKey(KeyEventArgs e)
    {
        if (DisplayMode == CalendarMode.Month)
        {
            if (LastSelectedDate.HasValue)
            {
                // If a blackout day is inactive, when clicked on it, the
                // previous inactive day which is not a blackout day can get
                // the focus.  In this case we should allow keyboard
                // functions on that inactive day
                if (DateTimeHelper.CompareYearMonth(LastSelectedDate.Value, DisplayDateInternal) != 0 &&
                    FocusButton != null && !FocusButton.IsInactive)
                {
                    return true;
                }
            }
        }

        // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
        var invariantKey = e.Key; //InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

        CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);

        switch (invariantKey)
        {
            case Key.Up:
            {
                ProcessUpKey(ctrl, shift);
                return true;
            }
            case Key.Down:
            {
                ProcessDownKey(ctrl, shift);
                return true;
            }
            case Key.Left:
            {
                ProcessLeftKey(shift);
                return true;
            }
            case Key.Right:
            {
                ProcessRightKey(shift);
                return true;
            }
            case Key.PageDown:
            {
                ProcessPageDownKey(shift);
                return true;
            }
            case Key.PageUp:
            {
                ProcessPageUpKey(shift);
                return true;
            }
            case Key.Home:
            {
                ProcessHomeKey(shift);
                return true;
            }
            case Key.End:
            {
                ProcessEndKey(shift);
                return true;
            }
            case Key.Enter:
            case Key.Space:
            {
                return ProcessEnterKey();
            }
        }

        return false;
    }

    internal void ProcessUpKey(bool ctrl, bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                if (ctrl)
                {
                    SelectedMonth = DisplayDateInternal;
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
                }
                break;
            }
            case CalendarMode.Year:
            {
                if (ctrl)
                {
                    SelectedYear = SelectedMonth;
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Decade);
                }
                else
                {
                    var selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -GetYearModeColumnCount());
                    OnSelectedMonthChanged(selectedMonth);
                }

                break;
            }
            case CalendarMode.Decade:
            {
                if (!ctrl)
                {
                    var selectedYear = DateTimeHelper.AddYears(SelectedYear, -ColumnsPerYear);
                    OnSelectedYearChanged(selectedYear);
                }

                break;
            }
        }
    }

    internal void ProcessDownKey(bool ctrl, bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                if (ctrl)
                {
                    SetCurrentValue(DisplayDateProperty, SelectedMonth);
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
                }
                else
                {
                    var selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, GetYearModeColumnCount());
                    OnSelectedMonthChanged(selectedMonth);
                }

                break;
            }
            case CalendarMode.Decade:
            {
                if (ctrl)
                {
                    SelectedMonth = SelectedYear;
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
                }
                else
                {
                    var selectedYear = DateTimeHelper.AddYears(SelectedYear, ColumnsPerYear);
                    OnSelectedYearChanged(selectedYear);
                }

                break;
            }
        }
    }

    internal void ProcessLeftKey(bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, -1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                var selectedYear = DateTimeHelper.AddYears(SelectedYear, -1);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    private int GetYearModeColumnCount()
    {
        return PickerMode == DatePickerMode.Quarter
            ? ColumnsPerYear
            : ColumnsPerMonthSelectionPanel;
    }

    internal void ProcessRightKey(bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = DateTimeHelper.AddMonths(_selectedMonth, 1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                var selectedYear = DateTimeHelper.AddYears(SelectedYear, 1);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    private bool ProcessEnterKey()
    {
        switch (DisplayMode)
        {
            case CalendarMode.Year:
            {
                if (PickerMode is DatePickerMode.Month or DatePickerMode.Quarter)
                {
                    SelectPickerDate(SelectedMonth);
                }
                else
                {
                    SetCurrentValue(DisplayDateProperty, SelectedMonth);
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Month);
                }
                return true;
            }
            case CalendarMode.Decade:
            {
                if (PickerMode == DatePickerMode.Year)
                {
                    SelectPickerDate(SelectedYear);
                }
                else
                {
                    SelectedMonth = SelectedYear;
                    SetCurrentValue(DisplayModeProperty, CalendarMode.Year);
                }
                return true;
            }
        }

        return false;
    }

    internal void ProcessHomeKey(bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = new DateTime(_selectedMonth.Year, 1, 1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                DateTime? selectedYear = new DateTime(DateTimeHelper.DecadeOfDate(SelectedYear), 1, 1);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    internal void ProcessEndKey(bool shift)
    {
        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = new DateTime(_selectedMonth.Year, 12, 1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                DateTime? selectedYear = new DateTime(DateTimeHelper.EndOfDecade(SelectedYear), 1, 1);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    internal void ProcessPageDownKey(bool shift)
    {
        if (!shift)
        {
            OnNextMonthClick();
            return;
        }

        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = DateTimeHelper.AddYears(_selectedMonth, 1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                var selectedYear = DateTimeHelper.AddYears(SelectedYear, 10);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    internal void ProcessPageUpKey(bool shift)
    {
        if (!shift)
        {
            OnPreviousMonthClick();
            return;
        }

        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                break;
            }
            case CalendarMode.Year:
            {
                var selectedMonth = DateTimeHelper.AddYears(_selectedMonth, -1);
                OnSelectedMonthChanged(selectedMonth);
                break;
            }
            case CalendarMode.Decade:
            {
                var selectedYear = DateTimeHelper.AddYears(SelectedYear, -10);
                OnSelectedYearChanged(selectedYear);
                break;
            }
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        HasFocusInternal = true;

        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                DateTime focusDate;
                if (LastSelectedDate.HasValue &&
                    DateTimeHelper.CompareYearMonth(DisplayDateInternal, LastSelectedDate.Value) == 0)
                {
                    focusDate = LastSelectedDate.Value;
                }
                else
                {
                    focusDate        = DisplayDate;
                    LastSelectedDate = DisplayDate;
                }

                FocusButton = FindDayButtonFromDay(focusDate);

                if (FocusButton != null)
                {
                    FocusButton.IsCurrent = true;
                }

                break;
            }
            case CalendarMode.Year:
            case CalendarMode.Decade:
            {
                if (FocusCalendarButton != null)
                {
                    FocusCalendarButton.IsCalendarButtonFocused = true;
                }

                break;
            }
        }
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        HasFocusInternal = false;

        switch (DisplayMode)
        {
            case CalendarMode.Month:
            {
                if (FocusButton != null)
                {
                    FocusButton.IsCurrent = false;
                }

                break;
            }
            case CalendarMode.Year:
            case CalendarMode.Decade:
            {
                if (FocusCalendarButton != null)
                {
                    FocusCalendarButton.IsCalendarButtonFocused = false;
                }

                break;
            }
        }
    }

    /// <summary>
    /// Called when the IsEnabled property changes.
    /// </summary>
    /// <param name="e">Property changed args.</param>
    private void OnIsEnabledChanged(AvaloniaPropertyChangedEventArgs change)
    {
        Debug.Assert(change.NewValue is bool, "NewValue should be a boolean!");
        var isEnabled = (bool)change.NewValue;

        CalendarItem?.UpdateDisabled(isEnabled);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        RefreshCultureFromThemeManager();
        CalendarItem = e.NameScope.Find<CalendarItem>("PART_CalendarItem");

        if (SelectedDate is not null && ShouldSelectedDateUpdateDisplayDate(SelectedDate.Value))
        {
            SetCurrentValue(DisplayDateProperty, SelectedDate);
        }
    
        SelectedMonth = DisplayDate;
        SelectedYear  = DisplayDate;
    
        if (CalendarItem != null)
        {
            CalendarItem.Owner = this;
            CalendarItem.UpdateDisabled(IsEnabled);
            UpdateMonths();
        }
    }
    
    internal virtual void UpdateHighlightDays()
    {
        SyncViewStateFromCurrentProperties();
        UpdateMonths();
    }
    
}
