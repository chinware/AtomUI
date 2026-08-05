using System.Runtime.CompilerServices;
using Avalonia;

namespace AtomUI;

internal static class AtomUIApplicationRuntimeStore
{
    private static readonly ConditionalWeakTable<Application, AtomUIApplicationRuntime> s_runtimes = new();

    internal static AtomUIApplicationRuntime? Get(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return s_runtimes.TryGetValue(application, out var runtime) ? runtime : null;
    }

    internal static void Attach(Application application, AtomUIApplicationRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(runtime);
        try
        {
            s_runtimes.Add(application, runtime);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                $"AtomUI is already initialized for Application '{application.GetType().FullName}'.",
                exception);
        }
    }

    internal static void Detach(Application application, AtomUIApplicationRuntime runtime)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(runtime);
        if (s_runtimes.TryGetValue(application, out var current) && ReferenceEquals(current, runtime))
        {
            s_runtimes.Remove(application);
        }
    }
}
