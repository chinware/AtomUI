using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
        var clipBounds   = CalculateClipBounds(finalSize, ShadowThickness);
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

    internal static Rect CalculateClipBounds(Size surfaceSize, Thickness shadow)
    {
        return new Rect(
            shadow.Left,
            shadow.Top,
            Math.Max(0, surfaceSize.Width - shadow.Left - shadow.Right),
            Math.Max(0, surfaceSize.Height - shadow.Top - shadow.Bottom));
    }
}
