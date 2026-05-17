using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class ImagePreviewerTitleBar : WindowTitleBar
{
    #region 内部属性定义

    internal static readonly DirectProperty<ImagePreviewerTitleBar, bool> IsScaleDownEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerTitleBar, bool>(
            nameof(IsScaleDownEnabled),
            o => o.IsScaleDownEnabled,
            (o, v) => o.IsScaleDownEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerTitleBar, bool> IsScaleUpEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerTitleBar, bool>(
            nameof(IsScaleUpEnabled),
            o => o.IsScaleUpEnabled,
            (o, v) => o.IsScaleUpEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerTitleBar, bool> IsImageFitToWindowProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerTitleBar, bool>(
            nameof(IsImageFitToWindow),
            o => o.IsImageFitToWindow,
            (o, v) => o.IsImageFitToWindow = v);

    private bool _isScaleDownEnabled;

    internal bool IsScaleDownEnabled
    {
        get => _isScaleDownEnabled;
        set => SetAndRaise(IsScaleDownEnabledProperty, ref _isScaleDownEnabled, value);
    }

    private bool _isScaleUpEnabled;

    internal bool IsScaleUpEnabled
    {
        get => _isScaleUpEnabled;
        set => SetAndRaise(IsScaleUpEnabledProperty, ref _isScaleUpEnabled, value);
    }

    private bool _isImageFitToWindow;

    internal bool IsImageFitToWindow
    {
        get => _isImageFitToWindow;
        set => SetAndRaise(IsImageFitToWindowProperty, ref _isImageFitToWindow, value);
    }

    #endregion

    public ImagePreviewerTitleBar()
    {
        var toolbar = new ImagePreviewToolbar();
        toolbar[!ImagePreviewBaseToolbar.IsScaleDownEnabledProperty] = this[!IsScaleDownEnabledProperty];
        toolbar[!ImagePreviewBaseToolbar.IsScaleUpEnabledProperty]   = this[!IsScaleUpEnabledProperty];
        toolbar[!ImagePreviewBaseToolbar.IsImageFitToWindowProperty] = this[!IsImageFitToWindowProperty];
        LeftAddOn = toolbar;
    }
}
