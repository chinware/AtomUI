using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Controls.Utils;

/// <summary>
/// Contains helper methods for rendering a <see cref="Border" />'s background and border to a given context.
/// </summary>
internal class BorderRenderHelper
{
    private Geometry? _backgroundGeometryCache;
    private BackgroundSizing _backgroundSizing;
    private Geometry? _borderGeometryCache;
    private Thickness _borderThickness;
    private IPen? _cachedPen;
    private CornerRadius _cornerRadius;
    private bool _initialized;
    private Size _size;
    private bool _useComplexRendering;

    private void Update(Size finalSize, Thickness borderThickness, CornerRadius cornerRadius,
                        BackgroundSizing backgroundSizing)
    {
        _size             = finalSize;
        _borderThickness  = borderThickness;
        _cornerRadius     = cornerRadius;
        _backgroundSizing = backgroundSizing;
        _initialized      = true;

        if (borderThickness.IsUniform &&
            cornerRadius.IsUniform &&
            backgroundSizing == BackgroundSizing.CenterBorder)
        {
            _backgroundGeometryCache = null;
            _borderGeometryCache     = null;
            _useComplexRendering     = false;
        }
        else
        {
            _useComplexRendering = true;

            var boundRect = new Rect(finalSize);
            var innerRect = boundRect.Deflate(borderThickness);

            if (innerRect.Width != 0 && innerRect.Height != 0)
            {
                var backgroundOuterKeypoints = RoundRectGeometryBuilder.CalculateRoundedCornersRectangleWinUI(
                    boundRect,
                    borderThickness,
                    cornerRadius,
                    backgroundSizing);

                var backgroundGeometry = new StreamGeometry();
                using (var ctx = backgroundGeometry.Open())
                {
                    RoundRectGeometryBuilder.DrawRoundedCornersRectangle(ctx, ref backgroundOuterKeypoints);
                }

                _backgroundGeometryCache = backgroundGeometry;
            }
            else
            {
                _backgroundGeometryCache = null;
            }

            if (boundRect.Width != 0 && boundRect.Height != 0)
            {
                var borderInnerKeypoints = RoundRectGeometryBuilder.CalculateRoundedCornersRectangleWinUI(
                    boundRect,
                    borderThickness,
                    cornerRadius,
                    BackgroundSizing.InnerBorderEdge);
                var borderOuterKeypoints = RoundRectGeometryBuilder.CalculateRoundedCornersRectangleWinUI(
                    boundRect,
                    borderThickness,
                    cornerRadius,
                    BackgroundSizing.OuterBorderEdge);

                var borderInnerGeometry = new StreamGeometry();
                using (var ctx = borderInnerGeometry.Open())
                {
                    RoundRectGeometryBuilder.DrawRoundedCornersRectangle(ctx, ref borderInnerKeypoints);
                }

                var borderOuterGeometry = new StreamGeometry();
                using (var ctx = borderOuterGeometry.Open())
                {
                    RoundRectGeometryBuilder.DrawRoundedCornersRectangle(ctx, ref borderOuterKeypoints);
                }

                _borderGeometryCache =
                    new CombinedGeometry(GeometryCombineMode.Exclude, borderOuterGeometry, borderInnerGeometry);
            }
            else
            {
                _borderGeometryCache = null;
            }
        }
    }

    public void Render(
        DrawingContext context,
        Size finalSize,
        Thickness borderThickness,
        CornerRadius cornerRadius,
        BackgroundSizing backgroundSizing,
        IBrush? background,
        IBrush? borderBrush,
        BoxShadows boxShadows)
    {
        if (_size != finalSize
            || _borderThickness != borderThickness
            || _cornerRadius != cornerRadius
            || _backgroundSizing != backgroundSizing
            || !_initialized)
        {
            Update(finalSize, borderThickness, cornerRadius, backgroundSizing);
        }

        if (_useComplexRendering)
        {
            if (_backgroundGeometryCache != null)
            {
                context.DrawGeometry(background, null, _backgroundGeometryCache);
            }

            if (_borderGeometryCache != null)
            {
                context.DrawGeometry(borderBrush, null, _borderGeometryCache);
            }
        }
        else
        {
            var thickness = _borderThickness.Top;

            PenUtils.TryModifyOrCreate(ref _cachedPen, borderBrush, thickness);

            var rect = new Rect(_size);
            if (!MathUtilities.IsZero(thickness))
            {
                rect = rect.Deflate(thickness * 0.5);
            }

            var rrect = new RoundedRect(rect, _cornerRadius.TopLeft, _cornerRadius.TopRight,
                _cornerRadius.BottomRight, _cornerRadius.BottomLeft);

            context.DrawRectangle(background, _cachedPen, rrect, boxShadows);
        }
    }
}