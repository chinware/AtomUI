using System.Reactive.Subjects;
using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableDoubleTransition : DoubleTransition, INotifyTransitionCompleted
{
    private readonly Subject<bool> _subject;

    public NotifiableDoubleTransition()
    {
        _subject = new Subject<bool>();
    }

    public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;

    void INotifyTransitionCompleted.NotifyTransitionCompleted(bool status)
    {
        _subject.OnNext(status);
        _subject.OnCompleted();
        TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs(status));
    }

    IObservable<bool> INotifyTransitionCompleted.CompletedObservable => _subject;
}