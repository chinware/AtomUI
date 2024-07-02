using Avalonia.Animation;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public interface IMotion
{
   bool IsRunning { get; }
   
   List<ITransition> BuildTransitions(Control control);
   
   // 生命周期接口
   void NotifyPreStart();
   void NotifyStarted();
   void NotifyStopped();

   void NotifyConfigMotionTarget(Control motionTarget);
   void NotifyRestoreMotionTarget(Control motionTarget);
}