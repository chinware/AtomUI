using System.Runtime.CompilerServices;
using Avalonia;

namespace AtomUI;

internal static class ApplicationScopeRegistry
{
    private static readonly ConditionalWeakTable<Application, ApplicationScope> s_scopes = new();

    internal static ApplicationScope? Get(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return s_scopes.TryGetValue(application, out var scope) ? scope : null;
    }

    internal static void Register(Application application, ApplicationScope scope)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(scope);
        try
        {
            s_scopes.Add(application, scope);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                $"AtomUI is already initialized for Application '{application.GetType().FullName}'.",
                exception);
        }
    }

    internal static void Unregister(Application application, ApplicationScope scope)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(scope);
        if (s_scopes.TryGetValue(application, out var current) && ReferenceEquals(current, scope))
        {
            s_scopes.Remove(application);
        }
    }
}
