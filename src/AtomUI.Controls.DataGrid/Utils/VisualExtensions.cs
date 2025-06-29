// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Avalonia;

namespace AtomUI.Controls.Utils;

internal static class VisualExtensions
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