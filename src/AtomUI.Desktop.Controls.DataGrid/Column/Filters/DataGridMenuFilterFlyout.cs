using AtomUI.Controls;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class DataGridMenuFilterFlyout : MenuFlyout
{
    public event EventHandler<DataGridFilterValuesSelectedEventArgs>? FilterValuesSelected;
    internal bool IsActiveShutdown = false;
    
    public DataGridMenuFilterFlyout()
    {
        OpenMotion  = new SlideUpInMotion();
        CloseMotion = new SlideUpOutMotion();
    }
    
    protected override Control CreatePresenter()
    {
        foreach (var item in Items)
        {
            if (item is Control control) 
            {
                control.SetLogicalParent(null);
                control.SetVisualParent(null);
            }
        }

        Presenter = new DataGridMenuFilterFlyoutPresenter
        {
            ItemsSource = Items,
            MenuFlyout  = this
        };

        Presenter[!MenuFlyoutPresenter.ItemTemplateProperty]          = this[!ItemTemplateProperty];
        Presenter[!MenuFlyoutPresenter.ItemContainerThemeProperty]    = this[!ItemContainerThemeProperty];
        Presenter[!MenuFlyoutPresenter.IsArrowVisibleProperty]           = this[!IsArrowVisibleEffectiveProperty];
        Presenter[!MenuFlyoutPresenter.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        Presenter[!MenuFlyoutPresenter.ArrowPositionProperty]         = this[!ArrowPositionProperty];

        ConfigureShowArrowEffective();
        ConfigureArrowPosition();
        return Presenter;
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
        if (Popup.Child is DataGridMenuFilterFlyoutPresenter presenter)
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

internal class DataGridFilterMenuItem : MenuItem
{
    public string? FilterValue { get; set; }
    public DataGridMenuFilterFlyoutPresenter? OwningPresenter { get; set; }

    static DataGridFilterMenuItem()
    {
        StaysOpenOnClickProperty.OverrideDefaultValue<DataGridFilterMenuItem>(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (ToggleType == MenuItemToggleType.Radio)
        {
            if (change.Property == IsCheckedProperty && IsChecked)
            {
                if (OwningPresenter != null)
                {
                    ClearCheckStateRecursive(OwningPresenter);
                }
            }
        }
    }
    
    private void ClearCheckStateRecursive(SelectingItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = itemsControl.ContainerFromIndex(i);
            if (item is MenuItem filterMenuItem)
            {
                ClearCheckStateRecursive(filterMenuItem);
            }
        }

        if (itemsControl is MenuItem menuItem && menuItem != this)
        {
            menuItem.IsChecked = false;
        }
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is DataGridFilterMenuItem menuItem)
        {
            menuItem.OwningPresenter = OwningPresenter;
        }
    }
}
