using System.Reactive.Disposables;
using Avalonia.Threading;

namespace AtomUI.MotionScene;

internal static class MotionInvoker
{
    public static void Invoke(MotionActorControl actor,
                              AbstractMotion motion,
                              Action? aboutToStart = null,
                              Action? completedAction = null)
    {
        motion.Run(actor, aboutToStart, completedAction);
    }
    
    public static void DispatchMotionInSceneLayer(SceneMotionActorControl actor,
                                                  AbstractMotion motion,
                                                  Action? aboutToStart = null,
                                                  Action? completedAction = null)
    {
        var sceneLayer          = PrepareSceneLayer(motion, actor);
        var compositeDisposable = new CompositeDisposable();
        compositeDisposable.Add(Disposable.Create(sceneLayer, (state) =>
        {
            sceneLayer.Hide();
            sceneLayer.Dispose();
        }));
        sceneLayer.SetMotionActor(actor);
        actor.NotifyMotionTargetAddedToScene();
        sceneLayer.Topmost = true;
        actor.IsVisible    = false;
        sceneLayer.Show();
        actor.NotifySceneShowed();
        aboutToStart?.Invoke();
        // 等待一个事件循环，让动画窗口置顶
        Dispatcher.UIThread.Post(() =>
        {
            motion.Run(actor, null, () =>
            {
                completedAction?.Invoke();
                compositeDisposable.Dispose();
                compositeDisposable = null;
            });
        });
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