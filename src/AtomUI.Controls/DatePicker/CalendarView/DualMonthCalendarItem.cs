using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.CalendarView;

internal class DualMonthCalendarItem : CalendarItem
{
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

    #endregion

    protected DateTime _nextMonth;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        SetupMonthViewMode();
    }

    protected override bool IsPointerInMonthView(Point position)
    {
        if (Owner is null || Owner.DisplayMode != CalendarMode.Month)
        {
            return false;
        }
        if (base.IsPointerInMonthView(position))
        {
            return true;
        }
        
        return GetMonthViewRect(SecondaryMonthView!).Contains(position);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        SecondaryMonthView    = e.NameScope.Get<Grid>(DualMonthCalendarItemTheme.SecondaryMonthViewPart);
        SecondaryHeaderButton = e.NameScope.Get<HeadTextButton>(DualMonthCalendarItemTheme.SecondaryHeaderButtonPart);
        SecondaryPreviousButton = e.NameScope.Get<IconButton>(DualMonthCalendarItemTheme.SecondaryPreviousButtonPart);
        SecondaryPreviousMonthButton = e.NameScope.Find<IconButton>(DualMonthCalendarItemTheme.SecondaryPreviousMonthButtonPart);
        SecondaryNextButton      = e.NameScope.Get<IconButton>(DualMonthCalendarItemTheme.SecondaryNextButtonPart);
        SecondaryNextMonthButton = e.NameScope.Get<IconButton>(DualMonthCalendarItemTheme.SecondaryNextMonthButtonPart);
        
        base.OnApplyTemplate(e);
        SetupMonthViewMode();
    }

    private void SetupMonthViewMode()
    {
        if (SecondaryMonthView is null ||
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
        
        if (IsMonthViewMode)
        {
            SecondaryMonthView.IsVisible        = true;
            _secondaryHeaderButton.IsVisible    = true;
            _previousButton.IsVisible           = true;
            _previousMonthButton.IsVisible      = true;
            _secondaryNextButton.IsVisible      = true;
            _secondaryNextMonthButton.IsVisible = true;
        }
        else
        {
            SecondaryMonthView.IsVisible     = false;
            _secondaryHeaderButton.IsVisible = false;
            _previousButton.IsVisible        = true;
            _previousMonthButton.IsVisible   = true;
            _nextButton.IsVisible            = true;
            _nextMonthButton.IsVisible       = true;
        }
    }

    protected override void PopulateMonthViewsGrid()
    {
        base.PopulateMonthViewsGrid();
        if (SecondaryMonthView != null)
        {
            PopulateMonthViewGrid(SecondaryMonthView);
        }
    }

    protected override void SetDayTitles()
    {
        base.SetDayTitles();
        if (SecondaryMonthView is not null)
        {
            SetDayTitles(SecondaryMonthView);
        }
    }

    protected override void SetupHeaderForDisplayModeChanged()
    {
        base.SetupHeaderForDisplayModeChanged();
        if (Owner is not null && _headerLayout is not null && MonthViewLayout is not null)
        {
            if (Owner.DisplayMode == CalendarMode.Month)
            {
                MonthViewLayout.Columns     = 2;
                _headerLayout.Columns = 2;
            }
            else if (Owner.DisplayMode == CalendarMode.Year || Owner.DisplayMode == CalendarMode.Decade)
            {
                MonthViewLayout.Columns     = 1;
                _headerLayout.Columns = 1;
            }
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
        if (SecondaryMonthView is not null)
        {
            SetCalendarDayButtons(_nextMonth, SecondaryMonthView);
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

    protected override void CheckButtonSelectedState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        // SET IF THE DAY IS SELECTED OR NOT
        childButton.IsSelected = false;
        if (Owner is DualMonthRangeCalendar owner)
        {
            DateTime? rangeStart = default;
            DateTime? rangeEnd   = default;
            owner.SortHoverIndexes(out rangeStart, out rangeEnd);
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
        }
    }

    protected override bool CheckDayInactiveState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        if (childButton.Parent == MonthView)
        {
            return base.CheckDayInactiveState(childButton, dateToAdd);
        } 
        
        var isSecondaryDayInactive = false;
        if (Owner is DualMonthRangeCalendar owner)
        {
            isSecondaryDayInactive = DateTimeHelper.CompareYearMonth(dateToAdd, owner.SecondaryDisplayDateInternal) != 0;
        }

        return isSecondaryDayInactive;
    }

    protected override void NotifyCellMouseEntered(CalendarDayButton dayButton, DateTime selectedDate)
    {
        if (Owner is DualMonthRangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(selectedDate);
            owner.UpdateHighlightDays();
        }
    }

    protected override void NotifyCellMouseLeftButtonDown(CalendarDayButton dayButton)
    {
        if (Owner is DualMonthRangeCalendar owner)
        {
            if (dayButton.IsEnabled && !dayButton.IsBlackout && dayButton.DataContext is DateTime selectedDate)
            {
                // Set the start or end of the selection
                // range
                if (owner.IsSelectRangeStart)
                {
                    owner.SelectedDate = selectedDate;
                }
                else
                {
                    owner.SecondarySelectedDate = selectedDate;
                }

                owner.UpdateHighlightDays();
                if (owner.SelectedDate is not null && owner.SecondarySelectedDate is not null)
                {
                    owner.NotifyRangeDateSelected();
                }
            }
        }
    }
    
    protected override void NotifyPointerOutMonthView(bool originInMonthView)
    {
        if (Owner is DualMonthRangeCalendar owner)
        {
            owner.HoverDateTime = owner.IsSelectRangeStart
                ? owner.SelectedDate
                : owner.SecondarySelectedDate;
            // Update the States of the buttons
            if (originInMonthView)
            {
                owner.HoverDateTime = null;
                Owner.UpdateHighlightDays();
            }
        }
    }

}