using AtomUI.Controls.Primitives;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

public class PopupMotionActor : MotionActor
{
   private BoxShadows _boxShadows;
   private PopupRoot _popupRoot;
   
   public PopupMotionActor(BoxShadows boxShadows, 
                           PopupRoot popupRoot, 
                           Control motionTarget, 
                           AbstractMotion motion)
      : base(motionTarget, motion)
   {
      _popupRoot = popupRoot;
      _boxShadows = boxShadows;
   }
   
   protected override Point CalculateTopLevelGhostPosition()
   {
      var boxShadowsThickness = _boxShadows.Thickness();
      var winPos = _popupRoot.PlatformImpl!.Position; // TODO 可能需要乘以 scaling
      var scaledThickness = boxShadowsThickness * _popupRoot.DesktopScaling;
      return new Point(winPos.X - scaledThickness.Left, winPos.Y - scaledThickness.Top);
   }

   protected override void BuildGhost()
   {
      if (_ghost is null) {
         _ghost = new MotionGhostControl(MotionTarget)
         {
            Shadows = _boxShadows,
            MaskCornerRadius = new CornerRadius(6)
         };
      }
   }
}