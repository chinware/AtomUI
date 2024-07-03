using Avalonia.Controls;

namespace AtomUI.MotionScene;

internal interface IMotionActor
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Finished;
   
   public Control MotionTarget { get; set; }
   public IMotion Motion { get; }
   public bool DispatchInSceneLayer { get; set; }
}