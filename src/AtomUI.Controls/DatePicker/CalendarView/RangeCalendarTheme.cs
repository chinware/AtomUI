using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class RangeCalendarTheme : CalendarTheme
{
    public RangeCalendarTheme()
        : this(typeof(RangeCalendar))
    {
    }

    public RangeCalendarTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override CalendarItem BuildCalendarItem(INameScope scope)
    {
        var calendarItem = new RangeCalendarItem
        {
            Name                = CalendarItemPart,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        calendarItem.RegisterInNameScope(scope);
        return calendarItem;
    }
}