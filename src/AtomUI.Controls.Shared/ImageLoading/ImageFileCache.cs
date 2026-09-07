namespace AtomUI.Controls;

internal sealed class ImageFileCache : IDisposable
{
    private const int FormatRevision = 1;
    private const string SharedPartition = "shared";
    private static readonly TimeSpan s_artifactGracePeriod = TimeSpan.FromDays(1);

    private readonly string _directory;
    private readonly string _contentDirectory;
    private readonly string _contentMetadataDirectory;
    private readonly string _sourceDirectory;
    private readonly string _lockDirectory;
    private readonly string _tempDirectory;
    private readonly string _manifestPath;
    private readonly long _maxBytes;
    private readonly int _maxEntries;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private int _disposed;

    internal ImageFileCache(string directory, long maxBytes, int maxEntries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        _contentDirectory = Path.Combine(_directory, "content");
        _contentMetadataDirectory = Path.Combine(_directory, "content-metadata");
        _sourceDirectory = Path.Combine(_directory, "sources");
        _lockDirectory = Path.Combine(_directory, "locks");
        _tempDirectory = Path.Combine(_directory, "temp");
        _manifestPath = Path.Combine(_directory, "manifest");
        _maxBytes = maxBytes;
        _maxEntries = maxEntries;
        InitializeLayout();
    }

    internal async Task<ImageEncodedContent?> TryGetContentAsync(
        ImageEncodedContentKey key,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var contentPath = GetContentPath(key);
            var metadataPath = GetContentMetadataPath(key);
            if (!File.Exists(contentPath) || !File.Exists(metadataPath))
            {
                RemoveContentCore(key);
                return null;
            }
            try
            {
                await using var metadataStream = new FileStream(
                    metadataPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                    4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                var metadata = ImageStoredContentMetadata.Read(metadataStream);
                if (metadata.PartitionHash != key.PartitionHash ||
                    metadata.ContentId != key.ContentId.Value ||
                    metadata.SecurityPolicyVersion != ImageSecurityPolicy.Version)
                {
                    RemoveContentCore(key);
                    return null;
                }
                var bytes = await File.ReadAllBytesAsync(contentPath, cancellationToken).ConfigureAwait(false);
                if (bytes.LongLength != metadata.ContentLength ||
                    ImageCacheKey.HashBytes(bytes) != metadata.ContentDigest ||
                    ImageContentId.Create(bytes) != key.ContentId)
                {
                    RemoveContentCore(key);
                    return null;
                }
                File.SetLastWriteTimeUtc(metadataPath, DateTime.UtcNow);
                return metadata.ToContent(bytes);
            }
            catch (Exception exception) when (IsCacheReadFailure(exception))
            {
                RemoveContentCore(key);
                return null;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task<ImageSourceSnapshot?> TryGetSourceSnapshotAsync(
        ImageSourceKey key,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var path = GetSourcePath(key);
            if (!File.Exists(path))
            {
                return null;
            }
            try
            {
                await using var stream = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.Read,
                    4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                var stored = ImageStoredSourceSnapshot.Read(stream);
                if (stored.PartitionHash != key.PartitionHash ||
                    stored.SourceKey != key.Value ||
                    stored.SecurityPolicyVersion != ImageSecurityPolicy.Version)
                {
                    File.Delete(path);
                    return null;
                }
                return stored.ToSnapshot();
            }
            catch (Exception exception) when (IsCacheReadFailure(exception))
            {
                TryDelete(path);
                return null;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task<bool> SetContentAsync(
        ImageEncodedContentKey key,
        ImageEncodedContent content,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (content.NoStore ||
            content.SecurityPolicyVersion != ImageSecurityPolicy.Version ||
            content.ContentId != key.ContentId ||
            content.Size > _maxBytes)
        {
            return false;
        }
        content = content.ForContentStore();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await using var cacheLock = await AcquireLockAsync(
                $"content-{key.PartitionHash}-{key.ContentId.Value}", cancellationToken).ConfigureAwait(false);
            var contentPath = GetContentPath(key);
            var metadataPath = GetContentMetadataPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(contentPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(metadataPath)!);
            await AtomicWriteAsync(
                contentPath,
                stream => stream.WriteAsync(content.Bytes, cancellationToken).AsTask(),
                cancellationToken).ConfigureAwait(false);
            var metadata = ImageStoredContentMetadata.FromContent(key, content);
            await AtomicWriteAsync(
                metadataPath,
                stream =>
                {
                    metadata.Write(stream);
                    return Task.CompletedTask;
                },
                cancellationToken).ConfigureAwait(false);
            TrimCore();
            return File.Exists(contentPath) && File.Exists(metadataPath);
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task SetSourceSnapshotAsync(
        ImageSourceSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var contentKey = new ImageEncodedContentKey(
                snapshot.SourceKey.PartitionHash,
                snapshot.ContentId);
            if (!File.Exists(GetContentPath(contentKey)) || !File.Exists(GetContentMetadataPath(contentKey)))
            {
                return;
            }
            await using var cacheLock = await AcquireLockAsync(
                $"source-{snapshot.SourceKey.PartitionHash}-{snapshot.SourceKey.Value}", cancellationToken)
                .ConfigureAwait(false);
            var path = GetSourcePath(snapshot.SourceKey);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            if (File.Exists(path))
            {
                try
                {
                    await using var currentStream = new FileStream(
                        path, FileMode.Open, FileAccess.Read, FileShare.Read,
                        4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    var current = ImageStoredSourceSnapshot.Read(currentStream);
                    if (current.CommitGeneration > snapshot.CommitGeneration)
                    {
                        return;
                    }
                }
                catch (Exception exception) when (IsCacheReadFailure(exception))
                {
                    TryDelete(path);
                }
            }
            var stored = ImageStoredSourceSnapshot.FromSnapshot(snapshot);
            await AtomicWriteAsync(
                path,
                stream =>
                {
                    stored.Write(stream);
                    return Task.CompletedTask;
                },
                cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task RemoveContentAsync(
        ImageEncodedContentKey key,
        CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            RemoveContentCore(key);
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task<bool> RemoveSourceSnapshotAsync(
        ImageSourceKey key,
        CancellationToken cancellationToken,
        long? maximumGeneration = null)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await using var cacheLock = await AcquireLockAsync(
                $"source-{key.PartitionHash}-{key.Value}", cancellationToken).ConfigureAwait(false);
            var path = GetSourcePath(key);
            if (maximumGeneration is not null && File.Exists(path))
            {
                try
                {
                    await using var stream = new FileStream(
                        path, FileMode.Open, FileAccess.Read, FileShare.Read,
                        4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                    var current = ImageStoredSourceSnapshot.Read(stream);
                    if (current.CommitGeneration > maximumGeneration.Value)
                    {
                        return false;
                    }
                }
                catch (Exception exception) when (IsCacheReadFailure(exception))
                {
                }
            }
            TryDelete(path);
            return !File.Exists(path);
        }
        finally
        {
            _gate.Release();
        }
    }

    internal async Task ClearAsync(string? partitionHash, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (partitionHash is null)
            {
                RecreateDirectory(_contentDirectory);
                RecreateDirectory(_contentMetadataDirectory);
                RecreateDirectory(_sourceDirectory);
                RecreateDirectory(_tempDirectory);
            }
            else
            {
                var segment = PartitionSegment(partitionHash);
                DeleteDirectory(Path.Combine(_contentDirectory, segment));
                DeleteDirectory(Path.Combine(_contentMetadataDirectory, segment));
                DeleteDirectory(Path.Combine(_sourceDirectory, segment));
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _disposed, 1);
    }

    private void InitializeLayout()
    {
        Directory.CreateDirectory(_directory);
        var current = TryReadManifest();
        if (current != FormatRevision)
        {
            DeleteDirectory(_contentDirectory);
            DeleteDirectory(_contentMetadataDirectory);
            DeleteDirectory(_sourceDirectory);
            DeleteDirectory(_tempDirectory);
        }
        Directory.CreateDirectory(_contentDirectory);
        Directory.CreateDirectory(_contentMetadataDirectory);
        Directory.CreateDirectory(_sourceDirectory);
        Directory.CreateDirectory(_lockDirectory);
        Directory.CreateDirectory(_tempDirectory);
        WriteManifest();
        CleanupStaleArtifacts();
    }

    private void CleanupStaleArtifacts()
    {
        var cutoff = DateTime.UtcNow - s_artifactGracePeriod;
        DeleteOldFiles(_tempDirectory, "*", cutoff);
        DeleteOrphans(_contentDirectory, _contentMetadataDirectory, ".bin", ".meta", cutoff);
        DeleteOrphans(_contentMetadataDirectory, _contentDirectory, ".meta", ".bin", cutoff);
    }

    private static void DeleteOldFiles(string directory, string pattern, DateTime cutoff)
    {
        foreach (var path in Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(path) < cutoff)
                {
                    TryDelete(path);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    private static void DeleteOrphans(
        string sourceRoot,
        string companionRoot,
        string sourceExtension,
        string companionExtension,
        DateTime cutoff)
    {
        foreach (var sourcePath in Directory.EnumerateFiles(
                     sourceRoot,
                     "*" + sourceExtension,
                     SearchOption.AllDirectories))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(sourcePath) >= cutoff)
                {
                    continue;
                }
                var relative = Path.GetRelativePath(sourceRoot, sourcePath);
                var companionRelative = Path.ChangeExtension(relative, companionExtension);
                if (!File.Exists(Path.Combine(companionRoot, companionRelative)))
                {
                    TryDelete(sourcePath);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    private void WriteManifest()
    {
        var tempPath = Path.Combine(_tempDirectory, Guid.NewGuid().ToString("N") + ".manifest.tmp");
        try
        {
            using (var stream = new FileStream(
                       tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                       4096, FileOptions.SequentialScan))
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.Write($"formatRevision={FormatRevision}\n");
                writer.Flush();
                stream.Flush(flushToDisk: true);
            }
            File.Move(tempPath, _manifestPath, overwrite: true);
        }
        finally
        {
            TryDelete(tempPath);
        }
    }

    private int? TryReadManifest()
    {
        try
        {
            if (!File.Exists(_manifestPath))
            {
                return null;
            }
            const string prefix = "formatRevision=";
            var text = File.ReadAllText(_manifestPath).Trim();
            return text.StartsWith(prefix, StringComparison.Ordinal) &&
                   int.TryParse(text[prefix.Length..], out var revision)
                ? revision
                : -1;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return -1;
        }
    }

    private async Task<FileStream> AcquireLockAsync(string identity, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_lockDirectory, ImageCacheKey.Hash(identity) + ".lock");
        for (var attempt = 0; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return new FileStream(
                    path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                    1, FileOptions.Asynchronous);
            }
            catch (IOException) when (attempt < 40)
            {
                await Task.Delay(25, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task AtomicWriteAsync(
        string targetPath,
        Func<FileStream, Task> write,
        CancellationToken cancellationToken)
    {
        var tempPath = Path.Combine(_tempDirectory, Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            await using (var stream = new FileStream(
                             tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                             64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                await write(stream).ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                stream.Flush(flushToDisk: true);
            }
            File.Move(tempPath, targetPath, overwrite: true);
        }
        finally
        {
            TryDelete(tempPath);
        }
    }

    private void TrimCore()
    {
        var metadataFiles = Directory.EnumerateFiles(
                _contentMetadataDirectory, "*.meta", SearchOption.AllDirectories)
            .Select(path => new FileInfo(path))
            .OrderByDescending(info => info.LastWriteTimeUtc)
            .ToArray();
        long retainedBytes = 0;
        for (var index = 0; index < metadataFiles.Length; index++)
        {
            var metadata = metadataFiles[index];
            ImageStoredContentMetadata? stored = null;
            try
            {
                using var stream = metadata.OpenRead();
                stored = ImageStoredContentMetadata.Read(stream);
            }
            catch (Exception exception) when (IsCacheReadFailure(exception))
            {
            }
            var retain = stored is not null &&
                         index < _maxEntries &&
                         retainedBytes + stored.ContentLength <= _maxBytes;
            if (retain)
            {
                retainedBytes += stored!.ContentLength;
                continue;
            }
            if (stored is not null)
            {
                RemoveContentCore(new ImageEncodedContentKey(
                    stored.PartitionHash,
                    new ImageContentId(stored.ContentId)));
            }
            else
            {
                TryDelete(metadata.FullName);
            }
        }
    }

    private string GetContentPath(ImageEncodedContentKey key)
    {
        var prefix = key.ContentId.Value[..2];
        return Path.Combine(
            _contentDirectory,
            PartitionSegment(key.PartitionHash),
            prefix,
            key.ContentId.Value + ".bin");
    }

    private string GetContentMetadataPath(ImageEncodedContentKey key)
    {
        var prefix = key.ContentId.Value[..2];
        return Path.Combine(
            _contentMetadataDirectory,
            PartitionSegment(key.PartitionHash),
            prefix,
            key.ContentId.Value + ".meta");
    }

    private string GetSourcePath(ImageSourceKey key) =>
        Path.Combine(_sourceDirectory, PartitionSegment(key.PartitionHash), key.Value + ".meta");

    private static string PartitionSegment(string partitionHash) =>
        partitionHash.Length == 0 ? SharedPartition : partitionHash;

    private void RemoveContentCore(ImageEncodedContentKey key)
    {
        TryDelete(GetContentPath(key));
        TryDelete(GetContentMetadataPath(key));
    }

    private static bool IsCacheReadFailure(Exception exception) =>
        exception is IOException or UnauthorizedAccessException or InvalidDataException or EndOfStreamException;

    private static void RecreateDirectory(string path)
    {
        DeleteDirectory(path);
        Directory.CreateDirectory(path);
    }

    private static void DeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

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
}
