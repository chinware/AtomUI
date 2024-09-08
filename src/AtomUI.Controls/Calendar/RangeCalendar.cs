using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class RangeCalendar : Calendar
{
   public RangeCalendar()
   {
      SelectionMode = CalendarSelectionMode.SingleRange;
   }
   
   internal DateTime SecondaryDisplayDateInternal { get; private set; }
   
   internal CalendarItem? SecondaryMonthControl
   {
      get
      {
         if (Root != null && Root.Children.Count > 1) {
            return Root.Children[1] as CalendarItem;
         }

         return null;
      }
   }
   
   protected override void OnDisplayDateChanged(AvaloniaPropertyChangedEventArgs e)
   {
      UpdateDisplayDate(this, (DateTime)e.NewValue!, (DateTime)e.OldValue!);
   }
   
   private static void UpdateDisplayDate(RangeCalendar c, DateTime addedDate, DateTime removedDate)
   {
      _ = c ?? throw new ArgumentNullException(nameof(c));

      // If DisplayDate < DisplayDateStart, DisplayDate = DisplayDateStart
      if (DateTime.Compare(addedDate, c.DisplayDateRangeStart) < 0) {
         c.DisplayDate = c.DisplayDateRangeStart;
         return;
      }

      // If DisplayDate > DisplayDateEnd, DisplayDate = DisplayDateEnd
      if (DateTime.Compare(addedDate, c.DisplayDateRangeEnd) > 0) {
         c.DisplayDate = c.DisplayDateRangeEnd;
         return;
      }

      c.DisplayDateInternal = DateTimeHelper.DiscardDayTime(addedDate);
      c.SecondaryDisplayDateInternal = DateTimeHelper.AddMonths(DateTimeHelper.DiscardDayTime(addedDate), 1) ?? c.DisplayDateInternal;
      c.UpdateMonths();
      c.OnDisplayDate(new CalendarDateChangedEventArgs(removedDate, addedDate));
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      if (Root != null) {
         CalendarItem? month = e.NameScope.Find<CalendarItem>(RangeCalendarTheme.SecondaryCalendarItemPart);

         if (month != null) {
            month.Owner = this;
         }
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == DisplayModeProperty) {
         HandleDisplayModeChanged();
      }
   }

   private void HandleDisplayModeChanged()
   {
      var secondaryMonthControl = SecondaryMonthControl;
      if (secondaryMonthControl is not null) {
         int columns = default;
         if (DisplayMode == CalendarMode.Month) {
            secondaryMonthControl.IsVisible = true;
            columns = 2;
         } else {
            secondaryMonthControl.IsVisible = false;
            columns = 1;
         }

         if (Root is UniformGrid uniformGrid) {
            uniformGrid.Columns = columns;
         }
      }
   }

   protected internal override void UpdateMonths()
   {
      base.UpdateMonths();
      if (SecondaryMonthControl is not null) {
         UpdateCalendarMonths(SecondaryMonthControl);
      }
   }
}