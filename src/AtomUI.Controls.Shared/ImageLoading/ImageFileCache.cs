namespace AtomUI.Controls;

internal sealed class ImageFileCache : IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _directory;
    private readonly long _maxBytes;
    private readonly int _maxEntries;
    private readonly object _lifecycleGate = new();
    private int _activeOperations;
    private int _disposed;
    private bool _gateDisposed;

    internal ImageFileCache(string directory, long maxBytes, int maxEntries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        _maxBytes = maxBytes;
        _maxEntries = maxEntries;
        Directory.CreateDirectory(_directory);
        RecoverCore();
        TrimCore();
    }

    internal async Task<ImageEncodedContent?> TryGetAsync(
        ImageEncodedCacheKey key,
        CancellationToken cancellationToken)
    {
        await EnterAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var dataPath = GetDataPath(key);
            var metadataPath = GetMetadataPath(key);
            await using var keyLock = TryAcquireKeyLock(key);
            if (keyLock is null)
            {
                return null;
            }
            if (!File.Exists(dataPath) || !File.Exists(metadataPath))
            {
                return null;
            }
            try
            {
                ImageFileCacheMetadata metadata;
                await using (var metadataStream = new FileStream(
                    metadataPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    if (metadataStream.Length > 64 * 1024)
                    {
                        throw new InvalidDataException("Image cache metadata is too large.");
                    }
                    metadata = ImageFileCacheMetadata.Read(metadataStream);
                }
                if (metadata.PartitionHash != key.PartitionHash ||
                    metadata.SecurityPolicyVersion != 1 ||
                    metadata.ContentLength < 0 ||
                    metadata.ContentLength > _maxBytes)
                {
                    DeleteEntryCore(key);
                    return null;
                }
                var bytes = await File.ReadAllBytesAsync(dataPath, cancellationToken).ConfigureAwait(false);
                if (bytes.LongLength != metadata.ContentLength ||
                    ImageCacheKey.HashBytes(bytes) != metadata.ContentDigest)
                {
                    DeleteEntryCore(key);
                    return null;
                }
                File.SetLastAccessTimeUtc(dataPath, DateTime.UtcNow);
                File.SetLastAccessTimeUtc(metadataPath, DateTime.UtcNow);
                return metadata.ToContent(bytes);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException)
            {
                DeleteEntryCore(key);
                return null;
            }
        }
        finally
        {
            _gate.Release();
            ExitOperation();
        }
    }

    internal async Task SetAsync(
        ImageEncodedCacheKey key,
        ImageEncodedContent content,
        CancellationToken cancellationToken)
    {
        if (content.NoStore || content.Size > _maxBytes)
        {
            return;
        }
        await EnterAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            Directory.CreateDirectory(_directory);
            var dataPath = GetDataPath(key);
            var metadataPath = GetMetadataPath(key);
            await using var keyLock = TryAcquireKeyLock(key);
            if (keyLock is null)
            {
                return;
            }
            var dataTemp = dataPath + $".{Guid.NewGuid():N}.tmp";
            var metadataTemp = metadataPath + $".{Guid.NewGuid():N}.tmp";
            try
            {
                await File.WriteAllBytesAsync(dataTemp, content.Bytes, cancellationToken).ConfigureAwait(false);
                await using (var metadataStream = new FileStream(
                    metadataTemp,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    4096,
                    FileOptions.Asynchronous))
                {
                    ImageFileCacheMetadata.FromContent(key.PartitionHash, content).Write(metadataStream);
                    await metadataStream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                File.Move(dataTemp, dataPath, overwrite: true);
                File.Move(metadataTemp, metadataPath, overwrite: true);
            }
            finally
            {
                TryDelete(dataTemp);
                TryDelete(metadataTemp);
            }
            TrimCore();
        }
        finally
        {
            _gate.Release();
            ExitOperation();
        }
    }

    internal async Task ClearAsync(string? partitionHash, CancellationToken cancellationToken)
    {
        await EnterAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            foreach (var metadataPath in Directory.EnumerateFiles(_directory, "*.meta"))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var key = Path.GetFileNameWithoutExtension(metadataPath);
                string? entryPartition = null;
                if (partitionHash is not null)
                {
                    try
                    {
                        using var stream = File.OpenRead(metadataPath);
                        entryPartition = ImageFileCacheMetadata.Read(stream).PartitionHash;
                        if (entryPartition != partitionHash)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        // Corrupt metadata is removed by the same clear pass.
                    }
                }
                else
                {
                    try
                    {
                        using var stream = File.OpenRead(metadataPath);
                        entryPartition = ImageFileCacheMetadata.Read(stream).PartitionHash;
                    }
                    catch
                    {
                        // Corrupt metadata is removed by the same clear pass.
                    }
                }
                await using var keyLock = TryAcquireKeyLock(
                    new ImageEncodedCacheKey(key, entryPartition ?? string.Empty));
                if (keyLock is null)
                {
                    continue;
                }
                TryDelete(Path.Combine(_directory, key + ".bin"));
                TryDelete(metadataPath);
            }
        }
        finally
        {
            _gate.Release();
            ExitOperation();
        }
    }

    internal async Task RemoveAsync(
        ImageEncodedCacheKey key,
        CancellationToken cancellationToken)
    {
        await EnterAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            await using var keyLock = TryAcquireKeyLock(key);
            if (keyLock is null)
            {
                return;
            }
            DeleteEntryCore(key);
        }
        finally
        {
            _gate.Release();
            ExitOperation();
        }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _disposed, 1);
        lock (_lifecycleGate)
        {
            DisposeGateIfIdle();
        }
    }

    private void TrimCore()
    {
        var entries = Directory.EnumerateFiles(_directory, "*.bin")
            .Select(path => new FileInfo(path))
            .OrderBy(info => info.LastAccessTimeUtc)
            .ToList();
        var bytes = entries.Sum(info => info.Length);
        var index = 0;
        while ((bytes > _maxBytes || entries.Count - index > _maxEntries) && index < entries.Count)
        {
            var entry = entries[index++];
            bytes -= entry.Length;
            var key = Path.GetFileNameWithoutExtension(entry.Name);
            using var keyLock = TryAcquireKeyLock(new ImageEncodedCacheKey(key, string.Empty));
            if (keyLock is null)
            {
                bytes += entry.Length;
                continue;
            }
            TryDelete(entry.FullName);
            TryDelete(Path.Combine(_directory, key + ".meta"));
        }
    }

    private void RecoverCore()
    {
        foreach (var tempPath in Directory.EnumerateFiles(_directory, "*.tmp"))
        {
            TryDelete(tempPath);
        }
        foreach (var dataPath in Directory.EnumerateFiles(_directory, "*.bin"))
        {
            var key = Path.GetFileNameWithoutExtension(dataPath);
            if (!File.Exists(Path.Combine(_directory, key + ".meta")))
            {
                using var keyLock = TryAcquireKeyLock(new ImageEncodedCacheKey(key, string.Empty));
                if (keyLock is not null)
                {
                    TryDelete(dataPath);
                }
            }
        }
        foreach (var metadataPath in Directory.EnumerateFiles(_directory, "*.meta"))
        {
            var key = Path.GetFileNameWithoutExtension(metadataPath);
            if (!File.Exists(Path.Combine(_directory, key + ".bin")))
            {
                using var keyLock = TryAcquireKeyLock(new ImageEncodedCacheKey(key, string.Empty));
                if (keyLock is not null)
                {
                    TryDelete(metadataPath);
                }
            }
        }
    }

    private void DeleteEntryCore(ImageEncodedCacheKey key)
    {
        TryDelete(GetDataPath(key));
        TryDelete(GetMetadataPath(key));
    }

    private string GetDataPath(ImageEncodedCacheKey key) => Path.Combine(_directory, key.Value + ".bin");

    private string GetMetadataPath(ImageEncodedCacheKey key) => Path.Combine(_directory, key.Value + ".meta");

    private FileStream? TryAcquireKeyLock(ImageEncodedCacheKey key)
    {
        try
        {
            return new FileStream(
                GetLockPath(key),
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.None,
                1,
                FileOptions.Asynchronous);
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private string GetLockPath(ImageEncodedCacheKey key) => Path.Combine(_directory, key.Value + ".lock");

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }

    private async Task EnterAsync(CancellationToken cancellationToken)
    {
        lock (_lifecycleGate)
        {
            ThrowIfDisposed();
            _activeOperations++;
        }
        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            ExitOperation();
            throw;
        }
    }

    private void ExitOperation()
    {
        lock (_lifecycleGate)
        {
            _activeOperations--;
            DisposeGateIfIdle();
        }
    }

    private void DisposeGateIfIdle()
    {
        if (Volatile.Read(ref _disposed) != 0 && _activeOperations == 0 && !_gateDisposed)
        {
            _gateDisposed = true;
            _gate.Dispose();
        }
    }
}
