using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;

namespace AtomUI.Theme.Compilation;

internal sealed class ControlCompilationCache
{
    private readonly BoundedLruCache<ControlCompilationCacheKey, ControlThemeSnapshot> _entries;

    internal ControlCompilationCache(ThemeCacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _entries = new BoundedLruCache<ControlCompilationCacheKey, ControlThemeSnapshot>(
            options.ControlEntryLimit,
            options.ControlRetainedBytesLimit,
            static snapshot => snapshot.EstimatedRetainedBytes);
    }

    internal int Count => _entries.Count;
    internal long RetainedBytes => _entries.RetainedBytes;
    internal int InFlightCount => _entries.InFlightCount;

    internal ControlThemeSnapshot GetOrCompile(
        GlobalCompilationCacheKey globalKey,
        ControlTokenDescriptor descriptor,
        NormalizedControlThemeConfig? controlConfig,
        ThemeAppearance appearance,
        Func<ControlThemeSnapshot> factory)
    {
        ArgumentNullException.ThrowIfNull(globalKey);
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(factory);

        var key = ControlCompilationCacheKey.Create(
            globalKey,
            descriptor,
            controlConfig,
            appearance);
        return _entries.GetOrCreateAsync(
                           key,
                           _ => ValueTask.FromResult(factory()))
                       .GetAwaiter()
                       .GetResult();
    }
}
