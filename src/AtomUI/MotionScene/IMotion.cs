using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public interface IMotion
{
   public bool IsRunning { get; }
   
   public List<ITransition> BuildTransitions(Control control);
   
   // 生命周期接口
   public void NotifyPreStart();
   public void NotifyStarted();
   public void NotifyStopped();

   public void NotifyConfigMotionTarget(Control motionTarget);
   public void NotifyRestoreMotionTarget(Control motionTarget);

   /// <summary>
   /// 计算顶层动画渲染层的大小
   /// </summary>
   /// <param name="motionTargetSize">
   /// 动画目标控件的大小，如果动画直接调度到控件本身，则是控件本身的大小，如果是顶层动画渲染，那么就是 ghost
   /// 的大小，如果有阴影这个大小包含阴影的 thickness
   /// 目前的实现没有加一个固定的 Padding
   /// </param>
   /// <returns></returns>
   public Size CalculateSceneSize(Size motionTargetSize);
   
   /// <summary>
   /// 计算动画层的全局坐标
   /// </summary>
   /// <param name="motionTargetSize">动画目标控件的大小，包含阴影</param>
   /// <param name="motionTargetPosition">动画目标控件的最终全局坐标位置</param>
   /// <returns></returns>
   public Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition);
}