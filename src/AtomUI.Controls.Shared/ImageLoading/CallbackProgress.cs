namespace AtomUI.Controls;

internal sealed class CallbackProgress<T> : IProgress<T>
{
    private readonly Action<T> _report;

    internal CallbackProgress(Action<T> report)
    {
        _report = report ?? throw new ArgumentNullException(nameof(report));
    }

    public void Report(T value)
    {
        _report(value);
    }
}
