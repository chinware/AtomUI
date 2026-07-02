using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using AtomUI.Desktop.Controls.CalendarView.Rendering;

namespace AtomUI.Desktop.Controls.CalendarView;

[TemplatePart("PART_SecondaryMonthView", typeof(Grid))]
[TemplatePart("PART_SecondaryHeaderButton", typeof(HeadTextButton))]
[TemplatePart("PART_SecondaryPreviousButton", typeof(IconButton))]
[TemplatePart("PART_SecondaryPreviousMonthButton", typeof(IconButton))]
[TemplatePart("PART_SecondaryNextButton", typeof(IconButton))]
[TemplatePart("PART_SecondaryNextMonthButton", typeof(IconButton))]
[TemplatePart("PART_YearViewLayout", typeof(UniformGrid))]
[TemplatePart("PART_SecondaryYearView", typeof(Grid))]
internal class DualMonthCalendarItem : RangeCalendarItem
{
    protected override Type StyleKeyOverride => typeof(DualMonthCalendarItem);
    #region 内部属性定义
    
    protected HeadTextButton? _secondaryHeaderButton;
    
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
    
    protected IconButton? _secondaryNextButton;

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
    
    protected IconButton? _secondaryNextMonthButton;
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
    
    protected IconButton? _secondaryPreviousButton;
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

    protected IconButton? _secondaryPreviousMonthButton;
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
    
    internal Grid? SecondaryMonthView { get; set; }

    internal UniformGrid? YearViewLayout { get; set; }

    internal Grid? SecondaryYearView { get; set; }

    #endregion

    protected DateTime _nextMonth;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        SetupMonthViewMode();
    }

    protected override bool IsPointerInMonthView(Point position)
    {
        if (Owner is null || Owner.DisplayMode != CalendarMode.Month || SecondaryMonthView is null)
        {
            return false;
        }
        if (base.IsPointerInMonthView(position))
        {
            return true;
        }
        
        return GetMonthViewRect(SecondaryMonthView!).Contains(position);
    }

    protected override bool TryGetWeekStartFromPointerPosition(Point position, out DateTime weekStart)
    {
        if (base.TryGetWeekStartFromPointerPosition(position, out weekStart))
        {
            return true;
        }

        return TryGetWeekStartFromMonthViewPosition(SecondaryMonthView, position, out weekStart);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (SecondaryMonthView is not null)
        {
            ClearGeneratedMonthView(SecondaryMonthView);
        }
        if (SecondaryYearView is not null)
        {
            ClearGeneratedYearView(SecondaryYearView);
        }

        SecondaryMonthView           = e.NameScope.Get<Grid>("PART_SecondaryMonthView");
        YearViewLayout               = e.NameScope.Get<UniformGrid>("PART_YearViewLayout");
        SecondaryYearView            = e.NameScope.Get<Grid>("PART_SecondaryYearView");
        SecondaryHeaderButton        = e.NameScope.Get<HeadTextButton>("PART_SecondaryHeaderButton");
        SecondaryPreviousButton      = e.NameScope.Get<IconButton>("PART_SecondaryPreviousButton");
        SecondaryPreviousMonthButton = e.NameScope.Find<IconButton>("PART_SecondaryPreviousMonthButton");
        SecondaryNextButton          = e.NameScope.Get<IconButton>("PART_SecondaryNextButton");
        SecondaryNextMonthButton     = e.NameScope.Get<IconButton>("PART_SecondaryNextMonthButton");
        
        base.OnApplyTemplate(e);
        SetupMonthViewMode();
    }

    private void SetupMonthViewMode()
    {
        if (SecondaryMonthView is null ||
            MonthViewLayout is null ||
            YearViewLayout is null ||
            SecondaryYearView is null ||
            _previousButton is null ||
            _previousMonthButton is null ||
            _nextButton is null ||
            _nextMonthButton is null ||
            _secondaryHeaderButton is null ||
            _secondaryPreviousButton is null ||
            _secondaryPreviousMonthButton is null ||
            _secondaryNextButton is null ||
            _secondaryNextMonthButton is null)
        {
            return;
        }

        _previousButton.IsVisible               = false;
        _previousMonthButton.IsVisible          = false;
        _nextButton.IsVisible                   = false;
        _nextMonthButton.IsVisible              = false;
        _secondaryPreviousButton.IsVisible      = false;
        _secondaryPreviousMonthButton.IsVisible = false;
        _secondaryNextButton.IsVisible          = false;
        _secondaryNextMonthButton.IsVisible     = false;
        _secondaryHeaderButton.IsVisible        = true;
        MonthViewLayout.Columns                 = 2;
        YearViewLayout.Columns                  = 2;
        MonthViewLayout.IsVisible               = IsMonthViewMode;
        if (MonthView is not null)
        {
            MonthView.IsVisible = IsMonthViewMode;
        }
        SecondaryMonthView.IsVisible            = IsMonthViewMode;
        YearViewLayout.IsVisible                = !IsMonthViewMode;
        if (YearView is not null)
        {
            YearView.IsVisible = !IsMonthViewMode;
        }
        SecondaryYearView.IsVisible = !IsMonthViewMode;
        
        if (IsMonthViewMode)
        {
            _previousButton.IsVisible           = true;
            _previousMonthButton.IsVisible      = true;
            _secondaryNextButton.IsVisible      = true;
            _secondaryNextMonthButton.IsVisible = true;
        }
        else
        {
            _previousButton.IsVisible      = true;
            _secondaryNextButton.IsVisible = true;
        }
    }

    protected override void PopulateMonthViewsGrid()
    {
        if (MonthView != null)
        {
            PopulateMonthViewGrid(MonthView);
        }
        if (SecondaryMonthView != null)
        {
            PopulateMonthViewGrid(SecondaryMonthView);
        }
    }

    protected override void PopulateYearViewsGrid()
    {
        base.PopulateYearViewsGrid();
        if (SecondaryYearView is not null)
        {
            PopulateYearViewGrid(SecondaryYearView);
        }
    }

    protected override void ClearGeneratedMonthViews()
    {
        base.ClearGeneratedMonthViews();
        if (SecondaryMonthView is not null)
        {
            ClearGeneratedMonthView(SecondaryMonthView);
        }
    }

    protected override void ClearGeneratedYearViews()
    {
        base.ClearGeneratedYearViews();
        if (SecondaryYearView is not null)
        {
            ClearGeneratedYearView(SecondaryYearView);
        }
    }

    protected override void SetDayTitles()
    {
        base.SetDayTitles();
        if (SecondaryMonthView is not null)
        {
            SetDayTitles(SecondaryMonthView, _nextMonth);
        }
    }

    protected override void SetupHeaderForDisplayModeChanged()
    {
        base.SetupHeaderForDisplayModeChanged();
        if (Owner is not null && _headerLayout is not null && MonthViewLayout is not null)
        {
            var headerLayout = _headerLayout as UniformGrid;
            MonthViewLayout.Columns = 2;
            if (YearViewLayout is not null)
            {
                YearViewLayout.Columns = 2;
            }
            if (headerLayout is not null)
            {
                headerLayout.Columns = 2;
            }

            ConfigureYearViewLayout(SecondaryYearView);
        }
    }

    protected internal override void UpdateMonthMode()
    {
        if (Owner is DualMonthRangeCalendar dualMonthRangeCalendar)
        {
            _nextMonth = dualMonthRangeCalendar.SecondaryDisplayDateInternal;
        }
        else
        {
            _nextMonth = DateTime.Today;
        }
        base.UpdateMonthMode();
    }

    protected override void SetCalendarDayButtons()
    {
        base.SetCalendarDayButtons();
        if (SecondaryMonthView is not null && Owner?.DisplayMode == CalendarMode.Month)
        {
            SetCalendarDayButtons(_nextMonth, SecondaryMonthView);
        }
    }

    protected override void SetMonthButtonsForYearMode()
    {
        base.SetMonthButtonsForYearMode();
        if (Owner is null || SecondaryYearView is null)
        {
            return;
        }

        var secondaryYear = DateTimeHelper.AddYears(_currentMonth, 1) ?? _currentMonth;
        ConfigureYearViewLayout(SecondaryYearView);
        if (Owner.PickerMode == DatePickerMode.Quarter)
        {
            var panel = CalendarPanelBuilder.BuildQuarterPanel(GetRenderState(secondaryYear), secondaryYear);
            CalendarItemRenderer.RenderQuarterPanel(Owner, SecondaryYearView, panel);
        }
        else
        {
            var panel = CalendarPanelBuilder.BuildYearPanel(GetRenderState(secondaryYear), secondaryYear);
            CalendarItemRenderer.RenderYearPanel(Owner, SecondaryYearView, panel);
        }

        SetSecondaryYearModeHeaderButton(secondaryYear);
        if (SecondaryNextButton is not null)
        {
            SecondaryNextButton.IsEnabled = Owner.DisplayDateRangeEnd.Year != secondaryYear.Year;
        }
    }

    protected override void SetYearButtons(DateTime selectedYear)
    {
        base.SetYearButtons(selectedYear);
        if (Owner is null || SecondaryYearView is null)
        {
            return;
        }

        var secondaryYear = DateTimeHelper.AddYears(selectedYear, 10) ?? selectedYear;
        ConfigureYearViewLayout(SecondaryYearView);
        var panel = CalendarPanelBuilder.BuildDecadePanel(GetRenderState(secondaryYear), secondaryYear);
        CalendarItemRenderer.RenderDecadePanel(Owner, SecondaryYearView, panel);

        var decade    = DateTimeHelper.DecadeOfDate(secondaryYear);
        var decadeEnd = DateTimeHelper.EndOfDecade(secondaryYear);
        SetSecondaryDecadeModeHeaderButton(decade, decadeEnd);
        if (SecondaryNextButton is not null)
        {
            SecondaryNextButton.IsEnabled = decadeEnd < Owner.DisplayDateRangeEnd.Year;
        }
    }

    protected override void SetMonthModeHeaderButton()
    {
        base.SetMonthModeHeaderButton();
        if (SecondaryHeaderButton is not null)
        {
            if (Owner is DualMonthRangeCalendar owner)
            {
                SecondaryHeaderButton.Content =
                    owner.SecondaryDisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
                SecondaryHeaderButton.IsEnabled = true;
            }
            else
            {
                SecondaryHeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            }
        }
    }

    private void SetSecondaryYearModeHeaderButton(DateTime secondaryYear)
    {
        if (SecondaryHeaderButton is not null)
        {
            SecondaryHeaderButton.Content = secondaryYear.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
            SecondaryHeaderButton.IsEnabled = true;
        }
    }

    private void SetSecondaryDecadeModeHeaderButton(int decade, int decadeEnd)
    {
        if (SecondaryHeaderButton is not null)
        {
            var format = DateTimeHelper.GetCurrentDateFormat();
            SecondaryHeaderButton.Content = decade.ToString(format) + "-" + decadeEnd.ToString(format);
            SecondaryHeaderButton.IsEnabled = false;
        }
    }
}
