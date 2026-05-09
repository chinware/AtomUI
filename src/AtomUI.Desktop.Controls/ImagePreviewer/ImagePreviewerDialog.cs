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

internal class ImagePreviewerDialog : Window,
                                      IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IList<PreviewImageSource>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, IList<PreviewImageSource>?>(nameof(ItemsSource));

    public static readonly StyledProperty<bool> IsImageMovableProperty =
        ImagePreviewer.IsImageMovableProperty.AddOwner<ImagePreviewerDialog>();

    public static readonly StyledProperty<double> ScaleStepProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, double>(nameof(ScaleStep), 0.5);

    public static readonly StyledProperty<double> MinScaleProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, double>(nameof(MinScale), 1.0);

    public static readonly StyledProperty<double> MaxScaleProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, double>(nameof(MaxScale), 50.0);

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, bool>(nameof(IsModal));

    public static readonly StyledProperty<int> CurrentIndexProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, int>(nameof(CurrentIndex), 0);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ImagePreviewerDialog>();

    public static readonly DirectProperty<ImagePreviewerDialog, int> CountProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, int>(
            nameof(Count),
            o => o.Count);

    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<ImagePreviewerDialog, Transform?>(nameof(Transform));

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

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ImagePreviewerDialog, PreviewImageSource?> CurrentImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, PreviewImageSource?>(
            nameof(CurrentImage),
            o => o.CurrentImage,
            (o, v) => o.CurrentImage = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsMultiImagesProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsMultiImages),
            o => o.IsMultiImages,
            (o, v) => o.IsMultiImages = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsLastImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsLastImage),
            o => o.IsLastImage,
            (o, v) => o.IsLastImage = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsFirstImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsFirstImage),
            o => o.IsFirstImage,
            (o, v) => o.IsFirstImage = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsScaleDownEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsScaleDownEnabled),
            o => o.IsScaleDownEnabled,
            (o, v) => o.IsScaleDownEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsScaleUpEnabledProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsScaleUpEnabled),
            o => o.IsScaleUpEnabled,
            (o, v) => o.IsScaleUpEnabled = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, double> ImageScaleXProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, double>(
            nameof(ImageScaleX),
            o => o.ImageScaleX,
            (o, v) => o.ImageScaleX = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, double> ImageScaleYProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, double>(
            nameof(ImageScaleY),
            o => o.ImageScaleY,
            (o, v) => o.ImageScaleY = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> ImageScaleChangedProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(ImageScaleChanged),
            o => o.ImageScaleChanged,
            (o, v) => o.ImageScaleChanged = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, double> ImageRotateProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, double>(
            nameof(ImageRotate),
            o => o.ImageRotate,
            (o, v) => o.ImageRotate = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> IsImageFitToWindowProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
            nameof(IsImageFitToWindow),
            o => o.IsImageFitToWindow,
            (o, v) => o.IsImageFitToWindow = v);

    internal static readonly DirectProperty<ImagePreviewerDialog, bool> SuppressTransformAnimationProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerDialog, bool>(
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

    #endregion

    protected override Type StyleKeyOverride { get; } = typeof(ImagePreviewerDialog);

    private readonly AbstractImagePreviewer _imagePreviewer;
    private readonly ImageViewer _imageViewer;
    private bool _firstSizeCalculated;
    private ImageSwitchTransformPolicy _switchTransformPolicy = ImageSwitchTransformPolicy.CreateDefault();

    static ImagePreviewerDialog()
    {
        AffectsRender<ImagePreviewerDialog>(CurrentImageProperty);
        ImagePreviewBaseToolbar.HorizontalFlipRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleHorizontalFlipRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.VerticalFlipRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleVerticalFlipRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.RotateLeftRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleRotateLeftRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.RotateRightRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleRotateRightRequest(args);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.ScaleDownRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleScaleDownRequest();
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.ScaleUpRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleScaleUpRequest();
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.FitToWindowRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleFitToWindowRequest(args.IsFitToWindow);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.PreviousRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandlePreviousRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImagePreviewBaseToolbar.NextRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleNextImageRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.PreviousRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandlePreviousRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.NextRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            dialog.HandleNextImageRequest(useTransformPolicy: true);
            args.Handled = true;
        });
        ImageViewer.ScaleDownRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            var viewer = args.Source as ImageViewer;
            dialog.HandleScaleDownRequest(viewer?.WheelScaleStep);
            args.Handled = true;
        });
        ImageViewer.ScaleUpRequestEvent.AddClassHandler<ImagePreviewerDialog>((dialog, args) =>
        {
            var viewer = args.Source as ImageViewer;
            dialog.HandleScaleUpRequest(viewer?.WheelScaleStep);
            args.Handled = true;
        });
    }

    public ImagePreviewerDialog(TopLevel parent, AbstractImagePreviewer imagePreviewer)
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

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        HandleCurrentIndexChanged();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var calculatedSize = CalculateWindowSize();
        base.MeasureOverride(calculatedSize);
        return calculatedSize;
    }

    private Size CalculateWindowSize()
    {
        if (!_firstSizeCalculated)
        {
            var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
            if (screen is null)
            {
                _firstSizeCalculated = true;
                return ClientSize;
            }

            const double ratio     = 0.70d;
            var workingArea        = screen.WorkingArea;
            var scale              = DesktopScaling;
            var logicalWorkingSize = new Size(workingArea.Width / scale, workingArea.Height / scale);
            var width              = logicalWorkingSize.Width * ratio;
            var height             = logicalWorkingSize.Height * ratio;
            ClientSize             = new Size(width, height);
            _firstSizeCalculated   = true;
            return new Size(width, height);
        }
        return ClientSize;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic || CloseByClickCloseCaptionButton)
        {
            e.Cancel = true;
            Dispatcher.Post(() =>
            {
                CloseByClickCloseCaptionButton = false;
                _imagePreviewer.NotifyDialogHostCloseRequest();
            });
        }
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
            // SetCurrentValue(CurrentIndexProperty, 0) 在 CurrentIndex 已经是 0(默认值)时不会
            // 触发 PropertyChanged,HandleCurrentIndexChanged 永远不被调用 → 首次 layout 时
            // CurrentImage 仍为 null,图片无法居中。这里直接再调一次保证 CurrentImage 被设上。
            HandleCurrentIndexChanged();
        }

        if (change.Property == ImageScaleXProperty ||
            change.Property == ImageScaleYProperty)
        {
            SetCurrentValue(IsScaleDownEnabledProperty, MathUtils.GreaterThan(Math.Abs(ImageScaleX), MinScale) && MathUtils.GreaterThan(Math.Abs(ImageScaleY), MinScale));
            SetCurrentValue(IsScaleUpEnabledProperty, MathUtils.LessThan(Math.Abs(ImageScaleX), MaxScale) && MathUtils.LessThan(Math.Abs(ImageScaleY), MaxScale));
        }
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

    public void Close(Action? callback = null)
    {
        base.Close();
        callback?.Invoke();
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
            if (scaleX > 0)
            {
                scaleX = MinScale;
            }
            else
            {
                scaleX = -MinScale;
            }
        }
        if (MathUtils.LessThan(Math.Abs(scaleY), MinScale))
        {
            if (scaleY > 0)
            {
                scaleY = MinScale;
            }
            else
            {
                scaleY = -MinScale;
            }
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
            if (scaleX > 0)
            {
                scaleX = MaxScale;
            }
            else
            {
                scaleX = -MaxScale;
            }
        }
        if (MathUtils.GreaterThan(Math.Abs(scaleY), MaxScale))
        {
            if (scaleY > 0)
            {
                scaleY = MaxScale;
            }
            else
            {
                scaleY = -MaxScale;
            }
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

    protected override WindowTitleBar? NotifyCreateTitleBar(WindowTitleBar? oldTitleBar)
    {
        return new ImagePreviewerTitleBar
        {
            Name = "PART_TitleBar"
        };
    }

    protected override void NotifyConfigureTitleBar(WindowTitleBar titleBar)
    {
        // 跳过 base 对 TitleProperty 的绑定,预览弹窗不展示 Window.Title。
        if (titleBar is ImagePreviewerTitleBar previewerTitleBar)
        {
            previewerTitleBar[!ImagePreviewerTitleBar.CurrentIndexProperty]       = this[!CurrentIndexProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.CountProperty]              = this[!CountProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.IsScaleDownEnabledProperty] = this[!IsScaleDownEnabledProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.IsScaleUpEnabledProperty]   = this[!IsScaleUpEnabledProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.IsImageFitToWindowProperty] = this[!IsImageFitToWindowProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.IsFirstImageProperty]       = this[!IsFirstImageProperty];
            previewerTitleBar[!ImagePreviewerTitleBar.IsLastImageProperty]        = this[!IsLastImageProperty];
        }
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
