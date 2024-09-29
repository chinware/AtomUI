using AtomUI.Controls.CalendarView;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DualMonthRangeDatePickerPresenterTheme : DatePickerPresenterTheme
{
    public DualMonthRangeDatePickerPresenterTheme() : this(typeof(DualMonthRangeDatePickerPresenter))
    {
    }

    public DualMonthRangeDatePickerPresenterTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override Control BuildCalendarView(DatePickerPresenter presenter, INameScope scope)
    {
        var calendarView = new DualMonthRangeCalendar()
        {
            Name = CalendarViewPart,
        };
        CreateTemplateParentBinding(calendarView, DualMonthRangeCalendar.SelectedDateProperty, DualMonthRangeDatePickerPresenter.SelectedDateTimeProperty);
        CreateTemplateParentBinding(calendarView, DualMonthRangeCalendar.SecondarySelectedDateProperty, DualMonthRangeDatePickerPresenter.SecondarySelectedDateTimeProperty);
        calendarView.RegisterInNameScope(scope);
        return calendarView;
    }
}