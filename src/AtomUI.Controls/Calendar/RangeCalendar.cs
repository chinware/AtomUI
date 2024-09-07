using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class RangeCalendar : Calendar
{
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
}