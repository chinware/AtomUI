using AtomUI.Controls;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

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
        foreach (var item in Items)
        {
            if (item is Control control) 
            {
                control.SetLogicalParent(null);
                control.SetVisualParent(null);
            }
        }

        presenter[!DataGridTreeFilterFlyoutPresenter.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        presenter[!DataGridTreeFilterFlyoutPresenter.IsShowArrowProperty]     = this[!IsShowArrowEffectiveProperty];
        presenter[!DataGridTreeFilterFlyoutPresenter.ArrowPositionProperty]   = this[!ArrowPositionProperty];
        presenter[!DataGridTreeFilterFlyoutPresenter.ToggleTypeProperty]      = this[!ToggleTypeProperty];
        
        ConfigureShowArrowEffective();
        ConfigureArrowPosition();
        
        return presenter;
    }

    protected override void OnOpened()
    {
        base.OnOpened();
        IsActiveShutdown = false;
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        var selectedItems = new List<string>();
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

internal class DataGridFilterTreeViewItem : TreeViewItem
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
