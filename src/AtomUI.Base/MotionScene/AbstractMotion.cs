using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media.Transformation;
using Avalonia.Threading;

namespace AtomUI.MotionScene;

public class AbstractMotion : IMotion
{
    public RelativePoint RenderTransformOrigin { get; protected set; }
    public IList<Animation> Animations { get; }
    public TimeSpan Duration { get; }
    public Easing Easing { get; }
    public FillMode PropertyValueFillMode { get; }

    public AbstractMotion(TimeSpan duration, Easing? easing = null, FillMode fillMode = FillMode.Forward)
    {
        Animations            = new List<Animation>();
        Duration              = duration;
        Easing                = easing ?? new LinearEasing();
        PropertyValueFillMode = fillMode;
    }

    public async Task RunAsync(MotionActorControl actor,
                               Action? aboutToStart = null,
                               Action? completedAction = null,
                               CancellationToken cancellationToken = default)
    {
        Configure();
        
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            actor.IsVisible = true;
            using var originRestore = new RenderTransformOriginRestore(actor);
            actor.RenderTransformOrigin = RenderTransformOrigin;
            actor.NotifyMotionPreStart();
            NotifyPreStart();
            aboutToStart?.Invoke();
      
            foreach (var animation in Animations)
            {
                await animation.RunAsync(actor, cancellationToken);
            }

            actor.NotifyMotionCompleted();
            NotifyCompleted();
            completedAction?.Invoke();
        });
    }

    protected virtual void Configure()
    {
    }

    protected virtual void NotifyPreStart()
    {
    }

    protected virtual void NotifyCompleted()
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

    protected static TransformOperations BuildScaleTransform(double scaleX, double scaleY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendScale(scaleX, scaleY);
        return builder.Build();
    }

    protected static TransformOperations BuildScaleTransform(double scale)
    {
        return BuildScaleTransform(scale, scale);
    }

    protected static TransformOperations BuildScaleXTransform(double scale)
    {
        return BuildScaleTransform(scale, 1.0);
    }

    protected static TransformOperations BuildScaleYTransform(double scale)
    {
        return BuildScaleTransform(1.0, scale);
    }

    protected static TransformOperations BuildTranslateTransform(double offsetX, double offsetY)
    {
        var builder = new TransformOperations.Builder(1);
        builder.AppendTranslate(offsetX, offsetY);
        return builder.Build();
    }

    protected static TransformOperations BuildTranslateScaleAndTransform(
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

internal class RenderTransformOriginRestore : IDisposable
{
    RelativePoint _origin;
    Control _target;

    public RenderTransformOriginRestore(Control target)
    {
        _target = target;
        _origin = target.RenderTransformOrigin;
    }

    public void Dispose()
    {
        _target.RenderTransformOrigin = _origin;
    }
}