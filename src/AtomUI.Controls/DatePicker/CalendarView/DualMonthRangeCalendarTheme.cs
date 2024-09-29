using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class DualMonthRangeCalendarTheme : CalendarTheme
{
    public DualMonthRangeCalendarTheme()
        : base(typeof(DualMonthRangeCalendar))
    {
    }
    
    protected override CalendarItem BuildCalendarItem(INameScope scope)
    {
        var calendarItem = new DualMonthCalendarItem
        {
            Name                = CalendarItemPart,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        calendarItem.RegisterInNameScope(scope);
        return calendarItem;
    }
}