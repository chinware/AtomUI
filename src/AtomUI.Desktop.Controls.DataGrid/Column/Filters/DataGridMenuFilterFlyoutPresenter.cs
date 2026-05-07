using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class DataGridMenuFilterFlyoutPresenter : MenuFlyoutPresenter
{
    private Button? _resetButton;
    private Button? _okButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _resetButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.ResetButtonPart);
        _okButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.OkButtonPart);

        if (_resetButton != null)
        {
            _resetButton.Click += HandleResetButtonClick;
        }

        if (_okButton != null)
        {
            _okButton.Click += HandleOkButtonClick;
        }
    }

    internal List<String> GetFilterValues()
    {
        var values =  new List<String>();
        CollectFilterValues(values, this);
        return values;
    }

    private void HandleResetButtonClick(object? sender, RoutedEventArgs e)
    {
        ClearCheckStateRecursive(this);
    }

    private void HandleOkButtonClick(object? sender, RoutedEventArgs e)
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
            var item = itemsControl.ContainerFromIndex(i);
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

    private void CollectFilterValues(List<string> filterValues, SelectingItemsControl itemsControl)
    {
        for (var i = 0; i < itemsControl.ItemCount; i++)
        {
            var item = itemsControl.ContainerFromIndex(i);
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

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is DataGridFilterMenuItem menuItem)
        {
            menuItem.OwningPresenter = this;
        }
    }
}