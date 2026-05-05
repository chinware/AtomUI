namespace AtomUI.Controls;

internal static class ArrowPositionUtils
{
    public static ArrowPosition FlipArrowPosition(ArrowPosition arrowPosition, bool isHorizontalFlipped, bool isVerticalFlipped)
    {
        if (isHorizontalFlipped)
        {
            arrowPosition = arrowPosition switch
            {
                ArrowPosition.Top                 => ArrowPosition.Top,
                ArrowPosition.Bottom              => ArrowPosition.Bottom,
                ArrowPosition.Left                => ArrowPosition.Right,
                ArrowPosition.Right               => ArrowPosition.Left,
                ArrowPosition.TopEdgeAlignedLeft   => ArrowPosition.TopEdgeAlignedLeft,
                ArrowPosition.TopEdgeAlignedRight  => ArrowPosition.TopEdgeAlignedRight,
                ArrowPosition.BottomEdgeAlignedLeft  => ArrowPosition.BottomEdgeAlignedLeft,
                ArrowPosition.BottomEdgeAlignedRight => ArrowPosition.BottomEdgeAlignedRight,
                ArrowPosition.LeftEdgeAlignedTop     => ArrowPosition.RightEdgeAlignedTop,
                ArrowPosition.LeftEdgeAlignedBottom  => ArrowPosition.RightEdgeAlignedBottom,
                ArrowPosition.RightEdgeAlignedTop    => ArrowPosition.LeftEdgeAlignedTop,
                ArrowPosition.RightEdgeAlignedBottom => ArrowPosition.LeftEdgeAlignedBottom,
                _ => arrowPosition
            };
        }

        if (isVerticalFlipped)
        {
            arrowPosition = arrowPosition switch
            {
                ArrowPosition.Top                 => ArrowPosition.Bottom,
                ArrowPosition.Bottom              => ArrowPosition.Top,
                ArrowPosition.Left                => ArrowPosition.Left,
                ArrowPosition.Right               => ArrowPosition.Right,
                ArrowPosition.TopEdgeAlignedLeft   => ArrowPosition.BottomEdgeAlignedLeft,
                ArrowPosition.TopEdgeAlignedRight  => ArrowPosition.BottomEdgeAlignedRight,
                ArrowPosition.BottomEdgeAlignedLeft  => ArrowPosition.TopEdgeAlignedLeft,
                ArrowPosition.BottomEdgeAlignedRight => ArrowPosition.TopEdgeAlignedRight,
                ArrowPosition.LeftEdgeAlignedTop     => ArrowPosition.LeftEdgeAlignedTop,
                ArrowPosition.LeftEdgeAlignedBottom  => ArrowPosition.LeftEdgeAlignedBottom,
                ArrowPosition.RightEdgeAlignedTop    => ArrowPosition.RightEdgeAlignedTop,
                ArrowPosition.RightEdgeAlignedBottom => ArrowPosition.RightEdgeAlignedBottom,
                _ => arrowPosition
            };
        }
        return arrowPosition;
    }
}