using System.Reactive.Subjects;
using Avalonia.Animation;

namespace AtomUI.Media;

public class NotifiableTransformOperationsTransition : TransformOperationsTransition
{
   public event EventHandler<TransitionCompletedEventArgs>? TransitionCompleted;
   private Subject<bool> _subject;
   
   public NotifiableTransformOperationsTransition()
   {
      _subject = new Subject<bool>();
   }
   
   internal protected void NotifyTransitionCompleted(bool status)
   {
      _subject.OnNext(status);
      _subject.OnCompleted();
      TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs(status));
   }

   internal IObservable<bool> GetCompletedObservable()
   {
      return _subject;
   }
}