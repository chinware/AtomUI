using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

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

    private void HandleResetButtonClick(object? sender, RoutedEventArgs e)
    {
        ClearCheckStateRecursive(this);
    }

    private void HandleOkButtonClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("HandleOkButtonClick");
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
}