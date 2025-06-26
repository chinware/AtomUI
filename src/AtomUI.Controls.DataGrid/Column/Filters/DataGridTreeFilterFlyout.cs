using AtomUI.Data;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class DataGridTreeFilterFlyout : TreeViewFlyout
{
    internal static readonly StyledProperty<ItemToggleType> ToggleTypeProperty =
        TreeView.ToggleTypeProperty.AddOwner<DataGridTreeFilterFlyout>();
    
    public ItemToggleType ToggleType
    {
        get => GetValue(ToggleTypeProperty);
        set => SetValue(ToggleTypeProperty, value);
    }
    
    public event EventHandler<DataGridFilterValuesSelectedEventArgs>? FilterValuesSelected;
    
    public DataGridTreeFilterFlyout()
    {
        OpenMotion = new SlideUpInMotion();
        CloseMotion = new SlideUpOutMotion();
    }
    
    protected override Control CreatePresenter()
    {
        var presenter = new DataGridTreeFilterFlyoutPresenter
        {
            IsDefaultExpandAll = true,
            ItemsSource        = Items
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, DataGridTreeFilterFlyoutPresenter.IsShowArrowProperty);
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, DataGridTreeFilterFlyoutPresenter.IsMotionEnabledProperty);
        BindUtils.RelayBind(this, ToggleTypeProperty, presenter, DataGridTreeFilterFlyoutPresenter.ToggleTypeProperty);
        return presenter;
    }
    
    protected override void OnPopupClosed(object? sender, EventArgs e)
    {
        base.OnPopupClosed(sender, e);
        Console.WriteLine("OnPopupClosed");
    }
}

internal class DataGridFilterTreeItem : TreeViewItem
{
    public string? FilterValue { get; set; }
    
    protected override void OnHeaderDoubleTapped(TappedEventArgs e)
    {
        if (ItemCount > 0)
        {
            e.Handled = true;
        }
    }
}