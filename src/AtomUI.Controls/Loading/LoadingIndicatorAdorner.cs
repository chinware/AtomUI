using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class LoadingIndicatorAdorner : TemplatedControl
{
    private LoadingIndicator? _loadingIndicator;

    public EventHandler<LoadingIndicatorCreatedEventArgs>? IndicatorCreated;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _loadingIndicator = scope.Find<LoadingIndicator>(LoadingIndicatorAdornerTheme.LoadingIndicatorPart);
        if (_loadingIndicator is not null)
        {
            IndicatorCreated?.Invoke(this, new LoadingIndicatorCreatedEventArgs(_loadingIndicator));
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var offsetX = (finalSize.Width - _loadingIndicator!.DesiredSize.Width) / 2;
        var offsetY = (finalSize.Height - _loadingIndicator.DesiredSize.Height) / 2;
        Canvas.SetLeft(_loadingIndicator, offsetX);
        Canvas.SetTop(_loadingIndicator, offsetY);
        return base.ArrangeOverride(finalSize);
    }
}

public class LoadingIndicatorCreatedEventArgs : EventArgs
{
    public LoadingIndicator LoadingIndicator { get; set; }

    public LoadingIndicatorCreatedEventArgs(LoadingIndicator indicator)
    {
        LoadingIndicator = indicator;
    }
}