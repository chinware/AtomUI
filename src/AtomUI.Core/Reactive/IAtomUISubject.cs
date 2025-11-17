namespace AtomUI.Reactive;

internal interface IAtomUISubject<T> : IObserver<T>, IObservable<T> /*, ISubject<T> */
{
}
