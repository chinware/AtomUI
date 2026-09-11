namespace AtomUI.Desktop.Controls;

public class DataGridSourceContractException : InvalidOperationException
{
    public DataGridSourceContractException(string message)
        : base(message)
    {
    }

    public DataGridSourceContractException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class DataGridPresentationLimitExceededException : InvalidOperationException
{
    public DataGridPresentationLimitExceededException(string message)
        : base(message)
    {
    }
}

internal sealed class DataGridPresentationInvariantException : InvalidOperationException
{
    public DataGridPresentationInvariantException(string message)
        : base(message)
    {
    }
}

public class DataGridSnapshotExpiredException : InvalidOperationException
{
    public DataGridSnapshotExpiredException(string message)
        : base(message)
    {
    }

    public DataGridSnapshotExpiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
