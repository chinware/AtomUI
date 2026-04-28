using System.Reactive.Subjects;
using AtomUI.Utils;
using Avalonia.Animation;

namespace AtomUI.Animations;

internal abstract class AbstractNotifiableTransition<T> : InterpolatingTransitionBase<T>, INotifyTransitionCompleted
{
    internal const double CompletedProgress = 1.0d;
    
    public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
    
    public IObservable<bool> CompletedObservable => _subject ?? throw new ObjectDisposedException(GetType().Name);
    private Subject<bool>? _subject;
    
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
        if (_subject == null) return;
        _subject.OnNext(status);
        _subject.OnCompleted();
        TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs(status));
    }

    /// <summary>
    /// Disposes the internal <see cref="Subject{T}"/> and any remaining subscriptions.
    /// Call this after the transition run is finished (completed or timed-out) to
    /// prevent subscriber reference leaks when the transition was never completed.
    /// </summary>
    public void Dispose()
    {
        _subject?.Dispose();
        _subject = null;
    }
}