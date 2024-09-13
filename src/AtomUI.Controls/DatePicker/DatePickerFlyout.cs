using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class DatePickerFlyout : Flyout
{
    protected override Control CreatePresenter()
    {
        var presenter = new DatePickerFlyoutPresenter();
    
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
    
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, presenter);
        return presenter;
    }
}