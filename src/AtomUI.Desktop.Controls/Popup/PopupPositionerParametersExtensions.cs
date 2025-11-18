using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal static class PopupPositionerParametersExtensions
{
    public static void ConfigurePosition(ref this PopupPositionerParameters positionerParameters,
        TopLevel topLevel,
        Visual target, PlacementMode placement, Point offset,
        PopupAnchor anchor, PopupGravity gravity,
        PopupPositionerConstraintAdjustment constraintAdjustment, Rect? rect,
        FlowDirection flowDirection)
    {
        positionerParameters.Offset = offset;
        positionerParameters.ConstraintAdjustment = constraintAdjustment;
        if (placement == PlacementMode.Pointer)
        {
            // We need a better way for tracking the last pointer position
            topLevel.TryGetProperty<PixelPoint?>("LastPointerPosition", out var lastPointerPosition);
            var position = topLevel.PointToClient(lastPointerPosition ?? default);

            positionerParameters.AnchorRectangle = new Rect(position, new Size(1, 1));
            positionerParameters.Anchor = PopupAnchor.TopLeft;
            positionerParameters.Gravity = PopupGravity.BottomRight;
        }
        else
        {
            Matrix? matrix;
            if (TryGetAdorner(target, out var adorned, out var adornerLayer))
            {
                matrix = adorned!.TransformToVisual(topLevel) * target.TransformToVisual(adornerLayer!);
            }
            else
            {
                matrix = target.TransformToVisual(topLevel);
            }

            if (matrix == null)
            {
                if (target.GetVisualRoot() == null)
                {
                    throw new InvalidOperationException("Target control is not attached to the visual tree");
                }

                throw new InvalidOperationException("Target control is not in the same tree as the popup parent");
            }

            var bounds = new Rect(default, target.Bounds.Size);
            var anchorRect = rect ?? bounds;
            positionerParameters.AnchorRectangle = anchorRect.Intersect(bounds).TransformToAABB(matrix.Value);

            var parameters = placement switch
            {
                PlacementMode.Bottom => (PopupAnchor.Bottom, PopupGravity.Bottom),
                PlacementMode.Right => (PopupAnchor.Right, PopupGravity.Right),
                PlacementMode.Left => (PopupAnchor.Left, PopupGravity.Left),
                PlacementMode.Top => (PopupAnchor.Top, PopupGravity.Top),
                PlacementMode.Center => (PopupAnchor.None, PopupGravity.None),
                PlacementMode.AnchorAndGravity => (anchor, gravity),
                PlacementMode.TopEdgeAlignedRight => (PopupAnchor.TopRight, PopupGravity.TopLeft),
                PlacementMode.TopEdgeAlignedLeft => (PopupAnchor.TopLeft, PopupGravity.TopRight),
                PlacementMode.BottomEdgeAlignedLeft => (PopupAnchor.BottomLeft, PopupGravity.BottomRight),
                PlacementMode.BottomEdgeAlignedRight => (PopupAnchor.BottomRight, PopupGravity.BottomLeft),
                PlacementMode.LeftEdgeAlignedTop => (PopupAnchor.TopLeft, PopupGravity.BottomLeft),
                PlacementMode.LeftEdgeAlignedBottom => (PopupAnchor.BottomLeft, PopupGravity.TopLeft),
                PlacementMode.RightEdgeAlignedTop => (PopupAnchor.TopRight, PopupGravity.BottomRight),
                PlacementMode.RightEdgeAlignedBottom => (PopupAnchor.BottomRight, PopupGravity.TopRight),
                _ => throw new ArgumentOutOfRangeException(nameof(placement), placement,
                    "Invalid value for Popup.PlacementMode")
            };
            positionerParameters.Anchor = parameters.Item1;
            positionerParameters.Gravity = parameters.Item2;
        }

        // Invert coordinate system if FlowDirection is RTL
        if (flowDirection == FlowDirection.RightToLeft)
        {
            if ((positionerParameters.Anchor & PopupAnchor.Right) == PopupAnchor.Right)
            {
                positionerParameters.Anchor ^= PopupAnchor.Right;
                positionerParameters.Anchor |= PopupAnchor.Left;
            }
            else if ((positionerParameters.Anchor & PopupAnchor.Left) == PopupAnchor.Left)
            {
                positionerParameters.Anchor ^= PopupAnchor.Left;
                positionerParameters.Anchor |= PopupAnchor.Right;
            }

            if ((positionerParameters.Gravity & PopupGravity.Right) == PopupGravity.Right)
            {
                positionerParameters.Gravity ^= PopupGravity.Right;
                positionerParameters.Gravity |= PopupGravity.Left;
            }
            else if ((positionerParameters.Gravity & PopupGravity.Left) == PopupGravity.Left)
            {
                positionerParameters.Gravity ^= PopupGravity.Left;
                positionerParameters.Gravity |= PopupGravity.Right;
            }
        }
    }

    private static bool TryGetAdorner(Visual target, out Visual? adorned, out Visual? adornerLayer)
    {
        var element = target;
        while (element != null)
        {
            if (AdornerLayer.GetAdornedElement(element) is { } adornedElement)
            {
                adorned = adornedElement;
                adornerLayer = AdornerLayer.GetAdornerLayer(adorned);
                return true;
            }

            element = element.GetVisualParent();
        }

        adorned = null;
        adornerLayer = null;
        return false;
    }
}