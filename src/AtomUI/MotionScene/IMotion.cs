using Avalonia.Animation;

namespace AtomUI.MotionScene;

public interface IMotion
{
   MotionActor? Actor { get; }
   bool IsRunning { get; }
   
   List<ITransition> BuildTransitions();
   
   // 生命周期接口
   void NotifyPreStart();
   void NotifyStarted();
   void NotifyStopped();
}