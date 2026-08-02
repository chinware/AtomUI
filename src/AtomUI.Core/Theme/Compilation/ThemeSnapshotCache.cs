namespace AtomUI.Theme.Compilation;

internal sealed class ThemeSnapshotCache
{
    internal const int DefaultInactiveCapacity = 32;

    private readonly BoundedLruCache<ThemeSnapshotCacheKey, ThemeCompileResult> _entries;
    private readonly ControlCompilationCache _controlCompilations;

    internal ThemeSnapshotCache()
        : this(new ThemeCacheOptions())
    {
    }

    internal ThemeSnapshotCache(int inactiveCapacity)
        : this(new ThemeCacheOptions(SnapshotEntryLimit: inactiveCapacity))
    {
    }

    internal ThemeSnapshotCache(ThemeCacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _entries = new BoundedLruCache<ThemeSnapshotCacheKey, ThemeCompileResult>(
            options.SnapshotEntryLimit,
            options.SnapshotRetainedBytesLimit,
            static result => result.Success
                ? result.Snapshot!.EstimatedRetainedBytes
                : long.MaxValue);
        _controlCompilations = new ControlCompilationCache(options);
    }

    internal int Count => _entries.Count;
    internal long RetainedBytes => _entries.RetainedBytes;
    internal int InFlightCount => _entries.InFlightCount;
    internal int ControlCount => _controlCompilations.Count;
    internal long ControlRetainedBytes => _controlCompilations.RetainedBytes;
    internal int ControlInFlightCount => _controlCompilations.InFlightCount;

    internal ThemeCompileResult GetOrCompile(ThemeCompileInput input, ThemeCompiler compiler)
    {
        return GetOrCompileAsync(input, compiler).GetAwaiter().GetResult();
    }

    internal ThemeCompileResult GetOrCompile(
        ThemeSnapshotCacheKey key,
        ThemeCompileInput input,
        ThemeCompiler compiler)
    {
        return GetOrCompileAsync(key, input, compiler).GetAwaiter().GetResult();
    }

    internal ValueTask<ThemeCompileResult> GetOrCompileAsync(
        ThemeCompileInput input,
        ThemeCompiler compiler)
    {
        return GetOrCompileAsync(input, compiler, CancellationToken.None);
    }

    internal ValueTask<ThemeCompileResult> GetOrCompileAsync(
        ThemeCompileInput input,
        ThemeCompiler compiler,
        CancellationToken callerCancellation)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(compiler);

        var key = ThemeSnapshotCacheKey.Create(input);
        return GetOrCompileAsync(key, input, compiler, callerCancellation);
    }

    internal ValueTask<ThemeCompileResult> GetOrCompileAsync(
        ThemeSnapshotCacheKey key,
        ThemeCompileInput input,
        ThemeCompiler compiler)
    {
        return GetOrCompileAsync(key, input, compiler, CancellationToken.None);
    }

    internal ValueTask<ThemeCompileResult> GetOrCompileAsync(
        ThemeSnapshotCacheKey key,
        ThemeCompileInput input,
        ThemeCompiler compiler,
        CancellationToken callerCancellation)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(compiler);

        return _entries.GetOrCreateAsync(
            key,
            _ => ValueTask.FromResult(compiler.Compile(input, _controlCompilations)),
            callerCancellation);
    }
}
