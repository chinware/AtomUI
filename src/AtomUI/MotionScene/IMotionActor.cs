using Avalonia.Controls;

namespace AtomUI.MotionScene;

public interface IMotionActor
{
   public event EventHandler? PreStart;
   public event EventHandler? Started;
   public event EventHandler? Finished;
   
   public Control Entity { get; set; }
   public IMotion? Motion { get; set; }
   public bool DispatchInGlobalScene { get; set; }

   public void Action();
}