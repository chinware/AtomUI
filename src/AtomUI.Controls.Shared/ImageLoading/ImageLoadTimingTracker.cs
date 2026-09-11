using System.Diagnostics;

namespace AtomUI.Controls;

internal sealed class ImageLoadTimingTracker
{
    private readonly object _gate = new();
    private readonly Dictionary<ImageLoadStage, TimeSpan> _durations = [];
    private ImageLoadStage? _currentStage;
    private long _currentStageStarted;

    internal IProgress<ImageLoadProgress> Wrap(IProgress<ImageLoadProgress>? progress)
    {
        return new TimingProgress(this, progress);
    }

    internal IReadOnlyDictionary<ImageLoadStage, TimeSpan> Snapshot()
    {
        lock (_gate)
        {
            var snapshot = new Dictionary<ImageLoadStage, TimeSpan>(_durations);
            if (_currentStage is { } currentStage)
            {
                AddDuration(snapshot, currentStage, Stopwatch.GetElapsedTime(_currentStageStarted));
            }
            return snapshot;
        }
    }

    private void Report(ImageLoadProgress value, IProgress<ImageLoadProgress>? progress)
    {
        lock (_gate)
        {
            var now = Stopwatch.GetTimestamp();
            if (_currentStage is { } currentStage)
            {
                AddDuration(_durations, currentStage, Stopwatch.GetElapsedTime(_currentStageStarted, now));
            }
            _currentStage = value.Stage;
            _currentStageStarted = now;
        }
        ImageProgressDispatcher.Report(progress, value);
    }

    private static void AddDuration(
        IDictionary<ImageLoadStage, TimeSpan> durations,
        ImageLoadStage stage,
        TimeSpan duration)
    {
        durations.TryGetValue(stage, out var current);
        durations[stage] = current + duration;
    }

    private sealed class TimingProgress : IProgress<ImageLoadProgress>
    {
        private readonly ImageLoadTimingTracker _owner;
        private readonly IProgress<ImageLoadProgress>? _progress;

        internal TimingProgress(
            ImageLoadTimingTracker owner,
            IProgress<ImageLoadProgress>? progress)
        {
            _owner = owner;
            _progress = progress;
        }

        public void Report(ImageLoadProgress value)
        {
            _owner.Report(value, _progress);
        }
    }
}
