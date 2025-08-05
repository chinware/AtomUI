namespace AtomUI.Theme;

public class AtomUIBootstrapException : InvalidOperationException
{
    public AtomUIBootstrapException(string? message)
        : base(message)
    {
    }

    public AtomUIBootstrapException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}