namespace AtomUI.Controls;

internal sealed class ImageResultLease : IDisposable
{
    private Action? _release;

    internal ImageResultLease(Action release)
    {
        _release = release ?? throw new ArgumentNullException(nameof(release));
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _release, null)?.Invoke();
    }
}
