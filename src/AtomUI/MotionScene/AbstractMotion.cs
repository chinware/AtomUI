using System.Diagnostics;
using System.Reactive.Linq;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.MotionScene;

public enum TransitionKind
{
   Double,
   TransformOperations,
}

public record class MotionConfig
{
   public AvaloniaProperty Property { get; set; }
   public object? StartValue { get; set; }
   public object? EndValue { get; set; }
   public Easing MotionEasing { get; set; } = new LinearEasing();
   public TimeSpan MotionDuration { get; set; } = TimeSpan.FromMilliseconds(300);
   public TransitionKind TransitionKind { get; set; }

   public MotionConfig(AvaloniaProperty targetProperty, object? startValue = null, object? endValue = null)
   {
      Property = targetProperty;
      StartValue = startValue;
      EndValue = endValue;
   }
}

public abstract class AbstractMotion : AvaloniaObject, IMotion
{
   private bool _isRunning = false;
   public bool IsRunning => _isRunning;

   private Dictionary<AvaloniaProperty, MotionConfig> _motionConfigs;
   private List<ITransition> _transitions;
   public IObservable<bool>? CompletedObservable { get; private set; }

   // 定义我们目前支持的动效属性
   public static readonly StyledProperty<double> MotionOpacityProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();
   
   public static readonly StyledProperty<double> MotionWidthProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();
   
   public static readonly StyledProperty<double> MotionHeightProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();

   public static readonly StyledProperty<RelativePoint> MotionRenderTransformOriginProperty =
      Visual.RenderTransformOriginProperty.AddOwner<AbstractMotion>();

   public static readonly StyledProperty<ITransform?> MotionRenderTransformProperty =
      Visual.RenderTransformProperty.AddOwner<AbstractMotion>();

   protected double MotionOpacity
   {
      get => GetValue(MotionOpacityProperty);
      set => SetValue(MotionOpacityProperty, value);
   }
   
   protected double MotionWidth
   {
      get => GetValue(MotionWidthProperty);
      set => SetValue(MotionWidthProperty, value);
   }

   protected double MotionHeight
   {
      get => GetValue(MotionHeightProperty);
      set => SetValue(MotionHeightProperty, value);
   }

   protected RelativePoint MotionRenderTransformOrigin
   {
      get => GetValue(MotionRenderTransformOriginProperty);
      set => SetValue(MotionRenderTransformOriginProperty, value);
   }
   
   protected ITransform? MotionRenderTransform
   {
      get => GetValue(MotionRenderTransformProperty);
      set => SetValue(MotionRenderTransformProperty, value);
   }
   
   public AbstractMotion()
   {
      _motionConfigs = new Dictionary<AvaloniaProperty, MotionConfig>();
      _transitions = new List<ITransition>();
   }

   /// <summary>
   /// 创建动效动画对象
   /// </summary>
   /// <param name="motionTarget"></param>
   /// <returns></returns>
   internal List<ITransition> BuildTransitions(Control motionTarget)
   {
      foreach (var entry in _motionConfigs) {
         var config = entry.Value;
         NotifyPreBuildTransition(config, motionTarget);
         var transition = NotifyBuildTransition(config);
         _transitions.Add(transition);
        
      }
      var completedObservables = new IObservable<bool>[_transitions.Count];
      for (int i = 0; i < _transitions.Count; ++i) {
         var transition = _transitions[i];
         if (transition is INotifyTransitionCompleted notifyTransitionCompleted) {
            completedObservables[i] = (notifyTransitionCompleted.CompletedObservable);
         }
      }
      
      CompletedObservable = Observable.CombineLatest(completedObservables).Select(list =>
      {
         return list.All(v=> v);
      });
      return _transitions;
   }

   // 生命周期接口
   internal virtual void NotifyPreStart() {}
   internal virtual void NotifyStarted() {}
   internal virtual void NotifyCompleted() {}

   internal virtual void NotifyConfigMotionTarget(Control motionTarget) {}
   internal virtual void NotifyRestoreMotionTarget(Control motionTarget) {}
   
   protected virtual void NotifyPreBuildTransition(MotionConfig config, Control motionTarget) {}
   protected virtual ITransition NotifyBuildTransition(MotionConfig config)
   {
      TransitionBase transition = default!;
      if (config.TransitionKind == TransitionKind.Double) {
         transition = new NotifiableDoubleTransition();
      } else if (config.TransitionKind == TransitionKind.TransformOperations) {
         transition = new NotifiableTransformOperationsTransition();
      }

      transition.Property = config.Property;
      transition.Duration = config.MotionDuration;
      transition.Easing = config.MotionEasing;
      return transition;
   }

   protected MotionConfig? GetMotionConfig(AvaloniaProperty property)
   {
      if (_motionConfigs.TryGetValue(property, out var motionConfig)) {
         return motionConfig;
      }
      return null;
   }

   protected void AddMotionConfig(MotionConfig config)
   {
      Debug.Assert(!_motionConfigs.ContainsKey(config.Property));
      _motionConfigs.Add(config.Property, config);
   }

   /// <summary>
   /// 计算顶层动画渲染层的大小
   /// </summary>
   /// <param name="motionTargetSize">
   /// 动画目标控件的大小，如果动画直接调度到控件本身，则是控件本身的大小，如果是顶层动画渲染，那么就是 ghost
   /// 的大小，如果有阴影这个大小包含阴影的 thickness
   /// 目前的实现没有加一个固定的 Padding
   /// </param>
   /// <returns></returns>
   internal virtual Size CalculateSceneSize(Size motionTargetSize)
   {
      return motionTargetSize;
   }

   /// <summary>
   /// 计算动画层的全局坐标
   /// </summary>
   /// <param name="motionTargetSize">动画目标控件的大小，包含阴影</param>
   /// <param name="motionTargetPosition">动画目标控件的最终全局坐标位置</param>
   /// <returns></returns>
   internal virtual Point CalculateScenePosition(Size motionTargetSize, Point motionTargetPosition)
   {
      return motionTargetPosition;
   }

   public IList<AvaloniaProperty> GetActivatedProperties()
   {
      return _motionConfigs.Keys.ToList();
   }

   public IList<MotionConfig> GetMotionConfigs()
   {
      return _motionConfigs.Values.ToList();
   }
}