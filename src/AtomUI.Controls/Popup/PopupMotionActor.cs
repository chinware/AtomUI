using AtomUI.Controls.Primitives;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupMotionActor : MotionActor
{
   private BoxShadows _boxShadows;
   private PopupPositionInfo _popupPositionInfo;
   
   public PopupMotionActor(BoxShadows boxShadows, 
                           PopupPositionInfo popupPositionInfo, 
                           Control motionTarget, 
                           AbstractMotion motion)
      : base(motionTarget, motion)
   {
      _popupPositionInfo = popupPositionInfo;
      _boxShadows = boxShadows;
   }
   
   protected override Point CalculateTopLevelGhostPosition()
   {
      var boxShadowsThickness = _boxShadows.Thickness();
      var winPos = _popupPositionInfo.Offset; // TODO 可能需要乘以 scaling
      var scaledThickness = boxShadowsThickness * _popupPositionInfo.Scaling;
      return new Point(winPos.X - scaledThickness.Left, winPos.Y - scaledThickness.Top);
   }

   protected override void BuildGhost()
   {
      if (_ghost is null) {
         var ghostBrush = new VisualBrush
         {
            Visual = MotionTarget,
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
         };
         Size motionTargetSize;
         // Popup.Child can't be null here, it was set in ShowAtCore.
         if (MotionTarget.DesiredSize == default) {
            // Popup may not have been shown yet. Measure content
            motionTargetSize = LayoutHelper.MeasureChild(MotionTarget, Size.Infinity, new Thickness());
         } else {
            motionTargetSize = MotionTarget.DesiredSize;
         }
         _ghost = new MotionGhostControl(ghostBrush)
         {
            Shadows = _boxShadows,
            MaskContentSize = motionTargetSize
         };
      }
   }
}