using AtomUI.Controls.CalendarView;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimedRangeDatePickerPresenterTheme : DatePickerPresenterTheme
{
    public TimedRangeDatePickerPresenterTheme() : this(typeof(TimedRangeDatePickerPresenter))
    {
    }

    public TimedRangeDatePickerPresenterTheme(Type targetType) : base(targetType)
    {
    }

    protected override Control BuildCalendarView(DatePickerPresenter presenter, INameScope scope)
    {
        var calendarLayout = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };
        var calendarView = new RangeCalendar()
        {
            Name = CalendarViewPart,
        };
        CreateTemplateParentBinding(calendarView, RangeCalendar.IsMotionEnabledProperty, TimedRangeDatePickerPresenter.IsMotionEnabledProperty);
        CreateTemplateParentBinding(calendarView, RangeCalendar.SelectedDateProperty, TimedRangeDatePickerPresenter.SelectedDateTimeProperty);
        CreateTemplateParentBinding(calendarView, RangeCalendar.SecondarySelectedDateProperty, TimedRangeDatePickerPresenter.SecondarySelectedDateTimeProperty);
        calendarView.RegisterInNameScope(scope);
        calendarLayout.Children.Add(calendarView);

        var timeView = new TimeView()
        {
            Name              = TimeViewPart,
            VerticalAlignment = VerticalAlignment.Top,
        };
        CreateTemplateParentBinding(timeView, TimeView.IsMotionEnabledProperty, TimedRangeDatePickerPresenter.IsMotionEnabledProperty);
        CreateTemplateParentBinding(timeView, TimeView.ClockIdentifierProperty, TimedRangeDatePickerPresenter.ClockIdentifierProperty);
        CreateTemplateParentBinding(timeView, TimeView.IsVisibleProperty, TimedRangeDatePickerPresenter.IsShowTimeProperty);
        timeView.RegisterInNameScope(scope);
        calendarLayout.Children.Add(timeView);
        
        return calendarLayout;
    }
}