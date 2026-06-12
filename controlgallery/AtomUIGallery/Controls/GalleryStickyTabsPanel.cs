using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUIGallery.Controls;

public class GalleryStickyTabsPanel : Panel
{
    public static readonly StyledProperty<int> StickyIndexProperty =
        AvaloniaProperty.Register<GalleryStickyTabsPanel, int>(nameof(StickyIndex), 1);

    public static readonly StyledProperty<double> StickyOffsetYProperty =
        AvaloniaProperty.Register<GalleryStickyTabsPanel, double>(nameof(StickyOffsetY));

    public static readonly DirectProperty<GalleryStickyTabsPanel, bool> IsStickyPinnedProperty =
        AvaloniaProperty.RegisterDirect<GalleryStickyTabsPanel, bool>(
            nameof(IsStickyPinned),
            o => o.IsStickyPinned);

    private ScrollViewer? _scrollViewer;
    private bool _isStickyPinned;

    public int StickyIndex
    {
        get => GetValue(StickyIndexProperty);
        set => SetValue(StickyIndexProperty, value);
    }

    public double StickyOffsetY
    {
        get => GetValue(StickyOffsetYProperty);
        set => SetValue(StickyOffsetYProperty, value);
    }

    public bool IsStickyPinned
    {
        get => _isStickyPinned;
        private set => SetAndRaise(IsStickyPinnedProperty, ref _isStickyPinned, value);
    }

    static GalleryStickyTabsPanel()
    {
        AffectsMeasure<GalleryStickyTabsPanel>(StickyIndexProperty);
        AffectsArrange<GalleryStickyTabsPanel>(StickyOffsetYProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachScrollViewer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachScrollViewer();
        base.OnDetachedFromVisualTree(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var measureWidth = double.IsInfinity(availableSize.Width)
            ? double.PositiveInfinity
            : Math.Max(0, availableSize.Width);
        var desiredWidth  = 0d;
        var desiredHeight = 0d;

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }

            child.Measure(new Size(measureWidth, double.PositiveInfinity));
            desiredWidth  = Math.Max(desiredWidth, child.DesiredSize.Width);
            desiredHeight += child.DesiredSize.Height;
        }

        if (!double.IsInfinity(availableSize.Width))
        {
            desiredWidth = Math.Max(desiredWidth, availableSize.Width);
        }

        return new Size(desiredWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var y              = 0d;
        var isStickyPinned = false;
        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (!child.IsVisible)
            {
                child.Arrange(default);
                continue;
            }

            var height   = child.DesiredSize.Height;
            var naturalY = y;
            var arrangeY = i == StickyIndex
                ? Math.Max(naturalY, StickyOffsetY)
                : naturalY;

            child.Arrange(new Rect(0, arrangeY, finalSize.Width, height));
            if (i == StickyIndex)
            {
                isStickyPinned = arrangeY > naturalY;
            }

            y += height;
        }

        IsStickyPinned = isStickyPinned;
        return finalSize;
    }

    private void AttachScrollViewer()
    {
        var scrollViewer = this.GetVisualAncestors().OfType<ScrollViewer>().FirstOrDefault();
        if (ReferenceEquals(scrollViewer, _scrollViewer))
        {
            return;
        }

        DetachScrollViewer();
        _scrollViewer = scrollViewer;
        if (_scrollViewer is not null)
        {
            SetCurrentValue(StickyOffsetYProperty, _scrollViewer.Offset.Y);
            _scrollViewer.ScrollChanged += HandleScrollChanged;
        }
    }

    private void DetachScrollViewer()
    {
        if (_scrollViewer is null)
        {
            return;
        }

        _scrollViewer.ScrollChanged -= HandleScrollChanged;
        _scrollViewer = null;
    }

    private void HandleScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            SetCurrentValue(StickyOffsetYProperty, scrollViewer.Offset.Y);
        }
    }
}
