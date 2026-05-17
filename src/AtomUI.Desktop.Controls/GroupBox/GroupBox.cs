using AtomUI.Controls.Utils;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum GroupBoxTitlePosition
{
    Left,
    Right,
    Center
}

public class GroupBox : ContentControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> HeaderTitleProperty =
        AvaloniaProperty.Register<GroupBox, string?>(nameof(HeaderTitle));

    public static readonly StyledProperty<IBrush?> HeaderTitleColorProperty =
        AvaloniaProperty.Register<GroupBox, IBrush?>(nameof(HeaderTitleColor));

    public static readonly StyledProperty<PathIcon?> HeaderIconProperty =
        AvaloniaProperty.Register<GroupBox, PathIcon?>(nameof(HeaderIcon));

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

    public PathIcon? HeaderIcon
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

    private static readonly IBrush TransparentBackground = Brushes.Transparent;
    private readonly BorderRenderHelper _borderRenderHelper;
    private Control? _headerContentContainer;
    private Panel? _headerLayout;
    private IconPresenter? _headerIconPresenter;
    private Border? _frame;
    private Rect _borderBounds;
    private Rect _headerOcclusionBounds;

    static GroupBox()
    {
        AffectsMeasure<GroupBox>(HeaderIconProperty);
        AffectsRender<GroupBox>(
            BackgroundProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            CornerRadiusProperty);
    }

    public GroupBox()
    {
        this.RegisterTokenResourceScope(GroupBoxToken.ScopeProvider);
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHeaderIconPresenter();
        base.OnApplyTemplate(e);
        _headerContentContainer = e.NameScope.Find<Decorator>("PART_HeaderContent");
        _headerLayout           = e.NameScope.Find<Panel>("PART_HeaderLayout");
        _frame                  = e.NameScope.Find<Border>("PART_Frame");
        UpdateHeaderIconPresenter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeaderIconProperty)
        {
            UpdateHeaderIconPresenter();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = LayoutHelper.ArrangeChild(_frame, finalSize, default, BorderThickness);
        _borderBounds          = new Rect(finalSize);
        _headerOcclusionBounds = default;
        if (_headerContentContainer is not null)
        {
            var headerOffset = _headerContentContainer.TranslatePoint(new Point(0, 0), this) ?? default;
            var offsetY      = headerOffset.Y + _headerContentContainer.DesiredSize.Height / 2;
            _borderBounds          = new Rect(new Point(0, offsetY), new Size(finalSize.Width, finalSize.Height - offsetY));
            _headerOcclusionBounds = new Rect(headerOffset, _headerContentContainer.DesiredSize);
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
                BorderBrush);
        }
        {
            // 绘制遮挡
            if (_headerOcclusionBounds != default)
            {
                context.FillRectangle(Background ?? TransparentBackground, _headerOcclusionBounds);
            }
        }
    }

    private void UpdateHeaderIconPresenter()
    {
        if (HeaderIcon is null)
        {
            DetachHeaderIconPresenter();
            return;
        }

        if (_headerLayout is null)
        {
            return;
        }

        if (_headerIconPresenter is null)
        {
            _headerIconPresenter = new IconPresenter
            {
                Name = "PART_HeaderIconPresenter"
            };
            _headerIconPresenter.SetTemplatedParent(this);
            _headerLayout.Children.Insert(0, _headerIconPresenter);
        }

        _headerIconPresenter.SetCurrentValue(IconPresenter.IconProperty, HeaderIcon);
    }

    private void DetachHeaderIconPresenter()
    {
        if (_headerIconPresenter is null)
        {
            return;
        }

        if (_headerIconPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_headerIconPresenter);
        }
        else
        {
            _headerLayout?.Children.Remove(_headerIconPresenter);
        }

        _headerIconPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
        _headerIconPresenter.SetTemplatedParent(null);
        _headerIconPresenter = null;
    }
}
