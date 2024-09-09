using System.Diagnostics;
using System.Globalization;
using AtomUI.Collections.Pooled;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
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
         if (_primaryHeaderButton != null) _primaryHeaderButton.Click -= HandleHeaderButtonClick;

         _primaryHeaderButton = value;

         if (_primaryHeaderButton != null) {
            _primaryHeaderButton.Click += HandleHeaderButtonClick;
            _primaryHeaderButton.Focusable = false;
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
         if (_secondaryHeaderButton != null) _secondaryHeaderButton.Click -= HandleHeaderButtonClick;

         _secondaryHeaderButton = value;

         if (_secondaryHeaderButton != null) {
            _secondaryHeaderButton.Click += HandleHeaderButtonClick;
            _secondaryHeaderButton.Focusable = false;
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
         if (_primaryNextButton != null) _primaryNextButton.Click -= HandleNextButtonClick;

         _primaryNextButton = value;

         if (_primaryNextButton != null) {
            _primaryNextButton.Click += HandleNextButtonClick;
            _primaryNextButton.Focusable = false;
         }
      }
   }
   
   internal IconButton? PrimaryNextMonthButton
   {
      get => _primaryNextMonthButton;
      private set
      {
         if (_primaryNextMonthButton != null) _primaryNextMonthButton.Click -= HandleNextMonthButtonClick;

         _primaryNextMonthButton = value;

         if (_primaryNextMonthButton != null) {
            _primaryNextMonthButton.Click += HandleNextMonthButtonClick;
            _primaryNextMonthButton.Focusable = false;
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
         if (_primaryPreviousButton != null) _primaryPreviousButton.Click -= HandlePreviousButtonClick;

         _primaryPreviousButton = value;

         if (_primaryPreviousButton != null) {
            _primaryPreviousButton.Click += HandlePreviousButtonClick;
            _primaryPreviousButton.Focusable = false;
         }
      }
   }
   
   internal IconButton? PrimaryPreviousMonthButton
   {
      get => _primaryPreviousMonthButton;
      private set
      {
         if (_primaryPreviousMonthButton != null) _primaryPreviousMonthButton.Click -= HandlePreviousMonthButtonClick;

         _primaryPreviousMonthButton = value;

         if (_primaryPreviousMonthButton != null) {
            _primaryPreviousMonthButton.Click += HandlePreviousMonthButtonClick;
            _primaryPreviousMonthButton.Focusable = false;
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
         if (_secondaryNextButton != null) _secondaryNextButton.Click -= HandleNextButtonClick;

         _secondaryNextButton = value;

         if (_secondaryNextButton != null) {
            _secondaryNextButton.Click += HandleNextButtonClick;
            _secondaryNextButton.Focusable = false;
         }
      }
   }
   
   internal IconButton? SecondaryNextMonthButton
   {
      get => _secondaryNextMonthButton;
      private set
      {
         if (_secondaryNextMonthButton != null) _secondaryNextMonthButton.Click -= HandleNextMonthButtonClick;

         _secondaryNextMonthButton = value;

         if (_secondaryNextMonthButton != null) {
            _secondaryNextMonthButton.Click += HandleNextMonthButtonClick;
            _secondaryNextMonthButton.Focusable = false;
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
         if (_secondaryPreviousButton != null) _secondaryPreviousButton.Click -= HandlePreviousButtonClick;

         _secondaryPreviousButton = value;

         if (_secondaryPreviousButton != null) {
            _secondaryPreviousButton.Click += HandlePreviousButtonClick;
            _secondaryPreviousButton.Focusable = false;
         }
      }
   }
   
   internal IconButton? SecondaryPreviousMonthButton
   {
      get => _secondaryPreviousMonthButton;
      private set
      {
         if (_secondaryPreviousMonthButton != null) _secondaryPreviousMonthButton.Click -= HandlePreviousMonthButtonClick;

         _secondaryPreviousMonthButton = value;

         if (_secondaryPreviousMonthButton != null) {
            _secondaryPreviousMonthButton.Click += HandlePreviousMonthButtonClick;
            _secondaryPreviousMonthButton.Focusable = false;
         }
      }
   }
   
   #endregion
   
   /// <summary>
   /// The number of days per week.
   /// </summary>
   internal const int NumberOfDaysPerWeek = 7;

   protected HeadTextButton? _primaryHeaderButton;
   protected HeadTextButton? _secondaryHeaderButton;
   protected IconButton? _primaryNextButton;
   protected IconButton? _primaryPreviousButton;
   protected IconButton? _primaryNextMonthButton;
   protected IconButton? _primaryPreviousMonthButton;
   
   protected IconButton? _secondaryNextButton;
   protected IconButton? _secondaryPreviousButton;
   protected IconButton? _secondaryNextMonthButton;
   protected IconButton? _secondaryPreviousMonthButton;
   protected UniformGrid? _headerLayout;

   protected DateTime _currentMonth;
   protected DateTime _nextMonth;
   protected bool _isMouseLeftButtonDown;
   protected bool _isMouseLeftButtonDownYearView;
   protected bool _isControlPressed;

   protected readonly System.Globalization.Calendar _calendar = new GregorianCalendar();

   internal RangeCalendar? Owner { get; set; }
   internal RangeCalendarDayButton? CurrentButton { get; set; }
   
   /// <summary>
   /// Gets the Grid that hosts the content when in month mode.
   /// </summary>
   internal UniformGrid? MonthView { get; set; }
   internal Grid? PrimaryMonthView { get; set; }
   internal Grid? SecondaryMonthView { get; set; }
   
   /// <summary>
   /// Gets the Grid that hosts the content when in year or decade mode.
   /// </summary>
   internal Grid? YearView { get; set; }

   private void PopulateGrids()
   {
      if (MonthView != null) {
         if (PrimaryMonthView is not null) {
            PopulateMonthViewGrids(PrimaryMonthView);
         }
         if (SecondaryMonthView is not null) {
            PopulateMonthViewGrids(SecondaryMonthView);
         }
      }

      if (YearView != null) {
         var childCount = RangeCalendar.RowsPerYear * RangeCalendar.ColumnsPerYear;
         using var children = new PooledList<Control>(childCount);

         EventHandler<PointerPressedEventArgs> monthCalendarButtonMouseDown = HandleMonthCalendarButtonMouseDown;
         EventHandler<PointerReleasedEventArgs> monthCalendarButtonMouseUp = HandleMonthCalendarButtonMouseUp;
         EventHandler<PointerEventArgs> monthMouseEntered = HandleMonthMouseEntered;

         for (int i = 0; i < RangeCalendar.RowsPerYear; i++) {
            for (int j = 0; j < RangeCalendar.ColumnsPerYear; j++) {
               var month = new RangeCalendarButton();

               if (Owner != null) {
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

   private void PopulateMonthViewGrids(Grid monthView)
   {
      var childCount = RangeCalendar.RowsPerMonth + RangeCalendar.RowsPerMonth * RangeCalendar.ColumnsPerMonth;
      using var children = new PooledList<Control>(childCount);

      for (int i = 0; i < RangeCalendar.ColumnsPerMonth; i++) {
         if (DayTitleTemplate?.Build() is Control cell) {
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

      for (int i = 1; i < RangeCalendar.RowsPerMonth; i++) {
         for (int j = 0; j < RangeCalendar.ColumnsPerMonth; j++) {
            var cell = new RangeCalendarDayButton();

            if (Owner != null) {
               cell.Owner = Owner;
            }

            cell.IsInPrimaryMonView = monthView == PrimaryMonthView;

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
      PrimaryHeaderButton = e.NameScope.Find<HeadTextButton>(RangeCalendarItemTheme.PrimaryHeaderButtonPart);
      SecondaryHeaderButton = e.NameScope.Find<HeadTextButton>(RangeCalendarItemTheme.SecondaryHeaderButtonPart);
      PrimaryPreviousButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryPreviousButtonPart);
      PrimaryPreviousMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryPreviousMonthButtonPart);
      PrimaryNextButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryNextButtonPart);
      PrimaryNextMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.PrimaryNextMonthButtonPart);
      
      SecondaryPreviousButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryPreviousButtonPart);
      SecondaryPreviousMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryPreviousMonthButtonPart);
      SecondaryNextButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryNextButtonPart);
      SecondaryNextMonthButton = e.NameScope.Find<IconButton>(RangeCalendarItemTheme.SecondaryNextMonthButtonPart);
      
      MonthView = e.NameScope.Find<UniformGrid>(RangeCalendarItemTheme.MonthViewPart);
      PrimaryMonthView = e.NameScope.Find<Grid>(RangeCalendarItemTheme.PrimaryMonthViewPart);
      SecondaryMonthView = e.NameScope.Find<Grid>(RangeCalendarItemTheme.SecondaryMonthViewPart);
      YearView = e.NameScope.Find<Grid>(RangeCalendarItemTheme.YearViewPart);
      _headerLayout = e.NameScope.Find<UniformGrid>(RangeCalendarItemTheme.HeaderLayoutPart);

      if (Owner != null) {
         UpdateDisabled(Owner.IsEnabled);
      }

      PopulateGrids();

      if (MonthView != null && YearView != null) {
         if (Owner != null) {
            Owner.SelectedMonth = Owner.DisplayDateInternal;
            Owner.SelectedYear = Owner.DisplayDateInternal;

            if (Owner.DisplayMode == CalendarMode.Year) {
               UpdateYearMode();
            } else if (Owner.DisplayMode == CalendarMode.Decade) {
               UpdateDecadeMode();
            }

            if (Owner.DisplayMode == CalendarMode.Month) {
               UpdateMonthMode();
               MonthView.IsVisible = true;
               YearView.IsVisible = false;
            } else {
               YearView.IsVisible = true;
               MonthView.IsVisible = false;
            }
         } else {
            UpdateMonthMode();
            MonthView.IsVisible = true;
            YearView.IsVisible = false;
         }
      }

      SetupHeaderForDisplayModeChanged();
   }

   private void SetupHeaderForDisplayModeChanged()
   {
      if (Owner is null || MonthView is null || _headerLayout is null) {
         return;
      }
      if (Owner.DisplayMode == CalendarMode.Month) {
         MonthView.Columns = 2;
         _headerLayout.Columns = 2;
      } else if (Owner.DisplayMode == CalendarMode.Year || Owner.DisplayMode == CalendarMode.Decade) {
         MonthView.Columns = 1;
         _headerLayout.Columns = 1;
      }

      IsMonthViewMode = Owner.DisplayMode == CalendarMode.Month;
   }

   protected void SetDayTitles()
   {
      if (PrimaryMonthView is not null) {
         SetDayTitles(PrimaryMonthView);
      }
      
      if (SecondaryMonthView is not null) {
         SetDayTitles(SecondaryMonthView);
      }
   }

   protected void SetDayTitles(Grid monthView)
   {
      for (int childIndex = 0; childIndex < RangeCalendar.ColumnsPerMonth; childIndex++) {
         var dayTitle = monthView.Children[childIndex];
         if (Owner != null) {
            dayTitle.DataContext = DateTimeHelper.GetCurrentDateFormat()
                                                 .ShortestDayNames[
                                                    (childIndex + (int)Owner.FirstDayOfWeek) % NumberOfDaysPerWeek];
         } else {
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
      DayOfWeek day = _calendar.GetDayOfWeek(firstOfMonth);
      int i;

      if (Owner != null) {
         i = ((day - Owner.FirstDayOfWeek + NumberOfDaysPerWeek) % NumberOfDaysPerWeek);
      } else {
         i = ((day - DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek + NumberOfDaysPerWeek) % NumberOfDaysPerWeek);
      }

      if (i == 0) {
         return NumberOfDaysPerWeek;
      } else {
         return i;
      }
   }

   protected internal virtual void UpdateMonthMode()
   {
      if (Owner != null) {
         _currentMonth = Owner.DisplayDateInternal;
         _nextMonth = Owner.SecondaryDisplayDateInternal;
      } else {
         _currentMonth = DateTime.Today;
         _nextMonth = DateTime.Today;
      }

      SetMonthModeHeaderButton();
      SetMonthModePreviousButton(_currentMonth);
      SetMonthModeNextButton(_currentMonth);

      if (MonthView != null) {
         SetDayTitles();
         if (PrimaryMonthView is not null) {
            SetCalendarDayButtons(_currentMonth, PrimaryMonthView, true);
         }
         if (SecondaryMonthView is not null) {
            SetCalendarDayButtons(_nextMonth, SecondaryMonthView, false);
         }
      }
   }

   protected virtual void SetMonthModeHeaderButton()
   {
      if (PrimaryHeaderButton is not null) {
         if (Owner is not null) {
            PrimaryHeaderButton.Content = Owner.DisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            PrimaryHeaderButton.IsEnabled = true;
         } else {
            PrimaryHeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
         }
      }

      if (SecondaryHeaderButton is not null) {
         if (Owner is not null) {
            SecondaryHeaderButton.Content = Owner.SecondaryDisplayDateInternal.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            SecondaryHeaderButton.IsEnabled = true;
         } else {
            SecondaryHeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
         }
      }
   }

   protected void SetMonthModeNextButton(DateTime firstDayOfMonth)
   {
      if (Owner != null && PrimaryNextButton != null) {
         // DisplayDate is equal to DateTime.MaxValue
         if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0) {
            PrimaryNextButton.IsEnabled = false;
         } else {
            // Since we are sure DisplayDate is not equal to
            // DateTime.MaxValue, it is safe to use AddMonths  
            DateTime firstDayOfNextMonth = _calendar.AddMonths(firstDayOfMonth, 1);
            PrimaryNextButton.IsEnabled = (DateTimeHelper.CompareDays(Owner.DisplayDateRangeEnd, firstDayOfNextMonth) > -1);
         }
      }
   }

   protected void SetMonthModePreviousButton(DateTime firstDayOfMonth)
   {
      if (Owner != null && PrimaryPreviousButton != null) {
         PrimaryPreviousButton.IsEnabled = (DateTimeHelper.CompareDays(Owner.DisplayDateRangeStart, firstDayOfMonth) < 0);
      }
   }

   private void SetButtonState(RangeCalendarDayButton childButton, DateTime dateToAdd, Grid monthView)
   {
      if (Owner != null) {
         childButton.Opacity = 1;

         // If the day is outside the DisplayDateStart/End boundary, do
         // not show it
         if (DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeStart) < 0 ||
             DateTimeHelper.CompareDays(dateToAdd, Owner.DisplayDateRangeEnd) > 0) {
            childButton.IsEnabled = false;
            childButton.IsToday = false;
            childButton.IsSelected = false;
            childButton.Opacity = 0;
         } else {
            // SET IF THE DAY IS SELECTABLE OR NOT
            if (Owner.BlackoutDates.Contains(dateToAdd)) {
               childButton.IsBlackout = true;
            } else {
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
            if (Owner.SelectedRangeStartDate is not null && Owner.SelectedRangeEndDate is not null) {
               childButton.IsSelected = DateTimeHelper.InRange(dateToAdd, Owner.SelectedRangeStartDate.Value, Owner.SelectedRangeEndDate.Value);
            }

            // SET THE FOCUS ELEMENT
            if (Owner.LastSelectedDate != null) {
               if (DateTimeHelper.CompareDays(Owner.LastSelectedDate.Value, dateToAdd) == 0) {
                  if (Owner.FocusButton != null) {
                     Owner.FocusButton.IsCurrent = false;
                  }

                  Owner.FocusButton = childButton;
                  if (Owner.HasFocusInternal) {
                     Owner.FocusButton.IsCurrent = true;
                  }
               } else {
                  childButton.IsCurrent = false;
               }
            }
         }
      }
   }

   protected virtual bool CheckDayInactiveState(DateTime dateToAdd, Grid monthView)
   {
      if (Owner is not null) {
         if (monthView == PrimaryMonthView) {
            return DateTimeHelper.CompareYearMonth(dateToAdd, Owner.DisplayDateInternal) != 0;
         } else if (monthView == SecondaryMonthView) {
            return DateTimeHelper.CompareYearMonth(dateToAdd, Owner.SecondaryDisplayDateInternal) != 0;
         }
      }

      return false;
   }
   
   protected virtual bool CheckDayIsTodayState(DateTime dateToAdd)
   {
      if (Owner is not null) {
         return Owner.IsTodayHighlighted && dateToAdd == DateTime.Today;
      }

      return false;
   }

   protected void SetCalendarDayButtons(DateTime firstDayOfMonth, Grid monthView, bool isClearHoverInfo)
   {
      int lastMonthToDisplay = PreviousMonthDays(firstDayOfMonth);
      DateTime dateToAdd;

      if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MinValue) > 0) {
         // DisplayDate is not equal to DateTime.MinValue we can subtract
         // days from the DisplayDate
         dateToAdd = _calendar.AddDays(firstDayOfMonth, -lastMonthToDisplay);
      } else {
         dateToAdd = firstDayOfMonth;
      }

      if (isClearHoverInfo) {
         if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null) {
            Owner.HoverEndIndex = null;
            Owner.HoverStartIndex = null;
         }
      }

      int count = RangeCalendar.RowsPerMonth * RangeCalendar.ColumnsPerMonth;

      for (int childIndex = RangeCalendar.ColumnsPerMonth; childIndex < count; childIndex++) {
         RangeCalendarDayButton childButton = (RangeCalendarDayButton)monthView!.Children[childIndex];

         childButton.Index = childIndex;
         if (monthView == SecondaryMonthView) {
            childButton.Index += MonthViewSize;
         }

         SetButtonState(childButton, dateToAdd, monthView);

         // Update the indexes of hoverStart and hoverEnd
         if (Owner != null && Owner.HoverEnd != null && Owner.HoverStart != null) {
            if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverEnd.Value) == 0) {
               Owner.HoverEndIndex = childButton.Index;
            }

            if (DateTimeHelper.CompareDays(dateToAdd, Owner.HoverStart.Value) == 0) {
               Owner.HoverStartIndex = childButton.Index;
            }
         }

         //childButton.Focusable = false;
         childButton.Content = dateToAdd.Day.ToString(DateTimeHelper.GetCurrentDateFormat());
         childButton.DataContext = dateToAdd;

         if (DateTime.Compare(DateTimeHelper.DiscardTime(DateTime.MaxValue), dateToAdd) > 0) {
            // Since we are sure DisplayDate is not equal to
            // DateTime.MaxValue, it is safe to use AddDays 
            dateToAdd = _calendar.AddDays(dateToAdd, 1);
         } else {
            // DisplayDate is equal to the DateTime.MaxValue, so there
            // are no trailing days
            childIndex++;
            for (int i = childIndex; i < count; i++) {
               childButton = (RangeCalendarDayButton)monthView.Children[i];
               // button needs a content to occupy the necessary space
               // for the content presenter
               childButton.Content = i.ToString(DateTimeHelper.GetCurrentDateFormat());
               childButton.IsEnabled = false;
               childButton.Opacity = 0;
            }

            return;
         }
      }

      // If the HoverStart or HoverEndInternal could not be found on the
      // DisplayMonth set the values of the HoverStartIndex or
      // HoverEndIndex to be the first or last day indexes on the current
      // month
      if (Owner != null && Owner.HoverStart.HasValue && Owner.HoverEndInternal.HasValue) {
         if (!Owner.HoverEndIndex.HasValue) {
            if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0) {
               Owner.HoverEndIndex = RangeCalendar.ColumnsPerMonth * RangeCalendar.RowsPerMonth - 1;
            } else {
               Owner.HoverEndIndex = RangeCalendar.ColumnsPerMonth;
            }
         }

         if (!Owner.HoverStartIndex.HasValue) {
            if (DateTimeHelper.CompareDays(Owner.HoverEndInternal.Value, Owner.HoverStart.Value) > 0) {
               Owner.HoverStartIndex = RangeCalendar.ColumnsPerMonth;
            } else {
               Owner.HoverStartIndex = RangeCalendar.ColumnsPerMonth * RangeCalendar.RowsPerMonth - 1;
            }
         }
      }
   }

   internal void UpdateYearMode()
   {
      if (Owner != null) {
         _currentMonth = Owner.SelectedMonth;
      } else {
         _currentMonth = DateTime.Today;
      }

      SetYearModeHeaderButton();
      SetYearModePreviousButton();
      SetYearModeNextButton();

      if (YearView != null) {
         SetMonthButtonsForYearMode();
      }
   }

   private void SetYearModeHeaderButton()
   {
      if (PrimaryHeaderButton != null) {
         PrimaryHeaderButton.IsEnabled = true;
         PrimaryHeaderButton.Content = _currentMonth.Year.ToString(DateTimeHelper.GetCurrentDateFormat());
      }
   }

   private void SetYearModePreviousButton()
   {
      if (Owner != null && PrimaryPreviousButton != null) {
         PrimaryPreviousButton.IsEnabled = (Owner.DisplayDateRangeStart.Year != _currentMonth.Year);
      }
   }

   private void SetYearModeNextButton()
   {
      if (Owner != null && PrimaryNextButton != null) {
         PrimaryNextButton.IsEnabled = (Owner.DisplayDateRangeEnd.Year != _currentMonth.Year);
      }
   }

   private void SetMonthButtonsForYearMode()
   {
      int count = 0;
      foreach (object child in YearView!.Children) {
         RangeCalendarButton childButton = (RangeCalendarButton)child;
         // There should be no time component. Time is 12:00 AM
         DateTime day = new DateTime(_currentMonth.Year, count + 1, 1);
         childButton.DataContext = day;

         childButton.Content = DateTimeHelper.GetCurrentDateFormat().AbbreviatedMonthNames[count];
         childButton.IsVisible = true;

         if (Owner != null) {
            if (day.Year == _currentMonth.Year && day.Month == _currentMonth.Month && day.Day == _currentMonth.Day) {
               Owner.FocusCalendarButton = childButton;
               childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
            } else {
               childButton.IsCalendarButtonFocused = false;
            }

            childButton.IsSelected = (DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateInternal) == 0);

            if (DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeStart) < 0 ||
                DateTimeHelper.CompareYearMonth(day, Owner.DisplayDateRangeEnd) > 0) {
               childButton.IsEnabled = false;
               childButton.Opacity = 0;
            } else {
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

      if (Owner != null) {
         selectedYear = Owner.SelectedYear;
         _currentMonth = Owner.SelectedMonth;
      } else {
         _currentMonth = DateTime.Today;
         selectedYear = DateTime.Today;
      }

      int decade = DateTimeHelper.DecadeOfDate(selectedYear);
      int decadeEnd = DateTimeHelper.EndOfDecade(selectedYear);

      SetDecadeModeHeaderButton(decade, decadeEnd);
      SetDecadeModePreviousButton(decade);
      SetDecadeModeNextButton(decadeEnd);

      if (YearView != null) {
         SetYearButtons(decade, decadeEnd);
      }
   }

   internal void UpdateYearViewSelection(RangeCalendarButton? calendarButton)
   {
      if (Owner != null && calendarButton?.DataContext is DateTime selectedDate) {
         Owner.FocusCalendarButton!.IsCalendarButtonFocused = false;
         Owner.FocusCalendarButton = calendarButton;
         calendarButton.IsCalendarButtonFocused = Owner.HasFocusInternal;

         if (Owner.DisplayMode == CalendarMode.Year) {
            Owner.SelectedMonth = selectedDate;
         } else {
            Owner.SelectedYear = selectedDate;
         }
      }
   }

   private void SetYearButtons(int decade, int decadeEnd)
   {
      int year;
      int count = -1;
      foreach (var child in YearView!.Children) {
         RangeCalendarButton childButton = (RangeCalendarButton)child;
         year = decade + count;

         if (year <= DateTime.MaxValue.Year && year >= DateTime.MinValue.Year) {
            // There should be no time component. Time is 12:00 AM
            DateTime day = new DateTime(year, 1, 1);
            childButton.DataContext = day;
            childButton.Content = year.ToString(DateTimeHelper.GetCurrentDateFormat());
            childButton.IsVisible = true;

            if (Owner != null) {
               if (year == Owner.SelectedYear.Year) {
                  Owner.FocusCalendarButton = childButton;
                  childButton.IsCalendarButtonFocused = Owner.HasFocusInternal;
               } else {
                  childButton.IsCalendarButtonFocused = false;
               }

               childButton.IsSelected = (Owner.DisplayDate.Year == year);

               if (year < Owner.DisplayDateRangeStart.Year || year > Owner.DisplayDateRangeEnd.Year) {
                  childButton.IsEnabled = false;
                  childButton.Opacity = 0;
               } else {
                  childButton.IsEnabled = true;
                  childButton.Opacity = 1;
               }
            }

            // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a
            // trailing year or not
            childButton.IsInactive = (year < decade || year > decadeEnd);
         } else {
            childButton.IsEnabled = false;
            childButton.Opacity = 0;
         }

         count++;
      }
   }

   private void SetDecadeModeHeaderButton(int decade, int decadeEnd)
   {
      if (PrimaryHeaderButton != null) {
         PrimaryHeaderButton.Content = decade.ToString(CultureInfo.CurrentCulture) + "-" +
                                decadeEnd.ToString(CultureInfo.CurrentCulture);
         PrimaryHeaderButton.IsEnabled = false;
      }
   }

   private void SetDecadeModeNextButton(int decadeEnd)
   {
      if (Owner != null && PrimaryNextButton != null) {
         PrimaryNextButton.IsEnabled = (Owner.DisplayDateRangeEnd.Year > decadeEnd);
      }
   }

   private void SetDecadeModePreviousButton(int decade)
   {
      if (Owner != null && PrimaryPreviousButton != null) {
         PrimaryPreviousButton.IsEnabled = (decade > Owner.DisplayDateRangeStart.Year);
      }
   }

   protected internal virtual void HandleHeaderButtonClick(object? sender, RoutedEventArgs e)
   {
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         HeadTextButton b = (HeadTextButton)sender!;
         DateTime d;

         if (b.IsEnabled) {
            if (Owner.DisplayMode == CalendarMode.Month) {
               d = Owner.DisplayDateInternal;
               Owner.SelectedMonth = new DateTime(d.Year, d.Month, 1);
               Owner.DisplayMode = CalendarMode.Year;
            } else {
               Debug.Assert(Owner.DisplayMode == CalendarMode.Year, "The Owner RangeCalendar's DisplayMode should be Year!");
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
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         IconButton b = (IconButton)sender!;
         if (b.IsEnabled) {
            Owner.OnPreviousMonthClick();
         }
      }
   }
   
   internal void HandlePreviousButtonClick(object? sender, RoutedEventArgs e)
   {
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         IconButton b = (IconButton)sender!;
         if (b.IsEnabled) {
            Owner.OnPreviousClick();
         }
      }
   }

   internal void HandleNextMonthButtonClick(object? sender, RoutedEventArgs e)
   {
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         IconButton b = (IconButton)sender!;

         if (b.IsEnabled) {
            Owner.OnNextMonthClick();
         }
      }
   }
   
   internal void HandleNextButtonClick(object? sender, RoutedEventArgs e)
   {
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         IconButton b = (IconButton)sender!;

         if (b.IsEnabled) {
            Owner.OnNextClick();
         }
      }
   }

   internal void HandleCellMouseEntered(object? sender, PointerEventArgs e)
   {
      if (Owner != null) {
         if (_isMouseLeftButtonDown
             && sender is RangeCalendarDayButton
             {
                IsEnabled: true, IsBlackout: false, DataContext: DateTime selectedDate
             } b) {
            // Update the states of all buttons to be selected starting
            // from HoverStart to b
            Owner.UnHighlightDays();
            Owner.HoverEndIndex = b.Index;
            Owner.HoverEnd = selectedDate;
            // Update the States of the buttons
            Owner.HighlightDays();
            return;
         }
      }
   }

   internal void HandleCellMouseLeftButtonDown(object? sender, PointerPressedEventArgs e)
   {
      if (Owner != null) {
         if (!Owner.HasFocusInternal) {
            Owner.Focus();
         }

         CalendarExtensions.GetMetaKeyState(e.KeyModifiers, out var ctrl, out var shift);

         if (sender is RangeCalendarDayButton b) {
            _isControlPressed = ctrl;
            if (b.IsEnabled && !b.IsBlackout && b.DataContext is DateTime selectedDate) {
               _isMouseLeftButtonDown = true;
               // Set the start or end of the selection
               // range
               if (Owner.HoverStart is null) {
                  Owner.UnHighlightDays();
                  Owner.HoverStart = selectedDate;
                  Owner.HoverStartIndex = b.Index;
               } else if (Owner.HoverStart is not null && Owner.HoverEnd is not null) {
                  Owner.UnHighlightDays();
                  Owner.HoverEnd = selectedDate;
                  Owner.HoverEndIndex = b.Index;
                  Owner.HighlightDays();
               }
            } else {
               // If a click occurs on a BlackOutDay we set the
               // HoverStart to be null
               Owner.HoverStart = null;
            }
         } else {
            _isControlPressed = false;
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

         _isMouseLeftButtonDown = false;
         if (b != null && b.DataContext is DateTime selectedDate) {

            if (Owner.HoverStart.HasValue) {
               AddSelection(b, selectedDate);
            } else {
               // If the day is Disabled but a trailing day we should
               // be able to switch months
               if (b.IsInactive && b.IsBlackout) {
                  Owner.OnDayClick(selectedDate);
               }
            }
         }
      }
   }

   private void AddSelection(RangeCalendarDayButton b, DateTime selectedDate)
   {
      if (Owner != null) {
         Owner.HoverEndIndex = b.Index;
         Owner.HoverEnd = selectedDate;

         if (Owner.HoverEnd != null && Owner.HoverStart != null) {
            // this is selection with Mouse, we do not guarantee the
            // range does not include BlackOutDates.  AddRange method
            // will throw away the BlackOutDates based on the
            // SelectionMode
            Owner.IsMouseSelection = true;
            Owner.OnDayClick(selectedDate);
         }
      }
   }

   private void HandleCellClick(object? sender, RoutedEventArgs e)
   {
      _isControlPressed = false;
   }

   private void HandleMonthCalendarButtonMouseDown(object? sender, PointerPressedEventArgs e)
   {
      _isMouseLeftButtonDownYearView = true;

      UpdateYearViewSelection(sender as RangeCalendarButton);
   }

   protected internal virtual void HandleMonthCalendarButtonMouseUp(object? sender, PointerReleasedEventArgs e)
   {
      _isMouseLeftButtonDownYearView = false;

      if (Owner != null && (sender as RangeCalendarButton)?.DataContext is DateTime newMonth) {
         if (Owner.DisplayMode == CalendarMode.Year) {
            Owner.DisplayDate = newMonth;
            Owner.DisplayMode = CalendarMode.Month;
         } else {
            Debug.Assert(Owner.DisplayMode == CalendarMode.Decade, "The owning Calendar should be in decade mode!");
            Owner.SelectedMonth = newMonth;
            Owner.DisplayMode = CalendarMode.Year;
         }

         SetupHeaderForDisplayModeChanged();
      }
   }

   private void HandleMonthMouseEntered(object? sender, PointerEventArgs e)
   {
      if (_isMouseLeftButtonDownYearView) {
         UpdateYearViewSelection(sender as RangeCalendarButton);
      }
   }

   internal void UpdateDisabled(bool isEnabled)
   {
      PseudoClasses.Set(CalendarDisabledPC, !isEnabled);
   }
   
}