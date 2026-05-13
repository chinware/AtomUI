using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.Primitives;

public class AdaptiveSpacingDockPanel : DockPanel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var parentWidth       = 0d;
        var parentHeight      = 0d;
        var accumulatedWidth  = 0d;
        var accumulatedHeight = 0d;

        var horizontalSpacing = false;
        var verticalSpacing   = false;
        var childrenCount     = LastChildFill ? Children.Count - 1 : Children.Count;

        for (var index = 0; index < childrenCount; ++index)
        {
            var child = Children[index];
            var childConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;

            switch (child.GetValue(DockProperty))
            {
                case Dock.Left:
                case Dock.Right:
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    if (child.IsVisible && childDesiredSize.Width > 0)
                    {
                        accumulatedWidth += HorizontalSpacing;
                        horizontalSpacing = true;
                    }
                    accumulatedWidth += childDesiredSize.Width;
                    break;

                case Dock.Top:
                case Dock.Bottom:
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    if (child.IsVisible && childDesiredSize.Height > 0)
                    {
                        accumulatedHeight += VerticalSpacing;
                        verticalSpacing = true;
                    }
                    accumulatedHeight += childDesiredSize.Height;
                    break;
            }
        }

        if (LastChildFill && Children.Count > 0)
        {
            var child = Children[Children.Count - 1];
            var childConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));

            child.Measure(childConstraint);
            var childDesiredSize = child.DesiredSize;
            parentHeight      = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
            parentWidth       = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
            accumulatedHeight += childDesiredSize.Height;
            accumulatedWidth  += childDesiredSize.Width;
        }
        else
        {
            if (horizontalSpacing)
            {
                accumulatedWidth -= HorizontalSpacing;
            }
            if (verticalSpacing)
            {
                accumulatedHeight -= VerticalSpacing;
            }
        }

        parentWidth  = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);
        return new Size(parentWidth, parentHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count is 0)
        {
            return finalSize;
        }

        var currentBounds = new Rect(finalSize);
        var childrenCount = LastChildFill ? Children.Count - 1 : Children.Count;

        for (var index = 0; index < childrenCount; ++index)
        {
            var child = Children[index];
            if (!child.IsVisible)
            {
                continue;
            }

            var    dock = child.GetValue(DockProperty);
            double width, height;
            switch (dock)
            {
                case Dock.Left:
                    width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                    child.Arrange(currentBounds.WithWidth(width));
                    if (width > 0)
                    {
                        width += HorizontalSpacing;
                    }
                    currentBounds = new Rect(currentBounds.X + width, currentBounds.Y,
                        Math.Max(0, currentBounds.Width - width), currentBounds.Height);
                    break;

                case Dock.Top:
                    height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                    child.Arrange(currentBounds.WithHeight(height));
                    if (height > 0)
                    {
                        height += VerticalSpacing;
                    }
                    currentBounds = new Rect(currentBounds.X, currentBounds.Y + height,
                        currentBounds.Width, Math.Max(0, currentBounds.Height - height));
                    break;

                case Dock.Right:
                    width = Math.Min(child.DesiredSize.Width, currentBounds.Width);
                    child.Arrange(new Rect(currentBounds.X + currentBounds.Width - width, currentBounds.Y,
                        width, currentBounds.Height));
                    if (width > 0)
                    {
                        width += HorizontalSpacing;
                    }
                    currentBounds = currentBounds.WithWidth(Math.Max(0, currentBounds.Width - width));
                    break;

                case Dock.Bottom:
                    height = Math.Min(child.DesiredSize.Height, currentBounds.Height);
                    child.Arrange(new Rect(currentBounds.X, currentBounds.Y + currentBounds.Height - height,
                        currentBounds.Width, height));
                    if (height > 0)
                    {
                        height += VerticalSpacing;
                    }
                    currentBounds = currentBounds.WithHeight(Math.Max(0, currentBounds.Height - height));
                    break;
            }
        }

        if (LastChildFill && Children.Count > 0)
        {
            var child = Children[Children.Count - 1];
            child.Arrange(new Rect(currentBounds.X, currentBounds.Y, currentBounds.Width, currentBounds.Height));
        }

        return finalSize;
    }
}
