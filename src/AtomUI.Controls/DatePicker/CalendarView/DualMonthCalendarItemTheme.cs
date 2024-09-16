using AtomUI.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
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
    
    protected override void NotifyConfigureHeaderLayout(UniformGrid headerLayout)
    {
        headerLayout.Columns = 2;
    }

    protected override void NotifyBuildHeaderItems(UniformGrid headerLayout, INameScope scope)
    {
        base.NotifyBuildHeaderItems(headerLayout, scope);
        BuildHeaderItem(headerLayout, 
            SecondaryPreviousButtonPart,
            SecondaryPreviousMonthButtonPart,
            SecondaryHeaderButtonPart,
            SecondaryNextButtonPart,
            SecondaryNextMonthButtonPart,
            scope);
    }

    protected override void NotifyConfigureMonthViewLayout(UniformGrid monthViewLayout, INameScope scope)
    {
        monthViewLayout.Columns = 2;
    }
    
    protected override void NotifyBuildMonthViews(UniformGrid monthViewLayout, INameScope scope)
    {
        base.NotifyBuildMonthViews(monthViewLayout, scope);
        var monthView = BuildMonthViewItem(SecondaryMonthViewPart);
        BindUtils.RelayBind(monthViewLayout, Visual.IsVisibleProperty, monthView, Visual.IsVisibleProperty);
        
        TokenResourceBinder.CreateTokenBinding(monthView, Layoutable.MarginProperty,
            DatePickerTokenResourceKey.RangeCalendarSpacing, BindingPriority.Template,
            v =>
            {
                if (v is double dval)
                {
                    return new Thickness(dval, 0, 0, 0);
                }

                return new Thickness();
            });
        
        monthView.RegisterInNameScope(scope);
        monthViewLayout.Children.Add(monthView);
    }
}