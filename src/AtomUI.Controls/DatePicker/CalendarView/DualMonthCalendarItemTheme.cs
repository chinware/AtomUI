using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

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
        BindUtils.RelayBind(monthViewLayout, Visual.IsVisibleProperty, monthView, Visual.IsVisibleProperty);
        
        RegisterTokenResourceBindings(calendarItem, () =>
        {
            calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(monthView,
                Layoutable.MarginProperty,
                DatePickerTokenKey.RangeCalendarSpacing, BindingPriority.Template,
                v =>
                {
                    if (v is double dval)
                    {
                        return new Thickness(dval, 0, 0, 0);
                    }

                    return new Thickness();
                }));
        });

        monthView.RegisterInNameScope(scope);
        monthViewLayout.Children.Add(monthView);
    }
}