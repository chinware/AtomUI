using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

internal static class TransparentBgBrushUtils
{
    public static IBrush Build(double size, Color fillColor)
    {
        var destRect = new Rect(new Point(0, 0), new Size(size, size));
        return new DrawingBrush
        {
            TileMode        = TileMode.Tile,
            Stretch         = Stretch.None,
            AlignmentX      = AlignmentX.Left,
            AlignmentY      = AlignmentY.Top,
            DestinationRect = new RelativeRect(0, 0, size, size, RelativeUnit.Absolute),
            Drawing = new GeometryDrawing()
            {
                Brush = new ConicGradientBrush
                {
                    Center = RelativePoint.Center,
                    GradientStops = new GradientStops()
                    {
                        new GradientStop(fillColor, 0.00),
                        new GradientStop(fillColor, 0.25),
                        new GradientStop(Colors.Transparent, 0.25),
                        new GradientStop(Colors.Transparent, 0.50),
                        new GradientStop(fillColor, 0.50),
                        new GradientStop(fillColor, 0.75),
                        new GradientStop(Colors.Transparent, 0.75),
                        new GradientStop(Colors.Transparent, 1.00),
                    }
                },
                Geometry = new RectangleGeometry()
                {
                    Rect = destRect
                }
            }
        };
    }
}