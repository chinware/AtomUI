using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Threading;
namespace AtomUI.Desktop.Controls;

/// <summary>
/// 预览窗口显示状态机：跟踪当前目标 entry，以"最近持有可用 FullImage 的 entry"作为
/// 保留帧回退，直到目标加载完成。ImagePreviewerDialog 与 ImagePreviewerOverlayHost 共用。
/// 两种 ImageSwitchMode 都保持显示连续性（切换→加载完成的间隙不产生空白帧）；
/// Immediate 模式在目标超过 <see cref="PlaceholderGraceMilliseconds"/> 仍无图时
/// 回退加载占位（短暂加载直接换图，真正慢的加载仍呈现占位），WaitForLoaded 无限期保持。
/// 资源约束：任一时刻至多额外持有 1 个保留帧 entry；当前项与保留帧的订阅分别只经
/// SetCurrentItem 与 SetRetained 变更，严格配对；热路径 Recompute 零分配。
/// </summary>
internal sealed class ImagePreviewDisplayTracker : IDisposable
{
    internal const int PlaceholderGraceMilliseconds = 300;

    private readonly Func<ImageSwitchMode> _modeAccessor;
    private readonly Func<ImagePreviewEntry?>? _retainedSeedAccessor;
    private readonly Action _stateChanged;
    private ImagePreviewEntry? _currentItem;
    private ImagePreviewEntry? _retainedItem;
    private bool _placeholderExpired;
    private long _graceGeneration;

    internal ImagePreviewDisplayTracker(
        Func<ImageSwitchMode> modeAccessor,
        Action stateChanged,
        Func<ImagePreviewEntry?>? retainedSeedAccessor = null)
    {
        _modeAccessor          = modeAccessor ?? throw new ArgumentNullException(nameof(modeAccessor));
        _stateChanged          = stateChanged ?? throw new ArgumentNullException(nameof(stateChanged));
        _retainedSeedAccessor  = retainedSeedAccessor;
    }

    internal IImage? EffectiveImage { get; private set; }

    internal bool IsCurrentLoading { get; private set; }

    internal bool IsCurrentFailed { get; private set; }

    internal void SetCurrentItem(ImagePreviewEntry? item)
    {
        if (!ReferenceEquals(_currentItem, item))
        {
            // 订阅不变量：entry 被订阅 ⇔ 它是 current 或 retained。
            // 旧 current 若同时是 retained（加载完成后的常态），
            // 退订会切断保留帧通知链，必须保留其订阅。
            _placeholderExpired = false;
            var previous = _currentItem;
            _currentItem = item;
            if (previous is not null && !ReferenceEquals(previous, _retainedItem))
            {
                previous.PropertyChanged -= HandleEntryPropertyChanged;
            }
            if (_currentItem is not null && !ReferenceEquals(_currentItem, _retainedItem))
            {
                _currentItem.PropertyChanged += HandleEntryPropertyChanged;
            }
        }
        Recompute();
    }

    internal void Clear()
    {
        if (_currentItem is not null)
        {
            _currentItem.PropertyChanged -= HandleEntryPropertyChanged;
            _currentItem = null;
        }
        _placeholderExpired = false;
        StopGraceTimer();
        SetRetained(null);
        Recompute();
    }

    private void Recompute()
    {
        var mode = _modeAccessor();
        if (_retainedItem is not null &&
            (_currentItem is null || _retainedItem.FullImage is null))
        {
            // 无目标项（集合清空/关闭）或保留帧源已卸载/释放/提交失败时丢弃
            //（经 SetRetained 以同步退订）
            SetRetained(null);
        }

        var currentImage = _currentItem?.FullImage;
        var needsFallback = false;
        if (currentImage is not null)
        {
            SetRetained(_currentItem);
            _placeholderExpired = false;
        }
        else if (_currentItem is { FullImage: null, IsFullFailed: false })
        {
            // 目标尚无图（含切换瞬间的 Idle 态）且未失败：两种模式都保持显示连续性，
            // 不产生空白帧（立即清空在高频切换下表现为频闪；web img 换源同样是
            // 保留旧图直到新图就绪）。Immediate 的差异在宽限期后回退占位。
            needsFallback = !_placeholderExpired;
        }
        if (needsFallback)
        {
            currentImage = _retainedItem?.FullImage;
            if (currentImage is null && _retainedSeedAccessor is not null)
            {
                // 保留帧饿死补充：切换快于加载完成时当前项可能永不完成，
                // 从"最近完成的全图加载"补充保留帧
                var seed = _retainedSeedAccessor.Invoke();
                if (seed?.FullImage is not null)
                {
                    SetRetained(seed);
                    currentImage = seed.FullImage;
                }
            }
            if (currentImage is not null && mode == ImageSwitchMode.Immediate)
            {
                StartGraceTimer();
            }
            else
            {
                StopGraceTimer();
            }
        }
        else
        {
            StopGraceTimer();
        }

        var isLoading = _currentItem?.IsFullLoading == true;
        var isFailed  = _currentItem?.IsFullFailed == true;
        if (!ReferenceEquals(EffectiveImage, currentImage) ||
            IsCurrentLoading != isLoading ||
            IsCurrentFailed != isFailed)
        {
            // 三值变更短路：未变化不触发宿主回调，避免冗余 SetCurrentValue/能力重算
            EffectiveImage    = currentImage;
            IsCurrentLoading = isLoading;
            IsCurrentFailed  = isFailed;
            _stateChanged();
        }
    }

    private void StartGraceTimer()
    {
        var generation = ++_graceGeneration;
        _ = Task.Delay(TimeSpan.FromMilliseconds(PlaceholderGraceMilliseconds)).ContinueWith(
            _ =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (generation != _graceGeneration)
                    {
                        return; // 已被后续状态变更取代
                    }
                    if (_modeAccessor() == ImageSwitchMode.Immediate &&
                        _currentItem is { FullImage: null, IsFullFailed: false })
                    {
                        // 宽限期届满仍未就绪：Immediate 回退加载占位（目标维度，切换即重置）
                        _placeholderExpired = true;
                    }
                    Recompute();
                });
            },
            TaskScheduler.Default);
    }

    private void StopGraceTimer()
    {
        // 使在途宽限回调失效（世代号递增）
        _graceGeneration++;
    }

    // 保留帧引用的唯一变更通道：替换时对新旧源（与当前项不同的那个）成对退订/订阅。
    // 禁止在 Recompute 中直接赋值 _retainedItem，避免订阅失去配对。
    private void SetRetained(ImagePreviewEntry? entry)
    {
        if (ReferenceEquals(_retainedItem, entry))
        {
            return;
        }
        if (_retainedItem is not null && !ReferenceEquals(_retainedItem, _currentItem))
        {
            _retainedItem.PropertyChanged -= HandleEntryPropertyChanged;
        }
        _retainedItem = entry;
        if (_retainedItem is not null && !ReferenceEquals(_retainedItem, _currentItem))
        {
            _retainedItem.PropertyChanged += HandleEntryPropertyChanged;
        }
    }

    private void HandleEntryPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ImagePreviewEntry.FullImage) ||
            args.PropertyName == nameof(ImagePreviewEntry.FullState) ||
            args.PropertyName == nameof(ImagePreviewEntry.IsFullLoading) ||
            args.PropertyName == nameof(ImagePreviewEntry.IsFullFailed))
        {
            Recompute();
        }
    }

    public void Dispose()
    {
        if (_currentItem is not null)
        {
            _currentItem.PropertyChanged -= HandleEntryPropertyChanged;
            _currentItem = null;
        }
        StopGraceTimer();
        SetRetained(null);
    }
}
