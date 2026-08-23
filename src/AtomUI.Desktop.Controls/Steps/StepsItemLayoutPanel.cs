using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AvaloniaFlowDirection = Avalonia.Media.FlowDirection;

namespace AtomUI.Desktop.Controls;

internal class StepsItemLayoutPanel : Panel
{
    public static readonly StyledProperty<StepsType> TypeProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, StepsType>(nameof(Type), StepsType.Default);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<StepsPanelVariant> PanelVariantProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, StepsPanelVariant>(nameof(PanelVariant), StepsPanelVariant.Filled);

    public static readonly StyledProperty<Orientation> TitlePlacementProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Orientation>(nameof(TitlePlacement), Orientation.Horizontal);

    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, Thickness>(nameof(Padding));

    public static readonly StyledProperty<double> IndicatorSpacingProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(IndicatorSpacing));

    public static readonly StyledProperty<double> ConnectorThicknessProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(ConnectorThickness), 1d);

    public static readonly StyledProperty<double> HorizontalConnectorGapProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(HorizontalConnectorGap));

    public static readonly StyledProperty<double> VerticalConnectorMarginProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(VerticalConnectorMargin));

    public static readonly StyledProperty<double> VerticalItemPaddingProperty =
        AvaloniaProperty.Register<StepsItemLayoutPanel, double>(nameof(VerticalItemPadding));

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

    public StepsPanelVariant PanelVariant
    {
        get => GetValue(PanelVariantProperty);
        set => SetValue(PanelVariantProperty, value);
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

    public double HorizontalConnectorGap
    {
        get => GetValue(HorizontalConnectorGapProperty);
        set => SetValue(HorizontalConnectorGapProperty, value);
    }

    public double VerticalConnectorMargin
    {
        get => GetValue(VerticalConnectorMarginProperty);
        set => SetValue(VerticalConnectorMarginProperty, value);
    }

    public double VerticalItemPadding
    {
        get => GetValue(VerticalItemPaddingProperty);
        set => SetValue(VerticalItemPaddingProperty, value);
    }

    public Orientation EffectiveTitlePlacement => ResolveTitlePlacement(Type, EffectiveOrientation, TitlePlacement);

    private Orientation EffectiveOrientation => Type == StepsType.Panel ? Orientation.Horizontal : Orientation;

    static StepsItemLayoutPanel()
    {
        AffectsMeasure<StepsItemLayoutPanel>(
            TypeProperty,
            OrientationProperty,
            TitlePlacementProperty,
            PaddingProperty,
            IndicatorSpacingProperty,
            ConnectorThicknessProperty,
            HorizontalConnectorGapProperty,
            VerticalConnectorMarginProperty,
            VerticalItemPaddingProperty);
        AffectsArrange<StepsItemLayoutPanel>(
            TypeProperty,
            OrientationProperty,
            PanelVariantProperty,
            TitlePlacementProperty,
            PaddingProperty,
            IndicatorSpacingProperty,
            ConnectorThicknessProperty,
            HorizontalConnectorGapProperty,
            VerticalConnectorMarginProperty,
            VerticalItemPaddingProperty);
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
            StepsType.OutlineDot => Orientation.Vertical,
            StepsType.Inline => Orientation.Vertical,
            StepsType.Navigation => Orientation.Horizontal,
            StepsType.Panel => Orientation.Horizontal,
            _ => requested
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var contentAvailableSize = Deflate(availableSize, Padding);
        var panelLeadingInset = GetPanelLeadingInset();
        if (panelLeadingInset > 0)
        {
            contentAvailableSize = new Size(
                Math.Max(0, contentAvailableSize.Width - panelLeadingInset),
                contentAvailableSize.Height);
        }

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
        if (EffectiveTitlePlacement == Orientation.Horizontal)
        {
            var headingHeight = Math.Max(indicator.Height, Math.Max(header.Height, subHeader.Height));
            var bodyWidth = Math.Max(header.Width + subHeader.Width, content.Width);
            var bodyHeight = headingHeight + content.Height;
            var indicatorSpacing = bodyWidth > 0 ? IndicatorSpacing : 0;

            if (Type == StepsType.Navigation)
            {
                return Inflate(
                    new Size(indicator.Width + indicatorSpacing + bodyWidth, bodyHeight),
                    Padding);
            }

            var desiredSize = EffectiveOrientation == Orientation.Vertical
                ? new Size(
                    indicator.Width + indicatorSpacing + bodyWidth,
                    Math.Max(bodyHeight, indicator.Height + connector.Height) + Math.Max(0, VerticalItemPadding))
                : new Size(
                    indicator.Width + indicatorSpacing + bodyWidth + connector.Width,
                    bodyHeight);
            if (Type == StepsType.Panel)
            {
                desiredSize = new Size(desiredSize.Width + panelLeadingInset, desiredSize.Height);
            }

            return Inflate(desiredSize, Padding);
        }

        var verticalBodyWidth = Math.Max(header.Width, Math.Max(subHeader.Width, content.Width));
        var verticalBodyHeight = header.Height + subHeader.Height + content.Height;
        var verticalTitleWidth = EffectiveOrientation == Orientation.Horizontal
            ? Math.Max(indicator.Width, verticalBodyWidth)
            : Math.Max(indicator.Width + connector.Width, verticalBodyWidth);
        return Inflate(
            new Size(
                verticalTitleWidth,
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
        var panelArrow = FindChild(StepsItemLayoutRole.PanelArrow, includeInvisible: true);
        var navigationActiveIndicator = FindChild(StepsItemLayoutRole.NavigationActiveIndicator);
        var itemWrapper = FindChild(StepsItemLayoutRole.ItemWrapper);

        var indicatorSize = indicator?.DesiredSize ?? default;
        var itemRect = new Rect(finalSize);
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
                panelArrow,
                itemWrapper,
                indicatorSize,
                itemRect);
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
                panelArrow,
                itemWrapper,
                indicatorSize,
                itemRect);
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
        Control? panelArrow,
        Control? itemWrapper,
        Size indicatorSize,
        Rect itemRect)
    {
        var layoutWidth  = layoutRect.Width;
        var layoutHeight = layoutRect.Height;

        var headerSize = header?.DesiredSize ?? default;
        var subHeaderSize = subHeader?.DesiredSize ?? default;
        var contentSize = content?.DesiredSize ?? default;
        var headingHeight = Math.Max(indicatorSize.Height, Math.Max(headerSize.Height, subHeaderSize.Height));
        var desiredBodyWidth = Math.Max(headerSize.Width + subHeaderSize.Width, contentSize.Width);
        var indicatorSpacing = desiredBodyWidth > 0 ? IndicatorSpacing : 0;
        var groupWidth = Math.Min(layoutWidth, indicatorSize.Width + indicatorSpacing + desiredBodyWidth);
        var groupX = Type == StepsType.Navigation && EffectiveOrientation == Orientation.Horizontal
            ? Math.Max(0, (layoutWidth - groupWidth) / 2)
            : 0;
        var bodyStart = groupX + indicatorSize.Width + indicatorSpacing;
        var panelLeadingInset = GetPanelLeadingInset(panelArrow);
        var bodyWidth = Math.Min(
            desiredBodyWidth,
            Math.Max(0, layoutWidth - bodyStart - panelLeadingInset));
        var bodyX = Type == StepsType.Panel && FlowDirection == AvaloniaFlowDirection.RightToLeft
            ? Math.Max(0, layoutWidth - bodyWidth - panelLeadingInset)
            : bodyStart + panelLeadingInset;

        var indicatorRect = new Rect(
            layoutRect.X + groupX,
            layoutRect.Y + Math.Max(0, (headingHeight - indicatorSize.Height) / 2),
            indicatorSize.Width,
            indicatorSize.Height);
        Arrange(indicator, indicatorRect);
        var arrangedIndicatorBounds = GetArrangedBounds(indicator, indicatorRect);

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

        var contentWidth = Math.Min(contentSize.Width, bodyWidth);
        Arrange(content, new Rect(
            layoutRect.X + bodyX,
            layoutRect.Y + headingHeight,
            contentWidth,
            Math.Min(contentSize.Height, Math.Max(0, layoutHeight - headingHeight))));

        if (connector is not null && Type != StepsType.Navigation)
        {
            var connectorThickness = Math.Max(0, ConnectorThickness);
            var connectorX = Math.Max(
                layoutRect.X + bodyX,
                Math.Max(header?.Bounds.Right ?? 0, subHeader?.Bounds.Right ?? 0));
            connector.Arrange(EffectiveOrientation == Orientation.Vertical
                ? new Rect(
                    arrangedIndicatorBounds.Center.X - connectorThickness / 2,
                    arrangedIndicatorBounds.Bottom + Math.Max(0, VerticalConnectorMargin),
                    connectorThickness,
                    GetVerticalConnectorHeight(layoutRect, headingHeight, indicatorSize.Height, arrangedIndicatorBounds))
                : new Rect(
                    connectorX,
                    arrangedIndicatorBounds.Center.Y - connectorThickness / 2,
                    Math.Max(0, layoutRect.Right - connectorX),
                    connectorThickness));
        }
        else
        {
            connector?.Arrange(default);
        }

        ArrangeItemWrapper(itemWrapper, itemRect, Padding, indicator, header, subHeader, content);
        ArrangeArrow(arrow, panelArrow, itemRect);
    }

    private void ArrangeVerticalTitleLayout(
        Rect layoutRect,
        Control? indicator,
        Control? header,
        Control? subHeader,
        Control? content,
        Control? connector,
        Control? arrow,
        Control? panelArrow,
        Control? itemWrapper,
        Size indicatorSize,
        Rect itemRect)
    {
        if (EffectiveOrientation == Orientation.Horizontal)
        {
            ArrangeCenteredHorizontalTitleVerticalLayout(
                layoutRect,
                indicator,
                header,
                subHeader,
                content,
                connector,
                arrow,
                panelArrow,
                itemWrapper,
                indicatorSize,
                itemRect);
            return;
        }

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
        ArrangeItemWrapper(itemWrapper, itemRect, Padding, indicator, header, subHeader, content);
        ArrangeArrow(arrow, panelArrow, itemRect);
    }

    private void ArrangeCenteredHorizontalTitleVerticalLayout(
        Rect layoutRect,
        Control? indicator,
        Control? header,
        Control? subHeader,
        Control? content,
        Control? connector,
        Control? arrow,
        Control? panelArrow,
        Control? itemWrapper,
        Size indicatorSize,
        Rect itemRect)
    {
        var indicatorSlot = new Rect(
            layoutRect.X + Math.Max(0, (layoutRect.Width - indicatorSize.Width) / 2),
            layoutRect.Y,
            indicatorSize.Width,
            indicatorSize.Height);
        Arrange(indicator, indicatorSlot);

        var indicatorBounds = GetArrangedBounds(indicator, indicatorSlot);
        var bodyY           = layoutRect.Y + indicatorSize.Height;
        if (header is not null || subHeader is not null || content is not null)
        {
            bodyY += IndicatorSpacing;
        }

        ArrangeVerticalBodyCentered(
            header,
            subHeader,
            content,
            layoutRect,
            indicatorBounds.Center.X,
            ref bodyY);

        var connectorThickness = Math.Max(0, ConnectorThickness);
        var connectorGap       = Math.Max(0, HorizontalConnectorGap);
        var connectorWidth = Type == StepsType.Inline
            ? itemRect.Width - indicatorBounds.Width - connectorGap * 2
            : layoutRect.Width - indicatorBounds.Width - connectorGap * 2;
        Arrange(connector, new Rect(
            indicatorBounds.Right + connectorGap,
            indicatorBounds.Center.Y - connectorThickness / 2,
            Math.Max(0, connectorWidth),
            connectorThickness));
        ArrangeItemWrapper(itemWrapper, itemRect, Padding, indicator, header, subHeader, content);
        ArrangeArrow(arrow, panelArrow, itemRect);
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

    private static void ArrangeVerticalBodyCentered(
        Control? header,
        Control? subHeader,
        Control? content,
        Rect layoutRect,
        double centerX,
        ref double y)
    {
        ArrangeBodyPartCentered(header, layoutRect, centerX, ref y);
        ArrangeBodyPartCentered(subHeader, layoutRect, centerX, ref y);
        ArrangeBodyPartCentered(content, layoutRect, centerX, ref y);
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

    private static void ArrangeBodyPartCentered(Control? child, Rect layoutRect, double centerX, ref double y)
    {
        if (child is null)
        {
            return;
        }

        var width  = Math.Min(child.DesiredSize.Width, layoutRect.Width);
        var height = child.DesiredSize.Height;
        var x      = Math.Clamp(centerX - width / 2, layoutRect.X, layoutRect.Right - width);
        child.Arrange(new Rect(x, y, width, height));
        y += height;
    }

    private void ArrangeItemWrapper(
        Control? wrapper,
        Rect itemRect,
        Thickness padding,
        params Control?[] wrappedChildren)
    {
        if (Type == StepsType.Inline)
        {
            ArrangeInlineItemWrapper(wrapper, itemRect);
            return;
        }

        if (Type == StepsType.Panel)
        {
            wrapper?.Arrange(itemRect);
            return;
        }

        ArrangeContentItemWrapper(wrapper, itemRect, padding, wrappedChildren);
    }

    private static void ArrangeInlineItemWrapper(Control? wrapper, Rect itemRect)
    {
        wrapper?.Arrange(itemRect);
    }

    private static void ArrangeContentItemWrapper(
        Control? wrapper,
        Rect itemRect,
        Thickness padding,
        params Control?[] wrappedChildren)
    {
        if (wrapper is null)
        {
            return;
        }

        Rect? contentBounds = null;
        foreach (var child in wrappedChildren)
        {
            if (child is null || !child.IsVisible)
            {
                continue;
            }

            var bounds = child.Bounds;
            if (bounds.Width <= 0 && bounds.Height <= 0)
            {
                continue;
            }

            contentBounds = contentBounds is null
                ? bounds
                : contentBounds.Value.Union(bounds);
        }

        if (contentBounds is null)
        {
            wrapper.Arrange(default);
            return;
        }

        var x = Math.Max(itemRect.Left, contentBounds.Value.Left - padding.Left);
        var y = Math.Max(itemRect.Top, contentBounds.Value.Top - padding.Top);
        var right = Math.Min(itemRect.Right, contentBounds.Value.Right + padding.Right);
        var bottom = Math.Min(itemRect.Bottom, contentBounds.Value.Bottom + padding.Bottom);
        wrapper.Arrange(new Rect(x, y, Math.Max(0, right - x), Math.Max(0, bottom - y)));
    }

    private void ArrangeArrow(Control? arrow, Control? panelArrow, Rect itemRect)
    {
        if (Type == StepsType.Panel)
        {
            arrow?.Arrange(default);
            ArrangePanelArrow(panelArrow, itemRect);
            return;
        }

        panelArrow?.Arrange(default);
        if (arrow is null)
        {
            return;
        }

        if (Type != StepsType.Navigation)
        {
            arrow.Arrange(default);
            return;
        }

        var arrowSize = GetNavigationArrowSize(arrow);
        arrow.Arrange(EffectiveOrientation == Orientation.Vertical
            ? new Rect(
                itemRect.X + Math.Max(0, (itemRect.Width - arrowSize.Width) / 2),
                itemRect.Bottom - arrowSize.Height / 2,
                arrowSize.Width,
                arrowSize.Height)
            : new Rect(
                itemRect.Right - arrowSize.Width / 2,
                itemRect.Y + Math.Max(0, (itemRect.Height - arrowSize.Height) / 2),
                arrowSize.Width,
                arrowSize.Height));
    }

    private void ArrangePanelArrow(Control? arrow, Rect itemRect)
    {
        if (arrow is null)
        {
            return;
        }

        var width = Math.Max(0, arrow.DesiredSize.Width);
        var stroke = arrow is StepsPanelArrow panelArrow
            ? Math.Max(0, panelArrow.StrokeThickness)
            : 0;
        var outlined = PanelVariant == StepsPanelVariant.Outlined;
        var verticalInset = outlined ? stroke / 2 : 0;
        var height = Math.Max(0, itemRect.Height - verticalInset * 2);
        if (FlowDirection == AvaloniaFlowDirection.RightToLeft)
        {
            arrow.Arrange(new Rect(
                itemRect.Left - width,
                itemRect.Top + verticalInset,
                width,
                height));
        }
        else
        {
            arrow.Arrange(new Rect(
                itemRect.Right,
                itemRect.Top + verticalInset,
                width,
                height));
        }
    }

    private Size GetNavigationArrowSize(Control arrow)
    {
        var size = arrow.DesiredSize;
        if (EffectiveOrientation == Orientation.Vertical)
        {
            return new Size(size.Width * 2 / 3, size.Height * 2 / 3);
        }

        return size;
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
        indicator.Arrange(EffectiveOrientation == Orientation.Horizontal
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

    private static Rect GetArrangedBounds(Control? control, Rect fallback)
    {
        if (control is null)
        {
            return fallback;
        }

        var bounds = control.Bounds;
        return bounds.Width > 0 || bounds.Height > 0 ? bounds : fallback;
    }

    private double GetPanelLeadingInset(Control? panelArrow = null)
    {
        if (Type != StepsType.Panel ||
            TemplatedParent is not StepsItem item ||
            item.IsFirst)
        {
            return 0;
        }

        panelArrow ??= FindChild(StepsItemLayoutRole.PanelArrow, includeInvisible: true);
        if (panelArrow is null)
        {
            return 0;
        }

        var width = panelArrow.Width;
        return !double.IsNaN(width) && width > 0
            ? width
            : Math.Max(0, panelArrow.DesiredSize.Width);
    }

    private double GetVerticalConnectorHeight(
        Rect layoutRect,
        double headingHeight,
        double indicatorHeight,
        Rect indicatorBounds)
    {
        var connectorMargin = Math.Max(0, VerticalConnectorMargin);
        var railOffset      = Math.Max(0, (headingHeight - indicatorHeight) / 2);
        var connectorTop    = indicatorBounds.Bottom + connectorMargin;
        var connectorBottom = layoutRect.Bottom - connectorMargin + railOffset;
        return Math.Max(0, connectorBottom - connectorTop);
    }

    private Control? FindChild(StepsItemLayoutRole role, bool includeInvisible = false)
    {
        return Children.FirstOrDefault(child =>
            GetRole(child) == role &&
            (includeInvisible || child.IsVisible));
    }
}
