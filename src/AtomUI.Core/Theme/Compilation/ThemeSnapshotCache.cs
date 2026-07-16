using System.Text;
using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Compilation;

internal sealed class ThemeSnapshotCache
{
    internal const int DefaultInactiveCapacity = 32;

    private readonly int _inactiveCapacity;
    private readonly Dictionary<ThemeSnapshotCacheKey, CacheEntry> _entries;
    private readonly Dictionary<ThemeSnapshot, ThemeSnapshotCacheKey> _snapshotKeys;
    private long _accessClock;

    internal ThemeSnapshotCache(int inactiveCapacity = DefaultInactiveCapacity)
    {
        if (inactiveCapacity < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(inactiveCapacity),
                inactiveCapacity,
                "Inactive snapshot cache capacity must be positive.");
        }

        _inactiveCapacity = inactiveCapacity;
        _entries          = new Dictionary<ThemeSnapshotCacheKey, CacheEntry>();
        _snapshotKeys     = new Dictionary<ThemeSnapshot, ThemeSnapshotCacheKey>();
    }

    internal ThemeCompileResult GetOrCompile(ThemeCompileRequest request, ThemeCompiler compiler)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(compiler);

        var key = request.CreateSnapshotCacheKey();
        if (_entries.TryGetValue(key, out var existing))
        {
            existing.LastAccess = NextAccess();
            return existing.Result;
        }

        var result = compiler.Compile(request);
        if (!result.Success)
        {
            return result;
        }

        if (_entries.TryGetValue(key, out existing))
        {
            existing.LastAccess = NextAccess();
            return existing.Result;
        }

        var entry = new CacheEntry(result, NextAccess());
        _entries.Add(key, entry);
        _snapshotKeys[result.Snapshot!] = key;
        TrimInactiveEntries();
        return result;
    }

    internal IDisposable Pin(ThemeSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (!_snapshotKeys.TryGetValue(snapshot, out var key) ||
            !_entries.TryGetValue(key, out var entry))
        {
            return DisposableAction.Empty;
        }

        entry.PinCount++;
        return new DisposableAction(() =>
        {
            if (!_entries.TryGetValue(key, out var pinnedEntry))
            {
                return;
            }

            if (pinnedEntry.PinCount > 0)
            {
                pinnedEntry.PinCount--;
            }
            TrimInactiveEntries();
        });
    }

    private long NextAccess()
    {
        return ++_accessClock;
    }

    private void TrimInactiveEntries()
    {
        while (InactiveCount() > _inactiveCapacity)
        {
            var hasOldestKey = false;
            var oldestKey = default(ThemeSnapshotCacheKey);
            CacheEntry? oldestEntry = null;
            foreach (var (key, entry) in _entries)
            {
                if (entry.PinCount != 0)
                {
                    continue;
                }

                if (oldestEntry is null || entry.LastAccess < oldestEntry.LastAccess)
                {
                    hasOldestKey = true;
                    oldestKey   = key;
                    oldestEntry = entry;
                }
            }

            if (!hasOldestKey || oldestEntry is null)
            {
                return;
            }

            _entries.Remove(oldestKey);
            _snapshotKeys.Remove(oldestEntry.Result.Snapshot!);
        }
    }

    private int InactiveCount()
    {
        var count = 0;
        foreach (var entry in _entries.Values)
        {
            if (entry.PinCount == 0)
            {
                count++;
            }
        }

        return count;
    }

    private sealed class CacheEntry
    {
        internal CacheEntry(ThemeCompileResult result, long lastAccess)
        {
            Result     = result;
            LastAccess = lastAccess;
        }

        internal ThemeCompileResult Result { get; }
        internal long LastAccess { get; set; }
        internal int PinCount { get; set; }
    }

    private sealed class DisposableAction : IDisposable
    {
        internal static readonly IDisposable Empty = new DisposableAction(null);

        private Action? _dispose;

        internal DisposableAction(Action? dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}

internal readonly record struct ThemeSnapshotCacheKey(
    string ThemeId,
    string Definition,
    long? ParentVersion,
    string Algorithms,
    string SharedOverrides,
    string ControlOverrides,
    string Registrations,
    string RuntimeOverrides)
{
    internal static ThemeSnapshotCacheKey Create(ThemeCompileRequest request)
    {
        return new ThemeSnapshotCacheKey(
            request.ThemeId,
            BuildDefinitionKey(request.Definition),
            request.Parent?.Version,
            BuildAlgorithmKey(request.Algorithms),
            BuildStringMapKey(request.SharedOverrides),
            BuildControlOverridesKey(request.ControlOverrides),
            BuildRegistrationKey(request.Registrations),
            BuildStringMapKey(request.RuntimeOverrides));
    }

    private static string BuildDefinitionKey(ThemeDefinition definition)
    {
        var builder = new StringBuilder();
        AppendValue(builder, definition.Id);
        AppendValue(builder, definition.DisplayName);
        AppendValue(builder, definition.IsDefault ? "1" : "0");
        AppendValue(builder, BuildAlgorithmKey(definition.Algorithms));
        AppendValue(builder, BuildStringMapKey(definition.SharedTokens));
        foreach (var control in definition.ControlTokens
                                            .OrderBy(static entry => entry.Key, StringComparer.Ordinal))
        {
            AppendValue(builder, control.Key);
            AppendValue(builder, control.Value.TokenId);
            AppendValue(builder, control.Value.EnableAlgorithm ? "1" : "0");
            AppendValue(builder, BuildStringMapKey(control.Value.Tokens));
            AppendValue(builder, BuildStringMapKey(control.Value.SharedTokens));
        }

        return builder.ToString();
    }

    private static string BuildAlgorithmKey(IEnumerable<ThemeAlgorithm> algorithms)
    {
        var builder = new StringBuilder();
        foreach (var algorithm in algorithms)
        {
            AppendValue(builder, ((int)algorithm).ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    private static string BuildStringMapKey(IEnumerable<KeyValuePair<string, string>> values)
    {
        var builder = new StringBuilder();
        foreach (var entry in values.OrderBy(static entry => entry.Key, StringComparer.Ordinal))
        {
            AppendValue(builder, entry.Key);
            AppendValue(builder, entry.Value);
        }

        return builder.ToString();
    }

    private static string BuildControlOverridesKey(
        IEnumerable<KeyValuePair<ControlTokenIdentity, ControlTokenConfigInfo>> values)
    {
        var builder = new StringBuilder();
        foreach (var entry in values.OrderBy(static entry => entry.Key.ResourceCatalog, StringComparer.Ordinal)
                                    .ThenBy(static entry => entry.Key.TokenId, StringComparer.Ordinal))
        {
            AppendValue(builder, entry.Key.ResourceCatalog ?? string.Empty);
            AppendValue(builder, entry.Key.TokenId);
            AppendValue(builder, entry.Value.TokenId);
            AppendValue(builder, entry.Value.EnableAlgorithm ? "1" : "0");
            AppendValue(builder, BuildStringMapKey(entry.Value.Tokens));
            AppendValue(builder, BuildStringMapKey(entry.Value.SharedTokens));
        }

        return builder.ToString();
    }

    private static string BuildRegistrationKey(IEnumerable<ControlTokenRegistration> registrations)
    {
        var builder = new StringBuilder();
        foreach (var registration in registrations.OrderBy(static registration => registration.TokenType.AssemblyQualifiedName, StringComparer.Ordinal)
                                                  .ThenBy(static registration => registration.ResourceCatalog, StringComparer.Ordinal)
                                                  .ThenBy(static registration => registration.TokenId, StringComparer.Ordinal))
        {
            AppendValue(builder, registration.TokenType.AssemblyQualifiedName ?? registration.TokenType.FullName ?? registration.TokenType.Name);
            AppendValue(builder, registration.ResourceCatalog ?? string.Empty);
            AppendValue(builder, registration.TokenId ?? string.Empty);
        }

        return builder.ToString();
    }

    private static void AppendValue(StringBuilder builder, string value)
    {
        builder.Append(value.Length);
        builder.Append(':');
        builder.Append(value);
        builder.Append('|');
    }
}
