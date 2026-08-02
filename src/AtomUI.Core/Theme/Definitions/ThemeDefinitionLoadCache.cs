using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Definitions;

internal sealed class ThemeDefinitionLoadCache : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<ThemeSourceCacheKey, ThemeSourceReadCacheEntry> _reads = new();
    private readonly Dictionary<ThemeBindingCacheKey, ThemeBindingCacheEntry> _bindings = new();
    private bool _disposed;

    internal bool TryGetRead(
        ThemeSourceCacheKey key,
        out ThemeSourceReadCacheEntry? entry)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                entry = null;
                return false;
            }

            return _reads.TryGetValue(key, out entry);
        }
    }

    internal void StoreRead(
        ThemeSourceCacheKey key,
        ThemeSourceReadCacheEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        lock (_gate)
        {
            if (!_disposed)
            {
                _reads[key] = entry;
            }
        }
    }

    internal bool TryGetBinding(
        ThemeBindingCacheKey key,
        out ThemeBindingCacheEntry? entry)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                entry = null;
                return false;
            }

            return _bindings.TryGetValue(key, out entry);
        }
    }

    internal void StoreBinding(
        ThemeBindingCacheKey key,
        ThemeBindingCacheEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        lock (_gate)
        {
            if (!_disposed)
            {
                _bindings[key] = entry;
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _reads.Clear();
            _bindings.Clear();
        }
    }
}

internal readonly record struct ThemeSourceCacheKey(
    string SourceIdentity,
    string SourceRevision);

internal readonly record struct ThemeBindingCacheKey(
    ThemeSourceCacheKey Source,
    ThemeSchemaRevision RegistryRevision);

internal sealed record ThemeSourceReadCacheEntry(
    ThemeDocument Document,
    string ContentDigest,
    IReadOnlyList<ThemeDiagnostic> Diagnostics);

internal sealed record ThemeBindingCacheEntry(
    BoundThemeDefinition Definition,
    IReadOnlyList<ThemeDiagnostic> Diagnostics);
