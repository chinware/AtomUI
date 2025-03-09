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

    public static void DispatchInMotionSceneLayer(SceneMotionActorControl actor,
                                                  AbstractMotion motion,
                                                  Action? aboutToStart = null,
                                                  Action? completedAction = null,
                                                  CancellationToken cancellationToken = default)
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
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await motion.RunAsync(actor, () =>
                {
                    actor.IsVisible = true;
                }, () =>
                {
                   completedAction?.Invoke();
                   Dispatcher.UIThread.Post(() => actor.Opacity = 0d);
                }, cancellationToken);
                DispatcherTimer.RunOnce(() =>
                {
                    compositeDisposable.Dispose();
                    compositeDisposable = null;
                }, TimeSpan.FromMilliseconds(600));
            });
        });
    }
    
    public static void DispatchOutMotionSceneLayer(SceneMotionActorControl actor,
                                                   AbstractMotion motion,
                                                   Action? aboutToStart = null,
                                                   Action? completedAction = null,
                                                   CancellationToken cancellationToken = default)
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
        sceneLayer.Show();
        actor.NotifySceneShowed();
        aboutToStart?.Invoke();
        // 等待一个事件循环，让动画窗口置顶
        Dispatcher.UIThread.Post(() =>
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                // 主要等待正常窗体显示出来再隐藏对话层，不然感觉会闪屏
                await motion.RunAsync(actor, null, () =>
                {
                    completedAction?.Invoke();
                    Dispatcher.UIThread.Post(() => actor.IsVisible = false);
                }, cancellationToken);
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