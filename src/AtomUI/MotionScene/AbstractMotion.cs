using System.Diagnostics;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.CSharp.RuntimeBinder;
using ColorTransition = Avalonia.Animation.ColorTransition;

namespace AtomUI.MotionScene;

public enum TransitionKind
{
   BoxShadows,
   Brush,
   Color,
   CornerRadius,
   Double,
   Float,
   Integer,
   Point,
   PixelPoint,
   RelativePoint,
   Size,
   Thickness,
   TransformOperations,
   Vector,
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

   private WeakReference<Control> _target;
   public IMotionAbilityTarget? MotionTarget => GetMotionTarget();

   private Dictionary<AvaloniaProperty, MotionConfig> _motionConfigs;
   private List<ITransition> _transitions;

   // 定义我们目前支持的动效属性
   protected static readonly StyledProperty<double> MotionWidthProperty =
      Layoutable.WidthProperty.AddOwner<AbstractMotion>();

   protected static readonly StyledProperty<double> MotionHeightProperty =
      Layoutable.HeightProperty.AddOwner<AbstractMotion>();

   protected static readonly StyledProperty<PixelPoint> MotionWindowPositionProperty =
      AvaloniaProperty.Register<AbstractMotion, PixelPoint>(nameof(MotionWindowPosition));

   protected static readonly StyledProperty<double> MotionOpacityProperty =
      Visual.OpacityProperty.AddOwner<AbstractMotion>();

   protected static readonly StyledProperty<Point> MotionRenderOffsetProperty =
      AvaloniaProperty.Register<AbstractMotion, Point>(nameof(MotionRenderOffset));

   protected static readonly StyledProperty<Rect> MotionRenderBoundsProperty =
      AvaloniaProperty.Register<AbstractMotion, Rect>(nameof(MotionRenderBounds));
   
   protected static readonly StyledProperty<ITransform> MotionRenderTransformProperty =
      AvaloniaProperty.Register<AbstractMotion, ITransform>(nameof(MotionRenderTransform));

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

   protected PixelPoint MotionWindowPosition
   {
      get => GetValue(MotionWindowPositionProperty);
      set => SetValue(MotionWindowPositionProperty, value);
   }

   protected double MotionOpacity
   {
      get => GetValue(MotionOpacityProperty);
      set => SetValue(MotionOpacityProperty, value);
   }

   protected Point MotionRenderOffset
   {
      get => GetValue(MotionRenderOffsetProperty);
      set => SetValue(MotionRenderOffsetProperty, value);
   }

   protected Rect MotionRenderBounds
   {
      get => GetValue(MotionRenderBoundsProperty);
      set => SetValue(MotionRenderBoundsProperty, value);
   }
   
   protected ITransform MotionRenderTransform
   {
      get => GetValue(MotionRenderTransformProperty);
      set => SetValue(MotionRenderTransformProperty, value);
   }
   
   public AbstractMotion(Control target)
   {
      if (target is not IMotionAbilityTarget) {
         throw new ArgumentException("argument target must the type of IMotionAbilityTarget");
      }

      _target = new WeakReference<Control>(target);
      _motionConfigs = new Dictionary<AvaloniaProperty, MotionConfig>();
      _transitions = new List<ITransition>();
   }

   public IMotionAbilityTarget? GetMotionTarget()
   {
      if (_target.TryGetTarget(out var target)) {
         return (IMotionAbilityTarget)target;
      }

      return null;
   }

   protected Control? GetControlTarget()
   {
      if (_target.TryGetTarget(out var target)) {
         return target;
      }

      return null;
   }

   public List<ITransition> BuildTransitions()
   {
      var target = GetMotionTarget();
      if (target is null) {
         return _transitions;
      }

      NotifyConfigureTarget(target);
      foreach (var entry in _motionConfigs) {
         var config = entry.Value;
         if (!target.IsSupportMotionProperty(config.Property)) {
            throw new RuntimeBinderException(
               $"Motion target does not support animation property: {config.Property.Name}");
         }

         NotifyPreBuildTransition(config);
         var transition = NotifyBuildTransition(config);
         _transitions.Add(transition);
      }

      return _transitions;
   }

   // 生命周期接口
   public virtual void NotifyPreStart() { }
   public virtual void NotifyStarted() { }
   public virtual void NotifyStopped() { }

   protected virtual void NotifyConfigureTarget(IMotionAbilityTarget target) { }
   protected virtual void NotifyPreBuildTransition(MotionConfig config) { }

   protected virtual ITransition NotifyBuildTransition(MotionConfig config)
   {
      TransitionBase transition = default!;
      if (config.TransitionKind == TransitionKind.Brush) {
         transition = new SolidColorBrushTransition();
      } else if (config.TransitionKind == TransitionKind.Color) {
         transition = new ColorTransition();
      } else if (config.TransitionKind == TransitionKind.Double) {
         transition = new DoubleTransition();
      } else if (config.TransitionKind == TransitionKind.Float) {
         transition = new FloatTransition();
      } else if (config.TransitionKind == TransitionKind.Integer) {
         transition = new IntegerTransition();
      } else if (config.TransitionKind == TransitionKind.Point) {
         transition = new PointTransition();
      } else if (config.TransitionKind == TransitionKind.PixelPoint) {
         transition = new PixelPointTransition();
      } else if (config.TransitionKind == TransitionKind.Size) {
         transition = new SizeTransition();
      } else if (config.TransitionKind == TransitionKind.Thickness) {
         transition = new ThicknessTransition();
      } else if (config.TransitionKind == TransitionKind.Vector) {
         transition = new VectorTransition();
      } else if (config.TransitionKind == TransitionKind.BoxShadows) {
         transition = new BoxShadowsTransition();
      } else if (config.TransitionKind == TransitionKind.CornerRadius) {
         transition = new CornerRadiusTransition();
      } else if (config.TransitionKind == TransitionKind.RelativePoint) {
         transition = new RelativePointTransition();
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
}