using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class DataGridTreeFilterFlyoutPresenter : TreeViewFlyoutPresenter
{
    private Button? _resetButton;
    private Button? _okButton;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _resetButton = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.ResetButtonPart);
        _okButton    = e.NameScope.Find<Button>(DataGridFilterFlyoutPresenterThemeConstants.OkButtonPart);
        
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
        Console.WriteLine("HandleResetButtonClick");
    }

    private void HandleOkButtonClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("HandleOkButtonClick");
    }
}