namespace AtomUI.Desktop.Controls;

public partial class Splash
{
    private static ISplashService s_defaultService = new SplashService();

    public static ISplashService DefaultService
    {
        get => s_defaultService;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            s_defaultService = value;
        }
    }

    public static Task<Splash> ShowAsync(
        SplashOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return DefaultService.ShowAsync(options, cancellationToken);
    }

    public static Task SetMessageAsync(
        string? message,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return DefaultService.SetMessageAsync(message, detail, cancellationToken);
    }

    public static Task SetProgressAsync(
        double? progress,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return DefaultService.SetProgressAsync(progress, message, detail, cancellationToken);
    }

    public static Task SetStatusAsync(
        SplashStatus status,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return DefaultService.SetStatusAsync(status, message, detail, cancellationToken);
    }

    public static Task SetErrorAsync(
        string message,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return DefaultService.SetErrorAsync(message, detail, cancellationToken);
    }

    public static Task CloseAsync(CancellationToken cancellationToken = default)
    {
        return DefaultService.CloseAsync(cancellationToken);
    }
}
