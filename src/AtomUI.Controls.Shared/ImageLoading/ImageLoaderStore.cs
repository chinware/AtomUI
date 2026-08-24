using System.Runtime.CompilerServices;
using Avalonia;

namespace AtomUI.Controls;

internal static class ImageLoaderStore
{
    private static readonly ConditionalWeakTable<Application, ImageLoader> s_loaders = new();

    internal static void Attach(Application application, ImageLoader loader)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(loader);
        try
        {
            s_loaders.Add(application, loader);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                $"An image loader is already attached to Application '{application.GetType().FullName}'.",
                exception);
        }
    }

    internal static ImageLoader? Get(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return s_loaders.TryGetValue(application, out var loader) && !loader.IsDisposed
            ? loader
            : null;
    }

    internal static void Detach(Application application, ImageLoader loader)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(loader);
        if (!s_loaders.TryGetValue(application, out var current))
        {
            return;
        }
        if (!ReferenceEquals(current, loader))
        {
            throw new InvalidOperationException("The attached image loader does not match the requested instance.");
        }
        s_loaders.Remove(application);
    }
}
