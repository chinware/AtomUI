using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class RangeCalendarItem : CalendarItem
{
   private const string PrimaryCalendarPC = ":primary";
   private const string SecondaryCalendarPC = ":secondary";
   
   #region 内部属性定义

   private bool _isPrimary = true;

   internal static readonly DirectProperty<RangeCalendarItem, bool> IsPrimaryProperty
      = AvaloniaProperty.RegisterDirect<RangeCalendarItem, bool>(nameof(IsPrimary),
                                                                 o => o.IsPrimary,
                                                                 (o, v) => o.IsPrimary = v);

   internal bool IsPrimary
   {
      get => _isPrimary;
      set => SetAndRaise(IsPrimaryProperty, ref _isPrimary, value);
   }
   
   private bool _isNextButtonVisible = true;

   internal static readonly DirectProperty<RangeCalendarItem, bool> IsNextButtonVisibleProperty
      = AvaloniaProperty.RegisterDirect<RangeCalendarItem, bool>(nameof(IsNextButtonVisible),
                                                                 o => o.IsNextButtonVisible,
                                                                 (o, v) => o.IsNextButtonVisible = v);

   internal bool IsNextButtonVisible
   {
      get => _isNextButtonVisible;
      set => SetAndRaise(IsNextButtonVisibleProperty, ref _isNextButtonVisible, value);
   }

   #endregion
   
   public RangeCalendarItem()
   {
      UpdatePseudoClasses();
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == IsPrimaryProperty) {
         UpdatePseudoClasses();
         SetupNextButtonVisible();
      }
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupNextButtonVisible();
   }

   private void SetupNextButtonVisible()
   {
      IsNextButtonVisible = !IsPrimary || Owner?.DisplayMode == CalendarMode.Decade ||
                            Owner?.DisplayMode == CalendarMode.Year;
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(PrimaryCalendarPC, IsPrimary);
      PseudoClasses.Set(SecondaryCalendarPC, !IsPrimary);
   }

   protected internal override void HandleHeaderButtonClick(object? sender, RoutedEventArgs e)
   {
      base.HandleHeaderButtonClick(sender, e);
      SetupNextButtonVisible();
   }

   protected internal override void HandleMonthCalendarButtonMouseUp(object? sender, PointerReleasedEventArgs e)
   {
      base.HandleMonthCalendarButtonMouseUp(sender, e);
      SetupNextButtonVisible();
   }
   
   protected internal override void UpdateMonthMode()
   {
      if (Owner is RangeCalendar owner) {
         _currentMonth = IsPrimary ? owner.DisplayDateInternal : owner.SecondaryDisplayDateInternal;
      } else {
         _currentMonth = DateTime.Today;
      }
   
      SetMonthModeHeaderButton();
      SetMonthModePreviousButton(_currentMonth);
      SetMonthModeNextButton(_currentMonth);
   
      if (MonthView != null) {
         SetDayTitles();
         SetCalendarDayButtons(_currentMonth);
      }
   }
   
   protected override void SetMonthModeHeaderButton()
   {
      if (HeaderButton != null) {
         if (Owner is RangeCalendar owner) {
            var targetDate = _isPrimary ? owner.DisplayDateInternal : owner.SecondaryDisplayDateInternal;
            HeaderButton.Content = targetDate.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
            HeaderButton.IsEnabled = true;
         } else {
            HeaderButton.Content = DateTime.Today.ToString("Y", DateTimeHelper.GetCurrentDateFormat());
         }
      }
   }
   
   protected override bool CheckDayInactiveState(DateTime dateToAdd)
   {
      if (Owner is RangeCalendar owner) {
         var targetDate = _isPrimary ? owner.DisplayDateInternal : owner.SecondaryDisplayDateInternal;
         return DateTimeHelper.CompareYearMonth(dateToAdd, targetDate) != 0;
      }
   
      return false;
   }
}