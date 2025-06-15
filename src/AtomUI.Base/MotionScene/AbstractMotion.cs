using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using AtomUI.Animations;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

namespace AtomUI.MotionScene;

internal class AbstractMotion : IMotion
{
    public RelativePoint RenderTransformOrigin { get; protected set; }
    public IList<Animation> Animations { get; }
    public IList<INotifyTransitionCompleted> Transitions { get; }
    public TimeSpan Duration { get; }
    public Easing Easing { get; }
    public FillMode PropertyValueFillMode { get; }
    public MotionSpiritType SpiritType { get; set; } = MotionSpiritType.Transition;

    public AbstractMotion(TimeSpan duration, Easing? easing = null, FillMode fillMode = FillMode.Forward)
    {
        Animations            = new List<Animation>();
        Duration              = duration;
        Easing                = easing ?? new LinearEasing();
        PropertyValueFillMode = fillMode;
        Transitions           = new List<INotifyTransitionCompleted>();
    }
    
    public void Run(MotionActorControl actor,
                    Action? aboutToStart = null,
                    Action? completedAction = null)
    {
        if (SpiritType == MotionSpiritType.Transition)
        {
            RunTransitions(actor, aboutToStart, completedAction);
            return;
        }
        RunAnimations(actor, aboutToStart, completedAction);
    }

    private void RunAnimations(MotionActorControl actor,
                               Action? aboutToStart = null,
                               Action? completedAction = null)
    {
        ConfigureAnimation();
        var originRenderTransformOrigin = actor.RenderTransformOrigin;
        
        actor.RenderTransformOrigin = RenderTransformOrigin;
        actor.NotifyMotionPreStart();
        NotifyPreStart(actor);
        aboutToStart?.Invoke();
        Dispatcher.UIThread.Post(() =>
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                foreach (var animation in Animations)
                {
                    await animation.RunAsync(actor);
                }
                actor.NotifyMotionCompleted();
                NotifyCompleted(actor);
                completedAction?.Invoke();
                actor.RenderTransformOrigin = originRenderTransformOrigin;
            });
        });
    }
    
    private void RunTransitions(MotionActorControl actor,
                                Action? aboutToStart = null,
                                Action? completedAction = null)
    {
        ConfigureTransitions();
        var originRenderTransformOrigin = actor.RenderTransformOrigin;
        actor.NotifyMotionPreStart();
        NotifyPreStart(actor);
        aboutToStart?.Invoke();
  
        actor.RenderTransformOrigin = RenderTransformOrigin;
        var observables = new List<IObservable<bool>>();

        // 暂时先不保存 actor 原有的 transitions
        actor.Transitions ??= new Transitions();
        actor.Transitions.Clear();
        foreach (var transition in Transitions)
        {
            observables.Add(transition.CompletedObservable);
            actor.Transitions.Add(transition);
        }
        
        actor.DisableTransitions();
        ConfigureMotionStartValue(actor);
        actor.EnableTransitions();

        Dispatcher.UIThread.Post(() =>
        {
            ConfigureMotionEndValue(actor);

            observables.Zip()
                       .LastAsync()
                       .ObserveOn(AvaloniaScheduler.Instance)
                       .Subscribe(_ =>
                       {
                           Dispatcher.UIThread.Post(() =>
                           {
                               actor.DisableTransitions();
                               actor.NotifyMotionCompleted();
                               NotifyCompleted(actor);
                               completedAction?.Invoke();
                               actor.RenderTransformOrigin = originRenderTransformOrigin;
                               actor.MotionTransform       = null;
                           });
                       });
        });
    }

    protected virtual void ConfigureAnimation()
    {
    }

    protected virtual void ConfigureTransitions()
    {
        var opacityTransition             = new NotifiableDoubleTransition()
        {
            Duration = Duration,
            Easing   = Easing,
            Property = MotionActorControl.OpacityProperty,
        };
        Transitions.Add(opacityTransition);
        
        var transformOperationsTransition = new NotifiableTransformOperationsTransition()
        {
            Duration = Duration,
            Easing   = Easing,
            Property = MotionActorControl.MotionTransformProperty,
        };
        Transitions.Add(transformOperationsTransition);
    }

    protected virtual void ConfigureMotionStartValue(MotionActorControl actor)
    {
    }

    protected virtual void ConfigureMotionEndValue(MotionActorControl actor)
    {
    }

    protected virtual void NotifyPreStart(MotionActorControl actor)
    {
    }

    protected virtual void NotifyCompleted(MotionActorControl actor)
    {
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

    protected static ITransform BuildScaleTransform(double scaleX, double scaleY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendScale(scaleX, scaleY);
        return builder.Build();
    }

    protected static ITransform BuildScaleTransform(double scale)
    {
        return BuildScaleTransform(scale, scale);
    }

    protected static ITransform BuildScaleXTransform(double scale)
    {
        return BuildScaleTransform(scale, 1.0);
    }

    protected static ITransform BuildScaleYTransform(double scale)
    {
        return BuildScaleTransform(1.0, scale);
    }

    protected static ITransform BuildTranslateTransform(double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }

    protected static ITransform BuildTranslateScaleAndTransform(
        double scaleX, double scaleY, double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(2);
        builder.AppendScale(scaleX, scaleY);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }

    protected Animation CreateAnimation()
    {
        var animation = new Animation
        {
            Duration = Duration,
            Easing   = Easing,
            FillMode = PropertyValueFillMode
        };
        return animation;
    }
}