using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal sealed class WindowVisualLayerClip : Decorator
{
    internal static readonly StyledProperty<Thickness> ShadowThicknessProperty =
        Window.FrameShadowThicknessProperty.AddOwner<WindowVisualLayerClip>();

    internal static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        TemplatedControl.CornerRadiusProperty.AddOwner<WindowVisualLayerClip>();

    internal Thickness ShadowThickness
    {
        get => GetValue(ShadowThicknessProperty);
        set => SetValue(ShadowThicknessProperty, value);
    }

    internal CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    static WindowVisualLayerClip()
    {
        AffectsArrange<WindowVisualLayerClip>(ShadowThicknessProperty, CornerRadiusProperty);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangedSize = base.ArrangeOverride(finalSize);
        var clipBounds = CalculatePixelAlignedClipBounds(
            finalSize,
            ShadowThickness,
            LayoutHelper.GetLayoutScale(this));
        if (clipBounds.Width <= 0 || clipBounds.Height <= 0)
        {
            Clip = null;
            return arrangedSize;
        }

        var keypoints = RoundRectGeometryBuilder.CalculateRoundedCornersRectangleWinUI(
            clipBounds,
            default,
            CornerRadius,
            BackgroundSizing.OuterBorderEdge);
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            RoundRectGeometryBuilder.DrawRoundedCornersRectangle(context, ref keypoints);
        }
        Clip = geometry;
        return arrangedSize;
    }

    /// <summary>
    /// Calculates the visible frame clip and aligns its edges to physical pixels.
    /// </summary>
    /// <remarks>
    /// The visual layer keeps the window's logical coordinate system, while its
    /// clip is rasterized at the current render scale.  Rounding the trailing
    /// edges up prevents a fractional right/bottom edge from leaving a partially
    /// covered physical pixel (which is visible as a one-pixel seam on Wayland).
    /// </remarks>
    internal static Rect CalculatePixelAlignedClipBounds(
        Size surfaceSize,
        Thickness shadow,
        double renderScaling)
    {
        var clipBounds = CalculateClipBounds(surfaceSize, shadow);
        if (clipBounds.Width <= 0 || clipBounds.Height <= 0 ||
            renderScaling <= 0 || double.IsNaN(renderScaling) || double.IsInfinity(renderScaling))
        {
            return clipBounds;
        }

        var left = LayoutHelper.RoundLayoutValue(clipBounds.Left, renderScaling);
        var top  = LayoutHelper.RoundLayoutValue(clipBounds.Top, renderScaling);

        // The compositor can expose the first physical pixel in the shadow
        // margin while an interactive resize commits the next buffer. Keep a
        // one-pixel bleed on the trailing edges so a full-window overlay can
        // cover that transition instead of revealing the transparent surface.
        var physicalPixel = 1 / renderScaling;
        var surfaceRight  = LayoutHelper.RoundLayoutValueUp(surfaceSize.Width, renderScaling);
        var surfaceBottom = LayoutHelper.RoundLayoutValueUp(surfaceSize.Height, renderScaling);
        var right = Math.Min(
            surfaceRight,
            LayoutHelper.RoundLayoutValueUp(clipBounds.Right + physicalPixel, renderScaling));
        var bottom = Math.Min(
            surfaceBottom,
            LayoutHelper.RoundLayoutValueUp(clipBounds.Bottom + physicalPixel, renderScaling));

        return new Rect(
            left,
            top,
            Math.Max(0, right - left),
            Math.Max(0, bottom - top));
    }

    /// <summary>
    /// Expands a logical extent to the smallest size that covers the complete
    /// physical render buffer allocated by Avalonia's Wayland backend.
    /// </summary>
    /// <remarks>
    /// Avalonia 12.1 allocates the render buffer with
    /// <c>PixelSize.FromSize(logicalSize, scaling)</c>.  A mask whose
    /// logical width is only rounded to an integer can therefore stop before the
    /// final physical buffer pixel (for example, 901.81 DIP at 1.666… scaling
    /// needs 1504 physical pixels, or 902.4 DIP).  Quantizing the mask upward in
    /// the same physical coordinate space keeps that pixel covered during a live
    /// resize.
    ///
    /// This is intentionally a geometry helper only; it must not be applied to the
    /// window root or to ordinary content layout.
    ///
    /// TODO(Avalonia upgrade): Re-check Avalonia 12.1's Wayland fractional-scale
    /// resize path and remove this mask-only workaround once buffer allocation
    /// and overlay composition use the same physical-pixel extent:
    /// https://github.com/AvaloniaUI/Avalonia/issues/11360
    /// </remarks>
    internal static Size CalculateWaylandMaskSize(Size logicalSize, double renderScaling)
    {
        if (logicalSize.Width <= 0 || logicalSize.Height <= 0 ||
            renderScaling <= 0 || double.IsNaN(renderScaling) || double.IsInfinity(renderScaling))
        {
            return logicalSize;
        }

        var physicalSize = PixelSize.FromSize(logicalSize, renderScaling);
        return new Size(
            physicalSize.Width / renderScaling,
            physicalSize.Height / renderScaling);
    }

    internal static Rect CalculateClipBounds(Size surfaceSize, Thickness shadow)
    {
        return new Rect(
            shadow.Left,
            shadow.Top,
            Math.Max(0, surfaceSize.Width - shadow.Left - shadow.Right),
            Math.Max(0, surfaceSize.Height - shadow.Top - shadow.Bottom));
    }
}
