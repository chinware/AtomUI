using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Paints the Linux title-bar brush in the drawn-decoration overlay while
/// keeping it clipped to the visible window frame.
/// </summary>
internal sealed class WindowTitleBarShadowBackground : Control
{
    internal static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<WindowTitleBarShadowBackground, IBrush?>(nameof(Fill));

    internal static readonly StyledProperty<Thickness> ShadowThicknessProperty =
        AvaloniaProperty.Register<WindowTitleBarShadowBackground, Thickness>(nameof(ShadowThickness));

    internal static readonly StyledProperty<double> TitleBarHeightProperty =
        AvaloniaProperty.Register<WindowTitleBarShadowBackground, double>(nameof(TitleBarHeight));

    internal static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<WindowTitleBarShadowBackground, CornerRadius>(nameof(CornerRadius));

    internal static readonly StyledProperty<OsType> OsTypeProperty =
        AvaloniaProperty.Register<WindowTitleBarShadowBackground, OsType>(
            nameof(OsType),
            defaultValue: OsType.Unknown);

    internal IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    internal Thickness ShadowThickness
    {
        get => GetValue(ShadowThicknessProperty);
        set => SetValue(ShadowThicknessProperty, value);
    }

    internal double TitleBarHeight
    {
        get => GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    internal CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    internal OsType OsType
    {
        get => GetValue(OsTypeProperty);
        set => SetValue(OsTypeProperty, value);
    }

    static WindowTitleBarShadowBackground()
    {
        AffectsRender<WindowTitleBarShadowBackground>(
            FillProperty,
            OsTypeProperty);
        AffectsArrange<WindowTitleBarShadowBackground>(
            ShadowThicknessProperty,
            TitleBarHeightProperty,
            CornerRadiusProperty,
            OsTypeProperty);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangedSize = base.ArrangeOverride(finalSize);
        Clip = CreateTitleBarClip(finalSize);
        return arrangedSize;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (OsType != OsType.Linux ||
            Fill is null ||
            TitleBarHeight <= 0 ||
            Bounds.Width <= 0 ||
            Bounds.Height <= 0)
        {
            return;
        }

        var titleBarBounds = CalculateTitleBarBounds(Bounds.Size, ShadowThickness, TitleBarHeight);
        if (titleBarBounds.Width > 0 && titleBarBounds.Height > 0)
        {
            context.FillRectangle(Fill, titleBarBounds);
        }
    }

    private static Rect CalculateTitleBarBounds(
        Size surfaceSize,
        Thickness shadowThickness,
        double titleBarHeight)
    {
        if (surfaceSize.Width <= 0 || surfaceSize.Height <= 0 || titleBarHeight <= 0)
        {
            return default;
        }

        var frameBounds = WindowVisualLayerClip.CalculateClipBounds(surfaceSize, shadowThickness);
        if (frameBounds.Width <= 0 || frameBounds.Height <= 0)
        {
            return default;
        }

        return new Rect(
            frameBounds.Position,
            new Size(frameBounds.Width, Math.Min(frameBounds.Height, titleBarHeight)));
    }

    private Geometry? CreateTitleBarClip(Size surfaceSize)
    {
        if (OsType != OsType.Linux ||
            surfaceSize.Width <= 0 ||
            surfaceSize.Height <= 0)
        {
            return null;
        }

        var clipBounds = CalculateTitleBarBounds(surfaceSize, ShadowThickness, TitleBarHeight);
        if (clipBounds.Width <= 0 || clipBounds.Height <= 0)
        {
            return null;
        }

        var keypoints = RoundRectGeometryBuilder.CalculateRoundedCornersRectangleWinUI(
            clipBounds,
            default,
            CornerRadius,
            BackgroundSizing.OuterBorderEdge);
        var geometry = new StreamGeometry();
        using (var geometryContext = geometry.Open())
        {
            RoundRectGeometryBuilder.DrawRoundedCornersRectangle(geometryContext, ref keypoints);
        }

        return geometry;
    }
}
