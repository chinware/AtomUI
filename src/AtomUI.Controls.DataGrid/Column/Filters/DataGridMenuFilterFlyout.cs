using AtomUI.Data;
using AtomUI.MotionScene;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class DataGridMenuFilterFlyout : MenuFlyout
{
    public event EventHandler<DataGridFilterValuesSelectedEventArgs>? FilterValuesSelected;
    
    public DataGridMenuFilterFlyout()
    {
        OpenMotion  = new SlideUpInMotion();
        CloseMotion = new SlideUpOutMotion();
    }
    
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
    
    protected override void OnPopupClosed(object? sender, EventArgs e)
    {
        base.OnPopupClosed(sender, e);
        Console.WriteLine("OnPopupClosed");
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