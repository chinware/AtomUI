using AtomUI.Animations;
using AtomUI.Theme;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls.Primitives;

internal class IndicatorScrollViewer : ScrollViewer
{
    public IndicatorScrollViewer()
    {
        this.RegisterTokenResourceScope(IndicatorScrollViewerToken.ScopeProvider);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.EnableTransitions();
    }
}