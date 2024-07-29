using System.Reactive.Subjects;
using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableTransformOperationsTransition : TransformOperationsTransition, INotifyTransitionCompleted
{
   public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
   private Subject<bool> _subject;
   
   public NotifiableTransformOperationsTransition()
   {
      _subject = new Subject<bool>();
   }
   
   void INotifyTransitionCompleted.NotifyTransitionCompleted(bool status)
   {
      _subject.OnNext(status);
      _subject.OnCompleted();
      TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs(status));
   }
   
   IObservable<bool> INotifyTransitionCompleted.CompletedObservable => _subject;
}