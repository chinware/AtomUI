using Avalonia.Animation;

namespace AtomUI.MotionScene;

public interface IMotionBuilder
{
   IEnumerable<ITransition> Build();
}