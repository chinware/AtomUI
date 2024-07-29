using Avalonia;

namespace AtomUI.MotionScene;

public interface IMotion
{
   public bool IsRunning { get; }

   /// <summary>
   /// 获取当前动效激活的动画属性列表
   /// </summary>
   /// <returns></returns>
   public IList<AvaloniaProperty> GetActivatedProperties();
   public IList<MotionConfig> GetMotionConfigs();
   public IObservable<bool>? CompletedObservable { get; }
}