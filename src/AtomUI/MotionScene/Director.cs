using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.MotionScene;

public class Director : IDirector
{
   public static IDirector? Instance => AvaloniaLocator.Current.GetService<IDirector>();
   private List<IMotionActor> _actors;

   public Director()
   {
      _actors = new List<IMotionActor>();
   }
   
   public void Schedule(IMotionActor actor)
   {
      _actors.Add(actor);
   }

   private Size CalculateSceneLayerOffset(Control ghost, IMotion motion)
   {
      return default;
   }
}