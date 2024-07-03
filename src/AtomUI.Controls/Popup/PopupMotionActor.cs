using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class PopupMotionActor : MotionActor
{
   private Thickness _boxShadowsThickness;
   
   public PopupMotionActor(Thickness boxShadowsThickness, PopupRoot entity, AbstractMotion motion)
      : base(entity, motion)
   {
      _boxShadowsThickness = boxShadowsThickness;
   }
   
   protected override Point CalculateTopLevelGhostPosition()
   {
      var popup = (MotionTarget as PopupRoot)!;
      var winPos = popup.PlatformImpl!.Position; // TODO 可能需要乘以 scaling
      var scaledThickness = _boxShadowsThickness * popup.DesktopScaling;
      return new Point(winPos.X - scaledThickness.Left, winPos.Y - scaledThickness.Top);
   }
}