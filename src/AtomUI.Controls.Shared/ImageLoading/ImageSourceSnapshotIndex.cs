namespace AtomUI.Controls;

internal sealed class ImageSourceSnapshotIndex
{
    private readonly object _gate = new();
    private readonly Dictionary<ImageSourceKey, ImageSourceSnapshot> _snapshots = [];
    private readonly Dictionary<ImageSourceKey, long> _nextGenerations = [];

    internal bool TryGet(ImageSourceKey key, out ImageSourceSnapshot? snapshot)
    {
        lock (_gate)
        {
            return _snapshots.TryGetValue(key, out snapshot);
        }
    }

    internal long BeginResolution(ImageSourceKey key)
    {
        lock (_gate)
        {
            _nextGenerations.TryGetValue(key, out var current);
            var next = checked(current + 1);
            _nextGenerations[key] = next;
            return next;
        }
    }

    internal bool TryCommit(ImageSourceSnapshot snapshot)
    {
        lock (_gate)
        {
            if (_snapshots.TryGetValue(snapshot.SourceKey, out var current) &&
                current.CommitGeneration > snapshot.CommitGeneration)
            {
                return false;
            }
            _snapshots[snapshot.SourceKey] = snapshot;
            _nextGenerations.TryGetValue(snapshot.SourceKey, out var next);
            if (snapshot.CommitGeneration > next)
            {
                _nextGenerations[snapshot.SourceKey] = snapshot.CommitGeneration;
            }
            return true;
        }
    }

    internal void Remove(ImageSourceKey key)
    {
        lock (_gate)
        {
            _snapshots.Remove(key);
        }
    }

    internal bool RemoveIfNotNewer(ImageSourceKey key, long generation)
    {
        lock (_gate)
        {
            if (_snapshots.TryGetValue(key, out var current) &&
                current.CommitGeneration > generation)
            {
                return false;
            }
            _snapshots.Remove(key);
            return true;
        }
    }

    internal void Clear(string? partitionHash)
    {
        lock (_gate)
        {
            if (partitionHash is null)
            {
                _snapshots.Clear();
                _nextGenerations.Clear();
                return;
            }
            foreach (var key in _snapshots.Keys.Where(key => key.PartitionHash == partitionHash).ToArray())
            {
                _snapshots.Remove(key);
            }
            foreach (var key in _nextGenerations.Keys.Where(key => key.PartitionHash == partitionHash).ToArray())
            {
                _nextGenerations.Remove(key);
            }
        }
    }
}
