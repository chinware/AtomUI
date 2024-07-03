namespace AtomUI.Media;

public class TransitionCompletedEventArgs : EventArgs
{
   public bool Status { get; }
   
   public TransitionCompletedEventArgs(bool status)
   {
      Status = status;
   }
}

internal interface INotifyTransitionCompleted
{
   internal event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
   internal void NotifyTransitionCompleted(bool status);
}