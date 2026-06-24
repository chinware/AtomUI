using AtomUI.Controls.Primitives;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[TemplatePart(ScrollViewerPart, typeof(ScrollViewer))]
[TemplatePart(StickyPanelPart, typeof(GalleryStickyTabsPanel))]
[TemplatePart(StickyContentHostPart, typeof(Border))]
public class GalleryStickyTabsHost : TemplatedControl
{
    private const string ScrollViewerPart      = "PART_ScrollViewer";
    private const string StickyPanelPart       = "PART_StickyPanel";
    private const string StickyContentHostPart = "PART_StickyContentHost";
    private const int StickyMirrorZIndex       = -1;

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, object?>(nameof(Header));

    public static readonly StyledProperty<object?> StickyContentProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, object?>(nameof(StickyContent));

    public static readonly StyledProperty<object?> ContentProperty =
        ContentControl.ContentProperty.AddOwner<GalleryStickyTabsHost>();

    public static readonly StyledProperty<Thickness> StickyContentPaddingProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, Thickness>(nameof(StickyContentPadding));

    public static readonly StyledProperty<IBrush?> StickyBackgroundProperty =
        Border.BackgroundProperty.AddOwner<GalleryStickyTabsHost>();

    public static readonly StyledProperty<IBrush?> StickyBorderBrushProperty =
        Border.BorderBrushProperty.AddOwner<GalleryStickyTabsHost>();

    public static readonly StyledProperty<bool> IsStickyMirrorEnabledProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, bool>(nameof(IsStickyMirrorEnabled), true);

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? StickyContent
    {
        get => GetValue(StickyContentProperty);
        set => SetValue(StickyContentProperty, value);
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public Thickness StickyContentPadding
    {
        get => GetValue(StickyContentPaddingProperty);
        set => SetValue(StickyContentPaddingProperty, value);
    }

    public IBrush? StickyBackground
    {
        get => GetValue(StickyBackgroundProperty);
        set => SetValue(StickyBackgroundProperty, value);
    }

    public IBrush? StickyBorderBrush
    {
        get => GetValue(StickyBorderBrushProperty);
        set => SetValue(StickyBorderBrushProperty, value);
    }

    public bool IsStickyMirrorEnabled
    {
        get => GetValue(IsStickyMirrorEnabledProperty);
        set => SetValue(IsStickyMirrorEnabledProperty, value);
    }

    private ScrollViewer? _scrollViewer;
    private GalleryStickyTabsPanel? _stickyPanel;
    private Border? _inlineStickyContentHost;
    private ScopeAwareAdornerLayer? _stickyMirrorLayer;
    private Border? _stickyMirror;
    private VisualBrush? _stickyMirrorBrush;
    private bool _stickyMirrorUpdateQueued;

    public GalleryStickyTabsHost()
    {
        this.RegisterTokenResourceScope(GalleryStickyTabsHostToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseTemplateParts();

        base.OnApplyTemplate(e);

        _scrollViewer            = e.NameScope.Get<ScrollViewer>(ScrollViewerPart);
        _stickyPanel             = e.NameScope.Get<GalleryStickyTabsPanel>(StickyPanelPart);
        _inlineStickyContentHost = e.NameScope.Get<Border>(StickyContentHostPart);

        _stickyPanel.PropertyChanged += HandleStickyPanelPropertyChanged;
        _scrollViewer.ScrollChanged  += HandleScrollChanged;
        _inlineStickyContentHost.LayoutUpdated += HandleStickyContentHostLayoutUpdated;

        UpdateStickyMirror();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseTemplateParts();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StickyContentProperty ||
            change.Property == IsStickyMirrorEnabledProperty)
        {
            QueueStickyMirrorUpdate();
        }
        else if (change.Property == StickyBackgroundProperty ||
                 change.Property == StickyBorderBrushProperty ||
                 change.Property == StickyContentPaddingProperty)
        {
            InvalidateStickyMirrorBrush();
        }
    }

    private void ReleaseTemplateParts()
    {
        RemoveStickyMirror();

        if (_stickyPanel is not null)
        {
            _stickyPanel.PropertyChanged -= HandleStickyPanelPropertyChanged;
            _stickyPanel = null;
        }

        if (_scrollViewer is not null)
        {
            _scrollViewer.ScrollChanged -= HandleScrollChanged;
            _scrollViewer = null;
        }

        if (_inlineStickyContentHost is not null)
        {
            _inlineStickyContentHost.LayoutUpdated -= HandleStickyContentHostLayoutUpdated;
            _inlineStickyContentHost = null;
        }

        _stickyMirrorUpdateQueued = false;
    }

    private void HandleStickyPanelPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == GalleryStickyTabsPanel.IsStickyPinnedProperty)
        {
            QueueStickyMirrorUpdate();
        }
    }

    private void HandleScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        QueueStickyMirrorUpdate();
    }

    private void HandleStickyContentHostLayoutUpdated(object? sender, EventArgs e)
    {
        QueueStickyMirrorUpdate();
    }

    private void QueueStickyMirrorUpdate()
    {
        if (_stickyMirrorUpdateQueued)
        {
            return;
        }

        _stickyMirrorUpdateQueued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _stickyMirrorUpdateQueued = false;
            UpdateStickyMirror();
        }, DispatcherPriority.Render);
    }

    private void UpdateStickyMirror()
    {
        if (!IsStickyMirrorEnabled)
        {
            RemoveStickyMirror();
            return;
        }

        if (_stickyPanel?.IsStickyPinned == true &&
            _inlineStickyContentHost is not null)
        {
            EnsureStickyMirror();
            UpdateStickyMirrorBounds();
            InvalidateStickyMirrorBrush();
        }
        else
        {
            RemoveStickyMirror();
        }
    }

    private void EnsureStickyMirror()
    {
        if (_stickyMirror is not null)
        {
            return;
        }

        _stickyMirrorLayer = ResolveStickyMirrorLayer();
        if (_stickyMirrorLayer is null ||
            _inlineStickyContentHost is null)
        {
            return;
        }

        _stickyMirrorBrush = new VisualBrush
        {
            Visual  = _inlineStickyContentHost,
            Stretch = Stretch.Fill
        };

        _stickyMirror = new Border
        {
            Background       = _stickyMirrorBrush,
            ClipToBounds     = true,
            Focusable        = false,
            IsHitTestVisible = false,
            ZIndex           = StickyMirrorZIndex
        };

        _stickyMirrorLayer.Children.Add(_stickyMirror);
    }

    private void RemoveStickyMirror()
    {
        if (_stickyMirror is not null &&
            _stickyMirrorLayer?.Children.Contains(_stickyMirror) == true)
        {
            _stickyMirrorLayer.Children.Remove(_stickyMirror);
        }

        if (_stickyMirrorBrush is not null)
        {
            _stickyMirrorBrush.Visual = null;
        }

        _stickyMirror      = null;
        _stickyMirrorBrush = null;
        _stickyMirrorLayer = null;
    }

    private void UpdateStickyMirrorBounds()
    {
        if (_stickyMirror is null ||
            _inlineStickyContentHost is null ||
            _stickyMirrorLayer is null)
        {
            return;
        }

        var transform = _inlineStickyContentHost.TransformToVisual(_stickyMirrorLayer);
        if (!transform.HasValue)
        {
            return;
        }

        var position = transform.Value.Transform(default);
        var width    = Math.Max(0, _inlineStickyContentHost.Bounds.Width);
        var height   = Math.Max(0, _inlineStickyContentHost.Bounds.Height);

        Canvas.SetLeft(_stickyMirror, position.X);
        Canvas.SetTop(_stickyMirror, position.Y);
        _stickyMirror.Width  = width;
        _stickyMirror.Height = height;
    }

    private ScopeAwareAdornerLayer? ResolveStickyMirrorLayer()
    {
        if (this.FindAncestorOfType<VisualLayerManager>() is { } visualLayerManager)
        {
            return ScopeAwareAdornerLayer.GetLayer(visualLayerManager);
        }

        if (TopLevel.GetTopLevel(this) is { } topLevel)
        {
            return ScopeAwareAdornerLayer.GetLayer(topLevel);
        }

        return ScopeAwareAdornerLayer.GetLayer(this);
    }

    private void InvalidateStickyMirrorBrush()
    {
        if (_stickyMirrorBrush is null ||
            _inlineStickyContentHost is null)
        {
            return;
        }

        _stickyMirrorBrush.Visual = null;
        _stickyMirrorBrush.Visual = _inlineStickyContentHost;
    }
}
