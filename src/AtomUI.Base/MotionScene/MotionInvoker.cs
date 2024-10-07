using System.Reactive.Disposables;
using Avalonia.Threading;

namespace AtomUI.MotionScene;

internal static class MotionInvoker
{
    public static void Invoke(MotionActorControl actor,
                              AbstractMotion motion,
                              Action? aboutToStart = null,
                              Action? completedAction = null,
                              CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await motion.RunAsync(actor, aboutToStart, completedAction, cancellationToken);
        });
    }

    public static async Task InvokeAsync(MotionActorControl actor,
                                         AbstractMotion motion,
                                         Action? aboutToStart = null,
                                         Action? completedAction = null,
                                         CancellationToken cancellationToken = default)
    {
        await motion.RunAsync(actor, aboutToStart, completedAction, cancellationToken);
    }

    public static void InvokeInPopupLayer(SceneMotionActorControl actor,
                                          AbstractMotion motion,
                                          Action? aboutToStart = null,
                                          Action? completedAction = null,
                                          CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await InvokeInPopupLayerAsync(actor, motion, aboutToStart, completedAction, cancellationToken);
        });
    }

    public static async Task InvokeInPopupLayerAsync(SceneMotionActorControl actor,
                                                     AbstractMotion motion,
                                                     Action? aboutToStart = null,
                                                     Action? completedAction = null,
                                                     CancellationToken cancellationToken = default)
    {
        actor.BuildGhost();
        SceneLayer sceneLayer          = PrepareSceneLayer(motion, actor);
        var        compositeDisposable = new CompositeDisposable();
        compositeDisposable.Add(Disposable.Create(sceneLayer, (state) =>
        {
            Dispatcher.UIThread.Invoke(async () =>
            {
                await Task.Delay(100);
                sceneLayer.Hide();
                sceneLayer.Dispose();
            });
        }));
        sceneLayer.SetMotionActor(actor);
        actor.NotifyMotionTargetAddedToScene();
        sceneLayer.Show();
        sceneLayer.Topmost = true;
        actor.NotifySceneShowed();
        actor.IsVisible = false;
        
        await motion.RunAsync(actor, aboutToStart, () =>
        {
            compositeDisposable.Dispose();
            if (completedAction is not null)
            {
                completedAction();
            }
        }, cancellationToken);
    }

    private static SceneLayer PrepareSceneLayer(AbstractMotion motion, SceneMotionActorControl actor)
    {
        if (actor.SceneParent is null)
        {
            throw new ArgumentException(
                "When the DispatchInSceneLayer property is true, the SceneParent property cannot be null.");
        }

        // TODO 这里除了 Popup 这种顶层元素以外，还会不会有其他的顶层元素种类
        // 暂时先处理 Popup 这种情况
        var sceneLayer = new SceneLayer(actor.SceneParent, actor.SceneParent.PlatformImpl!.CreatePopup()!);
        actor.NotifySceneLayerCreated(motion, sceneLayer);
        return sceneLayer;
    }
}