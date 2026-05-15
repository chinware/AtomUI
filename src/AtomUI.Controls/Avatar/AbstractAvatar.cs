using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Controls.Commons;

internal enum AvatarContentType
{
    Icon,
    BitmapImage,
    SvgImage,
    Text
}

public abstract class AbstractAvatar : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(Gap), defaultValue: 4.0);

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<AbstractAvatar, PathIcon?>(nameof(Icon));
    
    public static readonly StyledProperty<IImage?> BitmapSrcProperty =
        AvaloniaProperty.Register<AbstractAvatar, IImage?>(nameof(BitmapSrc));
    
    public static readonly StyledProperty<string?> SrcProperty =
        AvaloniaProperty.Register<AbstractAvatar, string?>(nameof(Src));
    
    public static readonly StyledProperty<string?> TextProperty = 
        AvaloniaProperty.Register<TemplatedControl, string?>(nameof (Text));

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<AbstractAvatar, CustomizableSizeType>(nameof(SizeType),
            CustomizableSizeType.Middle);
    
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(Size), Double.NaN);
    
    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<AbstractAvatar, AvatarShape>(nameof(Shape), AvatarShape.Circle);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractAvatar>();

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
    
    public IImage? BitmapSrc
    {
        get => GetValue(BitmapSrcProperty);
        set => SetValue(BitmapSrcProperty, value);
    }
    
    public string? Src
    {
        get => GetValue(SrcProperty);
        set => SetValue(SrcProperty, value);
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

    #endregion

    #region 内部属性定义
    internal static readonly StyledProperty<double> EffectiveIconSizeProperty =
        AvaloniaProperty.Register<AbstractAvatar, double>(nameof(EffectiveIconSize));
    
    internal static readonly StyledProperty<ITransform?> TextRenderTransformProperty = 
        AvaloniaProperty.Register<AbstractAvatar, ITransform?>(nameof(TextRenderTransform));
    
    internal static readonly DirectProperty<AbstractAvatar, AvatarContentType> ContentTypeProperty =
        AvaloniaProperty.RegisterDirect<AbstractAvatar, AvatarContentType>(
            nameof(ContentType),
            o => o.ContentType,
            (o, v) => o.ContentType = v);

    internal double EffectiveIconSize
    {
        get => GetValue(EffectiveIconSizeProperty);
        set => SetValue(EffectiveIconSizeProperty, value);
    }
    
    public ITransform? TextRenderTransform
    {
        get => GetValue(TextRenderTransformProperty);
        set => SetValue(TextRenderTransformProperty, value);
    }
    
    private AvatarContentType _contentType = AvatarContentType.Icon;

    internal AvatarContentType ContentType
    {
        get => _contentType;
        set => SetAndRaise(ContentTypeProperty, ref _contentType, value);
    }
    #endregion

    private CustomizableSizeType? _originSizeType;
    private Panel? _contentHost;
    private Control? _activeContentPresenter;
    private AvatarContentType? _activeContentType;
    private CompositeDisposable? _contentPresenterDisposables;
    private TextBlock? _textPresenter;
    private TextRenderTransformCache? _textRenderTransformCache;

    static AbstractAvatar()
    {
        AffectsMeasure<AbstractAvatar>(SizeTypeProperty, TextProperty);
        AffectsRender<AbstractAvatar>(ShapeProperty, IconProperty, SrcProperty, GapProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeProperty)
        {
            if (!double.IsNaN(Size))
            {
                _originSizeType = SizeType;
                SizeType        = CustomizableSizeType.Custom;
            }
            else
            {
                if (_originSizeType.HasValue)
                {
                    SizeType = _originSizeType.Value;
                }
            }
        }
        else if (change.Property == SizeTypeProperty)
        {
            ConfigureIconSize();
        }
        else if (change.Property == SrcProperty ||
                 change.Property == BitmapSrcProperty ||
                 change.Property == IconProperty ||
                 change.Property == TextProperty)
        {
            ConfigureIconSize();
            ConfigureContentType();
        }

        if (change.Property == ContentTypeProperty ||
            change.Property == GapProperty ||
            change.Property == TextProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FontFamilyProperty ||
            change.Property == WidthProperty)
        {
            ConfigureTextRenderTransform();
        }
        if (change.Property == ContentTypeProperty)
        {
            UpdateContentPresenter();
        }
        if (change.Property == ShapeProperty ||
            change.Property == WidthProperty)
        {
            ConfigureShape();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachActiveContentPresenter();
        _contentHost = e.NameScope.Find<Panel>("RootLayout");
        ConfigureShape();
        ConfigureIconSize();
        ConfigureContentType();
        UpdateContentPresenter();
    }

    private void HandleTextPresenterSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        ConfigureTextRenderTransform();
    }

    private void ConfigureIconSize()
    {
        if (SizeType == CustomizableSizeType.Custom)
        {
            Debug.Assert(!double.IsNaN(Size));
            // 不影响模板设置
            SetValue(WidthProperty, Size, BindingPriority.Template);
            SetValue(HeightProperty, Size, BindingPriority.Template);
            SetValue(EffectiveIconSizeProperty, 
                (Icon != null || Src != null) ? Size / 2 : 18, BindingPriority.Template);
        }
    }

    private void ConfigureContentType()
    {
        var contentType = AvatarContentType.Icon;
        if (Src != null)
        {
            contentType = AvatarContentType.SvgImage;
        }
        else if (BitmapSrc != null)
        {
            contentType = AvatarContentType.BitmapImage;
        }
        else if (Text != null)
        {
            contentType = AvatarContentType.Text;
        }

        if (ContentType == contentType)
        {
            UpdateContentPresenter();
        }
        else
        {
            ContentType = contentType;
        }
    }

    private void UpdateContentPresenter()
    {
        if (_contentHost == null)
        {
            return;
        }

        if (_activeContentPresenter != null &&
            _activeContentType == ContentType)
        {
            return;
        }

        DetachActiveContentPresenter();
        _activeContentType      = ContentType;
        _activeContentPresenter = CreateContentPresenter(ContentType);
        _activeContentPresenter.SetTemplatedParent(this);
        _contentHost.Children.Add(_activeContentPresenter);
        ConfigureTextRenderTransform();
    }

    private Control CreateContentPresenter(AvatarContentType contentType)
    {
        _contentPresenterDisposables = new CompositeDisposable();
        return contentType switch
        {
            AvatarContentType.BitmapImage => CreateImagePresenter(),
            AvatarContentType.SvgImage    => CreateSvgPresenter(),
            AvatarContentType.Text        => CreateTextPresenter(),
            _                             => CreateIconPresenter()
        };
    }

    private IconPresenter CreateIconPresenter()
    {
        Debug.Assert(_contentPresenterDisposables != null);
        var presenter = new IconPresenter
        {
            Name                = "IconPresenter",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, EffectiveIconSizeProperty, presenter, WidthProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, EffectiveIconSizeProperty, presenter, HeightProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, IconProperty, presenter, IconPresenter.IconProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, ForegroundProperty, presenter, IconPresenter.IconBrushProperty, BindingMode.Default, BindingPriority.Template));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, IconPresenter.IsMotionEnabledProperty));
        return presenter;
    }

    private Image CreateImagePresenter()
    {
        Debug.Assert(_contentPresenterDisposables != null);
        var presenter = new Image
        {
            Name                = "ImagePresenter",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, WidthProperty, presenter, WidthProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, HeightProperty, presenter, HeightProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, BitmapSrcProperty, presenter, Image.SourceProperty));
        return presenter;
    }

    private SvgControl CreateSvgPresenter()
    {
        Debug.Assert(_contentPresenterDisposables != null);
        var presenter = new SvgControl(new Uri("avares://AtomUI.Controls/"))
        {
            Name                = "SvgPresenter",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, WidthProperty, presenter, WidthProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, HeightProperty, presenter, HeightProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, SrcProperty, presenter, SvgControl.PathProperty));
        return presenter;
    }

    private TextBlock CreateTextPresenter()
    {
        Debug.Assert(_contentPresenterDisposables != null);
        _textPresenter = new TextBlock
        {
            Name                = "PART_TextPresenter",
            ClipToBounds        = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _textPresenter.SizeChanged += HandleTextPresenterSizeChanged;
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, TextProperty, _textPresenter, TextBlock.TextProperty));
        _contentPresenterDisposables.Add(BindUtils.RelayBind(this, TextRenderTransformProperty, _textPresenter, RenderTransformProperty));
        return _textPresenter;
    }

    private void DetachActiveContentPresenter()
    {
        if (_activeContentPresenter == null)
        {
            return;
        }

        _contentPresenterDisposables?.Dispose();
        _contentPresenterDisposables = null;
        if (_textPresenter != null)
        {
            _textPresenter.SizeChanged -= HandleTextPresenterSizeChanged;
            _textPresenter = null;
        }

        ClearContentPresenterValue(_activeContentPresenter);
        if (_activeContentPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_activeContentPresenter);
        }
        else
        {
            _contentHost?.Children.Remove(_activeContentPresenter);
        }
        _activeContentPresenter.SetTemplatedParent(null);
        _activeContentPresenter = null;
        _activeContentType      = null;
        _textRenderTransformCache = null;
    }

    private static void ClearContentPresenterValue(Control presenter)
    {
        switch (presenter)
        {
            case IconPresenter iconPresenter:
                iconPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
                break;
            case Image image:
                image.SetCurrentValue(Image.SourceProperty, null);
                break;
            case SvgControl svg:
                svg.SetCurrentValue(SvgControl.PathProperty, null);
                break;
            case TextBlock textBlock:
                textBlock.SetCurrentValue(TextBlock.TextProperty, null);
                textBlock.SetCurrentValue(RenderTransformProperty, null);
                break;
        }
    }

    private void ConfigureTextRenderTransform()
    {
        if (ContentType != AvatarContentType.Text)
        {
            _textRenderTransformCache = null;
            TextRenderTransform = null;
        }
        else
        {
            if (_textPresenter != null && Gap * 2 < Width)
            {
                var cache = new TextRenderTransformCache(Text ?? string.Empty, FontSize, FontFamily, Width, Gap);
                if (_textRenderTransformCache == cache)
                {
                    return;
                }
                _textRenderTransformCache = cache;

                double scale     = 1;
                double offsetX   = 0;
                var    textWidth = TextUtils.CalculateTextSize(Text ?? string.Empty, FontSize, FontFamily).Width;
                if (textWidth > 0 && Gap * 2 < Width)
                {
                    scale = (Width - Gap * 2) / textWidth;
                    scale = Math.Min(scale, 1.0);
                    offsetX = (scale * (textWidth - Width)) / 2;
                }
                var builder = new TransformOperations.Builder(2);
                builder.AppendScale(scale, scale);
                if (scale < 1.0)
                {
                    builder.AppendTranslate(-offsetX, 0);
                }
                TextRenderTransform = builder.Build();
            }
            else
            {
                _textRenderTransformCache = null;
                TextRenderTransform = null;
            }
        }
    }

    private void ConfigureShape()
    {
        if (Shape == AvatarShape.Circle)
        {
            SetValue(CornerRadiusProperty, new CornerRadius(Width / 2), BindingPriority.Template);
        }
    }

    private readonly record struct TextRenderTransformCache(
        string Text,
        double FontSize,
        FontFamily FontFamily,
        double Width,
        double Gap);
}
