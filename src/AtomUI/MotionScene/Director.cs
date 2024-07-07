using System.Reactive.Disposables;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Controls.MotionScene;

public class Director : IDirector
{
   public event EventHandler<MotionEventArgs>? MotionPreStart;
   public event EventHandler<MotionEventArgs>? MotionStarted;
   public event EventHandler<MotionEventArgs>? MotionCompleted;
   
   public static IDirector? Instance => AvaloniaLocator.Current.GetService<IDirector>();
   private Dictionary<IMotionActor, MotionActorState> _states;
   private CompositeDisposable? _compositeDisposable;
   
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
      _compositeDisposable = new CompositeDisposable();
      _compositeDisposable.Add(Disposable.Create((sceneLayer), state =>
      {
         if (sceneLayer is not null) {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
               await Task.Delay(100);
               sceneLayer.Hide();
               sceneLayer.Dispose();
            });
         }
      }));
      var state = new MotionActorState(actor, _compositeDisposable);
      _states.Add(actor, state);

      if (actor.DispatchInSceneLayer) {
         state.SceneLayer = sceneLayer;
         var ghost = actor.GetAnimatableGhost();
         sceneLayer!.SetMotionTarget(ghost);
         actor.NotifyMotionTargetAddedToScene(ghost);
         sceneLayer.Show();
      }
      
      HandleMotionPreStart(actor);
      ExecuteMotionAction(actor);
      HandleMotionStarted(actor);
   }

   private SceneLayer PrepareSceneLayer(MotionActor actor)
   {
      if (actor.SceneParent is null) {
         throw new ArgumentException("When the DispatchInSceneLayer property is true, the SceneParent property cannot be null.");
      }
      // TODO 这里除了 Popup 这种顶层元素以外，还会不会有其他的顶层元素种类
      // 暂时先处理 Popup 这种情况
      var sceneLayer = new SceneLayer(actor.SceneParent, actor.SceneParent.PlatformImpl!.CreatePopup()!);
      actor.NotifySceneLayerCreated(sceneLayer);
      return sceneLayer;
   }

   private void ExecuteMotionAction(MotionActor actor)
   {
      // 根据 Motion 配置的对 Actor 对象的属性赋值
      actor.EnableMotion();
      foreach (var motionConfig in actor.Motion.GetMotionConfigs()) {
         var property = motionConfig.Property;
         var endValue = motionConfig.EndValue;
         actor.SetValue(property, endValue);
      }
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

   private void HandleMotionPreStart(MotionActor actor)
   {
      actor.NotifyMotionPreStart();
      MotionPreStart?.Invoke(this, new MotionEventArgs(actor));
      if (actor.Motion.CompletedObservable is null) {
         throw new InvalidOperationException("The CompletedObservable property of the Motion is empty.");
      }
      // 设置相关的完成检测
      _compositeDisposable?.Add(actor.Motion.CompletedObservable.Subscribe(status =>
      {
         actor.CompletedStatus = status;
      }, onCompleted: () =>
      {
         HandleMotionCompleted(actor);
      }));
      
      // 设置动画对象初始值
      foreach (var motionConfig in actor.Motion.GetMotionConfigs()) {
         var property = motionConfig.Property;
         var startValue = motionConfig.StartValue;
         actor.SetValue(property, startValue);
      }
   }

   private void HandleMotionStarted(MotionActor actor)
   {
      actor.NotifyMotionStarted();
      MotionStarted?.Invoke(this, new MotionEventArgs(actor));
   }

   private void HandleMotionCompleted(MotionActor actor)
   {
      if (_states.TryGetValue(actor, out var state)) {
         if (state.SceneLayer is not null) {
            state.SceneLayer.Opacity = 0;
         }
         actor.NotifyMotionCompleted();
         MotionCompleted?.Invoke(this, new MotionEventArgs(actor));
         state.Dispose();
         _states.Remove(actor);
      }
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