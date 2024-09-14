using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimePickerFlyoutPresenterTheme : ArrowDecoratedBoxTheme
{
    public const string TimePickerPresenterPart = "PART_TimePickerPresenter";

    public TimePickerFlyoutPresenterTheme()
        : this(typeof(TimePickerFlyoutPresenter))
    {
    }
    
    protected TimePickerFlyoutPresenterTheme(Type targetType) : base(targetType)
    {
    }

    protected override Control BuildContent(INameScope scope)
    {
        var timePickerPresenter = new TimePickerPresenter()
        {
            Name = TimePickerPresenterPart,
        };
        timePickerPresenter.RegisterInNameScope(scope);
        return timePickerPresenter;
    }
}