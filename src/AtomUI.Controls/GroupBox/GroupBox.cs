using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public enum GroupBoxTitlePosition
{
    Left,
    Right,
    Center
}

public class GroupBox : ContentControl,
                        IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> HeaderTitleProperty =
        AvaloniaProperty.Register<GroupBox, string?>(nameof(HeaderTitle));

    public static readonly StyledProperty<IBrush?> HeaderTitleColorProperty =
        AvaloniaProperty.Register<GroupBox, IBrush?>(nameof(HeaderTitleColor));

    public static readonly StyledProperty<Icon?> HeaderIconProperty
        = AvaloniaProperty.Register<GroupBox, Icon?>(nameof(HeaderIcon));

    public static readonly StyledProperty<GroupBoxTitlePosition> HeaderTitlePositionProperty =
        AvaloniaProperty.Register<GroupBox, GroupBoxTitlePosition>(nameof(HeaderTitlePosition));

    public static readonly StyledProperty<double> HeaderFontSizeProperty =
        AvaloniaProperty.Register<GroupBox, double>(nameof(HeaderFontSize));

    public static readonly StyledProperty<FontStyle> HeaderFontStyleProperty =
        AvaloniaProperty.Register<GroupBox, FontStyle>(nameof(HeaderFontStyle));

    public static readonly StyledProperty<FontWeight> HeaderFontWeightProperty =
        AvaloniaProperty.Register<GroupBox, FontWeight>(nameof(HeaderFontWeight), FontWeight.Normal);

    public string? HeaderTitle
    {
        get => GetValue(HeaderTitleProperty);
        set => SetValue(HeaderTitleProperty, value);
    }

    public IBrush? HeaderTitleColor
    {
        get => GetValue(HeaderTitleColorProperty);
        set => SetValue(HeaderTitleColorProperty, value);
    }

    public Icon? HeaderIcon
    {
        get => GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    public GroupBoxTitlePosition HeaderTitlePosition
    {
        get => GetValue(HeaderTitlePositionProperty);
        set => SetValue(HeaderTitlePositionProperty, value);
    }

    public double HeaderFontSize
    {
        get => GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public FontStyle HeaderFontStyle
    {
        get => GetValue(HeaderFontStyleProperty);
        set => SetValue(HeaderFontStyleProperty, value);
    }

    public FontWeight HeaderFontWeight
    {
        get => GetValue(HeaderFontWeightProperty);
        set => SetValue(HeaderFontWeightProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => GroupBoxToken.ID;

    #endregion

    private readonly BorderRenderHelper _borderRenderHelper;
    private Control? _headerContentContainer;
    private Border? _frameDecorator;
    private Rect _borderBounds;

    static GroupBox()
    {
        AffectsMeasure<GroupBox>(HeaderIconProperty);
    }

    public GroupBox()
    {
        this.RegisterResources();
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerContentContainer = e.NameScope.Find<Decorator>(GroupBoxTheme.HeaderContentPart);
        _frameDecorator         = e.NameScope.Find<Border>(GroupBoxTheme.FrameDecoratorPart);
        if (HeaderIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(HeaderIcon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorIcon);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return LayoutHelper.MeasureChild(_frameDecorator, availableSize, default, BorderThickness);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = LayoutHelper.ArrangeChild(_frameDecorator, finalSize, Padding, BorderThickness);
        if (_headerContentContainer is not null)
        {
            var headerOffset = _headerContentContainer.TranslatePoint(new Point(0, 0), this) ?? default;
            var offsetY      = headerOffset.Y + _headerContentContainer.DesiredSize.Height / 2;
            _borderBounds = new Rect(new Point(0, offsetY), new Size(finalSize.Width, finalSize.Height - offsetY));
        }

        return size;
    }

    public override void Render(DrawingContext context)
    {
        {
            using var state = context.PushTransform(Matrix.CreateTranslation(0, _borderBounds.Y));
            _borderRenderHelper.Render(context,
                _borderBounds.Size,
                BorderThickness,
                CornerRadius,
                BackgroundSizing.InnerBorderEdge,
                Background,
                BorderBrush,
                default);
        }
        {
            // 绘制遮挡
            if (_headerContentContainer is not null)
            {
                var headerOffset = _headerContentContainer.TranslatePoint(new Point(0, 0), this) ?? default;
                var bounds       = new Rect(headerOffset, _headerContentContainer.DesiredSize);
                context.FillRectangle(Background ?? new SolidColorBrush(Colors.Transparent), bounds);
            }
        }
    }
}