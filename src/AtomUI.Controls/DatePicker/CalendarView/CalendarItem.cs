using System.Diagnostics;
using System.Globalization;
using AtomUI.Collections.Pooled;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls.CalendarView;

[TemplatePart(CalendarItemTheme.HeaderButtonPart, typeof(HeadTextButton))]
[TemplatePart(CalendarItemTheme.MonthViewPart, typeof(Grid))]
[TemplatePart(CalendarItemTheme.PreviousButtonPart, typeof(IconButton))]
[TemplatePart(CalendarItemTheme.PreviousMonthButtonPart, typeof(IconButton))]
[TemplatePart(CalendarItemTheme.NextButtonPart, typeof(IconButton))]
[TemplatePart(CalendarItemTheme.NextMonthButtonPart, typeof(IconButton))]
[TemplatePart(CalendarItemTheme.YearViewPart, typeof(Grid))]
internal class CalendarItem : TemplatedControl
{
    internal const string CalendarDisabledPC = ":calendardisabled";

    /// <summary>
    /// The number of days per week.
    /// </summary>
    internal const int NumberOfDaysPerWeek = 7;

    protected readonly System.Globalization.Calendar _calendar = new GregorianCalendar();

    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        Calendar.HeaderBackgroundProperty.AddOwner<CalendarItem>();

    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    public static readonly StyledProperty<ITemplate<Control>?> DayTitleTemplateProperty =
        AvaloniaProperty.Register<CalendarItem, ITemplate<Control>?>(
            nameof(DayTitleTemplate),
            defaultBindingMode: BindingMode.OneTime);

    public ITemplate<Control>? DayTitleTemplate
    {
        get => GetValue(DayTitleTemplateProperty);
        set => SetValue(DayTitleTemplateProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<CalendarItem, bool> IsMonthViewModeProperty =
        AvaloniaProperty.RegisterDirect<CalendarItem, bool>(nameof(IsMonthViewMode),
            o => o.IsMonthViewMode,
            (o, v) => o.IsMonthViewMode = v);

    private bool _isMonthViewMode = true;

    /// <summary>
    /// 主要方便在模板中控制导航按钮的显示和关闭
    /// </summary>
    internal bool IsMonthViewMode
    {
        get => _isMonthViewMode;
        set => SetAndRaise(IsMonthViewModeProperty, ref _isMonthViewMode, value);
    }

    /// <summary>
    /// Gets the button that allows switching between month mode, year mode,
    /// and decade mode.
    /// </summary>
    internal HeadTextButton? HeaderButton
    {
        get => _headerButton;

        private set
        {
            if (_headerButton != null)
            {
                _headerButton.Click -= HandleHeaderButtonClick;
            }

            _headerButton = value;

            if (_headerButton != null)
            {
                _headerButton.Click += HandleHeaderButtonClick;
                _headerButton.Focusable = false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the next page of the calendar when it
    /// is clicked.
    /// </summary>
    internal IconButton? NextButton
    {
        get => _nextButton;

        private set
        {
            if (_nextButton != null)
            {
                _nextButton.Click -= HandleNextButtonClick;
            }

            _nextButton = value;

            if (_nextButton != null)
            {
                _nextButton.Click += HandleNextButtonClick;
                _nextButton.Focusable = false;
            }
        }
    }

    internal IconButton? NextMonthButton
    {
        get => _nextMonthButton;

        private set
        {
            if (_nextMonthButton != null)
            {
                _nextMonthButton.Click -= HandleNextMonthButtonClick;
            }

            _nextMonthButton = value;

            if (_nextMonthButton != null)
            {
                _nextMonthButton.Click += HandleNextMonthButtonClick;
                _nextMonthButton.Focusable = false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the previous page of the calendar when
    /// it is clicked.
    /// </summary>
    internal IconButton? PreviousButton
    {
        get => _previousButton;

        private set
        {
            if (_previousButton != null)
            {
                _previousButton.Click -= HandlePreviousButtonClick;
            }

            _previousButton = value;

            if (_previousButton != null)
            {
                _previousButton.Click += HandlePreviousButtonClick;
                _previousButton.Focusable = false;
            }
        }
    }

    internal IconButton? PreviousMonthButton
    {
        get => _previousMonthButton;

        private set
        {
            if (_previousMonthButton != null)
            {
                _previousMonthButton.Click -= HandlePreviousMonthButtonClick;
            }

            _previousMonthButton = value;

            if (_previousMonthButton != null)
            {
                _previousMonthButton.Click += HandlePreviousMonthButtonClick;
                _previousMonthButton.Focusable = false;
            }
        }
    }

    #endregion

    protected DateTime _currentMonth;
    protected UniformGrid? _headerLayout;
    protected bool _isMouseLeftButtonDownYearView;

    protected HeadTextButton? _headerButton;
    protected IconButton? _nextButton;
    protected IconButton? _nextMonthButton;
    protected IconButton? _previousButton;
    protected IconButton? _previousMonthButton;

    // 当鼠标移动到日历单元格外面的时候还原 hover 临时的高亮
    private IDisposable? _pointerPositionDisposable;

    internal Calendar? Owner { get; set; }

    /// <summary>
    /// Gets the Grid that hosts the content when in month mode.
    /// </summary>
    internal UniformGrid? MonthViewLayout { get; set; }

    internal Grid? MonthView { get; set; }

    /// <summary>
    /// Gets the Grid that hosts the content when in year or decade mode.
    /// </summary>
    internal Grid? YearView { get; set; }

    private void PopulateGrids()
    {
        PopulateMonthViewsGrid();

        if (YearView != null)
        {
            var childCount = Calendar.RowsPerYear * Calendar.ColumnsPerYear;
            using var children = new PooledList<Control>(childCount);

            EventHandler<PointerPressedEventArgs> monthCalendarButtonMouseDown = HandleMonthCalendarButtonMouseDown;
            EventHandler<PointerReleasedEventArgs> monthCalendarButtonMouseUp = HandleMonthCalendarButtonMouseUp;
            EventHandler<PointerEventArgs> monthMouseEntered = HandleMonthMouseEntered;

            for (var i = 0; i < Calendar.RowsPerYear; i++)
            {
                for (var j = 0; j < Calendar.ColumnsPerYear; j++)
                {
                    var month = new CalendarButton();

                    if (Owner != null)
                    {
                        month.Owner = Owner;
                    }

                    month.SetValue(Grid.RowProperty, i);
                    month.SetValue(Grid.ColumnProperty, j);
                    month.CalendarLeftMouseButtonDown += monthCalendarButtonMouseDown;
                    month.CalendarLeftMouseButtonUp += monthCalendarButtonMouseUp;
                    month.PointerEntered += monthMouseEntered;
                    children.Add(month);
                }
            }

            YearView.Children.AddRange(children);
        }
    }

    protected virtual void PopulateMonthViewsGrid()
    {
        if (MonthView != null)
        {
            PopulateMonthViewGrid(MonthView);
        }
    }

    protected void PopulateMonthViewGrid(Grid monthView)
    {
        var childCount = Calendar.RowsPerMonth + Calendar.RowsPerMonth * Calendar.ColumnsPerMonth;
        using var children = new PooledList<Control>(childCount);

        for (var i = 0; i < Calendar.ColumnsPerMonth; i++)
        {
            var cell = DayTitleTemplate?.Build();
            if (cell is not null)
            {
                cell.DataContext = string.Empty;
                cell.SetValue(Grid.RowProperty, 0);
                cell.SetValue(Grid.ColumnProperty, i);
                children.Add(cell);
            }
        }

        EventHandler<PointerPressedEventArgs> cellMouseLeftButtonDown = HandleCellMouseLeftButtonDown;
        EventHandler<PointerReleasedEventArgs> cellMouseLeftButtonUp = HandleCellMouseLeftButtonUp;
        EventHandler<PointerEventArgs> cellMouseEntered = HandleCellMouseEntered;
        EventHandler<RoutedEventArgs> cellClick = HandleCellClick;

        for (var i = 1; i < Calendar.RowsPerMonth; i++)
        {
            for (var j = 0; j < Calendar.ColumnsPerMonth; j++)
            {
                var cell = new CalendarDayButton();

                if (Owner != null)
                {
                    cell.Owner = Owner;
                }

                cell.SetValue(Grid.RowProperty, i);
                cell.SetValue(Grid.ColumnProperty, j);
                cell.CalendarDayButtonMouseDown += cellMouseLeftButtonDown;
                cell.CalendarDayButtonMouseUp += cellMouseLeftButtonUp;
                cell.PointerEntered += cellMouseEntered;
                cell.Click += cellClick;
                children.Add(cell);
            }
        }

        monthView.Children.AddRange(children);
    }

    /// <summary>
    /// Builds the visual tree for the
    /// <see cref="T:System.Windows.Controls.Primitives.CalendarItem" />
    /// when a new template is applied.
    /// </summary>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        HeaderButton = e.NameScope.Get<HeadTextButton>(CalendarItemTheme.HeaderButtonPart);
        PreviousButton = e.NameScope.Get<IconButton>(CalendarItemTheme.PreviousButtonPart);
        PreviousMonthButton =
            e.NameScope.Get<IconButton>(CalendarItemTheme.PreviousMonthButtonPart);
        NextButton = e.NameScope.Get<IconButton>(CalendarItemTheme.NextButtonPart);
        NextMonthButton = e.NameScope.Get<IconButton>(CalendarItemTheme.NextMonthButtonPart);

        MonthViewLayout = e.NameScope.Get<UniformGrid>(CalendarItemTheme.MonthViewLayoutPart);
        MonthView = e.NameScope.Get<Grid>(CalendarItemTheme.MonthViewPart);
        YearView = e.NameScope.Get<Grid>(CalendarItemTheme.YearViewPart);
        _headerLayout = e.NameScope.Get<UniformGrid>(CalendarItemTheme.HeaderLayoutPart);

        if (Owner != null)
        {
            UpdateDisabled(Owner.IsEnabled);
        }

        PopulateGrids();

        if (MonthViewLayout != null && YearView != null)
        {
            if (Owner != null)
            {
                Owner.SelectedMonth = Owner.DisplayDateInternal;
                Owner.SelectedYear = Owner.DisplayDateInternal;

                if (Owner.DisplayMode == CalendarMode.Year)
                {
                    UpdateYearMode();
                }
                else if (Owner.DisplayMode == CalendarMode.Decade)
                {
                    UpdateDecadeMode();
                }

                if (Owner.DisplayMode == CalendarMode.Month)
                {
                    UpdateMonthMode();
                    MonthViewLayout.IsVisible = true;
                    YearView.IsVisible = false;
                }
                else
                {
                    YearView.IsVisible = true;
                    MonthViewLayout.IsVisible = false;
                }
            }
            else
            {
                UpdateMonthMode();
                MonthViewLayout.IsVisible = true;
                YearView.IsVisible = false;
            }
        }

        SetupHeaderForDisplayModeChanged();
    }

    protected virtual void SetupHeaderForDisplayModeChanged()
    {
        if (Owner is null || MonthViewLayout is null || _headerLayout is null)
        {
            return;
        }

        IsMonthViewMode = Owner.DisplayMode == CalendarMode.Month;
    }

    protected virtual void SetDayTitles()
    {
        if (MonthView is not null)
        {
            SetDayTitles(MonthView);
        }
    }

    protected void SetDayTitles(Grid monthView)
    {
        for (var childIndex = 0; childIndex < Calendar.ColumnsPerMonth; childIndex++)
        {
            var dayTitle = monthView.Children[childIndex];
            if (Owner != null)
            {
                dayTitle.DataContext = DateTimeHelper.GetCurrentDateFormat()
                    .ShortestDayNames[
                        (childIndex + (int)Owner.FirstDayOfWeek) %
                        NumberOfDaysPerWeek];
            }
            else
            {
                dayTitle.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[
                    (childIndex + (int)DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek) % NumberOfDaysPerWeek];
            }
        }
    }

    /// <summary>
    /// How many days of the previous month need to be displayed.
    /// </summary>
    private int PreviousMonthDays(DateTime firstOfMonth)
    {
        var day = _calendar.GetDayOfWeek(firstOfMonth);
        int i;

        if (Owner != null)
        {
            i = (day - Owner.FirstDayOfWeek + NumberOfDaysPerWeek) % NumberOfDaysPerWeek;
        }
        else
        {
            i = (day - DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek + NumberOfDaysPerWeek) %
                NumberOfDaysPerWeek;
        }

        if (i == 0)
        {
            return NumberOfDaysPerWeek;
        }

        return i;
    }

    protected internal virtual void UpdateMonthMode()
    {
        if (Owner != null)
        {
            _currentMonth = Owner.DisplayDateInternal;
        }
        else
        {
            _currentMonth = DateTime.Today;
        }

        SetMonthModeHeaderButton();
        SetMonthModePreviousButton(_currentMonth);
        SetMonthModeNextButton(_currentMonth);

        if (MonthViewLayout != null)
        {
            SetDayTitles();
            SetCalendarDayButtons();
        }
    }

    protected virtual void SetCalendarDayButtons()
    {
        if (MonthView is not null)
        {
            SetCalendarDayButtons(_currentMonth, MonthView);
        }
    }

    protected virtual void SetMonthModeHeaderButton()
    {
        if (HeaderButton is not null)
        {
            if (Owner is not null)
            {
                HeaderButton.Content =
                    Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                HeaderButton.IsEnabled = true;
            }
            else
            {
                HeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            }
        }
    }

    protected void SetMonthModeNextButton(DateTime firstDayOfMonth)
    {
        if (Owner != null && NextButton != null)
        {
            // DisplayDate is equal to DateTime.MaxValue
            if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0)
            {
                NextButton.IsEnabled = false;
            }
            else
            {
                // Since we are sure DisplayDate is not equal to
                // DateTime.MaxValue, it is safe to use AddMonths  
                var firstDayOfNextMonth = _calendar.AddMonths(firstDayOfMonth, 1);
                NextButton.IsEnabled =
                    DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth) > -1;
            }
        }
    }

    protected void SetMonthModePreviousButton(DateTime firstDayOfMonth)
    {
        if (Owner != null && PreviousButton != null)
        {
            PreviousButton.IsEnabled =
                DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
        }
    }

    private void SetButtonState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        if (Owner != null)
        {
            childButton.Opacity = 1;

            // If the day is outside the DisplayDateStart/End boundary, do
            // not show it
            if (DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeStart) < 0 ||
                DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeEnd) > 0)
            {
                childButton.IsEnabled = false;
                childButton.IsToday = false;
                childButton.IsSelected = false;
                childButton.Opacity = 0;
            }
            else
            {
                // SET IF THE DAY IS SELECTABLE OR NOT
                if (Owner.BlackoutDates.Contains(dateToAdd))
                {
                    childButton.IsBlackout = true;
                }
                else
                {
                    childButton.IsBlackout = false;
                }

                childButton.IsEnabled = true;

                // SET IF THE DAY IS INACTIVE OR NOT: set if the day is a
                // trailing day or not
                childButton.IsInactive = CheckDayInactiveState(childButton, dateToAdd);

                // SET IF THE DAY IS TODAY OR NOT
                childButton.IsToday = CheckDayIsTodayState(dateToAdd);

                CheckButtonSelectedState(childButton, dateToAdd);

                // SET THE FOCUS ELEMENT
                if (Owner.LastSelectedDate != null)
                {
                    if (DateTimeHelper.CompareDays(Owner.LastSelectedDate.Value, dateToAdd) == 0)
                    {
                        if (Owner.FocusButton != null)
                        {
                            Owner.FocusButton.IsCurrent = false;
                        }

                        Owner.FocusButton = childButton;
                        if (Owner.HasFocusInternal)
                        {
                            Owner.FocusButton.IsCurrent = true;
                        }
                    }
                    else
                    {
                        childButton.IsCurrent = false;
                    }
                }
            }
        }
    }

    protected virtual void CheckButtonSelectedState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        // SET IF THE DAY IS SELECTED OR NOT
        childButton.IsSelected = false;
        if (Owner is not null)
        {
            if (Owner.SelectedDate.HasValue)
            {
                childButton.IsSelected = DateTimeHelper.CompareDays(Owner.SelectedDate.Value, dateToAdd) == 0;
            }
        }
    }

    protected virtual bool CheckDayInactiveState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        if (Owner is not null)
        {
            return DateTimeHelper.CompareYearMonth(dateToAdd, Owner.DisplayDateInternal) != 0;
        }

        return false;
    }

    protected virtual bool CheckDayIsTodayState(DateTime dateToAdd)
    {
        if (Owner is not null)
        {
            return Owner.IsTodayHighlighted && dateToAdd == DateTime.Today;
        }

        return false;
    }

    protected void SetCalendarDayButtons(DateTime firstDayOfMonth, Grid monthView)
    {
        var lastMonthToDisplay = PreviousMonthDays(firstDayOfMonth);
        DateTime dateToAdd;

        if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MinValue) > 0)
        {
            // DisplayDate is not equal to DateTime.MinValue we can subtract
            // days from the DisplayDate
            dateToAdd = _calendar.AddDays(firstDayOfMonth, -lastMonthToDisplay);
        }
        else
        {
            dateToAdd = firstDayOfMonth;
        }

        var count = Calendar.RowsPerMonth * Calendar.ColumnsPerMonth;

        for (var childIndex = Calendar.ColumnsPerMonth; childIndex < count; childIndex++)
        {
            var childButton = (CalendarDayButton)monthView.Children[childIndex];
            childButton.Index = childIndex;
            SetButtonState(childButton, dateToAdd);

            //childButton.Focusable = false;
            childButton.Content = dateToAdd.Day.ToString(DateTimeHelper.GetCurrentDateFormat());
            childButton.DataContext = dateToAdd;

            if (DateTime.Compare(DateTimeHelper.DiscardTime(DateTime.MaxValue), dateToAdd) > 0)
            {
                // Since we are sure DisplayDate is not equal to
                // DateTime.MaxValue, it is safe to use AddDays 
                dateToAdd = _calendar.AddDays(dateToAdd, 1);
            }
            else
            {
                // DisplayDate is equal to the DateTime.MaxValue, so there
                // are no trailing days
                childIndex++;
                for (var i = childIndex; i < count; i++)
                {
                    childButton = (CalendarDayButton)monthView.Children[i];
                    // button needs a content to occupy the necessary space
                    // for the content presenter
                    childButton.Content = i.ToString(DateTimeHelper.GetCurrentDateFormat());
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }

                return;
            }
        }
    }

    internal void UpdateYearMode()
    {
        if (Owner != null)
        {
            _currentMonth = Owner.SelectedMonth;
        }
        else
        {
            _currentMonth = DateTime.Today;
        }

        SetYearModeHeaderButton();
        SetYearModePreviousButton();
        SetYearModeNextButton();

        if (YearView != null)
        {
            SetMonthButtonsForYearMode();
        }
    }

    private void SetYearModeHeaderButton()
    {
        if (HeaderButton != null)
        {
            HeaderButton.IsEnabled = true;
            HeaderButton.Content = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
        }
    }

    private void SetYearModePreviousButton()
    {
        if (Owner != null && PreviousButton != null)
        {
            PreviousButton.IsEnabled = Owner.DisplayDateRangeStart.Year != _currentMonth.Year;
        }
    }

    private void SetYearModeNextButton()
    {
        if (Owner != null && NextButton != null)
        {
            NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != _currentMonth.Year;
        }
    }

    private void SetMonthButtonsForYearMode()
    {
        var count = 0;
        foreach (object child in YearView!.Children)
        {
            var childButton = (CalendarButton)child;
            // There should be no time component. Time is 12:00 AM
            var day = new DateTime(_currentMonth.Year, count + 1, 1);
            childButton.DataContext = day;

            childButton.Content = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count];
            childButton.IsVisible = true;

            if (Owner != null)
            {
                if (day.Year == _currentMonth.Year && day.Month == _currentMonth.Month && day.Day == _currentMonth.Day)
                {
                    Owner.FocusCalendarButton = childButton;
                    childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                }
                else
                {
                    childButton.IsCalendarButtonFocused = false;
                }

                childButton.IsSelected = DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateInternal) == 0;

                if (DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeStart) < 0 ||
                    DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeEnd) > 0)
                {
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }
                else
                {
                    childButton.IsEnabled = true;
                    childButton.Opacity = 1;
                }
            }

            childButton.IsInactive = false;
            count++;
        }
    }

    internal void UpdateDecadeMode()
    {
        DateTime selectedYear;

        if (Owner != null)
        {
            selectedYear = Owner.SelectedYear;
            _currentMonth = Owner.SelectedMonth;
        }
        else
        {
            _currentMonth = DateTime.Today;
            selectedYear = DateTime.Today;
        }

        var decade = DateTimeHelper.DecadeOfDate(selectedYear);
        var decadeEnd = DateTimeHelper.EndOfDecade(selectedYear);

        SetDecadeModeHeaderButton(decade, decadeEnd);
        SetDecadeModePreviousButton(decade);
        SetDecadeModeNextButton(decadeEnd);

        if (YearView != null)
        {
            SetYearButtons(decade, decadeEnd);
        }
    }

    internal void UpdateYearViewSelection(CalendarButton? calendarButton)
    {
        if (Owner != null && calendarButton?.DataContext is DateTime selectedDate)
        {
            Owner.FocusCalendarButton!.IsCalendarButtonFocused = false;
            Owner.FocusCalendarButton = calendarButton;
            calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;

            if (Owner.DisplayMode == CalendarMode.Year)
            {
                Owner.SelectedMonth = selectedDate;
            }
            else
            {
                Owner.SelectedYear = selectedDate;
            }
        }
    }

    private void SetYearButtons(int decade, int decadeEnd)
    {
        int year;
        var count = -1;
        foreach (var child in YearView!.Children)
        {
            var childButton = (CalendarButton)child;
            year = decade + count;

            if (year <= DateTime.MaxValue.Year && year >= DateTime.MinValue.Year)
            {
                // There should be no time component. Time is 12:00 AM
                var day = new DateTime(year, 1, 1);
                childButton.DataContext = day;
                childButton.Content = year.ToString(DateTimeHelper.GetCurrentDateFormat());
                childButton.IsVisible = true;

                if (Owner != null)
                {
                    if (year == Owner.SelectedYear.Year)
                    {
                        Owner.FocusCalendarButton = childButton;
                        childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
                    }
                    else
                    {
                        childButton.IsCalendarButtonFocused = false;
                    }

                    childButton.IsSelected = Owner.DisplayDate.Year == year;

                    if (year < Owner.DisplayDateRangeStart.Year || year > Owner.DisplayDateRangeEnd.Year)
                    {
                        childButton.IsEnabled = false;
                        childButton.Opacity = 0;
                    }
                    else
                    {
                        childButton.IsEnabled = true;
                        childButton.Opacity = 1;
                    }
                }

                // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a
                // trailing year or not
                childButton.IsInactive = year < decade || year > decadeEnd;
            }
            else
            {
                childButton.IsEnabled = false;
                childButton.Opacity = 0;
            }

            count++;
        }
    }

    private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
    {
        if (HeaderButton != null)
        {
            HeaderButton.Content = decade.ToString(CultureInfo.CurrentCulture) + "-" +
                                   decadeEnd.ToString(CultureInfo.CurrentCulture);
            HeaderButton.IsEnabled = false;
        }
    }

    private void SetDecadeModeNextButton(int decadeEnd)
    {
        if (Owner != null && NextButton != null)
        {
            NextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year > decadeEnd;
        }
    }

    private void SetDecadeModePreviousButton(int decade)
    {
        if (Owner != null && PreviousButton != null)
        {
            PreviousButton.IsEnabled = decade > Owner.DisplayDateRangeStart.Year;
        }
    }

    protected internal virtual void HandleHeaderButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            var b = (HeadTextButton)sender!;
            DateTime d;

            if (b.IsEnabled)
            {
                if (Owner.DisplayMode == CalendarMode.Month)
                {
                    d = Owner.DisplayDateInternal;
                    Owner.SelectedMonth = new DateTime(d.Year, d.Month, 1);
                    Owner.DisplayMode = CalendarMode.Year;
                }
                else
                {
                    Debug.Assert(Owner.DisplayMode == CalendarMode.Year,
                        "The Owner Calendar's DisplayMode should be Year!");
                    d = Owner.SelectedMonth;
                    Owner.SelectedYear = new DateTime(d.Year, d.Month, 1);
                    Owner.DisplayMode = CalendarMode.Decade;
                }

                SetupHeaderForDisplayModeChanged();
            }
        }
    }

    internal void HandlePreviousMonthButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            var b = (IconButton)sender!;
            if (b.IsEnabled)
            {
                Owner.OnPreviousMonthClick();
            }
        }
    }

    internal void HandlePreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            var b = (IconButton)sender!;
            if (b.IsEnabled)
            {
                Owner.OnPreviousClick();
            }
        }
    }

    internal void HandleNextMonthButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            var b = (IconButton)sender!;

            if (b.IsEnabled)
            {
                Owner.OnNextMonthClick();
            }
        }
    }

    internal void HandleNextButtonClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            var b = (IconButton)sender!;

            if (b.IsEnabled)
            {
                Owner.OnNextClick();
            }
        }
    }

    internal void HandleCellMouseEntered(object? sender, PointerEventArgs e)
    {
        if (Owner != null)
        {
            if (sender is CalendarDayButton
                {
                    IsEnabled: true, IsBlackout: false, DataContext: DateTime selectedDate
                } dayButton)
            {
                NotifyCellMouseEntered(dayButton, selectedDate);
            }
        }
    }

    protected virtual void NotifyCellMouseEntered(CalendarDayButton dayButton, DateTime selectedDate)
    {
        if (Owner != null)
        {
            Owner.NotifyHoverDateChanged(selectedDate);
        }
    }

    internal void HandleCellMouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
    {
        if (Owner != null)
        {
            if (!Owner.HasFocusInternal)
            {
                Owner.Focus();
            }

            if (sender is CalendarDayButton dayButton)
            {
                NotifyCellMouseLeftButtonDown(dayButton);
            }
        }
    }

    protected virtual void NotifyCellMouseLeftButtonDown(CalendarDayButton dayButton)
    {
        if (Owner is not null)
        {
            if (dayButton.IsEnabled && !dayButton.IsBlackout && dayButton.DataContext is DateTime selectedDate)
            {
                Owner.SelectedDate = selectedDate;
                Owner.NotifyDateSelected();
                Owner.UpdateHighlightDays();
            }
        }
    }

    internal void HandleCellMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        if (Owner != null)
        {
            if (sender is CalendarDayButton dayButton)
            {
                if (!dayButton.IsBlackout)
                {
                    Owner.OnDayButtonMouseUp(e);
                }

                NotifyCellMouseLeftButtonUp(dayButton);
            }
        }
    }

    protected virtual void NotifyCellMouseLeftButtonUp(CalendarDayButton dayButton)
    {
        if (Owner is not null)
        {
            if (dayButton.DataContext is DateTime selectedDate)
            {
                // If the day is Disabled but a trailing day we should
                // be able to switch months
                if (dayButton.IsInactive)
                {
                    Owner.NotifyDayClick(selectedDate);
                }
            }
        }
    }

    private void HandleCellClick(object? sender, RoutedEventArgs e)
    {
    }

    private void HandleMonthCalendarButtonMouseDown(object? sender, PointerPressedEventArgs e)
    {
        _isMouseLeftButtonDownYearView = true;
        UpdateYearViewSelection(sender as CalendarButton);
    }

    protected internal virtual void HandleMonthCalendarButtonMouseUp(object? sender, PointerReleasedEventArgs e)
    {
        _isMouseLeftButtonDownYearView = false;

        if (Owner != null && (sender as CalendarButton)?.DataContext is DateTime newMonth)
        {
            if (Owner.DisplayMode == CalendarMode.Year)
            {
                Owner.DisplayDate = newMonth;
                Owner.DisplayMode = CalendarMode.Month;
            }
            else
            {
                Debug.Assert(Owner.DisplayMode == CalendarMode.Decade, "The owning Calendar should be in decade mode!");
                Owner.SelectedMonth = newMonth;
                Owner.DisplayMode = CalendarMode.Year;
            }

            SetupHeaderForDisplayModeChanged();
        }
    }

    private void HandleMonthMouseEntered(object? sender, PointerEventArgs e)
    {
        if (_isMouseLeftButtonDownYearView)
        {
            UpdateYearViewSelection(sender as CalendarButton);
        }
    }

    internal void UpdateDisabled(bool isEnabled)
    {
        PseudoClasses.Set(CalendarDisabledPC, !isEnabled);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateSharedTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, 0, 0, thickness.Bottom)));
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _pointerPositionDisposable = inputManager.Process.Subscribe(DetectPointerPosition);
        SetCalendarDayButtons();
    }

    private void DetectPointerPosition(RawInputEventArgs args)
    {
        if (Owner is null)
        {
            return;
        }

        if (args is RawPointerEventArgs pointerEventArgs)
        {
            var originState = Owner.IsPointerInMonthView;
            if (!IsPointerInMonthView(pointerEventArgs.Position))
            {
                Owner.IsPointerInMonthView = false;
                NotifyPointerOutMonthView(originState);
            }
            else
            {
                Owner.IsPointerInMonthView = true;
                NotifyPointerInMonthView(originState);
            }
        }
    }

    protected virtual void NotifyPointerInMonthView(bool originInMonthView)
    {
    }

    protected virtual void NotifyPointerOutMonthView(bool originInMonthView)
    {
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerPositionDisposable?.Dispose();
    }

    protected virtual bool IsPointerInMonthView(Point position)
    {
        if (Owner is null)
        {
            return false;
        }

        if (Owner.DisplayMode == CalendarMode.Month)
        {
            return GetMonthViewRect(MonthView!).Contains(position);
        }

        return false;
    }

    protected Rect GetMonthViewRect(Grid monthView)
    {
        var firstDay = (monthView.Children[7] as CalendarDayButton)!;
        var firstDayPos = firstDay.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(monthView)!) ?? default;
        var monthViewPos = monthView.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(monthView)!) ?? default;
        return new Rect(firstDayPos,
            new Size(monthView.Bounds.Width, monthViewPos.Y + monthView.Bounds.Height - firstDayPos.Y));
    }
}