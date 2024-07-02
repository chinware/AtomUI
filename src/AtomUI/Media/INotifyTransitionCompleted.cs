namespace AtomUI.Media;

internal interface INotifyTransitionCompleted
{
   internal event EventHandler? TransitionCompleted;
   internal void NotifyTransitionCompleted()
   {
   }
}