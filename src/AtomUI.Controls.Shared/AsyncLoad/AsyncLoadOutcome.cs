namespace AtomUI.Controls.AsyncLoad;

public sealed class AsyncLoadOutcome<TResult> where TResult : class
{
    public AsyncLoadStatus Status { get; init; }
    public TResult? Result { get; init; }
    public Exception? Error { get; init; }

    public bool IsSuccess => Status == AsyncLoadStatus.Success;
    public bool IsTimedOut => Status == AsyncLoadStatus.TimedOut;
    public bool IsCancelled => Status == AsyncLoadStatus.Cancelled;
    public bool IsFaulted => Status == AsyncLoadStatus.Faulted;
    public bool IsSkipped => Status == AsyncLoadStatus.Skipped;

    public static AsyncLoadOutcome<TResult> Successful(TResult result)
        => new() { Status = AsyncLoadStatus.Success, Result = result };

    public static AsyncLoadOutcome<TResult> Cancel()
        => new() { Status = AsyncLoadStatus.Cancelled };

    public static AsyncLoadOutcome<TResult> Timeout()
        => new() { Status = AsyncLoadStatus.TimedOut };

    public static AsyncLoadOutcome<TResult> Fault(Exception error)
        => new() { Status = AsyncLoadStatus.Faulted, Error = error };

    public static AsyncLoadOutcome<TResult> Skip()
        => new() { Status = AsyncLoadStatus.Skipped };
}
