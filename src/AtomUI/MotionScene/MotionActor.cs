using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.MotionScene;

/// <summary>
/// 动效配置类，只要给 Director 提供动效相关信息
/// </summary>
public class MotionActor : Animatable, IMotionActor
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Finished;
   
   public static readonly StyledProperty<double> MotionOpacityProperty =
      Visual.OpacityProperty.AddOwner<MotionActor>();
   
   public static readonly StyledProperty<double> MotionWidthProperty =
      Visual.OpacityProperty.AddOwner<MotionActor>();
   
   public static readonly StyledProperty<double> MotionHeightProperty =
      Visual.OpacityProperty.AddOwner<MotionActor>();

   public static readonly StyledProperty<ITransform?> MotionRenderTransformProperty =
      Visual.RenderTransformProperty.AddOwner<MotionActor>();
   
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
   
   /// <summary>
   /// 动画实体
   /// </summary>
   public Control MotionTarget { get; set; }

   public IMotion Motion => _motion;
   public bool DispatchInSceneLayer { get; set; } = true;
   
   private Control? _ghost;
   private AbstractMotion _motion;

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

   public MotionActor(Control motionTarget, AbstractMotion motion)
   {
      MotionTarget = motionTarget;
      _motion = motion;
   }
   
   public virtual Control BuildGhost()
   {
      _ghost = MotionTarget;
      return MotionTarget;
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

      var ghost = BuildGhost();
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
      BuildGhost();
      RelayMotionProperties();
      var transitions = new Transitions();
      foreach (var transition in _motion.BuildTransitions(_ghost!)) {
         transitions.Add(transition);
      }
      Transitions = transitions;
   }

   protected void RelayMotionProperties()
   {
      if (_ghost is null) {
         return;
      }
      // TODO 这个看是否需要管理起来
      var motionProperties = Motion.GetActivatedProperties();
      foreach (var property in motionProperties) {
         BindUtils.RelayBind(this, property, _ghost, property);
      }
   }

   /// <summary>
   /// 当动画目标控件被添加到动画场景中之后调用，这里需要根据 Motion 的种类设置初始位置和大小
   /// </summary>
   /// <param name="motionTarget"></param>
   public virtual void NotifyMotionTargetAddedToScene(Control motionTarget)
   {
      
   }
   
}