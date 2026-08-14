using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Shell;

internal sealed class GalleryContentMediaBreakHost : Border, IMediaBreakAwareControl, IDisposable
{
    internal static readonly StyledProperty<MediaBreakPoint> MediaBreakPointProperty =
        MediaBreakAwareControlProperty.MediaBreakPointProperty.AddOwner<GalleryContentMediaBreakHost>();

    private bool _isDisposed;

    public GalleryContentMediaBreakHost()
    {
        SizeChanged += HandleSizeChanged;
    }

    public MediaBreakPoint MediaBreakPoint => GetValue(MediaBreakPointProperty);

    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        SizeChanged -= HandleSizeChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Bounds.Width > 0)
        {
            NotifyMediaBreakPointChanged(GalleryMediaBreakPointResolver.Resolve(Bounds.Width, this));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        NotifyMediaBreakPointChanged(GalleryMediaBreakPointResolver.Resolve(e.NewSize.Width, this));
    }

    private void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        if (MediaBreakPoint == breakPoint)
        {
            return;
        }

        SetCurrentValue(MediaBreakPointProperty, breakPoint);
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }
}
