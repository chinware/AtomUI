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

    internal bool IsActiveShutdown = false;

    public ItemToggleType ToggleType
    {
        get => GetValue(ToggleTypeProperty);
        set => SetValue(ToggleTypeProperty, value);
    }

    public event EventHandler<DataGridFilterValuesSelectedEventArgs>? FilterValuesSelected;

    public DataGridTreeFilterFlyout()
    {
        OpenMotion  = new SlideUpInMotion();
        CloseMotion = new SlideUpOutMotion();
    }

    protected override Control CreatePresenter()
    {
        var presenter = new DataGridTreeFilterFlyoutPresenter
        {
            IsDefaultExpandAll = true,
            ItemsSource        = Items,
            TreeViewFlyout     = this
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter,
            DataGridTreeFilterFlyoutPresenter.IsShowArrowProperty);
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter,
            DataGridTreeFilterFlyoutPresenter.IsMotionEnabledProperty);
        BindUtils.RelayBind(this, ToggleTypeProperty, presenter, DataGridTreeFilterFlyoutPresenter.ToggleTypeProperty);
        return presenter;
    }

    protected override void NotifyPopupOpened(object? sender, EventArgs e)
    {
        base.NotifyPopupOpened(sender, e);
        IsActiveShutdown = false;
    }

    protected override void NotifyPopupClosed(object? sender, EventArgs e)
    {
        base.NotifyPopupClosed(sender, e);
        List<string> selectedItems = new List<string>();
        if (Popup.Child is DataGridTreeFilterFlyoutPresenter presenter)
        {
            selectedItems = presenter.GetFilterValues();
        }
        
        NotifyFilterValuesSelected(new DataGridFilterValuesSelectedEventArgs(IsActiveShutdown, selectedItems));
    }

    internal void NotifyFilterValuesSelected(DataGridFilterValuesSelectedEventArgs e)
    {
        FilterValuesSelected?.Invoke(this, e);
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