using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class DataGridTreeFilterFlyoutPresenter : TreeViewFlyoutPresenter
{
    private Button? _resetButton;
    private Button? _okButton;

    static DataGridTreeFilterFlyoutPresenter()
    {
        Button.ClickEvent.AddClassHandler<DataGridTreeFilterFlyoutPresenter>(
            (presenter, args) => presenter.HandleButtonClick(args));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _resetButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.ResetButtonPart);
        _okButton    = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.OkButtonPart);
    }
    private void HandleButtonClick(RoutedEventArgs e)
    {
        if (ReferenceEquals(e.Source, _resetButton))
        {
            ResetFilter();
        }
        else if (ReferenceEquals(e.Source, _okButton))
        {
            ConfirmFilter();
        }
    }
    
    private void ResetFilter()
    {
        ClearCheckStateRecursive(this);
    }
    
    internal List<String> GetFilterValues()
    {
        var values =  new List<String>();
        CollectFilterValues(values, this);
        return values;
    }
    
    private void CollectFilterValues(List<string> filterValues, ItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = itemsControl.ContainerFromIndex(i);
            if (item is DataGridFilterTreeViewItem filterTreeItem)
            {
                CollectFilterValues(filterValues, filterTreeItem);
            }
        }
        
        if (itemsControl is DataGridFilterTreeViewItem treeItem && itemsControl.ItemCount == 0)
        {
            if (treeItem.IsChecked == true && treeItem.FilterValue != null)
            {
                filterValues.Add(treeItem.FilterValue);
            }
        }
    }


    private void ConfirmFilter()
    {
        if (TreeViewFlyout is DataGridTreeFilterFlyout treeFilterFlyout)
        {
            treeFilterFlyout.IsActiveShutdown = true;
        }
        
        TreeViewFlyout?.Hide();
    }
    
    private void ClearCheckStateRecursive(ItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = itemsControl.ContainerFromIndex(i);
            if (item is TreeViewItem filterTreeViewItem)
            {
                ClearCheckStateRecursive(filterTreeViewItem);
            }
        }

        if (itemsControl is TreeViewItem treeViewItem && itemsControl.ItemCount == 0)
        {
            treeViewItem.IsChecked = false;
        }
    }
}
