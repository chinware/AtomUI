namespace AtomUI.Reactive;

internal class LightweightSubject<T> : LightweightObservableBase<T>, IAtomUISubject<T>
{
    public void OnCompleted()
    {
        PublishCompleted();
    }

    public void OnError(Exception error)
    {
        PublishError(error);
    }

    public void OnNext(T value)
    {
        PublishNext(value);
    }

    protected override void Initialize() { }

    protected override void Deinitialize() { }
}
