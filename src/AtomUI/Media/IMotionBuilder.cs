using Avalonia.Animation;

namespace AtomUI.Media;

public interface IMotionBuilder
{
   IEnumerable<ITransition> Build();
}