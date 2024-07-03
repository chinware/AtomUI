using System.Reactive.Disposables;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.MotionScene;

public class Director : IDirector
{
   public event EventHandler<MotionEventArgs>? PreStart;
   public event EventHandler<MotionEventArgs>? Started;
   public event EventHandler<MotionEventArgs>? Finished;
   
   public static IDirector? Instance => AvaloniaLocator.Current.GetService<IDirector>();
   private Dictionary<IMotionActor, MotionActorState> _states;

   public Director()
   {
      _states = new Dictionary<IMotionActor, MotionActorState>();
   }
   
   /// <summary>
   /// 目前的实现暂时一个 Actor 只能投递一次，后面可以实现一个等待队列
   /// </summary>
   /// <param name="actor"></param>
   public void Schedule(MotionActor actor)
   {
      if (_states.ContainsKey(actor)) {
         return;
      }
      actor.NotifyPostedToDirector();
      SceneLayer? sceneLayer = default;
      if (actor.DispatchInSceneLayer) {
         sceneLayer = PrepareSceneLayer(actor);
      }
      var cleanupPopup = Disposable.Create((sceneLayer), state =>
      {
         if (sceneLayer is not null) {
            sceneLayer.Hide();
            sceneLayer.Dispose();
         }
      });
      var state = new MotionActorState(actor, cleanupPopup);
      _states.Add(actor, state);

      if (actor.DispatchInSceneLayer) {
         var ghost = actor.BuildGhost();
         sceneLayer!.SetMotionTarget(ghost);
         actor.NotifyMotionTargetAddedToScene(ghost);
      }
   }

   private SceneLayer PrepareSceneLayer(MotionActor actor)
   {
      var motionTarget = actor.MotionTarget;
      var topLevel = (TopLevel.GetTopLevel(motionTarget) as PopupRoot)!;
      // TODO 这里除了 Popup 这种顶层元素以外，还会不会有其他的顶层元素种类
      // 暂时先处理 Popup 这种情况
      var sceneLayer = new SceneLayer(topLevel, topLevel.PlatformImpl!);
      actor.NotifySceneLayerCreated(sceneLayer);
      return sceneLayer;
   }

   private class MotionActorState : IDisposable
   {
      private readonly IDisposable _cleanup;
      
      public IMotionActor MotionActor { get; }
      public SceneLayer? SceneLayer;

      public MotionActorState(IMotionActor motionActor, IDisposable cleanup)
      {
         MotionActor = motionActor;
         _cleanup = cleanup;
      }
      
      public void Dispose()
      {
         _cleanup.Dispose();
      }
   }

   protected void HandleMotionActionCompleted(MotionActor actor)
   {
      if (_states.TryGetValue(actor, out var state)) {
         state.Dispose();
      }

      _states.Remove(actor);
   }
}

public class MotionEventArgs : EventArgs
{
   public MotionActor MotionActor;
   
   public MotionEventArgs(MotionActor motionActor)
   {
      MotionActor = motionActor;
   }
}