using AtomUI.Controls;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal class ImagePreviewerOverlayHost : ContentControl,
                                           IMotionAwareControl
{
    public static readonly StyledProperty<IList<PreviewImageSource>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, IList<PreviewImageSource>?>(nameof(ItemsSource));

    public static readonly StyledProperty<bool> IsImageMovableProperty =
        ImagePreviewer.IsImageMovableProperty.AddOwner<ImagePreviewerOverlayHost>();

    public static readonly StyledProperty<double> ScaleStepProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, double>(nameof(ScaleStep), 0.5);

    public static readonly StyledProperty<double> MinScaleProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, double>(nameof(MinScale), 1.0);

    public static readonly StyledProperty<double> MaxScaleProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, double>(nameof(MaxScale), 50.0);

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, bool>(nameof(IsModal));

    public static readonly StyledProperty<int> CurrentIndexProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, int>(nameof(CurrentIndex), 0);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ImagePreviewerOverlayHost>();

    public static readonly DirectProperty<ImagePreviewerOverlayHost, int> CountProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, int>(
            nameof(Count),
            o => o.Count);

    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<ImagePreviewerOverlayHost, Transform?>(nameof(Transform));

    public IList<PreviewImageSource>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public bool IsImageMovable
    {
        get => GetValue(IsImageMovableProperty);
        set => SetValue(IsImageMovableProperty, value);
    }

    public double ScaleStep
    {
        get => GetValue(ScaleStepProperty);
        set => SetValue(ScaleStepProperty, value);
    }

    public double MinScale
    {
        get => GetValue(MinScaleProperty);
        set => SetValue(MinScaleProperty, value);
    }

    public double MaxScale
    {
        get => GetValue(MaxScaleProperty);
        set => SetValue(MaxScaleProperty, value);
    }

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public int CurrentIndex
    {
        get => GetValue(CurrentIndexProperty);
        set => SetValue(CurrentIndexProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }

    private int _count;

    public int Count
    {
        get => _count;
        internal set => SetAndRaise(CountProperty, ref _count, value);
    }

    public TopLevel ParentTopLevel { get; }

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, PreviewImageSource?> CurrentImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, PreviewImageSource?>(
            nameof(CurrentImage),
            o => o.CurrentImage,
            (o, v) => o.CurrentImage = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsMultiImagesProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsMultiImages),
            o => o.IsMultiImages,
            (o, v) => o.IsMultiImages = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsLastImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsLastImage),
            o => o.IsLastImage,
            (o, v) => o.IsLastImage = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsFirstImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsFirstImage),
            o => o.IsFirstImage,
            (o, v) => o.IsFirstImage = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsScaleDownEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsScaleDownEnabled),
            o => o.IsScaleDownEnabled,
            (o, v) => o.IsScaleDownEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsScaleUpEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsScaleUpEnabled),
            o => o.IsScaleUpEnabled,
            (o, v) => o.IsScaleUpEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, double> ImageScaleXProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, double>(
            nameof(ImageScaleX),
            o => o.ImageScaleX,
            (o, v) => o.ImageScaleX = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, double> ImageScaleYProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, double>(
            nameof(ImageScaleY),
            o => o.ImageScaleY,
            (o, v) => o.ImageScaleY = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> ImageScaleChangedProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(ImageScaleChanged),
            o => o.ImageScaleChanged,
            (o, v) => o.ImageScaleChanged = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, double> ImageRotateProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, double>(
            nameof(ImageRotate),
            o => o.ImageRotate,
            (o, v) => o.ImageRotate = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> IsImageFitToWindowProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(IsImageFitToWindow),
            o => o.IsImageFitToWindow,
            (o, v) => o.IsImageFitToWindow = v);

    internal static readonly DirectProperty<ImagePreviewerOverlayHost, bool> SuppressTransformAnimationProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerOverlayHost, bool>(
            nameof(SuppressTransformAnimation),
            o => o.SuppressTransformAnimation,
            (o, v) => o.SuppressTransformAnimation = v);

    private PreviewImageSource? _currentImage;

    internal PreviewImageSource? CurrentImage
    {
        get => _currentImage;
        set => SetAndRaise(CurrentImageProperty, ref _currentImage, value);
    }

    private bool _isMultiImages;

    internal bool IsMultiImages
    {
        get => _isMultiImages;
        set => SetAndRaise(IsMultiImagesProperty, ref _isMultiImages, value);
    }

    private bool _isLastImage;

    internal bool IsLastImage
    {
        get => _isLastImage;
        set => SetAndRaise(IsLastImageProperty, ref _isLastImage, value);
    }

    private bool _isFirstImage;

    internal bool IsFirstImage
    {
        get => _isFirstImage;
        set => SetAndRaise(IsFirstImageProperty, ref _isFirstImage, value);
    }

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

    private double _imageScaleX;

    internal double ImageScaleX
    {
        get => _imageScaleX;
        set => SetAndRaise(ImageScaleXProperty, ref _imageScaleX, value);
    }

    private double _imageScaleY;

    internal double ImageScaleY
    {
        get => _imageScaleY;
        set => SetAndRaise(ImageScaleYProperty, ref _imageScaleY, value);
    }

    private bool _imageScaleChanged;

    internal bool ImageScaleChanged
    {
        get => _imageScaleChanged;
        set => SetAndRaise(ImageScaleChangedProperty, ref _imageScaleChanged, value);
    }

    private double _imageRotate;

    internal double ImageRotate
    {
        get => _imageRotate;
        set => SetAndRaise(ImageRotateProperty, ref _imageRotate, value);
    }

    private bool _isImageFitToWindow = true;

    internal bool IsImageFitToWindow
    {
        get => _isImageFitToWindow;
        set => SetAndRaise(IsImageFitToWindowProperty, ref _isImageFitToWindow, value);
    }

    private bool _suppressTransformAnimation;

    internal bool SuppressTransformAnimation
    {
        get => _suppressTransformAnimation;
        set => SetAndRaise(SuppressTransformAnimationProperty, ref _suppressTransformAnimation, value);
    }

    protected override Type StyleKeyOverride { get; } = typeof(ImagePreviewerOverlayHost);

    private readonly AbstractImagePreviewer _imagePreviewer;
    private readonly ImageViewer _imageViewer;
    private IconButton? _closeButton;
    private ImageSwitchTransformPolicy _switchTransformPolicy = ImageSwitchTransformPolicy.CreateDefault();

    static ImagePreviewerOverlayHost()
    {
        AffectsRender<ImagePreviewerOverlayHost>(CurrentImageProperty);
        FocusableProperty.OverrideDefaultValue<ImagePreviewerOverlayHost>(true);
        ImagePreviewBaseToolbar.HorizontalFlipRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleHorizontalFlipRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.VerticalFlipRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleVerticalFlipRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.RotateLeftRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleRotateLeftRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.RotateRightRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleRotateRightRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.ScaleDownRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleScaleDownRequest();
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.ScaleUpRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleScaleUpRequest();
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.FitToWindowRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleFitToWindowRequest(args.IsFitToWindow);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.PreviousRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandlePreviousRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.NextRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleNextImageRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.PreviousRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandlePreviousRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.NextRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            host.HandleNextImageRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.ScaleDownRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            var viewer = args.Source as ImageViewer;
            host.HandleScaleDownRequest(viewer?.WheelScaleStep);
            args.Handled = true;
        });
        ImageViewer.ScaleUpRequestEvent.AddClassHandler<ImagePreviewerOverlayHost>((host, args) =>
        {
            var viewer = args.Source as ImageViewer;
            host.HandleScaleUpRequest(viewer?.WheelScaleStep);
            args.Handled = true;
        });
    }

    public ImagePreviewerOverlayHost(TopLevel parent, AbstractImagePreviewer imagePreviewer)
    {
        ParentTopLevel  = parent;
        _imagePreviewer = imagePreviewer;
        _imageViewer    = CreateImageViewer();
        Content         = _imageViewer;

        AddHandler(KeyDownEvent, HandleDialogKeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
    }

    private ImageViewer CreateImageViewer()
    {
        var viewer = new ImageViewer
        {
            Name = "PART_ImageViewer"
        };
        viewer[!ImageViewer.CountProperty]                      = this[!CountProperty];
        viewer[!ImageViewer.CurrentIndexProperty]               = this[!CurrentIndexProperty];
        viewer[!ImageViewer.CurrentImageProperty]               = this[!CurrentImageProperty];
        viewer[!ImageViewer.IsFirstImageProperty]               = this[!IsFirstImageProperty];
        viewer[!ImageViewer.IsLastImageProperty]                = this[!IsLastImageProperty];
        viewer[!ImageViewer.IsMultiImagesProperty]              = this[!IsMultiImagesProperty];
        viewer[!ImageViewer.IsScaleUpEnabledProperty]           = this[!IsScaleUpEnabledProperty];
        viewer[!ImageViewer.IsScaleDownEnabledProperty]         = this[!IsScaleDownEnabledProperty];
        viewer[!ImageViewer.IsMotionEnabledProperty]            = this[!IsMotionEnabledProperty];
        viewer[!ImageViewer.ImageScaleXProperty]                = this[!ImageScaleXProperty];
        viewer[!ImageViewer.ImageScaleYProperty]                = this[!ImageScaleYProperty];
        viewer[!ImageViewer.ImageRotateProperty]                = this[!ImageRotateProperty];
        viewer[!ImageViewer.IsImageMovableProperty]             = this[!IsImageMovableProperty];
        viewer[!ImageViewer.IsImageFitToWindowProperty]         = this[!IsImageFitToWindowProperty];
        viewer[!ImageViewer.SuppressTransformAnimationProperty] = this[!SuppressTransformAnimationProperty];
        viewer[!ImageViewer.ImageScaleChangedProperty]          = this[!ImageScaleChangedProperty];
        return viewer;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_closeButton != null)
        {
            _closeButton.Click -= HandleCloseButtonClicked;
        }

        _closeButton = e.NameScope.Find<IconButton>("PART_CloseButton");
        if (_closeButton != null)
        {
            _closeButton.Click += HandleCloseButtonClicked;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        HandleCurrentIndexChanged();
        Dispatcher.Post(EnsureDialogKeyboardFocus, DispatcherPriority.Background);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = availableSize;
        if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
        {
            size = ParentTopLevel.ClientSize;
        }

        base.MeasureOverride(size);
        return size;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CurrentIndexProperty)
        {
            HandleCurrentIndexChanged();
        }
        else if (change.Property == ItemsSourceProperty)
        {
            if (ItemsSource?.Count > 0)
            {
                SetCurrentValue(CurrentIndexProperty, 0);
            }
            SetCurrentValue(IsMultiImagesProperty, ItemsSource?.Count > 1);
            Count = ItemsSource?.Count ?? 0;
            HandleCurrentIndexChanged();
        }

        if (change.Property == ImageScaleXProperty ||
            change.Property == ImageScaleYProperty)
        {
            SetCurrentValue(IsScaleDownEnabledProperty,
                MathUtils.GreaterThan(Math.Abs(ImageScaleX), MinScale) &&
                MathUtils.GreaterThan(Math.Abs(ImageScaleY), MinScale));
            SetCurrentValue(IsScaleUpEnabledProperty,
                MathUtils.LessThan(Math.Abs(ImageScaleX), MaxScale) &&
                MathUtils.LessThan(Math.Abs(ImageScaleY), MaxScale));
        }
    }

    public void Close(Action? callback = null)
    {
        callback?.Invoke();
    }

    private void HandleCloseButtonClicked(object? sender, RoutedEventArgs e)
    {
        _imagePreviewer.NotifyDialogHostCloseRequest();
    }

    private void HandleCurrentIndexChanged()
    {
        if (ItemsSource?.Count > 0 && CurrentIndex >= 0 && CurrentIndex < ItemsSource.Count)
        {
            SetCurrentValue(CurrentImageProperty, ItemsSource[CurrentIndex]);
            SetCurrentValue(IsFirstImageProperty, CurrentIndex == 0);
            SetCurrentValue(IsLastImageProperty, CurrentIndex == ItemsSource.Count - 1);
        }
        else if (ItemsSource == null || ItemsSource?.Count == 0)
        {
            SetCurrentValue(IsLastImageProperty, false);
            SetCurrentValue(IsFirstImageProperty, false);
        }
    }

    private void UpdateSwitchTransformPolicy(ImagePreviewToolbarSource source)
    {
        if (source == ImagePreviewToolbarSource.TitleBar)
        {
            _switchTransformPolicy.RetentionMode = ImageTransformRetentionMode.Retain;
            _switchTransformPolicy.SuppressResetAnimation = false;
            return;
        }

        _switchTransformPolicy.RetentionMode = ImageTransformRetentionMode.Reset;
        _switchTransformPolicy.SuppressResetAnimation = true;
    }

    private void CaptureRetainedFlipRotateState()
    {
        _switchTransformPolicy.RetainedScaleX = ImageScaleX > 0.0 ? 1.0 : -1.0;
        _switchTransformPolicy.RetainedScaleY = ImageScaleY > 0.0 ? 1.0 : -1.0;
        _switchTransformPolicy.RetainedRotate = ImageRotate;
    }

    private bool ShouldRetainFlipRotateOnSwitch => _switchTransformPolicy.RetentionMode == ImageTransformRetentionMode.Retain;

    private void HandleHorizontalFlipRequest(ImagePreviewToolbarRequestEventArgs args)
    {
        UpdateSwitchTransformPolicy(args.ToolbarSource);
        SetCurrentValue(ImageScaleXProperty, -1 * ImageScaleX);
        if (ShouldRetainFlipRotateOnSwitch)
        {
            CaptureRetainedFlipRotateState();
        }
    }

    private void HandleVerticalFlipRequest(ImagePreviewToolbarRequestEventArgs args)
    {
        UpdateSwitchTransformPolicy(args.ToolbarSource);
        SetCurrentValue(ImageScaleYProperty, -1 * ImageScaleY);
        if (ShouldRetainFlipRotateOnSwitch)
        {
            CaptureRetainedFlipRotateState();
        }
    }

    private void HandleRotateLeftRequest(ImagePreviewToolbarRequestEventArgs args)
    {
        UpdateSwitchTransformPolicy(args.ToolbarSource);
        SetCurrentValue(ImageRotateProperty, ImageRotate - MathUtils.Deg2Rad(90));
        if (ShouldRetainFlipRotateOnSwitch)
        {
            CaptureRetainedFlipRotateState();
        }
    }

    private void HandleRotateRightRequest(ImagePreviewToolbarRequestEventArgs args)
    {
        UpdateSwitchTransformPolicy(args.ToolbarSource);
        SetCurrentValue(ImageRotateProperty, ImageRotate + MathUtils.Deg2Rad(90));
        if (ShouldRetainFlipRotateOnSwitch)
        {
            CaptureRetainedFlipRotateState();
        }
    }

    private void HandleScaleDownRequest(double? scaleStep = null)
    {
        var step = ResolveScaleStep(scaleStep);
        var scaleX = ImageScaleX * (1 / (1 + step));
        var scaleY = ImageScaleY * (1 / (1 + step));
        if (MathUtils.LessThan(Math.Abs(scaleX), MinScale))
        {
            scaleX = scaleX > 0 ? MinScale : -MinScale;
        }
        if (MathUtils.LessThan(Math.Abs(scaleY), MinScale))
        {
            scaleY = scaleY > 0 ? MinScale : -MinScale;
        }
        SetCurrentValue(ImageScaleXProperty, scaleX);
        SetCurrentValue(ImageScaleYProperty, scaleY);
        SetCurrentValue(ImageScaleChangedProperty, true);
    }

    private void HandleScaleUpRequest(double? scaleStep = null)
    {
        var step = ResolveScaleStep(scaleStep);
        var scaleX = ImageScaleX * (1 + step);
        var scaleY = ImageScaleY * (1 + step);

        if (MathUtils.GreaterThan(Math.Abs(scaleX), MaxScale))
        {
            scaleX = scaleX > 0 ? MaxScale : -MaxScale;
        }
        if (MathUtils.GreaterThan(Math.Abs(scaleY), MaxScale))
        {
            scaleY = scaleY > 0 ? MaxScale : -MaxScale;
        }
        SetCurrentValue(ImageScaleXProperty, scaleX);
        SetCurrentValue(ImageScaleYProperty, scaleY);
        SetCurrentValue(ImageScaleChangedProperty, true);
    }

    private double ResolveScaleStep(double? scaleStep)
    {
        var step = scaleStep ?? ScaleStep;
        return MathUtils.LessThanOrClose(step, 0.0) ? 0.1 : step;
    }

    private void HandleImageSwitchKeyDown(KeyEventArgs e)
    {
        var imageCount = ItemsSource?.Count ?? Count;
        if (imageCount <= 1)
        {
            return;
        }

        if (e.Key == Key.Right)
        {
            e.Handled = true;
            if (CurrentIndex < imageCount - 1)
            {
                HandleNextImageRequest(useTransformPolicy: true);
            }
        }
        else if (e.Key == Key.Left)
        {
            e.Handled = true;
            if (CurrentIndex > 0)
            {
                HandlePreviousRequest(useTransformPolicy: true);
            }
        }
    }

    private void HandleDialogKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            _imagePreviewer.NotifyDialogHostCloseRequest();
            return;
        }

        HandleImageSwitchKeyDown(e);
    }

    private void HandleFitToWindowRequest(bool isFitToWindow)
    {
        SetCurrentValue(ImageScaleXProperty, ImageScaleX > 0.0 ? 1.0 : -1.0);
        SetCurrentValue(ImageScaleYProperty, ImageScaleY > 0.0 ? 1.0 : -1.0);
        SetCurrentValue(ImageScaleChangedProperty, false);
        SetCurrentValue(IsImageFitToWindowProperty, isFitToWindow);
    }

    private void EnsureDialogKeyboardFocus()
    {
        if (_imageViewer.Focus() != true)
        {
            Focus();
        }

        Dispatcher.Post(() =>
        {
            if (_imageViewer.Focus() == true)
            {
                return;
            }

            Focus();
        }, DispatcherPriority.Background);
    }

    private void ResetImageStateBeforeSwitch(bool useTransformPolicy)
    {
        SetCurrentValue(IsImageFitToWindowProperty, true);
        SetCurrentValue(ImageScaleChangedProperty, false);

        if (useTransformPolicy && ShouldRetainFlipRotateOnSwitch)
        {
            SetCurrentValue(ImageScaleXProperty, _switchTransformPolicy.RetainedScaleX);
            SetCurrentValue(ImageScaleYProperty, _switchTransformPolicy.RetainedScaleY);
            SetCurrentValue(ImageRotateProperty, _switchTransformPolicy.RetainedRotate);
            return;
        }

        SetCurrentValue(ImageScaleXProperty, 1.0);
        SetCurrentValue(ImageScaleYProperty, 1.0);
        if (useTransformPolicy)
        {
            SetCurrentValue(ImageRotateProperty, 0.0);
        }
    }

    private void SwitchImage(int offset, bool useTransformPolicy)
    {
        if (Count <= 0 || offset == 0)
        {
            return;
        }

        var suppressTransformAnimation = useTransformPolicy && _switchTransformPolicy.SuppressResetAnimation;
        if (suppressTransformAnimation)
        {
            SetCurrentValue(SuppressTransformAnimationProperty, true);
        }

        ResetImageStateBeforeSwitch(useTransformPolicy);
        var targetIndex = offset > 0
            ? Math.Min(CurrentIndex + offset, Count - 1)
            : Math.Max(CurrentIndex + offset, 0);
        SetCurrentValue(CurrentIndexProperty, targetIndex);

        if (suppressTransformAnimation)
        {
            SetCurrentValue(SuppressTransformAnimationProperty, false);
        }

        EnsureDialogKeyboardFocus();
    }

    private void HandleNextImageRequest(bool useTransformPolicy = false)
    {
        SwitchImage(1, useTransformPolicy);
    }

    private void HandlePreviousRequest(bool useTransformPolicy = false)
    {
        SwitchImage(-1, useTransformPolicy);
    }

    private enum ImageTransformRetentionMode
    {
        Reset,
        Retain
    }

    private struct ImageSwitchTransformPolicy
    {
        public ImageTransformRetentionMode RetentionMode;
        public bool SuppressResetAnimation;
        public double RetainedScaleX;
        public double RetainedScaleY;
        public double RetainedRotate;

        public static ImageSwitchTransformPolicy CreateDefault()
        {
            return new ImageSwitchTransformPolicy
            {
                RetentionMode          = ImageTransformRetentionMode.Reset,
                SuppressResetAnimation = false,
                RetainedScaleX         = 1.0,
                RetainedScaleY         = 1.0,
                RetainedRotate         = 0.0
            };
        }
    }
}
