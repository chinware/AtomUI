using AtomUI.Theme.Styling;

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
}