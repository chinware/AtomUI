using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DatePickerFlyoutPresenterTheme : ArrowDecoratedBoxTheme
{
    public const string DatePickerPresenterPart = "PART_DatePickerPresenter";
    
    public DatePickerFlyoutPresenterTheme()
        : this(typeof(DatePickerFlyoutPresenter))
    {
    }
    
    protected DatePickerFlyoutPresenterTheme(Type targetType) : base(targetType)
    {
    }

    protected override Control BuildContent(INameScope scope)
    {
        var datePickerPresenter = new DatePickerPresenter()
        {
            Name = DatePickerPresenterPart,
        };
        datePickerPresenter.RegisterInNameScope(scope);
        return datePickerPresenter;
    }
}