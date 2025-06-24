using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class DataGridMenuFilterFlyout : MenuFlyout
{
    protected override Control CreatePresenter()
    {
        var presenter = new DataGridMenuFilterFlyoutPresenter
        {
            ItemsSource                                = Items,
            [!ItemsControl.ItemTemplateProperty]       = this[!ItemTemplateProperty],
            [!ItemsControl.ItemContainerThemeProperty] = this[!ItemContainerThemeProperty],
            MenuFlyout                                 = this
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, MenuFlyoutPresenter.IsShowArrowProperty);
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, MenuFlyoutPresenter.IsMotionEnabledProperty);
        SetupArrowPosition(Popup, presenter);
        CalculateShowArrowEffective();
        return presenter;
    }
}

internal class DataGridFilterMenuItem : MenuItem
{
    public string? FilterValue { get; set; }

    static DataGridFilterMenuItem()
    {
        StaysOpenOnClickProperty.OverrideDefaultValue<DataGridFilterMenuItem>(true);
    }
}