using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class DualMonthCalendarItemTheme : CalendarItemTheme
{
    public const string SecondaryMonthViewPart = "PART_SecondaryMonthView";
    public const string SecondaryPreviousButtonPart = "PART_SecondaryPreviousButton";
    public const string SecondaryPreviousMonthButtonPart = "PART_SecondaryPreviousMonthButton";
    public const string SecondaryHeaderButtonPart = "PART_SecondaryHeaderButton";
    public const string SecondaryNextMonthButtonPart = "PART_SecondaryNextMonthButton";
    public const string SecondaryNextButtonPart = "PART_SecondaryNextButton";

    public DualMonthCalendarItemTheme()
        : this(typeof(DualMonthCalendarItem))
    {
    }

    public DualMonthCalendarItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override void NotifyConfigureHeaderLayout(CalendarItem calendarItem, UniformGrid headerLayout)
    {
        headerLayout.Columns = 2;
    }

    protected override void NotifyBuildHeaderItems(CalendarItem calendarItem, UniformGrid headerLayout,
                                                   INameScope scope)
    {
        base.NotifyBuildHeaderItems(calendarItem, headerLayout, scope);
        BuildHeaderItem(calendarItem, headerLayout,
            SecondaryPreviousButtonPart,
            SecondaryPreviousMonthButtonPart,
            SecondaryHeaderButtonPart,
            SecondaryNextButtonPart,
            SecondaryNextMonthButtonPart,
            scope);
    }

    protected override void NotifyConfigureMonthViewLayout(CalendarItem calendarItem, UniformGrid monthViewLayout,
                                                           INameScope scope)
    {
        monthViewLayout.Columns = 2;
    }

    protected override void NotifyBuildMonthViews(CalendarItem calendarItem, UniformGrid monthViewLayout,
                                                  INameScope scope)
    {
        base.NotifyBuildMonthViews(calendarItem, monthViewLayout, scope);
        var monthView = BuildMonthViewItem(SecondaryMonthViewPart);
        // 不会造成内存泄漏
        BindUtils.RelayBind(monthViewLayout, Visual.IsVisibleProperty, monthView, Visual.IsVisibleProperty);
        
        monthView.RegisterInNameScope(scope);
        monthViewLayout.Children.Add(monthView);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var secondaryMonthViewStyle = new Style(selector => selector.Nesting().Template().Name(SecondaryMonthViewPart));
        secondaryMonthViewStyle.Add(Layoutable.MarginProperty, DatePickerTokenKey.RangeCalendarMonthViewMargin);
        Add(secondaryMonthViewStyle);
    }
}