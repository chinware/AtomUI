using System.Threading;
using System.Threading.Tasks;

namespace AtomUI.Desktop.Controls;

public interface ISplashService
{
    SplashWindow? CurrentWindow { get; }

    Task<Splash> ShowAsync(SplashOptions? options = null, CancellationToken cancellationToken = default);

    Task SetMessageAsync(string? message, string? detail = null, CancellationToken cancellationToken = default);

    Task SetProgressAsync(
        double? progress,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default);

    Task SetStatusAsync(
        SplashStatus status,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default);

    Task SetErrorAsync(string message, string? detail = null, CancellationToken cancellationToken = default);

    Task CloseAsync(CancellationToken cancellationToken = default);
}
