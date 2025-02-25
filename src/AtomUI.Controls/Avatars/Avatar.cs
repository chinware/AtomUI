using AtomUI.Controls.Loader;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;

namespace AtomUI.Controls;

public enum AvatarShape
{
    Circle,
    Square
}

public enum AvatarSizeType
{
    Large,
    Small,
    Default
}

public class Avatar : ContentControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> AltProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Alt));

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<Avatar, Icon?>(nameof(Icon));

    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<Avatar, double>(nameof(Gap), 4);

    public static readonly StyledProperty<AvatarShape?> AvatarShapeProperty =
        AvaloniaProperty.Register<Avatar, AvatarShape?>(nameof(Shape), defaultValue: null, inherits:true);

    public static readonly StyledProperty<string?> SrcProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Src));
    
    public static readonly StyledProperty<string?> SvgProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Svg));

    public static readonly StyledProperty<AvatarSize?> AvatarSizeProperty =
        AvaloniaProperty.Register<Avatar, AvatarSize?>(nameof(Size));
    
    public string? Alt
    {
        get => GetValue(AltProperty);
        set => SetValue(AltProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public AvatarShape? Shape
    {
        get => GetValue(AvatarShapeProperty);
        set => SetValue(AvatarShapeProperty, value);
    }

    private AvatarShape EffectiveShape  => 
         Shape ?? this.FindLogicalAncestorOfType<AvatarGroup>()?.Shape ?? AvatarShape.Circle;
    
    private AvatarSize EffectiveSize  => 
        Size ?? this.FindLogicalAncestorOfType<AvatarGroup>()?.Size ?? new AvatarSize(AvatarSizeType.Default);

    public string? Src
    {
        get => GetValue(SrcProperty);
        set => SetValue(SrcProperty, value);
    }
    
    public string? Svg
    {
        get => GetValue(SvgProperty);
        set => SetValue(SvgProperty, value);
    }

    public AvatarSize? Size
    {
        get => GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }
    
    #endregion

    public const string IconOnlyPC = ":icononly";

    public const string SizeNumber = "sizeNumber";

    internal static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<Avatar, double>(
            nameof(IconSize));
    
    internal static readonly DirectProperty<Avatar, IImage?> CurrentImageProperty =
        AvaloniaProperty.RegisterDirect<Avatar, IImage?>(
            nameof(CurrentImage),
            image => image._currentImage);

    internal static readonly DirectProperty<Avatar, bool> IsLoadingProperty =
        AvaloniaProperty.RegisterDirect<Avatar, bool>(
            nameof(IsLoading),
            image => image._isLoading);
    
    internal static readonly DirectProperty<Avatar, bool> IsIconProperty =
        AvaloniaProperty.RegisterDirect<Avatar, bool>(
            nameof(IsIcon),
            image => image._isIcon);
    
    internal static readonly DirectProperty<Avatar, bool> IsTxtProperty =
        AvaloniaProperty.RegisterDirect<Avatar, bool>(
            nameof(IsTxt),
            image => image._isTxt);
    
    internal static readonly StyledProperty<AvatarSizeType?> SizeTypeProperty =
        AvaloniaProperty.Register<Avatar, AvatarSizeType?>(
            nameof(SizeType), AvatarSizeType.Default);
    
    /*internal static readonly StyledProperty<double> TxtScaleProperty =
        AvaloniaProperty.Register<Avatar, double>(
            nameof(TxtScale), 1);*/
    
    internal static readonly StyledProperty<double?> LabelFontSizeProperty =
        AvaloniaProperty.Register<Avatar, double?>(
            nameof(LabelFontSize));
    
    private IImage? _currentImage;
    private bool _isLoading;
    private bool _isTxt;
    private bool _isIcon;
    private bool _isNumber;
    
    private CancellationTokenSource? _updateCancellationToken;
    
    internal double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    /*internal double TxtScale
    {
        get => GetValue(TxtScaleProperty);
        set => SetValue(TxtScaleProperty, value);
    }*/
    
    internal double? LabelFontSize
    {
        get => GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }
    
    internal AvatarSizeType? SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    public IImage? CurrentImage
    {
        get => _currentImage;
        set => SetAndRaise(CurrentImageProperty, ref _currentImage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetAndRaise(IsLoadingProperty, ref _isLoading, value);
    }
    
    internal bool IsIcon
    {
        get => _isIcon;
        private set => SetAndRaise(IsIconProperty, ref _isIcon, value);
    }
    
    internal bool IsTxt
    {
        get => _isTxt;
        private set => SetAndRaise(IsTxtProperty, ref _isTxt, value);
    }

    internal static IAsyncImageLoader AsyncImageLoader { get; set; } = new RamCachedWebImageLoader();

    private readonly Uri? _baseUri;

    static Avatar()
    {
        AffectsMeasure<Segmented>(
            AvatarSizeProperty, IconSizeProperty, IconProperty, IsIconProperty, IsTxtProperty, FontSizeProperty);
        AffectsRender<Segmented>(CurrentImageProperty,
            AvatarSizeProperty,
            AvatarShapeProperty, IconSizeProperty, IconProperty
            , IconProperty, IsIconProperty, IsTxtProperty, FontSizeProperty);
    }

    public Avatar(Uri? baseUri)
    {
        _baseUri = baseUri;
    }


    public Avatar()
    {
    }

    public Avatar(IServiceProvider serviceProvider)
        : this((serviceProvider.GetService(typeof(IUriContext)) as IUriContext)?.BaseUri)
    {
    }

    
    protected override Size MeasureOverride(Size availableSize)
    {
        if (EffectiveShape == AvatarShape.Circle)
        {
            CornerRadius = new CornerRadius(availableSize.Width);
        }
        if (IsTxt && _isNumber)
        {
            this.FontSize = 18;
        }

        this.Width = this.Height = availableSize.Width;
        return availableSize;
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!sizeChange)
        {
            calSize(EffectiveSize);
        }

        if (Alt is null && Content is string content)
        {
            Alt    = content;
            Content = null;
        }
        if (Src == null && Icon == null && Svg == null)
        {
            _isIcon = IsIcon = false;
            _isTxt = IsTxt = true;
        }
        /*if (IsTxt && _isNumber)
        {
            this.FontSize = 18;
        }*/
        
    }

    private bool sizeChange = false;
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {

        if (change.Property == SrcProperty)
        {
            _isIcon = IsIcon = false;
            _isTxt = IsTxt = false;
            UpdateImage(change.GetNewValue<string>());
        }else if (change.Property == SvgProperty)
        {
            _isIcon = IsIcon = false;
            _isTxt = IsTxt = false;
        }
        else if(change.Property == GapProperty)
        {
            calScale();
        }
        else if(change.Property == FontSizeProperty)
        {
            calScale();
        }
        else if (change.Property == IconProperty)
        {
            if (Src == null)
            {
                _isTxt = IsTxt = false;
                _isIcon = IsIcon = true;
                PseudoClasses.Set(IconOnlyPC, Icon is not null);
            }
            SetupIcon();
        }else if (change.Property == AltProperty || change.Property == ContentControl.ContentProperty)
        {
            if (Src == null && Icon == null && Svg == null)
            {
                _isIcon = IsIcon = false;
                _isTxt = IsTxt = true;
                if (IsTxt && _isNumber)
                {
                    this.FontSize = 18;
                }
                calScale();
            }
        }
        else if(change.Property == AvatarSizeProperty)
        {
            sizeChange = true;
            calSize(EffectiveSize);
        }
        base.OnPropertyChanged(change);
    }

    private void calSize(AvatarSize value)
    {

        _isNumber = false;
        if (value.Status == AvatarStatus.STATUS_NUM || value.Status == AvatarStatus.STATUS_RESPONSIVE)
        {
            Classes.Set(SizeNumber, true);
            Width = value.Size;
            Height = value.Size;
            FontSize = value.Size / 2;
            _isNumber = true;
            SizeType = null;
            if (IsTxt)
            {
                FontSize = 18;
            }
            calScale();
        }else if (value.Status == AvatarStatus.STATUS_ENUM)
        {
            SizeType = value.Type;
        }
    }

    private async void UpdateImage(string? source)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var oldCancellationToken = Interlocked.Exchange(ref _updateCancellationToken, cancellationTokenSource);

        try
        {
            oldCancellationToken?.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        if (source is null)
        {
            _currentImage = null;
            CurrentImage = null;
            return;
        }

        IsLoading = true;
        _currentImage = null;
        CurrentImage = null;

        var bitmap = await Task.Run(async () =>
        {
            try
            {
                await Task.Delay(10, cancellationTokenSource.Token);

                try
                {
                    var uri = new Uri(source, UriKind.RelativeOrAbsolute);
                    if (AssetLoader.Exists(uri, _baseUri))
                        return new Bitmap(AssetLoader.Open(uri, _baseUri));
                }
                catch (Exception)
                {
                    // ignored
                }

                return await AsyncImageLoader.ProvideImageAsync(source);
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }, CancellationToken.None);

        if (cancellationTokenSource.IsCancellationRequested)
            return;
        CurrentImage  = bitmap;
        _currentImage = bitmap;
        if (CurrentImage == null && Alt != null)
        {
            _isTxt = IsTxt = true;
        }
    }

    private double? _winWidth;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {

        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
        var topLevel = TopLevel.GetTopLevel(this);
       
        if (topLevel is Avalonia.Controls.Window)
        {
            _hostWindow = topLevel as Avalonia.Controls.Window;
        }

        if (_hostWindow != null)
        {
            // 订阅窗口大小变化事件
            if (EffectiveSize.Status == AvatarStatus.STATUS_RESPONSIVE)
            {
                _calResponse(_hostWindow.Width);
            }
            _hostWindow.SizeChanged -= OnHostWindowSizeChanged;
            _hostWindow.SizeChanged += OnHostWindowSizeChanged;
        }
    }

    private void HandleTemplateApplied(INameScope scope){
        SetupIcon();
    }
    
    private void SetupIcon()
    {
        if (Icon is not null)
        {
            BindUtils.RelayBind(this, FontSizeProperty, this, IconSizeProperty);
            BindUtils.RelayBind(this, IconSizeProperty, Icon, WidthProperty);
            BindUtils.RelayBind(this, IconSizeProperty, Icon, HeightProperty);
        }
        SetupIconBrush();
    }
     private void SetupIconBrush()
    {
        var normalFilledBrushKey   = SharedTokenKey.ColorTextLightSolid;
        var selectedFilledBrushKey = SharedTokenKey.ColorPrimaryActive;
        if (Icon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                normalFilledBrushKey);
            TokenResourceBinder.CreateTokenBinding(Icon, Icon.SelectedFilledBrushProperty,
                selectedFilledBrushKey);
        }
        
    }

    private void calScale()
    {
        double TxtScale = 1;
        if (IsTxt && Alt != null)
        {
            var textLayout = new TextLayout(Alt, GetDefaultTypeface(), FontSize, null);
            var w = textLayout.Width;
            if (w != 0 &&Width > 0 &&Width - Gap * 2 < w)
            {
                TxtScale = (Width - Gap * 2) / w;
            }
            else
            {
                TxtScale = 1;
            }
        }
        LabelFontSize = FontSize * TxtScale;
    }

    public Typeface GetDefaultTypeface()
    {
        var fontManager = FontManager.Current;
        var fontFamily = fontManager.DefaultFontFamily;
        return new Typeface(fontFamily);
    }
    
    private Avalonia.Controls.Window? _hostWindow;
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (_hostWindow != null)
        {
            _hostWindow.SizeChanged -= OnHostWindowSizeChanged;
            _hostWindow = null;
        } 
    }

    private void OnHostWindowSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // 处理窗口大小变化逻辑
        if (EffectiveSize.Status == AvatarStatus.STATUS_RESPONSIVE)
        {
            _calResponse(e.NewSize.Width);
        }
    }

    /**
     * Bootstrap的默认断点是xs（<576px）、sm（≥576px）、md（≥768px）、lg（≥992px）、xl（≥1200px）、xxl（≥1400px）
     */
    private void _calResponse(double? width)
    {
        if (width == null)
        {
            calSize(EffectiveSize);
            return;
        }
        
        Classes.Set(SizeNumber, true);
        
        if (width >= 1400)
        {
            Width = EffectiveSize.Xxl;
            Height = EffectiveSize.Xxl;
            FontSize = EffectiveSize.Xxl / 2;
        }else if (width >= 1200)
        {
            Width = EffectiveSize.Xl;
            Height = EffectiveSize.Xl;
            FontSize = EffectiveSize.Xl / 2;
        }else if (width >= 992)
        {
            Width = EffectiveSize.Lg;
            Height = EffectiveSize.Lg;
            FontSize = EffectiveSize.Lg / 2;
        }else if (width >= 768)
        {
            Width = EffectiveSize.Md;
            Height = EffectiveSize.Md;
            FontSize = EffectiveSize.Md / 2;
        }else if (width >= 576)
        {
            Width = EffectiveSize.Sm;
            Height = EffectiveSize.Sm;
            FontSize = EffectiveSize.Sm / 2;
        }
        else 
        {
            Width = EffectiveSize.Xs;
            Height = EffectiveSize.Xs;
            FontSize = EffectiveSize.Xs / 2;
        }

        _winWidth = width;
        _isNumber = true;
        SizeType = null;
        if (IsTxt)
        {
            FontSize = 18;
        }
        calScale();
    }
}