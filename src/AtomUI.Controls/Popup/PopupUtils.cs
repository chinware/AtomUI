using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Controls;

internal static class PopupUtils
{
   internal static ArrowPosition? CalculateArrowPosition(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (!CanEnabledArrow(placement, anchor, gravity)) {
         return null;
      }
      
      if (placement != PlacementMode.AnchorAndGravity) {
         var ret = Popup.GetAnchorAndGravity(placement);
         anchor = ret.Item1;
         gravity = ret.Item2;
      }

      ArrowPosition? arrowPosition;
      switch (anchor, gravity) {
         case (PopupAnchor.Bottom, PopupGravity.Bottom):
            arrowPosition = ArrowPosition.Top;
            break;
         case (PopupAnchor.Right, PopupGravity.Right):
            arrowPosition = ArrowPosition.Left;
            break;
         case (PopupAnchor.Left, PopupGravity.Left):
            arrowPosition = ArrowPosition.Right;
            break;
         case (PopupAnchor.Top, PopupGravity.Top):
            arrowPosition = ArrowPosition.Bottom;
            break;
         case (PopupAnchor.TopRight, PopupGravity.TopLeft):
            arrowPosition = ArrowPosition.BottomEdgeAlignedRight;
            break;
         case (PopupAnchor.TopLeft, PopupGravity.TopRight):
            arrowPosition = ArrowPosition.BottomEdgeAlignedLeft;
            break;
         case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
            arrowPosition = ArrowPosition.TopEdgeAlignedLeft;
            break;
         case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
            arrowPosition = ArrowPosition.TopEdgeAlignedRight;
            break;
         case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
            arrowPosition = ArrowPosition.RightEdgeAlignedTop;
            break;
         case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
            arrowPosition = ArrowPosition.RightEdgeAlignedBottom;
            break;
         case (PopupAnchor.TopRight, PopupGravity.BottomRight):
            arrowPosition = ArrowPosition.LeftEdgeAlignedTop;
            break;
         case (PopupAnchor.BottomRight, PopupGravity.TopRight):
            arrowPosition = ArrowPosition.LeftEdgeAlignedBottom;
            break;
         default:
            arrowPosition = null;
            break;
      }

      return arrowPosition;
   }
   
   /// <summary>
   /// 判断是否可以启用箭头，有些组合是不能启用箭头绘制的，因为没有意义
   /// </summary>
   /// <param name="placement"></param>
   /// <param name="anchor"></param>
   /// <param name="gravity"></param>
   /// <returns></returns>
   internal static bool CanEnabledArrow(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (placement == PlacementMode.Center ||
          placement == PlacementMode.Pointer) {
         return false;
      }

      return IsCanonicalAnchorType(placement, anchor, gravity);
   }
   
   /// <summary>
   /// 是否为标准的 anchor 类型
   /// </summary>
   /// <param name="placement"></param>
   /// <param name="anchor"></param>
   /// <param name="gravity"></param>
   /// <returns></returns>
   internal static bool IsCanonicalAnchorType(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (placement == PlacementMode.AnchorAndGravity) {
         switch (anchor, gravity) {
            case (PopupAnchor.Bottom, PopupGravity.Bottom):
            case (PopupAnchor.Right, PopupGravity.Right):
            case (PopupAnchor.Left, PopupGravity.Left):
            case (PopupAnchor.Top, PopupGravity.Top):
            case (PopupAnchor.TopRight, PopupGravity.TopLeft):
            case (PopupAnchor.TopLeft, PopupGravity.TopRight):
            case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
            case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
            case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
            case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
            case (PopupAnchor.TopRight, PopupGravity.BottomRight):
            case (PopupAnchor.BottomRight, PopupGravity.TopRight):
               break;
            default:
               return false;
         }
      }
      return true;
   }
}