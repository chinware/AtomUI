using System.Diagnostics;
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

   // 定义我们目前支持的动效属性
   public static readonly StyledProperty<double> MotionOpacityProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();
   
   public static readonly StyledProperty<double> MotionWidthProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();
   
   public static readonly StyledProperty<double> MotionHeightProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();

   public static readonly StyledProperty<RelativePoint> MotionRenderTransformOriginProperty =
      AvaloniaProperty.Register<AbstractMotion, RelativePoint>(nameof(MotionRenderTransformOrigin));
   
   public static readonly StyledProperty<ITransform> MotionRenderTransformProperty =
      AvaloniaProperty.Register<AbstractMotion, ITransform>(nameof(MotionRenderTransform));

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
   
   protected ITransform MotionRenderTransform
   {
      get => GetValue(MotionRenderTransformProperty);
      set => SetValue(MotionRenderTransformProperty, value);
   }
   
   public AbstractMotion()
   {
      _motionConfigs = new Dictionary<AvaloniaProperty, MotionConfig>();
      _transitions = new List<ITransition>();
   }

   public List<ITransition> BuildTransitions(Control motionTarget)
   {
      foreach (var entry in _motionConfigs) {
         var config = entry.Value;
         NotifyPreBuildTransition(config, motionTarget);
         var transition = NotifyBuildTransition(config);
         _transitions.Add(transition);
      }
      return _transitions;
   }

   // 生命周期接口
   public virtual void NotifyPreStart() {}
   public virtual void NotifyStarted() {}
   public virtual void NotifyStopped() {}

   public virtual void NotifyConfigMotionTarget(Control motionTarget) {}
   public virtual void NotifyRestoreMotionTarget(Control motionTarget) {}
   
   protected virtual void NotifyPreBuildTransition(MotionConfig config, Control motionTarget) {}
   protected virtual ITransition NotifyBuildTransition(MotionConfig config)
   {
      TransitionBase transition = default!;
      if (config.TransitionKind == TransitionKind.Double) {
         transition = new DoubleTransition();
      } else if (config.TransitionKind == TransitionKind.TransformOperations) {
         transition = new TransformOperationsTransition();
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

   public virtual Size CalculateSceneSize(Size motionTargetSize)
   {
      return motionTargetSize;
   }

   public virtual Point CalculateScenePosition(Point motionTargetPosition)
   {
      return motionTargetPosition;
   }
}