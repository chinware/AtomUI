using Avalonia;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class MotionActor : AvaloniaObject
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Finished;
   
   /// <summary>
   /// 动画实体
   /// </summary>
   public Control Entity { get; set; }
   public IMotion? Motion { get; set; }
   public bool DispatchInGlobalScene { get; set; } = false;

   public bool IsSupportMotionProperty(AvaloniaProperty property)
   {
      return true;
   }

   public MotionActor(Control entity)
   {
      Entity = entity;
   }
}