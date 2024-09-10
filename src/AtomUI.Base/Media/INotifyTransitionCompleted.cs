namespace AtomUI.Media;

public class TransitionCompletedEventArgs : EventArgs
{
    public TransitionCompletedEventArgs(bool status)
    {
        Status = status;
    }

    public bool Status { get; }
}

internal interface INotifyTransitionCompleted
{
    public IObservable<bool> CompletedObservable { get; }
    public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
    public void NotifyTransitionCompleted(bool status);
}