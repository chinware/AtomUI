using AtomUI.MotionScene;
using Avalonia;

namespace AtomUI.Controls.MotionScene;

public class Director : IDirector
{
   public static IDirector? Instance => AvaloniaLocator.Current.GetService<IDirector>();
   
   public void Schedule(IMotionActor actor)
   {
      
   }
}