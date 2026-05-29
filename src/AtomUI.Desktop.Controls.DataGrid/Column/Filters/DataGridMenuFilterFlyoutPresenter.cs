using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class DataGridMenuFilterFlyoutPresenter : MenuFlyoutPresenter
{
    private Button? _resetButton;
    private Button? _okButton;

    static DataGridMenuFilterFlyoutPresenter()
    {
        Button.ClickEvent.AddClassHandler<DataGridMenuFilterFlyoutPresenter>(
            (presenter, args) => presenter.HandleButtonClick(args));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _resetButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.ResetButtonPart);
        _okButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.OkButtonPart);
    }

    internal List<string> GetFilterValues()
    {
        var selectedValueCount = CountSelectedFilterValueLeaves(this);
        if (selectedValueCount == 0)
        {
            return DataGridFilterValuesSelectedEventArgs.EmptyValues;
        }

        var values = new List<string>(selectedValueCount);
        CollectFilterValues(values, this);
        return values;
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

    private void ConfirmFilter()
    {
        if (MenuFlyout is DataGridMenuFilterFlyout dataGridMenuFlyout)
        {
            dataGridMenuFlyout.IsActiveShutdown = true;
        }
        
        MenuFlyout?.Hide();
    }

    private void ClearCheckStateRecursive(SelectingItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = GetFilterItem(itemsControl, i);
            if (item is MenuItem filterMenuItem)
            {
                ClearCheckStateRecursive(filterMenuItem);
            }
        }

        if (itemsControl is MenuItem menuItem && itemsControl.ItemCount == 0)
        {
            menuItem.IsChecked = false;
        }
    }

    private void CollectFilterValues(
        List<string> filterValues,
        SelectingItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = GetFilterItem(itemsControl, i);
            if (item is DataGridFilterMenuItem filterMenuItem)
            {
                CollectFilterValues(filterValues, filterMenuItem);
            }
        }
        
        if (itemsControl is DataGridFilterMenuItem menuItem && itemsControl.ItemCount == 0)
        {
            if (menuItem.IsChecked && menuItem.FilterValue != null)
            {
                filterValues.Add(menuItem.FilterValue);
            }
        }
    }

    private int CountSelectedFilterValueLeaves(SelectingItemsControl itemsControl)
    {
        var count = 0;
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = GetFilterItem(itemsControl, i);
            if (item is DataGridFilterMenuItem filterMenuItem)
            {
                count += CountSelectedFilterValueLeaves(filterMenuItem);
            }
        }

        if (itemsControl is DataGridFilterMenuItem menuItem &&
            itemsControl.ItemCount == 0 &&
            menuItem.IsChecked &&
            menuItem.FilterValue != null)
        {
            count++;
        }

        return count;
    }

    internal static Control? GetFilterItem(SelectingItemsControl itemsControl, int index)
    {
        if (itemsControl.ContainerFromIndex(index) is Control container)
        {
            return container;
        }

        return index >= 0 && index < itemsControl.ItemsView.Count
            ? itemsControl.ItemsView[index] as Control
            : null;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is DataGridFilterMenuItem menuItem)
        {
            menuItem.OwningPresenter = this;
        }
    }
}
