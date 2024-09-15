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
    public IObservable<bool> CompletedObservable { get; }
    public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
    public void NotifyTransitionCompleted(bool status);
}