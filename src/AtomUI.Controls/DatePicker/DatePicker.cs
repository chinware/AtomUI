using AtomUI.Controls.Internal;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class DatePicker : InfoPickerInput
{
    protected override Flyout CreatePickerFlyout()
    {
        return new DatePickerFlyout();
    }
    
    protected override void NotifyPresenterCreated(Control presenter)
    {
    }
}