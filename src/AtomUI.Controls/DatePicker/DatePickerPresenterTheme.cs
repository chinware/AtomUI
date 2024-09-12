using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DatePickerPresenterTheme : BaseControlTheme
{
    public DatePickerPresenterTheme() : this(typeof(DatePickerPresenter))
    {
    }

    public DatePickerPresenterTheme(Type targetType) : base(targetType)
    {
    }
}