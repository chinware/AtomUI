using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class TimePickerFlyout : Flyout
{
    protected override Control CreatePresenter()
    {
        var presenter = new TimePickerFlyoutPresenter();

        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);

        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, presenter);
        return presenter;
    }
}
