using System.Reactive.Subjects;
using AtomUI.Utils;
using Avalonia.Animation;

namespace AtomUI.Animations;

public abstract class AbstractNotifiableTransition<T> : InterpolatingTransitionBase<T>, INotifyTransitionCompleted
{
    internal const double CompletedProgress = 1.0d;
    public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
    
    public IObservable<bool> CompletedObservable => _subject;
    private readonly Subject<bool> _subject;
    
    public AbstractNotifiableTransition()
    {
        _subject = new Subject<bool>();
    }

    protected bool CheckCompletedStatus(double progress)
    {
        if (MathUtils.AreClose(progress, CompletedProgress))
        {
            return true;
        }
        return false;
    }
    
    protected void NotifyTransitionCompleted(bool status)
    {
        _subject.OnNext(status);
        _subject.OnCompleted();
        TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs(status));
    }
}