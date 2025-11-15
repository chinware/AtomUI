using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.CalendarView;

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
        SecondaryMonthView    = e.NameScope.Get<Grid>(DualMonthCalendarItemThemeConstants.SecondaryMonthViewPart);
        SecondaryHeaderButton = e.NameScope.Get<HeadTextButton>(DualMonthCalendarItemThemeConstants.SecondaryHeaderButtonPart);
        SecondaryPreviousButton = e.NameScope.Get<IconButton>(DualMonthCalendarItemThemeConstants.SecondaryPreviousButtonPart);
        SecondaryPreviousMonthButton = e.NameScope.Find<IconButton>(DualMonthCalendarItemThemeConstants.SecondaryPreviousMonthButtonPart);
        SecondaryNextButton      = e.NameScope.Get<IconButton>(DualMonthCalendarItemThemeConstants.SecondaryNextButtonPart);
        SecondaryNextMonthButton = e.NameScope.Get<IconButton>(DualMonthCalendarItemThemeConstants.SecondaryNextMonthButtonPart);
        
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
        DayBtnBindingDisposables?.Dispose();
        DayBtnBindingDisposables = new CompositeDisposable(Calendar.RowsPerMonth * Calendar.ColumnsPerMonth * 2);
        if (MonthView != null)
        {
            PopulateMonthViewGrid(MonthView);
        }
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
}