using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableDoubleTransition : DoubleTransition
{
   public event EventHandler? TransitionCompleted;
   private bool _completed = false;
   
   internal protected void NotifyTransitionCompleted()
   {
      _completed = true;
      TransitionCompleted?.Invoke(this, EventArgs.Empty);
   }

   internal async Task StatusCheckTask()
   {
      if (_completed) {
         return;
      }

      while (!_completed) {
         await Task.Delay(50);
      }
   }
}