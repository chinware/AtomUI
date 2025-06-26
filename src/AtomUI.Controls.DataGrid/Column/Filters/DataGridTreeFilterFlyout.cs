using AtomUI.Data;
using AtomUI.MotionScene;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Metadata;

namespace AtomUI.Controls;

internal class DataGridTreeFilterFlyout : Flyout
{
    public event EventHandler<DataGridFilterValuesSelectedEventArgs>? FilterValuesSelected;
    
    [Content] public ItemCollection Items { get; }
    
    public DataGridTreeFilterFlyout()
    {
        var itemCollectionType = typeof(ItemCollection);
        Items      = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
        OpenMotion = new SlideUpInMotion();
        CloseMotion = new SlideUpOutMotion();
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
    
    protected override void OnPopupClosed(object? sender, EventArgs e)
    {
        base.OnPopupClosed(sender, e);
        Console.WriteLine("OnPopupClosed");
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