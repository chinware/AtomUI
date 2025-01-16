using Avalonia;

namespace AtomUI.Controls.Utils;

public static class VisualExtensions
{
    internal static Point Translate(this Visual fromElement, Visual toElement, Point fromPoint)
    {
        if (fromElement == toElement)
        {
            return fromPoint;
        }

        var transform = fromElement.TransformToVisual(toElement);
        if (transform.HasValue)
        {
            return fromPoint.Transform(transform.Value);
        }

        return fromPoint;
    }
}