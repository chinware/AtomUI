using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableDoubleTransition : DoubleTransition
{
   public event EventHandler? TransitionCompleted;
   
   internal protected void NotifyTransitionCompleted()
   {
   }
}