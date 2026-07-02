using System.Diagnostics;
using System.Globalization;
using AtomUI.Collections.Pooled;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.CalendarView.Infrastructure;
using AtomUI.Desktop.Controls.CalendarView.Rendering;
using AtomUI.Desktop.Controls.CalendarView.State;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.CalendarView;

[TemplatePart("PART_HeaderButton", typeof(HeadTextButton))]
[TemplatePart("PART_HeaderLayout", typeof(Panel))]
[TemplatePart("PART_MonthViewLayout", typeof(UniformGrid))]
[TemplatePart("PART_MonthView", typeof(Grid))]
[TemplatePart("PART_PreviousButton", typeof(IconButton))]
[TemplatePart("PART_PreviousMonthButton", typeof(IconButton))]
[TemplatePart("PART_NextButton", typeof(IconButton))]
[TemplatePart("PART_NextMonthButton", typeof(IconButton))]
[TemplatePart("PART_YearView", typeof(Grid))]
internal class CalendarItem : TemplatedControl
{
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

    #region 内部协作 API

    internal const string CalendarDisabledPC = ":calendardisabled";
    internal const int NumberOfDaysPerWeek = 7;

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<CalendarItem, bool> IsMonthViewModeProperty =
        AvaloniaProperty.RegisterDirect<CalendarItem, bool>(nameof(IsMonthViewMode),
            o => o.IsMonthViewMode,
            (o, v) => o.IsMonthViewMode = v);

    internal static readonly DirectProperty<CalendarItem, bool> IsQuarterViewModeProperty =
        AvaloniaProperty.RegisterDirect<CalendarItem, bool>(nameof(IsQuarterViewMode),
            o => o.IsQuarterViewMode,
            (o, v) => o.IsQuarterViewMode = v);
    
    internal static readonly StyledProperty<Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.Register<CalendarItem, Thickness>(nameof(HeaderBorderThickness));
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CalendarItem>();

    private bool _isMonthViewMode = true;
    private bool _isQuarterViewMode;

    /// <summary>
    /// 主要方便在模板中控制导航按钮的显示和关闭
    /// </summary>
    internal bool IsMonthViewMode
    {
        get => _isMonthViewMode;
        set => SetAndRaise(IsMonthViewModeProperty, ref _isMonthViewMode, value);
    }

    internal bool IsQuarterViewMode
    {
        get => _isQuarterViewMode;
        set => SetAndRaise(IsQuarterViewModeProperty, ref _isQuarterViewMode, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal Thickness HeaderBorderThickness
    {
        get => GetValue(HeaderBorderThicknessProperty);
        set => SetValue(HeaderBorderThicknessProperty, value);
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
    
    protected readonly System.Globalization.Calendar _calendar = new GregorianCalendar();
    protected DateTime _currentMonth;
    protected Panel? _headerLayout;
    protected bool _isMouseLeftButtonDownYearView;

    protected HeadTextButton? _headerButton;
    protected IconButton? _nextButton;
    protected IconButton? _nextMonthButton;
    protected IconButton? _previousButton;
    protected IconButton? _previousMonthButton;

    // 当鼠标移动到日历单元格外面的时候还原 hover 临时的高亮
    private CalendarPointerTracker? _pointerTracker;
    private CalendarGeneratedContainerManager? _generatedContainerManager;

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

    private CalendarGeneratedContainerManager GeneratedContainerManager =>
        _generatedContainerManager ??= new CalendarGeneratedContainerManager(this);
    
    private void PopulateGrids()
    {
        PopulateMonthViewsGrid();
        PopulateYearViewsGrid();
    }

    protected virtual void PopulateYearViewsGrid()
    {
        if (YearView != null)
        {
            PopulateYearViewGrid(YearView);
        }
    }

    protected void PopulateYearViewGrid(Grid yearView)
    {
        ConfigureYearViewLayout(yearView);

        var childCount = Calendar.RowsPerYear * Calendar.ColumnsPerYear;
        using var children = new PooledList<Control>(childCount);

        EventHandler<PointerPressedEventArgs> monthCalendarButtonMouseDown = HandleMonthCalendarButtonMouseDown;
        EventHandler<PointerReleasedEventArgs> monthCalendarButtonMouseUp = HandleMonthCalendarButtonMouseUp;
        EventHandler<PointerEventArgs> monthMouseEntered = HandleMonthMouseEntered;

        for (var i = 0; i < childCount; i++)
        {
            var month = new CalendarButton();

            if (Owner != null)
            {
                month.Owner = Owner;
                month[!CalendarButton.IsMotionEnabledProperty] = Owner[!Calendar.IsMotionEnabledProperty];
            }

            month.CalendarLeftMouseButtonDown += monthCalendarButtonMouseDown;
            month.CalendarLeftMouseButtonUp   += monthCalendarButtonMouseUp;
            month.PointerEntered              += monthMouseEntered;
            children.Add(month);
        }

        yearView.Children.AddRange(children);
        ConfigureYearViewLayout(yearView);
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
        ConfigureMonthViewColumns(monthView);
        var columnCount = GetMonthPanelColumnCount();
        var childCount  = columnCount + (Calendar.RowsPerMonth - 1) * columnCount;
        using var children = new PooledList<Control>(childCount);

        for (var i = 0; i < columnCount; i++)
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
        for (var i = 1; i < Calendar.RowsPerMonth; i++)
        {
            for (var j = 0; j < columnCount; j++)
            {
                var cell = new CalendarDayButton();

                if (Owner != null)
                {
                    cell.Owner = Owner;
                    cell[!CalendarDayButton.IsMotionEnabledProperty] = Owner[!Calendar.IsMotionEnabledProperty];
                }

                cell.SetValue(Grid.RowProperty, i);
                cell.SetValue(Grid.ColumnProperty, j);
                cell.CalendarDayButtonMouseDown += cellMouseLeftButtonDown;
                cell.CalendarDayButtonMouseUp += cellMouseLeftButtonUp;
                cell.PointerEntered += cellMouseEntered;
                children.Add(cell);
            }
        }

        monthView.Children.AddRange(children);
    }

    private int GetMonthPanelColumnCount()
    {
        return Owner?.PickerMode == DatePickerMode.Week
            ? Calendar.ColumnsPerWeekPanel
            : Calendar.ColumnsPerMonth;
    }

    private int GetExpectedMonthButtonCount()
    {
        return (Calendar.RowsPerMonth - 1) * GetMonthPanelColumnCount();
    }

    private int GetYearPanelRowCount()
    {
        if (Owner?.DisplayMode != CalendarMode.Year)
        {
            return Calendar.RowsPerYear;
        }

        return Owner.PickerMode == DatePickerMode.Quarter
            ? 1
            : Calendar.RowsPerMonthSelectionPanel;
    }

    private int GetYearPanelColumnCount()
    {
        if (Owner?.DisplayMode != CalendarMode.Year)
        {
            return Calendar.ColumnsPerYear;
        }

        return Owner.PickerMode == DatePickerMode.Quarter
            ? Calendar.ColumnsPerYear
            : Calendar.ColumnsPerMonthSelectionPanel;
    }

    private void ConfigureMonthViewColumns(Grid monthView)
    {
        var columnCount = GetMonthPanelColumnCount();
        if (monthView.ColumnDefinitions.Count == columnCount)
        {
            return;
        }

        monthView.ColumnDefinitions.Clear();
        for (var i = 0; i < columnCount; i++)
        {
            monthView.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }
    }

    private void ConfigureYearViewLayout()
    {
        ConfigureYearViewLayout(YearView);
    }

    protected void ConfigureYearViewLayout(Grid? yearView)
    {
        if (yearView is null)
        {
            return;
        }

        var rowCount = GetYearPanelRowCount();
        var columnCount = GetYearPanelColumnCount();
        if (yearView.RowDefinitions.Count != rowCount)
        {
            yearView.RowDefinitions.Clear();
            for (var i = 0; i < rowCount; i++)
            {
                yearView.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            }
        }

        if (yearView.ColumnDefinitions.Count != columnCount)
        {
            yearView.ColumnDefinitions.Clear();
            for (var i = 0; i < columnCount; i++)
            {
                yearView.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            }
        }

        var buttons = yearView.Children.OfType<CalendarButton>().ToArray();
        for (var i = 0; i < buttons.Length; i++)
        {
            var row = Math.Min(i / columnCount, rowCount - 1);
            buttons[i].SetValue(Grid.RowProperty, row);
            buttons[i].SetValue(Grid.ColumnProperty, i % columnCount);
        }
    }

    protected virtual void ClearGeneratedMonthViews()
    {
        if (MonthView is not null)
        {
            ClearGeneratedMonthView(MonthView);
        }
    }

    protected void ClearGeneratedMonthView(Grid monthView)
    {
        GeneratedContainerManager.ReleaseMonthView(monthView);
    }

    protected virtual void ClearGeneratedYearViews()
    {
        if (YearView is not null)
        {
            ClearGeneratedYearView(YearView);
        }
    }

    protected void ClearGeneratedYearView(Grid yearView)
    {
        GeneratedContainerManager.ReleaseYearView(yearView);
    }

    private void ClearGeneratedGrids()
    {
        ClearGeneratedMonthViews();
        ClearGeneratedYearViews();
    }

    private void EnsureGeneratedGrids()
    {
        if (MonthView is null && YearView is null)
        {
            return;
        }

        var monthViewMissing = MonthView is not null &&
                               MonthView.Children.OfType<CalendarDayButton>().Count() != GetExpectedMonthButtonCount();
        var yearViewMissing = YearView is not null &&
                              !YearView.Children.OfType<CalendarButton>().Any();
        if (!monthViewMissing && !yearViewMissing)
        {
            return;
        }

        ClearGeneratedGrids();
        PopulateGrids();
    }

    /// <summary>
    /// Builds the visual tree for the
    /// <see cref="T:Controls.Primitives.CalendarItem" />
    /// when a new template is applied.
    /// </summary>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ClearGeneratedGrids();
        base.OnApplyTemplate(e);
        HeaderButton   = e.NameScope.Get<HeadTextButton>("PART_HeaderButton");
        PreviousButton = e.NameScope.Get<IconButton>("PART_PreviousButton");
        PreviousMonthButton =
            e.NameScope.Get<IconButton>("PART_PreviousMonthButton");
        NextButton      = e.NameScope.Get<IconButton>("PART_NextButton");
        NextMonthButton = e.NameScope.Get<IconButton>("PART_NextMonthButton");

        MonthViewLayout = e.NameScope.Get<UniformGrid>("PART_MonthViewLayout");
        MonthView       = e.NameScope.Get<Grid>("PART_MonthView");
        YearView        = e.NameScope.Get<Grid>("PART_YearView");
        _headerLayout   = e.NameScope.Get<Panel>("PART_HeaderLayout");

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

        var isMonthViewMode = Owner.DisplayMode == CalendarMode.Month;
        IsMonthViewMode   = isMonthViewMode;
        IsQuarterViewMode = Owner.DisplayMode == CalendarMode.Year && Owner.PickerMode == DatePickerMode.Quarter;

        MonthViewLayout.IsVisible = isMonthViewMode;
        if (MonthView is not null)
        {
            MonthView.IsVisible = isMonthViewMode;
        }
        if (YearView is not null)
        {
            YearView.IsVisible = !isMonthViewMode;
        }
    }

    protected virtual void SetDayTitles()
    {
        if (MonthView is not null)
        {
            SetDayTitles(MonthView, _currentMonth);
        }
    }

    protected void SetDayTitles(Grid monthView, DateTime displayMonth)
    {
        var state = GetRenderState(displayMonth);
        var panel = CalendarPanelBuilder.BuildMonthPanel(state, displayMonth);
        var columnOffset = panel.WeekCells.Count > 0 ? 1 : 0;
        if (columnOffset == 1 && monthView.Children.Count > 0)
        {
            monthView.Children[0].DataContext = string.Empty;
        }
        for (var titleIndex = 0; titleIndex < Calendar.ColumnsPerMonth &&
                               titleIndex + columnOffset < monthView.Children.Count &&
                               titleIndex < panel.DayTitles.Count; titleIndex++)
        {
            monthView.Children[titleIndex + columnOffset].DataContext = panel.DayTitles[titleIndex];
        }
    }

    protected internal virtual void UpdateMonthMode()
    {
        SetupHeaderForDisplayModeChanged();
        EnsureGeneratedGrids();

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

    protected void SetCalendarDayButtons(DateTime firstDayOfMonth, Grid monthView)
    {
        var state = GetRenderState(firstDayOfMonth);
        var panel = CalendarPanelBuilder.BuildMonthPanel(state, firstDayOfMonth);
        CalendarItemRenderer.RenderMonthPanel(Owner, monthView, panel);
    }

    protected CalendarViewState GetRenderState(DateTime displayMonth)
    {
        return Owner?.SyncAndGetCurrentViewState()
               ?? CalendarViewState.CreateDefault(displayMonth, DateTimeHelper.GetCurrentDateFormat());
    }

    internal void UpdateYearMode()
    {
        SetupHeaderForDisplayModeChanged();
        ConfigureYearViewLayout();

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

    protected virtual void SetMonthButtonsForYearMode()
    {
        var state = GetRenderState(_currentMonth);
        if (Owner?.PickerMode == DatePickerMode.Quarter)
        {
            var panel = CalendarPanelBuilder.BuildQuarterPanel(state, _currentMonth);
            CalendarItemRenderer.RenderQuarterPanel(Owner, YearView!, panel);
        }
        else
        {
            var panel = CalendarPanelBuilder.BuildYearPanel(state, _currentMonth);
            CalendarItemRenderer.RenderYearPanel(Owner, YearView!, panel);
        }
    }

    internal void UpdateDecadeMode()
    {
        SetupHeaderForDisplayModeChanged();
        ConfigureYearViewLayout();

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
            SetYearButtons(selectedYear);
        }
    }

    internal void UpdateYearViewSelection(CalendarButton? calendarButton)
    {
        if (Owner != null && calendarButton?.DataContext is DateTime selectedDate)
        {
            Owner.FocusCalendarButton?.IsCalendarButtonFocused = false;
            Owner.FocusCalendarButton = calendarButton;
            calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;

            if (Owner.DisplayMode == CalendarMode.Year)
            {
                Owner.SelectedMonth = Owner.NormalizePickerDate(selectedDate);
            }
            else
            {
                Owner.SelectedYear = selectedDate;
            }
        }
    }

    protected virtual void SetYearButtons(DateTime selectedYear)
    {
        var state = GetRenderState(selectedYear);
        var panel = CalendarPanelBuilder.BuildDecadePanel(state, selectedYear);
        CalendarItemRenderer.RenderDecadePanel(Owner, YearView!, panel);
    }

    private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
    {
        if (HeaderButton != null)
        {
            var format = DateTimeHelper.GetCurrentDateFormat();
            HeaderButton.Content = decade.ToString(format) + "-" +
                                   decadeEnd.ToString(format);
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
            if (Owner.PickerMode == DatePickerMode.Week)
            {
                Owner.UpdateHighlightDays();
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
                Owner.SelectPickerDate(selectedDate);
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

    internal void HandleMonthCalendarButtonMouseDown(object? sender, PointerPressedEventArgs e)
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
                if (Owner.PickerMode is DatePickerMode.Month or DatePickerMode.Quarter)
                {
                    Owner.SelectPickerDate(newMonth);
                }
                else
                {
                    Owner.DisplayDate = newMonth;
                    Owner.DisplayMode = CalendarMode.Month;
                }
            }
            else
            {
                Debug.Assert(Owner.DisplayMode == CalendarMode.Decade, "The owning Calendar should be in decade mode!");
                if (Owner.PickerMode == DatePickerMode.Year)
                {
                    Owner.SelectPickerDate(newMonth);
                }
                else
                {
                    Owner.SelectedMonth = newMonth;
                    Owner.DisplayMode = CalendarMode.Year;
                }
            }

            SetupHeaderForDisplayModeChanged();
        }
    }

    internal void HandleMonthMouseEntered(object? sender, PointerEventArgs e)
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

    internal void UpdatePointerMonthViewState(Point position)
    {
        if (Owner is null)
        {
            return;
        }

        var originState = Owner.IsPointerInMonthView;
        if (!IsPointerInMonthView(position))
        {
            Owner.IsPointerInMonthView = false;
            NotifyPointerOutMonthView(originState);
        }
        else
        {
            Owner.IsPointerInMonthView = true;
            UpdateWeekHoverDateFromPointerPosition(position);
            NotifyPointerInMonthView(originState);
        }
    }

    private void UpdateWeekHoverDateFromPointerPosition(Point position)
    {
        if (Owner?.PickerMode != DatePickerMode.Week || Owner.DisplayMode != CalendarMode.Month || MonthView is null)
        {
            return;
        }

        if (TryGetWeekStartFromPointerPosition(position, out var weekStart))
        {
            if (Owner.HoverDate is null ||
                !DatePickerFormattingHelper.IsSamePickerUnit(Owner.HoverDate.Value, weekStart, DatePickerMode.Week))
            {
                Owner.NotifyHoverDateChanged(weekStart);
                Owner.UpdateHighlightDays();
            }
        }
        else if (Owner.HoverDate is not null)
        {
            Owner.NotifyHoverDateChanged(null);
            Owner.UpdateHighlightDays();
        }
    }

    protected virtual bool TryGetWeekStartFromPointerPosition(Point position, out DateTime weekStart)
    {
        return TryGetWeekStartFromMonthViewPosition(MonthView, position, out weekStart);
    }

    protected bool TryGetWeekStartFromMonthViewPosition(Grid? monthView, Point position, out DateTime weekStart)
    {
        weekStart = default;
        if (monthView is null)
        {
            return false;
        }

        var topLevel = TopLevel.GetTopLevel(monthView);
        if (topLevel is null)
        {
            return false;
        }

        var monthViewPosition = monthView.TranslatePoint(new Point(0, 0), topLevel);
        if (monthViewPosition is null)
        {
            return false;
        }

        var weekButtons = monthView.Children
                                    .OfType<CalendarDayButton>()
                                    .Where(button => button.IsWeekNumber &&
                                                     button.IsEnabled &&
                                                     button.DataContext is DateTime)
                                    .ToArray();
        foreach (var weekButton in weekButtons)
        {
            var buttonPosition = weekButton.TranslatePoint(new Point(0, 0), topLevel);
            if (buttonPosition is null)
            {
                continue;
            }

            var rowBounds = new Rect(
                new Point(monthViewPosition.Value.X, buttonPosition.Value.Y),
                new Size(monthView.Bounds.Width, weekButton.Bounds.Height));
            if (rowBounds.Contains(position) && weekButton.DataContext is DateTime date)
            {
                weekStart = date;
                return true;
            }
        }

        return false;
    }

    protected virtual void NotifyPointerInMonthView(bool originInMonthView)
    {
    }

    protected virtual void NotifyPointerOutMonthView(bool originInMonthView)
    {
        if (originInMonthView && Owner?.PickerMode == DatePickerMode.Week)
        {
            Owner.NotifyHoverDateChanged(null);
            Owner.UpdateHighlightDays();
        }
    }

    private IThemeManager? _subscribedThemeManager;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var inputManager = AvaloniaLocator.Current.GetService(typeof(IInputManager)) as IInputManager;
        _pointerTracker ??= new CalendarPointerTracker(this);
        _pointerTracker.Attach(inputManager);
        EnsureGeneratedGrids();
        AttachLanguageVariantListener();
        RefreshLocalizedContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerTracker?.Detach();
        DetachLanguageVariantListener();
        ClearGeneratedGrids();
    }

    private void AttachLanguageVariantListener()
    {
        if (_subscribedThemeManager is not null)
        {
            return;
        }

        var themeManager = ThemeManager.Current;
        if (themeManager is null)
        {
            return;
        }

        themeManager.LanguageVariantChanged += HandleLanguageVariantChanged;
        _subscribedThemeManager = themeManager;
    }

    private void DetachLanguageVariantListener()
    {
        if (_subscribedThemeManager is null)
        {
            return;
        }

        _subscribedThemeManager.LanguageVariantChanged -= HandleLanguageVariantChanged;
        _subscribedThemeManager = null;
    }

    private void HandleLanguageVariantChanged(object? sender, LanguageVariantChangedEventArgs e)
    {
        Owner?.RefreshCultureFromThemeManager();
        RefreshLocalizedContent();
    }

    protected virtual void RefreshLocalizedContent()
    {
        if (Owner is null)
        {
            return;
        }

        switch (Owner.DisplayMode)
        {
            case CalendarMode.Month:
                UpdateMonthMode();
                break;
            case CalendarMode.Year:
                UpdateYearMode();
                break;
            case CalendarMode.Decade:
                UpdateDecadeMode();
                break;
        }
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

    protected Rect GetMonthViewRect(Grid? monthView)
    {
        if (monthView is null)
        {
            return default;
        }

        var firstDay = monthView.Children.OfType<CalendarDayButton>().FirstOrDefault();
        var topLevel = TopLevel.GetTopLevel(monthView);
        if (firstDay is null || topLevel is null)
        {
            return default;
        }

        var firstDayPos  = firstDay.TranslatePoint(new Point(0, 0), topLevel);
        var monthViewPos = monthView.TranslatePoint(new Point(0, 0), topLevel);
        if (firstDayPos is null || monthViewPos is null)
        {
            return default;
        }

        return new Rect(firstDayPos.Value,
            new Size(monthView.Bounds.Width, monthViewPos.Value.Y + monthView.Bounds.Height - firstDayPos.Value.Y));
    }
}
