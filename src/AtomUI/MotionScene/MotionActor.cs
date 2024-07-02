using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.MotionScene;

/// <summary>
/// 动效配置类，只要给 Director 提供动效相关信息
/// </summary>
public class MotionActor : Animatable, IMotionActor
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Finished;
   
   /// <summary>
   /// 动画实体
   /// </summary>
   public Control MotionTarget { get; set; }
   public IMotion Motion { get; }
   public bool DispatchInSceneLayer { get; set; } = true;
   
   private Control? _ghost;

   public bool IsSupportMotionProperty(AvaloniaProperty property)
   {
      if (property == AbstractMotion.MotionOpacityProperty ||
          property == AbstractMotion.MotionWidthProperty ||
          property == AbstractMotion.MotionHeightProperty ||
          property == AbstractMotion.MotionRenderTransformProperty) {
         return true;
      }
      return false;
   }

   public MotionActor(Control motionTarget, IMotion motion)
   {
      MotionTarget = motionTarget;
      Motion = motion;
   }
   
   public virtual Control BuildGhost()
   {
      return MotionTarget;
   }

   /// <summary>
   /// 当在 DispatchInSceneLayer 渲染的时候，Ghost 的全局坐标
   /// </summary>
   /// <returns></returns>
   public Point CalculateGhostPosition()
   {
      Point point = default;
      if (!DispatchInSceneLayer) {
         var visualParent = MotionTarget.GetVisualParent();
         if (visualParent is not null) {
            var parentPoint = MotionTarget.TranslatePoint(new Point(0, 0), visualParent);
            if (parentPoint.HasValue) {
               point = parentPoint.Value;
            } else {
               point = MotionTarget.Bounds.Position;
            }
         }
      } else {
         point = CalculateTopLevelGhostPosition();
      }
      return point;
   }

   protected virtual Point CalculateTopLevelGhostPosition()
   {
      return default;
   }

   public virtual void NotifySceneLayerCreated(SceneLayer sceneLayer)
   {
   }
}