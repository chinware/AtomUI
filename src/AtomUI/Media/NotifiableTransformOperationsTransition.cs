using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableTransformOperationsTransition : TransformOperationsTransition
{
   public event EventHandler? TransitionCompleted;
   
   internal protected void NotifyTransitionCompleted()
   {
   }
}