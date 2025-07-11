﻿using System.Diagnostics;
using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Collections.Pooled;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Controls;

[TemplatePart(CalendarItemThemeConstants.HeaderButtonPart, typeof(AvaloniaButton))]
[TemplatePart(CalendarItemThemeConstants.MonthViewPart, typeof(Grid))]
[TemplatePart(CalendarItemThemeConstants.NextMonthButtonPart, typeof(AvaloniaButton))]
[TemplatePart(CalendarItemThemeConstants.PreviousMonthButtonPart, typeof(AvaloniaButton))]
[TemplatePart(CalendarItemThemeConstants.YearViewPart, typeof(Grid))]
[PseudoClasses(CalendarDisabledPC)]
internal class CalendarItem : TemplatedControl,
                              IResourceBindingManager
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
    
    public static readonly StyledProperty<GridLength> DayTitleHeightProperty =
        AvaloniaProperty.Register<CalendarItem, GridLength>(
            nameof(DayTitleHeight));

    /// <summary>
    /// 主要方便在模板中控制导航按钮的显示和关闭
    /// </summary>
    internal bool IsMonthViewMode
    {
        get => _isMonthViewMode;
        set => SetAndRaise(IsMonthViewModeProperty, ref _isMonthViewMode, value);
    }
    private bool _isMonthViewMode = true;
    
    public GridLength DayTitleHeight
    {
        get => GetValue(DayTitleHeightProperty);
        set => SetValue(DayTitleHeightProperty, value);
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
                _headerButton.Click     += HandleHeaderButtonClick;
                _headerButton.Focusable =  false;
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
                // If the user does not provide a Content value in template,
                // we provide a helper text that can be used in
                // Accessibility this text is not shown on the UI, just used
                // for Accessibility purposes
                if (_nextButton.Content == null)
                {
                    _nextButton.Content = "next button";
                }

                _nextButton.Click     += HandleNextButtonClick;
                _nextButton.Focusable =  false;
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
                // If the user does not provide a Content value in template,
                // we provide a helper text that can be used in
                // Accessibility this text is not shown on the UI, just used
                // for Accessibility purposes
                if (_nextMonthButton.Content == null)
                {
                    _nextMonthButton.Content = "next button";
                }

                _nextMonthButton.Click     += HandleNextMonthButtonClick;
                _nextMonthButton.Focusable =  false;
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
                // If the user does not provide a Content value in template,
                // we provide a helper text that can be used in
                // Accessibility this text is not shown on the UI, just used
                // for Accessibility purposes
                if (_previousButton.Content == null)
                {
                    _previousButton.Content = "previous button";
                }

                _previousButton.Click     += HandlePreviousButtonClick;
                _previousButton.Focusable =  false;
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
                // If the user does not provide a Content value in template,
                // we provide a helper text that can be used in
                // Accessibility this text is not shown on the UI, just used
                // for Accessibility purposes
                if (_previousMonthButton.Content == null)
                {
                    _previousMonthButton.Content = "previous button";
                }

                _previousMonthButton.Click     += HandlePreviousMonthButtonClick;
                _previousMonthButton.Focusable =  false;
            }
        }
    }

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;

    protected DateTime _currentMonth;

    protected HeadTextButton? _headerButton;
    protected bool _isControlPressed;
    protected bool _isMouseLeftButtonDown;
    protected bool _isMouseLeftButtonDownYearView;
    protected IconButton? _nextButton;
    protected IconButton? _nextMonthButton;
    protected IconButton? _previousButton;
    protected IconButton? _previousMonthButton;
    protected Grid? _headerLayout;

    internal Calendar? Owner { get; set; }
    internal CalendarDayButton? CurrentButton { get; set; }

    /// <summary>
    /// Gets the Grid that hosts the content when in month mode.
    /// </summary>
    internal Grid? MonthView { get; set; }

    /// <summary>
    /// Gets the Grid that hosts the content when in year or decade mode.
    /// </summary>
    internal Grid? YearView { get; set; }

    private void PopulateGrids()
    {
        if (MonthView != null)
        {
            var       childCount = Calendar.RowsPerMonth + Calendar.RowsPerMonth * Calendar.ColumnsPerMonth;
            using var children   = new PooledList<Control>(childCount);

            for (var i = 0; i < Calendar.ColumnsPerMonth; i++)
            {
                var cell = DayTitleTemplate?.Build();
                if (cell is TextBlock textBlockCell)
                {
                    this.AddThemeResourceBindingAction(() =>
                    {
                        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(textBlockCell,
                            HeightProperty, CalendarTokenKey.DayTitleHeight));
                    });
                }
                if (cell is not null)
                {
                    cell.DataContext = string.Empty;
                    cell.SetValue(Grid.RowProperty, 0);
                    cell.SetValue(Grid.ColumnProperty, i);
                    children.Add(cell);
                }
            }

            EventHandler<PointerPressedEventArgs>  cellMouseLeftButtonDown = HandleCellMouseLeftButtonDown;
            EventHandler<PointerReleasedEventArgs> cellMouseLeftButtonUp   = HandleCellMouseLeftButtonUp;
            EventHandler<PointerEventArgs>         cellMouseEntered        = HandleCellMouseEntered;
            EventHandler<RoutedEventArgs>          cellClick               = HandleCellClick;

            for (var i = 1; i < Calendar.RowsPerMonth; i++)
            {
                for (var j = 0; j < Calendar.ColumnsPerMonth; j++)
                {
                    var cell = new CalendarDayButton();

                    if (Owner != null)
                    {
                        cell.Owner = Owner;
                        BindUtils.RelayBind(Owner, Calendar.IsMotionEnabledProperty, cell,
                            BaseCalendarDayButton.IsMotionEnabledProperty);
                    }

                    cell.SetValue(Grid.RowProperty, i);
                    cell.SetValue(Grid.ColumnProperty, j);
                    cell.CalendarDayButtonMouseDown += cellMouseLeftButtonDown;
                    cell.CalendarDayButtonMouseUp   += cellMouseLeftButtonUp;
                    cell.PointerEntered             += cellMouseEntered;
                    cell.Click                      += cellClick;
                    children.Add(cell);
                }
            }

            MonthView.Children.AddRange(children);
        }

        if (YearView != null)
        {
            var       childCount = Calendar.RowsPerYear * Calendar.ColumnsPerYear;
            using var children   = new PooledList<Control>(childCount);

            EventHandler<PointerPressedEventArgs>  monthCalendarButtonMouseDown = HandleMonthCalendarButtonMouseDown;
            EventHandler<PointerReleasedEventArgs> monthCalendarButtonMouseUp   = HandleMonthCalendarButtonMouseUp;
            EventHandler<PointerEventArgs>         monthMouseEntered            = HandleMonthMouseEntered;

            for (var i = 0; i < Calendar.RowsPerYear; i++)
            {
                for (var j = 0; j < Calendar.ColumnsPerYear; j++)
                {
                    var month = new CalendarButton();

                    if (Owner != null)
                    {
                        BindUtils.RelayBind(Owner, Calendar.IsMotionEnabledProperty, month,
                            BaseCalendarButton.IsMotionEnabledProperty);
                        month.Owner = Owner;
                    }

                    month.SetValue(Grid.RowProperty, i);
                    month.SetValue(Grid.ColumnProperty, j);
                    month.CalendarLeftMouseButtonDown += monthCalendarButtonMouseDown;
                    month.CalendarLeftMouseButtonUp   += monthCalendarButtonMouseUp;
                    month.PointerEntered              += monthMouseEntered;
                    children.Add(month);
                }
            }

            YearView.Children.AddRange(children);
        }
    }

    /// <summary>
    /// Builds the visual tree for the
    /// <see cref="T:System.Windows.Controls.Primitives.CalendarItem" />
    /// when a new template is applied.
    /// </summary>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        HeaderButton        = e.NameScope.Find<HeadTextButton>(CalendarItemThemeConstants.HeaderButtonPart);
        PreviousButton      = e.NameScope.Find<IconButton>(CalendarItemThemeConstants.PreviousButtonPart);
        PreviousMonthButton = e.NameScope.Find<IconButton>(CalendarItemThemeConstants.PreviousMonthButtonPart);
        NextButton          = e.NameScope.Find<IconButton>(CalendarItemThemeConstants.NextButtonPart);
        NextMonthButton     = e.NameScope.Find<IconButton>(CalendarItemThemeConstants.NextMonthButtonPart);
        MonthView           = e.NameScope.Find<Grid>(CalendarItemThemeConstants.MonthViewPart);
        YearView            = e.NameScope.Find<Grid>(CalendarItemThemeConstants.YearViewPart);
        _headerLayout       = e.NameScope.Get<Grid>(CalendarItemThemeConstants.HeaderLayoutPart);
        
        if (Owner != null)
        {
            UpdateDisabled(Owner.IsEnabled);
            if (HeaderButton != null)
            {
                BindUtils.RelayBind(Owner, Calendar.IsMotionEnabledProperty, HeaderButton, HeadTextButton.IsMotionEnabledProperty);
            }
        }

        PopulateGrids();

        if (MonthView != null && YearView != null)
        {
            if (Owner != null)
            {
                Owner.SelectedMonth = Owner.DisplayDateInternal;
                Owner.SelectedYear  = Owner.DisplayDateInternal;

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
                    MonthView.IsVisible = true;
                    YearView.IsVisible  = false;
                }
                else
                {
                    YearView.IsVisible  = true;
                    MonthView.IsVisible = false;
                }
            }
            else
            {
                UpdateMonthMode();
                MonthView.IsVisible = true;
                YearView.IsVisible  = false;
            }
        }
        SetupHeaderForDisplayModeChanged();
    }
    
    protected virtual void SetupHeaderForDisplayModeChanged()
    {
        if (Owner is null || MonthView is null || _headerLayout is null)
        {
            return;
        }

        IsMonthViewMode = Owner.DisplayMode == CalendarMode.Month;
    }

    protected void SetDayTitles()
    {
        for (var childIndex = 0; childIndex < Calendar.ColumnsPerMonth; childIndex++)
        {
            var daytitle = MonthView!.Children[childIndex];
            if (Owner != null)
            {
                daytitle.DataContext = DateTimeHelper.GetCurrentDateFormat()
                                                     .ShortestDayNames[
                                                         (childIndex + (int)Owner.FirstDayOfWeek) %
                                                         NumberOfDaysPerWeek];
            }
            else
            {
                daytitle.DataContext = DateTimeHelper.GetCurrentDateFormat().ShortestDayNames[
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

        if (MonthView != null)
        {
            SetDayTitles();
            SetCalendarDayButtons(_currentMonth);
        }
    }

    protected virtual void SetMonthModeHeaderButton()
    {
        if (HeaderButton != null)
        {
            if (Owner != null)
            {
                HeaderButton.Content   = Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
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
                NextButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth) > -1;
            }
        }
    }

    protected void SetMonthModePreviousButton(DateTime firstDayOfMonth)
    {
        if (Owner != null && PreviousButton != null)
        {
            PreviousButton.IsEnabled = DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
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
                childButton.IsEnabled  = false;
                childButton.IsToday    = false;
                childButton.IsSelected = false;
                childButton.Opacity    = 0;
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
                childButton.IsInactive = CheckDayInactiveState(dateToAdd);

                // SET IF THE DAY IS TODAY OR NOT
                childButton.IsToday = CheckDayIsTodayState(dateToAdd);

                // SET IF THE DAY IS SELECTED OR NOT
                childButton.IsSelected = false;
                foreach (var item in Owner.SelectedDates)
                {
                    // Since we should be comparing the Date values not
                    // DateTime values, we can't use
                    // Owner.SelectedDates.Contains(dateToAdd) directly
                    childButton.IsSelected |= DateTimeHelper.CompareDays(dateToAdd, item) == 0;
                }

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

    protected virtual bool CheckDayInactiveState(DateTime dateToAdd)
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

    protected void SetCalendarDayButtons(DateTime firstDayOfMonth)
    {
        var      lastMonthToDisplay = PreviousMonthDays(firstDayOfMonth);
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

        if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
        {
            Owner.HoverEndIndex   = null;
            Owner.HoverStartIndex = null;
        }

        var count = Calendar.RowsPerMonth * Calendar.ColumnsPerMonth;

        for (var childIndex = Calendar.ColumnsPerMonth; childIndex < count; childIndex++)
        {
            var childButton = (CalendarDayButton)MonthView!.Children[childIndex];

            childButton.Index = childIndex;
            SetButtonState(childButton, dateToAdd);

            // Update the indexes of hoverStart and hoverEnd
            if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null)
            {
                if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverEnd.Value) == 0)
                {
                    Owner.HoverEndIndex = childIndex;
                }

                if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverStart.Value) == 0)
                {
                    Owner.HoverStartIndex = childIndex;
                }
            }

            //childButton.Focusable = false;
            childButton.Content     = dateToAdd.Day.ToString(DateTimeHelper.GetCurrentDateFormat());
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
                    childButton = (CalendarDayButton)MonthView.Children[i];
                    // button needs a content to occupy the necessary space
                    // for the content presenter
                    childButton.Content   = i.ToString(DateTimeHelper.GetCurrentDateFormat());
                    childButton.IsEnabled = false;
                    childButton.Opacity   = 0;
                }

                return;
            }
        }

        // If the HoverStart or HoverEndInternal could not be found on the
        // DisplayMonth set the values of the HoverStartIndex or
        // HoverEndIndex to be the first or last day indexes on the current
        // month
        if (Owner != null && Owner.HoverStart.HasValue && Owner.HoverEndInternal.HasValue)
        {
            if (!Owner.HoverEndIndex.HasValue)
            {
                if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                {
                    Owner.HoverEndIndex = Calendar.ColumnsPerMonth * Calendar.RowsPerMonth - 1;
                }
                else
                {
                    Owner.HoverEndIndex = Calendar.ColumnsPerMonth;
                }
            }

            if (!Owner.HoverStartIndex.HasValue)
            {
                if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0)
                {
                    Owner.HoverStartIndex = Calendar.ColumnsPerMonth;
                }
                else
                {
                    Owner.HoverStartIndex = Calendar.ColumnsPerMonth * Calendar.RowsPerMonth - 1;
                }
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
            HeaderButton.Content   = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
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
        foreach (var child in YearView!.Children)
        {
            var childButton = (CalendarButton)child;
            // There should be no time component. Time is 12:00 AM
            var day = new DateTime(_currentMonth.Year, count + 1, 1);
            childButton.DataContext = day;

            childButton.Content   = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count];
            childButton.IsVisible = true;

            if (Owner != null)
            {
                if (day.Year == _currentMonth.Year && day.Month == _currentMonth.Month && day.Day == _currentMonth.Day)
                {
                    Owner.FocusCalendarButton           = childButton;
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
                    childButton.Opacity   = 0;
                }
                else
                {
                    childButton.IsEnabled = true;
                    childButton.Opacity   = 1;
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
            selectedYear  = Owner.SelectedYear;
            _currentMonth = Owner.SelectedMonth;
        }
        else
        {
            _currentMonth = DateTime.Today;
            selectedYear  = DateTime.Today;
        }

        var decade    = DateTimeHelper.DecadeOfDate(selectedYear);
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
            Owner.FocusCalendarButton                          = calendarButton;
            calendarButton.IsCalendarButtonFocused             = Owner.HasFocusInternal;

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
                childButton.Content     = year.ToString(DateTimeHelper.GetCurrentDateFormat());
                childButton.IsVisible   = true;

                if (Owner != null)
                {
                    if (year == Owner.SelectedYear.Year)
                    {
                        Owner.FocusCalendarButton           = childButton;
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
                        childButton.Opacity   = 0;
                    }
                    else
                    {
                        childButton.IsEnabled = true;
                        childButton.Opacity   = 1;
                    }
                }

                // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a
                // trailing year or not
                childButton.IsInactive = year < decade || year > decadeEnd;
            }
            else
            {
                childButton.IsEnabled = false;
                childButton.Opacity   = 0;
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

            var      b = (HeadTextButton)sender!;
            DateTime d;

            if (b.IsEnabled)
            {
                if (Owner.DisplayMode == CalendarMode.Month)
                {
                    d                   = Owner.DisplayDateInternal;
                    Owner.SelectedMonth = new DateTime(d.Year, d.Month, 1);
                    Owner.DisplayMode   = CalendarMode.Year;
                }
                else
                {
                    Debug.Assert(Owner.DisplayMode == CalendarMode.Year,
                        "The Owner Calendar's DisplayMode should be Year!");
                    d                  = Owner.SelectedMonth;
                    Owner.SelectedYear = new DateTime(d.Year, d.Month, 1);
                    Owner.DisplayMode  = CalendarMode.Decade;
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

            var b = (AvaloniaButton)sender!;
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

            var b = (AvaloniaButton)sender!;
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

            var b = (AvaloniaButton)sender!;

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

            var b = (AvaloniaButton)sender!;

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
            if (_isMouseLeftButtonDown
                && sender is CalendarDayButton
                {
                    IsEnabled: true, IsBlackout: false, DataContext: DateTime selectedDate
                } b)
            {
                // Update the states of all buttons to be selected starting
                // from HoverStart to b
                switch (Owner.SelectionMode)
                {
                    case CalendarSelectionMode.SingleDate:
                    {
                        Owner.CalendarDatePickerDisplayDateFlag = true;
                        if (Owner.SelectedDates.Count == 0)
                        {
                            Owner.SelectedDates.Add(selectedDate);
                        }
                        else
                        {
                            Owner.SelectedDates[0] = selectedDate;
                        }

                        return;
                    }
                    case CalendarSelectionMode.SingleRange:
                    case CalendarSelectionMode.MultipleRange:
                    {
                        Owner.UnHighlightDays();
                        Owner.HoverEndIndex = b.Index;
                        Owner.HoverEnd      = selectedDate;
                        // Update the States of the buttons
                        Owner.HighlightDays();
                        return;
                    }
                }
            }
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

            CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);

            if (sender is CalendarDayButton b)
            {
                _isControlPressed = ctrl;
                if (b.IsEnabled && !b.IsBlackout && b.DataContext is DateTime selectedDate)
                {
                    _isMouseLeftButtonDown = true;

                    switch (Owner.SelectionMode)
                    {
                        case CalendarSelectionMode.None:
                        {
                            return;
                        }
                        case CalendarSelectionMode.SingleDate:
                        {
                            Owner.CalendarDatePickerDisplayDateFlag = true;
                            if (Owner.SelectedDates.Count == 0)
                            {
                                Owner.SelectedDates.Add(selectedDate);
                            }
                            else
                            {
                                Owner.SelectedDates[0] = selectedDate;
                            }

                            return;
                        }
                        case CalendarSelectionMode.SingleRange:
                        {
                            // Set the start or end of the selection
                            // range
                            if (shift)
                            {
                                Owner.UnHighlightDays();
                                Owner.HoverEnd      = selectedDate;
                                Owner.HoverEndIndex = b.Index;
                                Owner.HighlightDays();
                            }
                            else
                            {
                                Owner.UnHighlightDays();
                                Owner.HoverStart      = selectedDate;
                                Owner.HoverStartIndex = b.Index;
                            }

                            return;
                        }
                        case CalendarSelectionMode.MultipleRange:
                        {
                            if (shift)
                            {
                                if (!ctrl)
                                {
                                    // clear the list, set the states to
                                    // default
                                    foreach (var item in Owner.SelectedDates)
                                    {
                                        Owner.RemovedItems.Add(item);
                                    }

                                    Owner.SelectedDates.ClearInternal();
                                }

                                Owner.HoverEnd      = selectedDate;
                                Owner.HoverEndIndex = b.Index;
                                Owner.HighlightDays();
                            }
                            else
                            {
                                if (!ctrl)
                                {
                                    // clear the list, set the states to
                                    // default
                                    foreach (var item in Owner.SelectedDates)
                                    {
                                        Owner.RemovedItems.Add(item);
                                    }

                                    Owner.SelectedDates.ClearInternal();
                                    Owner.UnHighlightDays();
                                }

                                Owner.HoverStart      = selectedDate;
                                Owner.HoverStartIndex = b.Index;
                            }

                            return;
                        }
                    }
                }
                else
                {
                    // If a click occurs on a BlackOutDay we set the
                    // HoverStart to be null
                    Owner.HoverStart = null;
                }
            }
            else
            {
                _isControlPressed = false;
            }
        }
    }

    private void AddSelection(CalendarDayButton b, DateTime selectedDate)
    {
        if (Owner != null)
        {
            Owner.HoverEndIndex = b.Index;
            Owner.HoverEnd      = selectedDate;

            if (Owner.HoverEnd != null && Owner.HoverStart != null)
            {
                // this is selection with Mouse, we do not guarantee the
                // range does not include BlackOutDates.  AddRange method
                // will throw away the BlackOutDates based on the
                // SelectionMode
                Owner.IsMouseSelection = true;
                Owner.SelectedDates.AddRange(Owner.HoverStart.Value, Owner.HoverEnd.Value);
                Owner.OnDayClick(selectedDate);
            }
        }
    }

    internal void HandleCellMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        if (Owner != null)
        {
            var b = sender as CalendarDayButton;
            if (b != null && !b.IsBlackout)
            {
                Owner.OnDayButtonMouseUp(e);
            }

            _isMouseLeftButtonDown = false;
            if (b != null && b.DataContext is DateTime selectedDate)
            {
                if (Owner.SelectionMode == CalendarSelectionMode.None ||
                    Owner.SelectionMode == CalendarSelectionMode.SingleDate)
                {
                    Owner.OnDayClick(selectedDate);
                    return;
                }

                if (Owner.HoverStart.HasValue)
                {
                    switch (Owner.SelectionMode)
                    {
                        case CalendarSelectionMode.SingleRange:
                        {
                            // Update SelectedDates
                            foreach (var item in Owner.SelectedDates)
                            {
                                Owner.RemovedItems.Add(item);
                            }

                            Owner.SelectedDates.ClearInternal();
                            AddSelection(b, selectedDate);
                            return;
                        }
                        case CalendarSelectionMode.MultipleRange:
                        {
                            // add the selection (either single day or
                            // SingleRange day)
                            AddSelection(b, selectedDate);
                            return;
                        }
                    }
                }
                else
                {
                    // If the day is Disabled but a trailing day we should
                    // be able to switch months
                    if (b.IsInactive && b.IsBlackout)
                    {
                        Owner.OnDayClick(selectedDate);
                    }
                }
            }
        }
    }

    private void HandleCellClick(object? sender, RoutedEventArgs e)
    {
        if (Owner != null)
        {
            if (_isControlPressed && Owner.SelectionMode == CalendarSelectionMode.MultipleRange)
            {
                var b = (CalendarDayButton)sender!;

                if (b.IsSelected)
                {
                    Owner.HoverStart       = null;
                    _isMouseLeftButtonDown = false;
                    b.IsSelected           = false;
                    if (b.DataContext is DateTime selectedDate)
                    {
                        Owner.SelectedDates.Remove(selectedDate);
                    }
                }
            }
        }

        _isControlPressed = false;
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
                Owner.DisplayMode   = CalendarMode.Year;
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
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }
}