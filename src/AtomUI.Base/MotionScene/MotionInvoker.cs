using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.MotionScene;

internal static class MotionInvoker
{
    public static void Invoke(MotionActorControl actor,
                              MotionConfig motionConfig,
                              Action? aboutToStart = null,
                              Action? completedAction = null,
                              CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            using var originRestore = new RenderTransformOriginRestore(actor);
            actor.RenderTransformOrigin = motionConfig.RenderTransformOrigin;
            if (aboutToStart != null)
            {
                aboutToStart();
            }

            foreach (var animation in motionConfig.Animations)
            {
                await animation.RunAsync(actor, cancellationToken);
            }

            if (completedAction != null)
            {
                completedAction();
            }
        });
    }

    public static void InvokeInPopupLayer(SceneMotionActorControl actor,
                                          MotionConfig motionConfig,
                                          Action? aboutToStart = null,
                                          Action? completedAction = null,
                                          CancellationToken cancellationToken = default)
    {
        SceneLayer? sceneLayer = PrepareSceneLayer(actor);
        
    }
    
    private static SceneLayer PrepareSceneLayer(SceneMotionActorControl actor)
    {
        if (actor.SceneParent is null)
        {
            throw new ArgumentException(
                "When the DispatchInSceneLayer property is true, the SceneParent property cannot be null.");
        }

        // TODO 这里除了 Popup 这种顶层元素以外，还会不会有其他的顶层元素种类
        // 暂时先处理 Popup 这种情况
        var sceneLayer = new SceneLayer(actor.SceneParent, actor.SceneParent.PlatformImpl!.CreatePopup()!);
        actor.NotifySceneLayerCreated(sceneLayer);
        return sceneLayer;
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