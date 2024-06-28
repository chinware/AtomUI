using Avalonia.Animation;
using Avalonia.Controls;

namespace AtomUI.Media;

public interface IMotion
{
   IMotionAbilityTarget? MotionTarget { get; }
   bool IsRunning { get; }
   List<ITransition> BuildTransitions();
   
   // 生命周期接口
   void NotifyPreStart();
   void NotifyStarted();
   void NotifyStopped();
}