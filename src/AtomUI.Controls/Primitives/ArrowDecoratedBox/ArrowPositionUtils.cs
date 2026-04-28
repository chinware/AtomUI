namespace AtomUI.Controls;

internal static class ArrowPositionUtils
{
    public static ArrowPosition FlipArrowPosition(ArrowPosition arrowPosition)
    {
        return arrowPosition switch
        {
            ArrowPosition.Top                 => ArrowPosition.Bottom,
            ArrowPosition.Bottom              => ArrowPosition.Top,
            ArrowPosition.Left                => ArrowPosition.Right,
            ArrowPosition.Right               => ArrowPosition.Left,
            ArrowPosition.TopEdgeAlignedLeft   => ArrowPosition.BottomEdgeAlignedLeft,
            ArrowPosition.TopEdgeAlignedRight  => ArrowPosition.BottomEdgeAlignedRight,
            ArrowPosition.BottomEdgeAlignedLeft  => ArrowPosition.TopEdgeAlignedLeft,
            ArrowPosition.BottomEdgeAlignedRight => ArrowPosition.TopEdgeAlignedRight,
            ArrowPosition.LeftEdgeAlignedTop     => ArrowPosition.RightEdgeAlignedTop,
            ArrowPosition.LeftEdgeAlignedBottom  => ArrowPosition.RightEdgeAlignedBottom,
            ArrowPosition.RightEdgeAlignedTop    => ArrowPosition.LeftEdgeAlignedTop,
            ArrowPosition.RightEdgeAlignedBottom => ArrowPosition.LeftEdgeAlignedBottom,
            _ => arrowPosition
        };
    }
}