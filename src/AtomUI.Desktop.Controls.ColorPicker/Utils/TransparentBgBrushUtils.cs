using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.Primitives;

internal static class TransparentBgBrushUtils
{
    private const int MaxCachedBrushes = 32;
    private static readonly object CacheLock = new();
    private static readonly Dictionary<CacheKey, IBrush> BrushCache = [];
    private static readonly Queue<CacheKey> BrushCacheOrder = [];

    public static IBrush Build(double size, Color fillColor)
    {
        var key = new CacheKey(size, fillColor);
        lock (CacheLock)
        {
            if (BrushCache.TryGetValue(key, out var brush))
            {
                return brush;
            }

            brush = Create(size, fillColor);
            BrushCache.Add(key, brush);
            BrushCacheOrder.Enqueue(key);
            if (BrushCache.Count > MaxCachedBrushes)
            {
                var oldestKey = BrushCacheOrder.Dequeue();
                BrushCache.Remove(oldestKey);
            }
            return brush;
        }
    }

    private static IBrush Create(double size, Color fillColor)
    {
        var destRect = new Rect(new Point(0, 0), new Size(size, size));
        return new DrawingBrush
        {
            TileMode = TileMode.Tile,
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
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

    private readonly record struct CacheKey(double Size, Color FillColor);
}
