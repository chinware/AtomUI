using AtomUI.Data;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class DataGridTreeFilterFlyout : Flyout
{
    [Content] public ItemCollection Items { get; }
    
    public DataGridTreeFilterFlyout()
    {
        var itemCollectionType = typeof(ItemCollection);
        Items = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
    }
    
    protected override Control CreatePresenter()
    {
        var presenter = new DataGridTreeFilterFlyoutPresenter
        {
            IsDefaultExpandAll = true,
            ItemsSource        = Items,
            IsCheckable        = true
        };
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, MenuFlyoutPresenter.IsMotionEnabledProperty);
        return presenter;
    }
}

internal class DataGridFilterTreeItem : TreeViewItem
{
    public string? FilterValue { get; set; }

    protected override void NotifyHeaderClick()
    {
        if (IsChecked is null)
        {
            IsChecked =  true;
        }
        else
        {
            IsChecked = !IsChecked;
        }
    }
    
    protected override void OnHeaderDoubleTapped(TappedEventArgs e)
    {
        if (ItemCount > 0)
        {
            e.Handled = true;
        }
    }
}