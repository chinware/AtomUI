using System.Reflection;
using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.MotionScene;

/// <summary>
/// 动效配置类，只要给 Director 提供动效相关信息
/// 动效驱动 Actor 的属性，然后由 Actor 驱动动画控件，防止污染动画控件的 Transitions 配置
/// </summary>
public class MotionActor : Animatable, IMotionActor
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Completed;
   public event EventHandler? SceneShowed;
   
   public static readonly StyledProperty<double> MotionOpacityProperty =
      Visual.OpacityProperty.AddOwner<MotionActor>();
   
   public static readonly StyledProperty<double> MotionWidthProperty =
      Layoutable.WidthProperty.AddOwner<MotionActor>();
   
   public static readonly StyledProperty<double> MotionHeightProperty =
      Layoutable.HeightProperty.AddOwner<MotionActor>();

   public static readonly StyledProperty<ITransform?> MotionRenderTransformProperty =
      Visual.RenderTransformProperty.AddOwner<MotionActor>();

   private static readonly MethodInfo EnableTransitionsMethodInfo;
   private static readonly MethodInfo DisableTransitionsMethodInfo;

   public bool CompletedStatus { get; internal set; } = true;
   
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

   protected ITransform? MotionRenderTransform
   {
      get => GetValue(MotionRenderTransformProperty);
      set => SetValue(MotionRenderTransformProperty, value);
   }

   private double _originOpacity;
   private double _originWidth;
   private double _originHeight;
   private ITransform? _originRenderTransform;
   private RelativePoint _originRenderTransformOrigin;
   private Dictionary<AvaloniaProperty, AnimationState> _transitionsMap;

   private class AnimationState
   {
      public ITransition? Transition { get; set; }
      public object? StartValue { get; set; }
      public object? EndValue { get; set; }
   }
   
   /// <summary>
   /// 动画实体
   /// </summary>
   public Control MotionTarget { get; set; }
   
   /// <summary>
   /// 当 DispatchInSceneLayer 为 true 的时候，必须指定一个动画 SceneLayer 的父窗口，最好不要是 Popup
   /// </summary>
   public TopLevel? SceneParent { get; set; }

   public IMotion Motion => _motion;
   public bool DispatchInSceneLayer { get; set; } = true;
   
   protected Control? _ghost;
   protected AbstractMotion _motion;

   static MotionActor()
   {
      EnableTransitionsMethodInfo = typeof(Animatable).GetMethod("EnableTransitions",BindingFlags.Instance | BindingFlags.NonPublic)!;
      DisableTransitionsMethodInfo =
         typeof(Animatable).GetMethod("DisableTransitions", BindingFlags.Instance | BindingFlags.NonPublic)!;
      MotionWidthProperty.Changed.AddClassHandler<MotionActor>(HandlePropertyChanged);
      MotionHeightProperty.Changed.AddClassHandler<MotionActor>(HandlePropertyChanged);
      MotionOpacityProperty.Changed.AddClassHandler<MotionActor>(HandlePropertyChanged);
      MotionRenderTransformProperty.Changed.AddClassHandler<MotionActor>(HandlePropertyChanged);
   }

   private static void HandlePropertyChanged(MotionActor actor, AvaloniaPropertyChangedEventArgs args)
   {
      var property = args.Property;
      var oldValue = args.OldValue;
      var newValue = args.NewValue;
      var priority = args.Priority;
      if (!actor._transitionsMap.ContainsKey(property)) {
         return;
      }
      var state = actor._transitionsMap[property];
      if (actor.IsAnimating(property) && priority == BindingPriority.Animation) {
         // 判断新值是否相等
         if (property.PropertyType == typeof(double)) {
            var currentValue = (double)newValue!;
            var endValue = (double)state.EndValue!;
             if (MathUtils.AreClose(currentValue, endValue)) {
                var transition = state.Transition;
                if (transition is INotifyTransitionCompleted notifyTransitionCompleted) {
                   notifyTransitionCompleted.NotifyTransitionCompleted(true);
                }
             }
         } else if (property.PropertyType.IsAssignableTo(typeof(ITransform))) {
            var currentValue = (ITransform)newValue!;
            var endValue = (ITransform)state.EndValue!;
            if (currentValue.Value == endValue.Value) {
               var transition = state.Transition;
               if (transition is INotifyTransitionCompleted notifyTransitionCompleted) {
                  notifyTransitionCompleted.NotifyTransitionCompleted(true);
               }
            }
         }
      }
   }

   public MotionActor(Control motionTarget, AbstractMotion motion)
   {
      MotionTarget = motionTarget;
      _motion = motion;
      _transitionsMap = new Dictionary<AvaloniaProperty, AnimationState>();
   }
   
   public bool IsSupportMotionProperty(AvaloniaProperty property)
   {
      if (property == AbstractMotion.MotionOpacityProperty ||
          property == AbstractMotion.MotionWidthProperty ||
          property == AbstractMotion.MotionHeightProperty ||
          property == AbstractMotion.MotionRenderTransformProperty) {
         return true;
      }
      return false;
   }
   
   protected virtual void BuildGhost() {}
   
   public Control GetAnimatableGhost()
   {
      return _ghost ?? MotionTarget;
   }

   /// <summary>
   /// 当在 DispatchInSceneLayer 渲染的时候，Ghost 的全局坐标
   /// </summary>
   /// <returns></returns>
   public Point CalculateGhostPosition()
   {
      Point point = default;
      if (!DispatchInSceneLayer) {
         var visualParent = MotionTarget.GetVisualParent();
         if (visualParent is not null) {
            var parentPoint = MotionTarget.TranslatePoint(new Point(0, 0), visualParent);
            if (parentPoint.HasValue) {
               point = parentPoint.Value;
            } else {
               point = MotionTarget.Bounds.Position;
            }
         }
      } else {
         point = CalculateTopLevelGhostPosition();
      }
      return point;
   }

   protected virtual Point CalculateTopLevelGhostPosition()
   {
      return default;
   }

   /// <summary>
   /// 在这个接口中，Actor 根据自己的需求对 sceneLayer 进行设置，主要就是位置和大小
   /// </summary>
   /// <param name="sceneLayer"></param>
   public virtual void NotifySceneLayerCreated(SceneLayer sceneLayer)
   {
      if (!DispatchInSceneLayer) {
         return;
      }

      var ghost = GetAnimatableGhost();
      
      Size motionTargetSize;
      // Popup.Child can't be null here, it was set in ShowAtCore.
      if (ghost.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         motionTargetSize = LayoutHelper.MeasureChild(ghost, Size.Infinity, new Thickness());
      } else {
         motionTargetSize = ghost.DesiredSize;
      }

      var sceneSize = _motion.CalculateSceneSize(motionTargetSize);
      var scenePosition = _motion.CalculateScenePosition(motionTargetSize, CalculateGhostPosition());
      sceneLayer.MoveAndResize(scenePosition, sceneSize);
   }

   public virtual void NotifyPostedToDirector()
   {
      DisableMotion();
      if (DispatchInSceneLayer) {
         BuildGhost();
      }
      
      RelayMotionProperties();
      var transitions = new Transitions();
      foreach (var transition in _motion.BuildTransitions(GetAnimatableGhost())) {
         transitions.Add(transition);
      }
      Transitions = transitions;
   }

   protected void RelayMotionProperties()
   {
      var ghost = GetAnimatableGhost();
      // TODO 这个看是否需要管理起来
      
      var motionProperties = Motion.GetActivatedProperties();
      foreach (var property in motionProperties) {
         if (property == MotionRenderTransformProperty) {
            BindUtils.RelayBind(this, property, ghost, Visual.RenderTransformProperty);
         } else {
            BindUtils.RelayBind(this, property, ghost, property);
         }
      }
   }

   /// <summary>
   /// 当动画目标控件被添加到动画场景中之后调用，这里需要根据 Motion 的种类设置初始位置和大小
   /// </summary>
   /// <param name="motionTarget"></param>
   public virtual void NotifyMotionTargetAddedToScene(Control motionTarget)
   {
      Canvas.SetLeft(motionTarget, 0);
      Canvas.SetTop(motionTarget, 0);
   }

   public virtual void NotifySceneShowed()
   {
      SceneShowed?.Invoke(this, EventArgs.Empty);
   }

   internal void EnableMotion()
   {
      EnableTransitionsMethodInfo.Invoke(this, new object[]{});
   }

   internal void DisableMotion()
   {
      DisableTransitionsMethodInfo.Invoke(this, new object[]{});
   }

   internal virtual void NotifyMotionPreStart()
   {
      if (Transitions is not null) {
         foreach (var transition in Transitions) {
            _transitionsMap.Add(transition.Property, new AnimationState()
            {
               Transition = transition
            });
         }
      }

      foreach (var motionConfig in _motion.GetMotionConfigs()) {
         var property = motionConfig.Property;
         var state = _transitionsMap[property];
         state.StartValue = motionConfig.StartValue;
         state.EndValue = motionConfig.EndValue;
      }

      SaveMotionTargetState();
      _motion.NotifyPreStart();
      _motion.NotifyConfigMotionTarget(GetAnimatableGhost());
      PreStart?.Invoke(this, EventArgs.Empty);
   }

   internal virtual void NotifyMotionStarted()
   {
      _motion.NotifyStarted();
      Started?.Invoke(this, EventArgs.Empty);
   }

   internal virtual void NotifyMotionCompleted()
   {
      RestoreMotionTargetState();
      Completed?.Invoke(this, EventArgs.Empty);
      _motion.NotifyCompleted();
      _motion.NotifyRestoreMotionTarget(GetAnimatableGhost());
      _transitionsMap.Clear();
   }

   private void SaveMotionTargetState()
   {
      var target = GetAnimatableGhost();
      foreach (var motionConfig in _motion.GetMotionConfigs()) {
         if (motionConfig.Property == MotionHeightProperty) {
            _originHeight = target.Height;
         } else if (motionConfig.Property == MotionWidthProperty) {
            _originWidth = target.Width;
         } else if (motionConfig.Property == MotionOpacityProperty) {
            _originOpacity = target.Opacity;
         } else if (motionConfig.Property == MotionRenderTransformProperty) {
            _originRenderTransform = target.RenderTransform;
            _originRenderTransformOrigin = target.RenderTransformOrigin;
         }
      }
   }

   private void RestoreMotionTargetState()
   {
      var target = GetAnimatableGhost();
      foreach (var motionConfig in _motion.GetMotionConfigs()) {
         if (motionConfig.Property == MotionHeightProperty) {
            target.Height = _originHeight;
         } else if (motionConfig.Property == MotionWidthProperty) {
            target.Width = _originWidth;
         } else if (motionConfig.Property == MotionOpacityProperty) {
            target.Opacity = _originOpacity;
         } else if (motionConfig.Property == MotionRenderTransformProperty) {
            target.RenderTransform = _originRenderTransform;
            target.RenderTransformOrigin = _originRenderTransformOrigin;
         }
      }
   }
}