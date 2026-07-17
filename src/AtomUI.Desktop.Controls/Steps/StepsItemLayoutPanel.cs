using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

internal class StepsItemLayoutPanel : Panel
{
    public static readonly StyledProperty<StepsType> TypeProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, StepsType>(nameof(Type), StepsType.Default);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<Orientation> TitlePlacementProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Orientation>(nameof(TitlePlacement), Orientation.Horizontal);

    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Thickness>(nameof(Padding));

    public static readonly StyledProperty<double> IndicatorSpacingProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(IndicatorSpacing));

    public static readonly StyledProperty<double> ConnectorThicknessProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(ConnectorThickness), 1d);

    public static readonly AttachedProperty<StepsItemLayoutRole> RoleProperty =
        AvaloniaProperty.RegisterAttached<StepsItemLayoutPanel, Control, StepsItemLayoutRole>("Role");

    public StepsType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public Orientation TitlePlacement
    {
        get => GetValue(TitlePlacementProperty);
        set => SetValue(TitlePlacementProperty, value);
    }

    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public double IndicatorSpacing
    {
        get => GetValue(IndicatorSpacingProperty);
        set => SetValue(IndicatorSpacingProperty, value);
    }

    public double ConnectorThickness
    {
        get => GetValue(ConnectorThicknessProperty);
        set => SetValue(ConnectorThicknessProperty, value);
    }

    public Orientation EffectiveTitlePlacement => ResolveTitlePlacement(Type, Orientation, TitlePlacement);

    static StepsItemLayoutPanel()
    {
        AffectsMeasure<StepsItemLayoutPanel>(
            TypeProperty,
            OrientationProperty,
            TitlePlacementProperty,
            PaddingProperty,
            IndicatorSpacingProperty,
            ConnectorThicknessProperty);
        AffectsArrange<StepsItemLayoutPanel>(
            TypeProperty,
            OrientationProperty,
            TitlePlacementProperty,
            PaddingProperty,
            IndicatorSpacingProperty,
            ConnectorThicknessProperty);
        AffectsParentMeasure<StepsItemLayoutPanel>(RoleProperty);
        AffectsParentArrange<StepsItemLayoutPanel>(RoleProperty);
    }

    public static void SetRole(Control element, StepsItemLayoutRole value)
    {
        element.SetValue(RoleProperty, value);
    }

    public static StepsItemLayoutRole GetRole(Control element)
    {
        return element.GetValue(RoleProperty);
    }

    internal static Orientation ResolveTitlePlacement(
        StepsType type,
        Orientation orientation,
        Orientation requested)
    {
        if (orientation == Orientation.Vertical)
        {
            return Orientation.Horizontal;
        }

        return type switch
        {
            StepsType.Dot => Orientation.Vertical,
            StepsType.Inline => Orientation.Vertical,
            StepsType.Navigation => Orientation.Horizontal,
            _ => requested
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var contentAvailableSize = Deflate(availableSize, Padding);
        foreach (var child in Children)
        {
            child.Measure(contentAvailableSize);
        }

        var indicator = FindChild(StepsItemLayoutRole.Indicator)?.DesiredSize ?? default;
        var header = FindChild(StepsItemLayoutRole.Header)?.DesiredSize ?? default;
        var subHeader = FindChild(StepsItemLayoutRole.SubHeader)?.DesiredSize ?? default;
        var content = FindChild(StepsItemLayoutRole.Content)?.DesiredSize ?? default;
        var connector = Type == StepsType.Navigation
            ? default
            : FindChild(StepsItemLayoutRole.Connector)?.DesiredSize ?? default;
        var arrow = Type == StepsType.Navigation
            ? FindChild(StepsItemLayoutRole.NavigationArrow)?.DesiredSize ?? default
            : default;

        if (EffectiveTitlePlacement == Orientation.Horizontal)
        {
            var headingHeight = Math.Max(indicator.Height, Math.Max(header.Height, subHeader.Height));
            var bodyWidth = Math.Max(header.Width + subHeader.Width, content.Width);
            var bodyHeight = headingHeight + content.Height;
            var indicatorSpacing = bodyWidth > 0 ? IndicatorSpacing : 0;

            if (Type == StepsType.Navigation)
            {
                return Inflate(
                    Orientation == Orientation.Vertical
                        ? new Size(indicator.Width + indicatorSpacing + bodyWidth, bodyHeight + arrow.Height)
                        : new Size(
                            indicator.Width + indicatorSpacing + bodyWidth + arrow.Width,
                            Math.Max(bodyHeight, arrow.Height)),
                    Padding);
            }

            return Inflate(
                Orientation == Orientation.Vertical
                    ? new Size(
                        indicator.Width + indicatorSpacing + bodyWidth,
                        Math.Max(bodyHeight, indicator.Height + connector.Height))
                    : new Size(
                        indicator.Width + indicatorSpacing + bodyWidth + connector.Width,
                        bodyHeight),
                Padding);
        }

        var verticalBodyWidth = Math.Max(header.Width, Math.Max(subHeader.Width, content.Width));
        var verticalBodyHeight = header.Height + subHeader.Height + content.Height;
        return Inflate(
            new Size(
                Math.Max(indicator.Width + connector.Width, verticalBodyWidth),
                indicator.Height + (verticalBodyHeight > 0 ? IndicatorSpacing : 0) + verticalBodyHeight),
            Padding);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var indicator = FindChild(StepsItemLayoutRole.Indicator);
        var header = FindChild(StepsItemLayoutRole.Header);
        var subHeader = FindChild(StepsItemLayoutRole.SubHeader);
        var connector = FindChild(StepsItemLayoutRole.Connector);
        var content = FindChild(StepsItemLayoutRole.Content);
        var arrow = FindChild(StepsItemLayoutRole.NavigationArrow);
        var navigationActiveIndicator = FindChild(StepsItemLayoutRole.NavigationActiveIndicator);

        var indicatorSize = indicator?.DesiredSize ?? default;
        var arrowWidth = Type == StepsType.Navigation ? arrow?.DesiredSize.Width ?? 0 : 0;
        var layoutRect = Deflate(new Rect(finalSize), Padding);

        if (EffectiveTitlePlacement == Orientation.Horizontal)
        {
            ArrangeHorizontalTitleLayout(
                layoutRect,
                indicator,
                header,
                subHeader,
                content,
                connector,
                arrow,
                indicatorSize,
                arrowWidth);
        }
        else
        {
            ArrangeVerticalTitleLayout(
                layoutRect,
                indicator,
                header,
                subHeader,
                content,
                connector,
                arrow,
                indicatorSize,
                arrowWidth);
        }

        ArrangeNavigationActiveIndicator(navigationActiveIndicator, finalSize);

        return finalSize;
    }

    private void ArrangeHorizontalTitleLayout(
        Rect layoutRect,
        Control? indicator,
        Control? header,
        Control? subHeader,
        Control? content,
        Control? connector,
        Control? arrow,
        Size indicatorSize,
        double arrowWidth)
    {
        var arrowHeight = Type == StepsType.Navigation ? arrow?.DesiredSize.Height ?? 0 : 0;
        var layoutWidth = Type == StepsType.Navigation && Orientation == Orientation.Horizontal
            ? Math.Max(0, layoutRect.Width - arrowWidth)
            : layoutRect.Width;
        var layoutHeight = Type == StepsType.Navigation && Orientation == Orientation.Vertical
            ? Math.Max(0, layoutRect.Height - arrowHeight)
            : layoutRect.Height;

        var headerSize = header?.DesiredSize ?? default;
        var subHeaderSize = subHeader?.DesiredSize ?? default;
        var contentSize = content?.DesiredSize ?? default;
        var headingHeight = Math.Max(indicatorSize.Height, Math.Max(headerSize.Height, subHeaderSize.Height));
        var desiredBodyWidth = Math.Max(headerSize.Width + subHeaderSize.Width, contentSize.Width);
        var indicatorSpacing = desiredBodyWidth > 0 ? IndicatorSpacing : 0;
        var groupWidth = Math.Min(layoutWidth, indicatorSize.Width + indicatorSpacing + desiredBodyWidth);
        var groupX = Type == StepsType.Navigation
            ? Math.Max(0, (layoutWidth - groupWidth) / 2)
            : 0;
        var bodyX = groupX + indicatorSize.Width + indicatorSpacing;
        var bodyWidth = Math.Min(desiredBodyWidth, Math.Max(0, layoutWidth - bodyX));

        Arrange(indicator, new Rect(
            layoutRect.X + groupX,
            layoutRect.Y + Math.Max(0, (headingHeight - indicatorSize.Height) / 2),
            indicatorSize.Width,
            indicatorSize.Height));

        var headerWidth = Math.Min(headerSize.Width, bodyWidth);
        Arrange(header, new Rect(
            layoutRect.X + bodyX,
            layoutRect.Y + Math.Max(0, (headingHeight - headerSize.Height) / 2),
            headerWidth,
            headerSize.Height));

        var subHeaderWidth = Math.Min(subHeaderSize.Width, Math.Max(0, bodyWidth - headerWidth));
        Arrange(subHeader, new Rect(
            layoutRect.X + bodyX + headerWidth,
            layoutRect.Y + Math.Max(0, (headingHeight - subHeaderSize.Height) / 2),
            subHeaderWidth,
            subHeaderSize.Height));

        Arrange(content, new Rect(
            layoutRect.X + bodyX,
            layoutRect.Y + headingHeight,
            bodyWidth,
            Math.Min(contentSize.Height, Math.Max(0, layoutHeight - headingHeight))));

        if (connector is not null && Type != StepsType.Navigation)
        {
            var connectorThickness = Math.Max(0, ConnectorThickness);
            connector.Arrange(Orientation == Orientation.Vertical
                ? new Rect(
                    layoutRect.X + groupX + Math.Max(0, (indicatorSize.Width - connectorThickness) / 2),
                    layoutRect.Y + headingHeight,
                    connectorThickness,
                    Math.Max(0, layoutHeight - headingHeight))
                : new Rect(
                    layoutRect.X + bodyX + bodyWidth,
                    layoutRect.Y + Math.Max(0, (headingHeight - connectorThickness) / 2),
                    Math.Max(0, layoutWidth - bodyX - bodyWidth),
                    connectorThickness));
        }
        else
        {
            connector?.Arrange(default);
        }

        ArrangeArrow(arrow, layoutRect, arrowWidth, arrowHeight);
    }

    private void ArrangeVerticalTitleLayout(
        Rect layoutRect,
        Control? indicator,
        Control? header,
        Control? subHeader,
        Control? content,
        Control? connector,
        Control? arrow,
        Size indicatorSize,
        double arrowWidth)
    {
        var indicatorX = Math.Max(0, (layoutRect.Width - indicatorSize.Width) / 2);
        Arrange(indicator, new Rect(
            layoutRect.X + indicatorX,
            layoutRect.Y,
            indicatorSize.Width,
            indicatorSize.Height));

        var bodyY = layoutRect.Y + indicatorSize.Height;
        if (header is not null || subHeader is not null || content is not null)
        {
            bodyY += IndicatorSpacing;
        }

        ArrangeVerticalBody(header, subHeader, content, layoutRect.X, layoutRect.Width, ref bodyY);
        var connectorThickness = Math.Max(0, ConnectorThickness);
        Arrange(connector, new Rect(
            layoutRect.X + indicatorX + indicatorSize.Width,
            layoutRect.Y + Math.Max(0, (indicatorSize.Height - connectorThickness) / 2),
            Math.Max(0, layoutRect.Width - indicatorX - indicatorSize.Width),
            connectorThickness));
        ArrangeArrow(arrow, layoutRect, arrowWidth, arrow?.DesiredSize.Height ?? 0);
    }

    private static void ArrangeVerticalBody(
        Control? header,
        Control? subHeader,
        Control? content,
        double x,
        double width,
        ref double y)
    {
        ArrangeBodyPart(header, x, ref y, width);
        ArrangeBodyPart(subHeader, x, ref y, width);
        ArrangeBodyPart(content, x, ref y, width);
    }

    private static void ArrangeBodyPart(Control? child, double x, ref double y, double width)
    {
        if (child is null)
        {
            return;
        }

        var height = child.DesiredSize.Height;
        child.Arrange(new Rect(x, y, width, height));
        y += height;
    }

    private void ArrangeArrow(Control? arrow, Rect layoutRect, double arrowWidth, double arrowHeight)
    {
        if (arrow is null)
        {
            return;
        }

        if (Type != StepsType.Navigation)
        {
            arrow.Arrange(default);
            return;
        }

        arrow.Arrange(Orientation == Orientation.Vertical
            ? new Rect(
                layoutRect.X + Math.Max(0, (layoutRect.Width - arrowWidth) / 2),
                layoutRect.Bottom - arrowHeight,
                arrowWidth,
                arrowHeight)
            : new Rect(
                layoutRect.Right - arrowWidth,
                layoutRect.Y + Math.Max(0, (layoutRect.Height - arrowHeight) / 2),
                arrowWidth,
                arrowHeight));
    }

    private void ArrangeNavigationActiveIndicator(Control? indicator, Size finalSize)
    {
        if (indicator is null || Type != StepsType.Navigation)
        {
            indicator?.Arrange(default);
            return;
        }

        var width = Math.Max(1, indicator.DesiredSize.Width);
        var height = Math.Max(1, indicator.DesiredSize.Height);
        indicator.Arrange(Orientation == Orientation.Horizontal
            ? new Rect(0, Math.Max(0, finalSize.Height - height), finalSize.Width, height)
            : new Rect(Math.Max(0, finalSize.Width - width), 0, width, finalSize.Height));
    }

    private static void Arrange(Control? control, Rect bounds)
    {
        control?.Arrange(bounds);
    }

    private static Size Deflate(Size size, Thickness padding)
    {
        return new Size(
            Math.Max(0, size.Width - padding.Left - padding.Right),
            Math.Max(0, size.Height - padding.Top - padding.Bottom));
    }

    private static Rect Deflate(Rect rect, Thickness padding)
    {
        return new Rect(
            rect.X + padding.Left,
            rect.Y + padding.Top,
            Math.Max(0, rect.Width - padding.Left - padding.Right),
            Math.Max(0, rect.Height - padding.Top - padding.Bottom));
    }

    private static Size Inflate(Size size, Thickness padding)
    {
        return new Size(
            size.Width + padding.Left + padding.Right,
            size.Height + padding.Top + padding.Bottom);
    }

    private Control? FindChild(StepsItemLayoutRole role)
    {
        return Children.FirstOrDefault(child => GetRole(child) == role && child.IsVisible);
    }
}
