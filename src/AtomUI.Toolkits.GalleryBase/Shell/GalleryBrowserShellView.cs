using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace AtomUI.Toolkits.GalleryBase.Shell;

public class GalleryBrowserShellView : UserControl, IScreen, IMediaBreakAwareControl, IDisposable
{
    private readonly GalleryWorkspaceViewModel _workspaceViewModel;
    private readonly GalleryShellView          _shellView;
    private readonly bool                      _isBrowserMediaBreakpointsEnabled;
    private bool _isDisposed;

    public RoutingState Router => _workspaceViewModel.Router;

    public MediaBreakPoint MediaBreakPoint { get; private set; } = MediaBreakPoint.Large;

    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    protected GalleryBrowserShellView(GalleryBaseConfiguration configuration,
                                      Func<GalleryBaseConfiguration, GalleryWorkspaceViewModel> workspaceFactory,
                                      Func<GalleryWorkspaceViewModel, Control> navigationViewFactory)
    {
        _workspaceViewModel = workspaceFactory(configuration);
        var navigationView = navigationViewFactory(_workspaceViewModel);
        _shellView = new GalleryShellView(configuration, navigationView, Router);
        _isBrowserMediaBreakpointsEnabled = configuration.Platform.EnableBrowserMediaBreakpoints;
        if (_isBrowserMediaBreakpointsEnabled)
        {
            _shellView.ContentHost.SizeChanged += HandleContentHostSizeChanged;
        }

        var visualLayerManager = new VisualLayerManager
        {
            Child = _shellView
        };
        if (configuration.Platform.ConfigureBrowserOverlayLayers)
        {
            ConfigureOverlayLayers(visualLayerManager);
        }

        Content = visualLayerManager;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        if (_isBrowserMediaBreakpointsEnabled)
        {
            _shellView.ContentHost.SizeChanged -= HandleContentHostSizeChanged;
        }
        _shellView.Dispose();
        _workspaceViewModel.Dispose();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleContentHostSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        NotifyMediaBreakPointChanged(ResolveMediaBreakPoint(e.NewSize.Width));
    }

    private void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        if (MediaBreakPoint == breakPoint)
        {
            return;
        }

        MediaBreakPoint = breakPoint;
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }

    private static MediaBreakPoint ResolveMediaBreakPoint(double width)
    {
        if (width >= (double)MediaBreakPoint.ExtraExtraLarge)
        {
            return MediaBreakPoint.ExtraExtraLarge;
        }

        if (width >= (double)MediaBreakPoint.ExtraLarge)
        {
            return MediaBreakPoint.ExtraLarge;
        }

        if (width >= (double)MediaBreakPoint.Large)
        {
            return MediaBreakPoint.Large;
        }

        if (width >= (double)MediaBreakPoint.Medium)
        {
            return MediaBreakPoint.Medium;
        }

        if (width >= (double)MediaBreakPoint.Small)
        {
            return MediaBreakPoint.Small;
        }

        return MediaBreakPoint.ExtraSmall;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(VisualLayerManager))]
    private static void ConfigureOverlayLayers(VisualLayerManager visualLayerManager)
    {
        visualLayerManager.EnableOverlayLayer = true;
        SetVisualLayerManagerProperty(visualLayerManager, "EnablePopupOverlayLayer", true);

        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "OverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "PopupOverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "LightDismissOverlayLayer");
    }

    private static void SetVisualLayerManagerProperty(VisualLayerManager visualLayerManager,
                                                      string propertyName,
                                                      object? value)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        propertyInfo.SetValue(visualLayerManager, value);
    }

    private static object? GetVisualLayerManagerPropertyValue(VisualLayerManager visualLayerManager,
                                                              string propertyName)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        return propertyInfo.GetValue(visualLayerManager);
    }
}
