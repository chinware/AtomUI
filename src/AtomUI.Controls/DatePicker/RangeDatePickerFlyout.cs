using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class RangeDatePickerFlyout : Flyout
{
    protected override Control CreatePresenter()
    {
        var presenter = new RangeDatePickerFlyoutPresenter();
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, presenter);
        return presenter;
    }
}