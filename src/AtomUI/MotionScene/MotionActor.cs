using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

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
      return true;
   }

   public MotionActor(Control motionTarget, IMotion motion)
   {
      MotionTarget = motionTarget;
      Motion = motion;
   }
   
   public virtual Control BuildGhost()
   {
      return default!;
   }

   public virtual void NotifySceneLayerCreated(SceneLayer sceneLayer)
   {
   }
}