using AtomUI.Controls.Internal;

namespace AtomUI.Controls;

public class DatePicker : InfoPickerInput
{
    protected override Flyout CreatePickerFlyout()
    {
        return new Flyout();
    }
}