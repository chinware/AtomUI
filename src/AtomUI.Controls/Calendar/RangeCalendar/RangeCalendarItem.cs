using System.Diagnostics;
using System.Globalization;
using AtomUI.Collections.Pooled;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls;

[TemplatePart(RangeCalendarItemTheme.PrimaryHeaderButtonPart, typeof(HeadTextButton))]
[TemplatePart(RangeCalendarItemTheme.SecondaryHeaderButtonPart, typeof(HeadTextButton))]
[TemplatePart(RangeCalendarItemTheme.MonthViewPart, typeof(Grid))]
[TemplatePart(RangeCalendarItemTheme.PrimaryPreviousButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.PrimaryPreviousMonthButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.PrimaryNextButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.PrimaryNextMonthButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.SecondaryPreviousButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.SecondaryPreviousMonthButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.SecondaryNextButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.SecondaryNextMonthButtonPart, typeof(IconButton))]
[TemplatePart(RangeCalendarItemTheme.YearViewPart, typeof(Grid))]
internal class RangeCalendarItem : TemplatedControl
{
    internal const string CalendarDisabledPC = ":calendardisabled";
    internal const int MonthViewSize = 49; // 7 x 7

    /// <summary>
    /// The number of days per week.
    /// </summary>
    internal const int NumberOfDaysPerWeek = 7;

    protected readonly System.Globalization.Calendar _calendar = new GregorianCalendar();
    
        #region 公共属性定义

    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        RangeCalendar.HeaderBackgroundProperty.AddOwner<RangeCalendarItem>();

    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    public static readonly StyledProperty<ITemplate<Control>?> DayTitleTemplateProperty =
        AvaloniaProperty.Register<RangeCalendarItem, ITemplate<Control>?>(
            nameof(DayTitleTemplate),
            defaultBindingMode: BindingMode.OneTime);

    public ITemplate<Control>? DayTitleTemplate
    {
        get => GetValue(DayTitleTemplateProperty);
        set => SetValue(DayTitleTemplateProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<RangeCalendarItem, bool> IsMonthViewModeProperty =
        AvaloniaProperty.RegisterDirect<RangeCalendarItem, bool>(nameof(IsMonthViewMode),
            o => o.IsMonthViewMode,
            (o, v) => o.IsMonthViewMode = v);

    private bool _isMonthViewMode = true;

    // 当鼠标移动到日历单元格外面的时候还原 hover 临时的高亮
    private IDisposable? _restoreHoverPositionDisposable;

    internal bool IsMonthViewMode
    {
        get => _isMonthViewMode;
        set => SetAndRaise(IsMonthViewModeProperty, ref _isMonthViewMode, value);
    }

    /// <summary>
    /// Gets the button that allows switching between month mode, year mode,
    /// and decade mode.
    /// </summary>
    internal HeadTextButton? PrimaryHeaderButton
    {
        get => _primaryHeaderButton;

        private set
        {
            if (_primaryHeaderButton != null)
            {
                _primaryHeaderButton.Click -= HandleHeaderButtonClick;
            }

            _primaryHeaderButton = value;

            if (_primaryHeaderButton != null)
            {
                _primaryHeaderButton.Click     += HandleHeaderButtonClick;
                _primaryHeaderButton.Focusable =  false;
            }
        }
    }

    /// <summary>
    /// Gets the button that allows switching between month mode, year mode,
    /// and decade mode.
    /// </summary>
    internal HeadTextButton? SecondaryHeaderButton
    {
        get => _secondaryHeaderButton;

        private set
        {
            if (_secondaryHeaderButton != null)
            {
                _secondaryHeaderButton.Click -= HandleHeaderButtonClick;
            }

            _secondaryHeaderButton = value;

            if (_secondaryHeaderButton != null)
            {
                _secondaryHeaderButton.Click     += HandleHeaderButtonClick;
                _secondaryHeaderButton.Focusable =  false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the next page of the calendar when it
    /// is clicked.
    /// </summary>
    internal IconButton? PrimaryNextButton
    {
        get => _primaryNextButton;

        private set
        {
            if (_primaryNextButton != null)
            {
                _primaryNextButton.Click -= HandleNextButtonClick;
            }

            _primaryNextButton = value;

            if (_primaryNextButton != null)
            {
                _primaryNextButton.Click     += HandleNextButtonClick;
                _primaryNextButton.Focusable =  false;
            }
        }
    }

    internal IconButton? PrimaryNextMonthButton
    {
        get => _primaryNextMonthButton;

        private set
        {
            if (_primaryNextMonthButton != null)
            {
                _primaryNextMonthButton.Click -= HandleNextMonthButtonClick;
            }

            _primaryNextMonthButton = value;

            if (_primaryNextMonthButton != null)
            {
                _primaryNextMonthButton.Click     += HandleNextMonthButtonClick;
                _primaryNextMonthButton.Focusable =  false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the previous page of the calendar when
    /// it is clicked.
    /// </summary>
    internal IconButton? PrimaryPreviousButton
    {
        get => _primaryPreviousButton;

        private set
        {
            if (_primaryPreviousButton != null)
            {
                _primaryPreviousButton.Click -= HandlePreviousButtonClick;
            }

            _primaryPreviousButton = value;

            if (_primaryPreviousButton != null)
            {
                _primaryPreviousButton.Click     += HandlePreviousButtonClick;
                _primaryPreviousButton.Focusable =  false;
            }
        }
    }

    internal IconButton? PrimaryPreviousMonthButton
    {
        get => _primaryPreviousMonthButton;

        private set
        {
            if (_primaryPreviousMonthButton != null)
            {
                _primaryPreviousMonthButton.Click -= HandlePreviousMonthButtonClick;
            }

            _primaryPreviousMonthButton = value;

            if (_primaryPreviousMonthButton != null)
            {
                _primaryPreviousMonthButton.Click     += HandlePreviousMonthButtonClick;
                _primaryPreviousMonthButton.Focusable =  false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the next page of the calendar when it
    /// is clicked.
    /// </summary>
    internal IconButton? SecondaryNextButton
    {
        get => _secondaryNextButton;

        private set
        {
            if (_secondaryNextButton != null)
            {
                _secondaryNextButton.Click -= HandleNextButtonClick;
            }

            _secondaryNextButton = value;

            if (_secondaryNextButton != null)
            {
                _secondaryNextButton.Click     += HandleNextButtonClick;
                _secondaryNextButton.Focusable =  false;
            }
        }
    }

    internal IconButton? SecondaryNextMonthButton
    {
        get => _secondaryNextMonthButton;

        private set
        {
            if (_secondaryNextMonthButton != null)
            {
                _secondaryNextMonthButton.Click -= HandleNextMonthButtonClick;
            }

            _secondaryNextMonthButton = value;

            if (_secondaryNextMonthButton != null)
            {
                _secondaryNextMonthButton.Click     += HandleNextMonthButtonClick;
                _secondaryNextMonthButton.Focusable =  false;
            }
        }
    }

    /// <summary>
    /// Gets the button that displays the previous page of the calendar when
    /// it is clicked.
    /// </summary>
    internal IconButton? SecondaryPreviousButton
    {
        get => _secondaryPreviousButton;

        private set
        {
            if (_secondaryPreviousButton != null)
            {
                _secondaryPreviousButton.Click -= HandlePreviousButtonClick;
            }

            _secondaryPreviousButton = value;

            if (_secondaryPreviousButton != null)
            {
                _secondaryPreviousButton.Click     += HandlePreviousButtonClick;
                _secondaryPreviousButton.Focusable =  false;
            }
        }
    }

    internal IconButton? SecondaryPreviousMonthButton
    {
        get => _secondaryPreviousMonthButton;

        private set
        {
            if (_secondaryPreviousMonthButton != null)
            {
                _secondaryPreviousMonthButton.Click -= HandlePreviousMonthButtonClick;
            }

            _secondaryPreviousMonthButton = value;

            if (_secondaryPreviousMonthButton != null)
            {
                _secondaryPreviousMonthButton.Click     += HandlePreviousMonthButtonClick;
                _secondaryPreviousMonthButton.Focusable =  false;
            }
        }
    }

    #endregion

    protected DateTime _currentMonth;
    protected UniformGrid? _headerLayout;
    protected bool _isMouseLeftButtonDownYearView;
    protected DateTime _nextMonth;

    protected HeadTextButton? _primaryHeaderButton;
    protected IconButton? _primaryNextButton;
    protected IconButton? _primaryNextMonthButton;
    protected IconButton? _primaryPreviousButton;
    protected IconButton? _primaryPreviousMonthButton;
    protected HeadTextButton? _secondaryHeaderButton;

    protected IconButton? _secondaryNextButton;
    protected IconButton? _secondaryNextMonthButton;
    protected IconButton? _secondaryPreviousButton;
    protected IconButton? _secondaryPreviousMonthButton;

    internal RangeCalendar? Owner { get; set; }

    /// <summary>
    /// Gets the Grid that hosts the content when in month mode.
    /// </summary>
    internal UniformGrid? MonthView { get; set; }

    internal Grid? PrimaryMonthView { get; set; }
    internal Grid? SecondaryMonthView { get; set; }
    private bool _pointerInMonthView;

    /// <summary>
    /// Gets the Grid that hosts the content when in year or decade mode.
    /// </summary>
    internal Grid? YearView { get; set; }

    private void PopulateGrids()
    {
        if (MonthView != null)
        {
            if (PrimaryMonthView is not null)
            {
                PopulateMonthViewGrids(PrimaryMonthView);
            }

            if (SecondaryMonthView is not null)
            {
                PopulateMonthViewGrids(SecondaryMonthView);
            }
        }

        if (YearView != null)
        {
            var       childCount = RangeCalendar.RowsPerYear * RangeCalendar.ColumnsPerYear;
            using var children   = new PooledList<Control>(childCount);

            EventHandler<PointerPressedEventArgs>  monthCalendarButtonMouseDown = HandleMonthCalendarButtonMouseDown;
            EventHandler<PointerReleasedEventArgs> monthCalendarButtonMouseUp   = HandleMonthCalendarButtonMouseUp;
            EventHandler<PointerEventArgs>         monthMouseEntered            = HandleMonthMouseEntered;

            for (var i = 0; i < RangeCalendar.RowsPerYear; i++)
            {
                for (var j = 0; j < RangeCalendar.ColumnsPerYear; j++)
                {
                    var month = new RangeCalendarButton();

                    if (Owner != null)
                    {
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

    private void PopulateMonthViewGrids(Grid monthView)
    {
        var       childCount = RangeCalendar.RowsPerMonth + RangeCalendar.RowsPerMonth * RangeCalendar.ColumnsPerMonth;
        using var children   = new PooledList<Control>(childCount);

        for (var i = 0; i < RangeCalendar.ColumnsPerMonth; i++)
        {
            if (DayTitleTemplate?.Build() is Control cell)
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

        for (var i = 1; i < RangeCalendar.RowsPerMonth; i++)
        {
            for (var j = 0; j < RangeCalendar.ColumnsPerMonth; j++)
            {
                var cell = new RangeCalendarDayButton();

                if (Owner != null)
                {
                    cell.Owner = Owner;
                }

                cell.IsInPrimaryMonView = monthView == PrimaryMonthView;

                cell.SetValue(Grid.RowProperty, i);
                cell.SetValue(Grid.ColumnProperty, j);
                cell.CalendarDayButtonMouseDown += cellMouseLeftButtonDown;
                cell.CalendarDayButtonMouseUp   += cellMouseLeftButtonUp;
                cell.PointerEntered             += cellMouseEntered;
                cell.Click                      += cellClick;
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
        PrimaryHeaderButton   = e.NameScope.Find<HeadTextButton>(RangeCalendarItemTheme.PrimaryHeaderButtonPart);
        SecondaryHeaderButton = e.NameScope.Find<HeadTextButton>(RangeCalendarItemTheme.SecondaryHeaderButtonPart);
        PrimaryPreviousButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryPreviousButtonPart);
        PrimaryPreviousMonthButton =
            e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryPreviousMonthButtonPart);
        PrimaryNextButton      = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryNextButtonPart);
        PrimaryNextMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryNextMonthButtonPart);

        SecondaryPreviousButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryPreviousButtonPart);
        SecondaryPreviousMonthButton =
            e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryPreviousMonthButtonPart);
        SecondaryNextButton      = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryNextButtonPart);
        SecondaryNextMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryNextMonthButtonPart);

        MonthView          = e.NameScope.Find<UniformGrid>(RangeCalendarItemTheme.MonthViewPart);
        PrimaryMonthView   = e.NameScope.Find<Grid>(RangeCalendarItemTheme.PrimaryMonthViewPart);
        SecondaryMonthView = e.NameScope.Find<Grid>(RangeCalendarItemTheme.SecondaryMonthViewPart);
        YearView           = e.NameScope.Find<Grid>(RangeCalendarItemTheme.YearViewPart);
        _headerLayout      = e.NameScope.Find<UniformGrid>(RangeCalendarItemTheme.HeaderLayoutPart);

        if (Owner != null)
        {
            UpdateDisabled(Owner.IsEnabled);
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

    private void SetupHeaderForDisplayModeChanged()
    {
        if (Owner is null || MonthView is null || _headerLayout is null)
        {
            return;
        }

        if (Owner.DisplayMode == CalendarMode.Month)
        {
            MonthView.Columns     = 2;
            _headerLayout.Columns = 2;
        }
        else if (Owner.DisplayMode == CalendarMode.Year || Owner.DisplayMode == CalendarMode.Decade)
        {
            MonthView.Columns     = 1;
            _headerLayout.Columns = 1;
        }

        IsMonthViewMode = Owner.DisplayMode == CalendarMode.Month;
    }

    protected void SetDayTitles()
    {
        if (PrimaryMonthView is not null)
        {
            SetDayTitles(PrimaryMonthView);
        }

        if (SecondaryMonthView is not null)
        {
            SetDayTitles(SecondaryMonthView);
        }
    }

    protected void SetDayTitles(Grid monthView)
    {
        for (var childIndex = 0; childIndex < RangeCalendar.ColumnsPerMonth; childIndex++)
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
            _nextMonth    = Owner.SecondaryDisplayDateInternal;
        }
        else
        {
            _currentMonth = DateTime.Today;
            _nextMonth    = DateTime.Today;
        }

        SetMonthModeHeaderButton();
        SetMonthModePreviousButton(_currentMonth);
        SetMonthModeNextButton(_currentMonth);

        if (MonthView != null)
        {
            SetDayTitles();
            if (PrimaryMonthView is not null)
            {
                SetCalendarDayButtons(_currentMonth, PrimaryMonthView);
            }

            if (SecondaryMonthView is not null)
            {
                SetCalendarDayButtons(_nextMonth, SecondaryMonthView);
            }
        }
    }

    protected virtual void SetMonthModeHeaderButton()
    {
        if (PrimaryHeaderButton is not null)
        {
            if (Owner is not null)
            {
                PrimaryHeaderButton.Content =
                    Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                PrimaryHeaderButton.IsEnabled = true;
            }
            else
            {
                PrimaryHeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            }
        }

        if (SecondaryHeaderButton is not null)
        {
            if (Owner is not null)
            {
                SecondaryHeaderButton.Content =
                    Owner.SecondaryDisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                SecondaryHeaderButton.IsEnabled = true;
            }
            else
            {
                SecondaryHeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            }
        }
    }

    protected void SetMonthModeNextButton(DateTime firstDayOfMonth)
    {
        if (Owner != null && PrimaryNextButton != null)
        {
            // DisplayDate is equal to DateTime.MaxValue
            if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0)
            {
                PrimaryNextButton.IsEnabled = false;
            }
            else
            {
                // Since we are sure DisplayDate is not equal to
                // DateTime.MaxValue, it is safe to use AddMonths  
                var firstDayOfNextMonth = _calendar.AddMonths(firstDayOfMonth, 1);
                PrimaryNextButton.IsEnabled =
                    DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth) > -1;
            }
        }
    }

    protected void SetMonthModePreviousButton(DateTime firstDayOfMonth)
    {
        if (Owner != null && PrimaryPreviousButton != null)
        {
            PrimaryPreviousButton.IsEnabled =
                DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0;
        }
    }

    private void SetButtonState(RangeCalendarDayButton childButton, DateTime dateToAdd, Grid monthView)
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
                childButton.IsInactive = CheckDayInactiveState(dateToAdd, monthView);

                // SET IF THE DAY IS TODAY OR NOT
                childButton.IsToday = CheckDayIsTodayState(dateToAdd);

                // SET IF THE DAY IS SELECTED OR NOT
                childButton.IsSelected = false;
                DateTime? rangeStart = default;
                DateTime? rangeEnd   = default;
                Owner.SortHoverIndexes(out rangeStart, out rangeEnd);
                if (rangeStart != null && rangeEnd != null)
                {
                    childButton.IsSelected = DateTimeHelper.InRange(dateToAdd, rangeStart.Value, rangeEnd.Value);
                }
                else if (rangeStart is not null)
                {
                    childButton.IsSelected = DateTimeHelper.CompareDays(rangeStart.Value, dateToAdd) == 0;
                }
                else if (rangeEnd is not null)
                {
                    childButton.IsSelected = DateTimeHelper.CompareDays(rangeEnd.Value, dateToAdd) == 0;
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

    protected virtual bool CheckDayInactiveState(DateTime dateToAdd, Grid monthView)
    {
        if (Owner is not null)
        {
            if (monthView == PrimaryMonthView)
            {
                return DateTimeHelper.CompareYearMonth(dateToAdd, Owner.DisplayDateInternal) != 0;
            }

            if (monthView == SecondaryMonthView)
            {
                return DateTimeHelper.CompareYearMonth(dateToAdd, Owner.SecondaryDisplayDateInternal) != 0;
            }
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

        var count = RangeCalendar.RowsPerMonth * RangeCalendar.ColumnsPerMonth;

        for (var childIndex = RangeCalendar.ColumnsPerMonth; childIndex < count; childIndex++)
        {
            var childButton = (RangeCalendarDayButton)monthView!.Children[childIndex];

            childButton.Index = childIndex;
            if (monthView == SecondaryMonthView)
            {
                childButton.Index += MonthViewSize;
            }

            SetButtonState(childButton, dateToAdd, monthView);

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
                    childButton = (RangeCalendarDayButton)monthView.Children[i];
                    // button needs a content to occupy the necessary space
                    // for the content presenter
                    childButton.Content   = i.ToString(DateTimeHelper.GetCurrentDateFormat());
                    childButton.IsEnabled = false;
                    childButton.Opacity   = 0;
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
        if (PrimaryHeaderButton != null)
        {
            PrimaryHeaderButton.IsEnabled = true;
            PrimaryHeaderButton.Content   = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
        }
    }

    private void SetYearModePreviousButton()
    {
        if (Owner != null && PrimaryPreviousButton != null)
        {
            PrimaryPreviousButton.IsEnabled = Owner.DisplayDateRangeStart.Year != _currentMonth.Year;
        }
    }

    private void SetYearModeNextButton()
    {
        if (Owner != null && PrimaryNextButton != null)
        {
            PrimaryNextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != _currentMonth.Year;
        }
    }

    private void SetMonthButtonsForYearMode()
    {
        var count = 0;
        foreach (object child in YearView!.Children)
        {
            var childButton = (RangeCalendarButton)child;
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

    internal void UpdateYearViewSelection(RangeCalendarButton? calendarButton)
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
            var childButton = (RangeCalendarButton)child;
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
        if (PrimaryHeaderButton != null)
        {
            PrimaryHeaderButton.Content = decade.ToString(CultureInfo.CurrentCulture) + "-" +
                                          decadeEnd.ToString(CultureInfo.CurrentCulture);
            PrimaryHeaderButton.IsEnabled = false;
        }
    }

    private void SetDecadeModeNextButton(int decadeEnd)
    {
        if (Owner != null && PrimaryNextButton != null)
        {
            PrimaryNextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year > decadeEnd;
        }
    }

    private void SetDecadeModePreviousButton(int decade)
    {
        if (Owner != null && PrimaryPreviousButton != null)
        {
            PrimaryPreviousButton.IsEnabled = decade > Owner.DisplayDateRangeStart.Year;
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
                        "The Owner RangeCalendar's DisplayMode should be Year!");
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
            if (sender is RangeCalendarDayButton
                {
                    IsEnabled: true, IsBlackout: false, DataContext: DateTime selectedDate
                } b)
            {
                Owner.HoverDateTime = selectedDate;
                // Update the States of the buttons
                Owner.UpdateHighlightDays();
            }
        }
    }

    internal void HandleCellMouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
    {
        if (Owner != null) {
            if (!Owner.HasFocusInternal) {
                Owner.Focus();
            }
           
            if (sender is RangeCalendarDayButton b) {
                if (b.IsEnabled && !b.IsBlackout && b.DataContext is DateTime selectedDate) {
                    // Set the start or end of the selection
                    // range
                    if (Owner.FixedRangeStart)
                    {
                        Owner.SelectedRangeStartDate = selectedDate;
                    } else {
                        Owner.SelectedRangeEndDate = selectedDate;
                    }
                    Owner.UpdateHighlightDays();
                    if (Owner.SelectedRangeStartDate is not null && Owner.SelectedRangeEndDate is not null)
                    {
                        Owner.NotifyRangeDateSelected();
                    }
                }
            }
        }
    }

    internal void HandleCellMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        if (Owner != null) {
           RangeCalendarDayButton? b = sender as RangeCalendarDayButton;
           if (b != null && !b.IsBlackout) {
              Owner.OnDayButtonMouseUp(e);
           }
           if (b != null && b.DataContext is DateTime selectedDate) {
               // If the day is Disabled but a trailing day we should
               // be able to switch months
               if (b.IsInactive) {
                   Owner.OnDayClick(selectedDate);
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

        UpdateYearViewSelection(sender as RangeCalendarButton);
    }

    protected internal virtual void HandleMonthCalendarButtonMouseUp(object? sender, PointerReleasedEventArgs e)
    {
        _isMouseLeftButtonDownYearView = false;

        if (Owner != null && (sender as RangeCalendarButton)?.DataContext is DateTime newMonth)
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
            UpdateYearViewSelection(sender as RangeCalendarButton);
        }
    }

    internal void UpdateDisabled(bool isEnabled)
    {
        PseudoClasses.Set(CalendarDisabledPC, !isEnabled);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _restoreHoverPositionDisposable = inputManager.Process.Subscribe(DetectRestoreHoverPosition);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _restoreHoverPositionDisposable?.Dispose();
    }

    private void DetectRestoreHoverPosition(RawInputEventArgs args)
    {
        if (Owner is null) {
            return;
        }
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            if (!IsPointerInMonthView(pointerEventArgs.Position))
            {
                Owner.HoverDateTime = Owner.FixedRangeStart 
                    ? Owner.SelectedRangeStartDate 
                    : Owner.SelectedRangeEndDate;
                // Update the States of the buttons
                if (_pointerInMonthView)
                {
                    Owner.HoverDateTime = null;
                    Owner.UpdateHighlightDays();
                }

                _pointerInMonthView = false;
            }
            else
            {
                _pointerInMonthView = true;
            }
        }
    }

    private bool IsPointerInMonthView(Point position)
    {
        if (Owner is null)
        {
            return false;
        }
        if (Owner.DisplayMode == CalendarMode.Month)
        {
            return GetMonthViewRect(PrimaryMonthView!).Contains(position) || GetMonthViewRect(SecondaryMonthView!).Contains(position);
        }
        return false;
    }

    private Rect GetMonthViewRect(Grid monthView)
    {
        var firstDay     = (monthView.Children[7] as RangeCalendarDayButton)!;
        var firstDayPos  = firstDay.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(monthView)!) ?? default;
        var monthViewPos = monthView.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(monthView)!) ?? default;
        return new Rect(firstDayPos, new Size(monthView.Bounds.Width, monthViewPos.Y + monthView.Bounds.Height - firstDayPos.Y ));
    }
}