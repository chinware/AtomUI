namespace AtomUI.Exceptions;

public class BootstrapException : InvalidOperationException
{
    public BootstrapException(string? message)
        : base(message)
    {
    }

    public BootstrapException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}