using System.Diagnostics;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal enum AvatarContentType
{
    Icon,
    Image,
    Text
}

[PseudoClasses(":loading", ":loaded", ":failed", ":fallback")]
[TemplatePart("PART_TextPresenter", typeof(TextBlock))]
public abstract class AbstractAvatar : TemplatedControl, IMotionAwareControl, IImageLoadControl, IImageLoadControllerHost
{
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(Gap), 4.0);

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<AbstractAvatar, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<ImageLoadSource?> SourceProperty =
        AvaloniaProperty.Register<AbstractAvatar, ImageLoadSource?>(nameof(Source));

    public static readonly StyledProperty<ImageLoadSource?> FallbackSourceProperty =
        AvaloniaProperty.Register<AbstractAvatar, ImageLoadSource?>(nameof(FallbackSource));

    public static readonly StyledProperty<ImageRequestOptions?> RequestOptionsProperty =
        AvaloniaProperty.Register<AbstractAvatar, ImageRequestOptions?>(nameof(RequestOptions));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AbstractAvatar, string?>(nameof(Text));

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<AbstractAvatar, CustomizableSizeType>(
            nameof(SizeType),
            CustomizableSizeType.Middle);

    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(Size), double.NaN);

    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<AbstractAvatar, AvatarShape>(nameof(Shape), AvatarShape.Circle);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractAvatar>();

    public static readonly DirectProperty<AbstractAvatar, ImageLoadState> LoadStateProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, ImageLoadState>(nameof(LoadState), control => control.LoadState);

    public static readonly DirectProperty<AbstractAvatar, ImageLoadError?> LoadErrorProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, ImageLoadError?>(nameof(LoadError), control => control.LoadError);

    public static readonly DirectProperty<AbstractAvatar, ImageLoadProgress?> LoadProgressProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, ImageLoadProgress?>(nameof(LoadProgress), control => control.LoadProgress);

    public static readonly DirectProperty<AbstractAvatar, bool> IsLoadingProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, bool>(nameof(IsLoading), control => control.IsLoading);

    public static readonly DirectProperty<AbstractAvatar, bool> IsLoadedProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, bool>(nameof(IsLoaded), control => control.IsLoaded);

    public static readonly DirectProperty<AbstractAvatar, bool> IsFailedProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, bool>(nameof(IsFailed), control => control.IsFailed);

    internal static readonly StyledProperty<double> EffectiveIconSizeProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(EffectiveIconSize));

    internal static readonly StyledProperty<ITransform?> TextRenderTransformProperty =
        AvaloniaProperty.Register<AbstractAvatar, ITransform?>(nameof(TextRenderTransform));

    internal static readonly DirectProperty<AbstractAvatar, AvatarContentType> ContentTypeProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, AvatarContentType>(
            nameof(ContentType),
            control => control.ContentType,
            (control, value) => control.ContentType = value);

    internal static readonly DirectProperty<AbstractAvatar, IImage?> LoadedImageProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, IImage?>(nameof(LoadedImage), control => control.LoadedImage);

    private readonly ImageLoadController _controller;
    private CustomizableSizeType? _originSizeType;
    private TextBlock? _textPresenter;
    private AvatarContentType _contentType = AvatarContentType.Icon;
    private IImage? _loadedImage;
    private ImageLoadState _loadState;
    private ImageLoadError? _loadError;
    private ImageLoadProgress? _loadProgress;
    private bool _isLoading;
    private bool _isLoaded;
    private bool _isFailed;
    private bool _isAttached;

    static AbstractAvatar()
    {
        AffectsMeasure<AbstractAvatar>(SizeTypeProperty, TextProperty);
        AffectsRender<AbstractAvatar>(ShapeProperty, IconProperty, SourceProperty, GapProperty);
    }

    protected AbstractAvatar()
    {
        _controller = new ImageLoadController(this);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ImageLoadSource? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public ImageLoadSource? FallbackSource
    {
        get => GetValue(FallbackSourceProperty);
        set => SetValue(FallbackSourceProperty, value);
    }

    public ImageRequestOptions? RequestOptions
    {
        get => GetValue(RequestOptionsProperty);
        set => SetValue(RequestOptionsProperty, value);
    }

    [Content]
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public ImageLoadState LoadState => _loadState;

    public ImageLoadError? LoadError => _loadError;

    public ImageLoadProgress? LoadProgress => _loadProgress;

    public bool IsLoading => _isLoading;

    public new bool IsLoaded => _isLoaded;

    public bool IsFailed => _isFailed;

    public event EventHandler<ImageOpenedEventArgs>? ImageOpened;

    public event EventHandler<ImageFailedEventArgs>? ImageFailed;

    internal double EffectiveIconSize
    {
        get => GetValue(EffectiveIconSizeProperty);
        set => SetValue(EffectiveIconSizeProperty, value);
    }

    internal ITransform? TextRenderTransform
    {
        get => GetValue(TextRenderTransformProperty);
        set => SetValue(TextRenderTransformProperty, value);
    }

    internal AvatarContentType ContentType
    {
        get => _contentType;
        set => SetAndRaise(ContentTypeProperty, ref _contentType, value);
    }

    internal IImage? LoadedImage => _loadedImage;

    Visual IImageLoadControllerHost.Visual => this;

    ImageRequestPriority IImageLoadControllerHost.Priority => ImageRequestPriority.Normal;

    bool IImageLoadControllerHost.IsImageLoadAttached => _isAttached;

    public void Reload() => _controller.Reload();

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeProperty)
        {
            ConfigureCustomSize();
        }
        else if (change.Property == SizeTypeProperty)
        {
            ConfigureIconSize();
        }

        if (change.Property == SourceProperty ||
            change.Property == FallbackSourceProperty ||
            change.Property == RequestOptionsProperty)
        {
            _controller.SourceConfigurationChanged();
        }
        if (change.Property == IconProperty || change.Property == TextProperty)
        {
            ConfigureContentType();
        }
        if (change.Property == ContentTypeProperty || change.Property == GapProperty)
        {
            ConfigureTextRenderTransform();
        }
        else if (change.Property == ShapeProperty)
        {
            ConfigureShape();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_textPresenter is not null)
        {
            _textPresenter.SizeChanged -= HandleTextPresenterSizeChanged;
        }
        _textPresenter = e.NameScope.Find<TextBlock>("PART_TextPresenter");
        if (_textPresenter is not null)
        {
            _textPresenter.SizeChanged += HandleTextPresenterSizeChanged;
        }
        ConfigureShape();
        ConfigureIconSize();
        ConfigureContentType();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arranged = base.ArrangeOverride(finalSize);
        _controller.RefreshSize();
        return arranged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        _controller.Attach();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _controller.Detach();
        base.OnDetachedFromVisualTree(e);
    }

    (int Width, int Height)? IImageLoadControllerHost.GetDecodePixelSize()
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var width = Quantize(Bounds.Width * scaling);
        var height = Quantize(Bounds.Height * scaling);
        return width == 0 && height == 0 ? null : (width, height);
    }

    ImageLoadError? IImageLoadControllerHost.GetConfigurationError() => null;

    void IImageLoadControllerHost.SetLoadedImage(IImage? image)
    {
        SetAndRaise(LoadedImageProperty, ref _loadedImage, image);
        ConfigureContentType();
    }

    void IImageLoadControllerHost.SetLoadState(
        ImageLoadState state,
        ImageLoadError? error,
        ImageLoadProgress? progress,
        bool isFallback)
    {
        SetAndRaise(LoadStateProperty, ref _loadState, state);
        SetAndRaise(LoadErrorProperty, ref _loadError, error);
        SetAndRaise(LoadProgressProperty, ref _loadProgress, progress);
        SetAndRaise(IsLoadingProperty, ref _isLoading, state == ImageLoadState.Loading);
        SetAndRaise(IsLoadedProperty, ref _isLoaded, state == ImageLoadState.Loaded);
        SetAndRaise(IsFailedProperty, ref _isFailed, state == ImageLoadState.Failed);
        PseudoClasses.Set(":loading", state == ImageLoadState.Loading);
        PseudoClasses.Set(":loaded", state == ImageLoadState.Loaded);
        PseudoClasses.Set(":failed", state == ImageLoadState.Failed);
        PseudoClasses.Set(":fallback", state == ImageLoadState.Loaded && isFallback);
    }

    void IImageLoadControllerHost.RaiseImageOpened(ImageOpenedEventArgs eventArgs) =>
        ImageOpened?.Invoke(this, eventArgs);

    void IImageLoadControllerHost.RaiseImageFailed(ImageFailedEventArgs eventArgs) =>
        ImageFailed?.Invoke(this, eventArgs);

    private void ConfigureCustomSize()
    {
        if (!double.IsNaN(Size))
        {
            if (SizeType != CustomizableSizeType.Custom)
            {
                _originSizeType = SizeType;
            }
            SizeType = CustomizableSizeType.Custom;
        }
        else if (_originSizeType.HasValue)
        {
            SizeType = _originSizeType.Value;
            _originSizeType = null;
        }
    }

    private void ConfigureIconSize()
    {
        if (SizeType != CustomizableSizeType.Custom)
        {
            return;
        }
        Debug.Assert(!double.IsNaN(Size));
        SetValue(WidthProperty, Size, BindingPriority.Template);
        SetValue(HeightProperty, Size, BindingPriority.Template);
        SetValue(
            EffectiveIconSizeProperty,
            Icon is not null || Source is not null ? Size / 2 : 18,
            BindingPriority.Template);
    }

    private void ConfigureContentType()
    {
        ContentType = LoadedImage is not null
            ? AvatarContentType.Image
            : Text is not null
                ? AvatarContentType.Text
                : AvatarContentType.Icon;
    }

    private void HandleTextPresenterSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureTextRenderTransform();
    }

    private void ConfigureTextRenderTransform()
    {
        if (ContentType != AvatarContentType.Text || _textPresenter is null || Gap * 2 >= Width)
        {
            TextRenderTransform = null;
            return;
        }
        var textWidth = TextUtils.CalculateTextSize(Text ?? string.Empty, FontSize, FontFamily).Width;
        var scale = Math.Min((Width - Gap * 2) / textWidth, 1.0);
        var builder = new TransformOperations.Builder(2);
        builder.AppendScale(scale, scale);
        if (scale < 1.0)
        {
            var offsetX = scale * (textWidth - Width) / 2;
            builder.AppendTranslate(-offsetX, 0);
        }
        TextRenderTransform = builder.Build();
    }

    private void ConfigureShape()
    {
        if (Shape == AvatarShape.Circle)
        {
            SetValue(CornerRadiusProperty, new CornerRadius(Width / 2), BindingPriority.Template);
        }
    }

    private static int Quantize(double value)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return 0;
        }
        return checked((int)(Math.Ceiling(value / 16) * 16));
    }
}
