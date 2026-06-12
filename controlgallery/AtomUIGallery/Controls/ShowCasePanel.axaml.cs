using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Threading;
using AvaloniaControlList = Avalonia.Controls.Controls;

namespace AtomUIGallery.Controls;

public class ShowCasePanel : TemplatedControl
{
    internal const string MainPanelPart = "PART_MainPanel";
    private const int BrowserInitialShowCaseItemCount = 4;
    private static readonly TimeSpan s_browserProgressiveMountInterval = TimeSpan.FromMilliseconds(650);

    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<ShowCasePanel, double>(nameof(MinItemWidth), 320);

    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<ShowCasePanel, int>(nameof(MaxColumns), 2);

    public static readonly StyledProperty<double> ColumnGapProperty =
        AvaloniaProperty.Register<ShowCasePanel, double>(nameof(ColumnGap), 16);

    public static readonly StyledProperty<double> RowGapProperty =
        AvaloniaProperty.Register<ShowCasePanel, double>(nameof(RowGap), 16);

    public static readonly StyledProperty<Thickness> ContentMarginProperty =
        AvaloniaProperty.Register<ShowCasePanel, Thickness>(nameof(ContentMargin));

    public static readonly StyledProperty<bool> IsScrollEnabledProperty =
        AvaloniaProperty.Register<ShowCasePanel, bool>(nameof(IsScrollEnabled), true);

    private ShowCaseMasonryPanel? _layoutPanel;
    private DispatcherTimer? _progressiveMountTimer;
    private int _nextProgressiveMountIndex;

    [Content]
    public AvaloniaControlList Children { get; } = new();

    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    public double ColumnGap
    {
        get => GetValue(ColumnGapProperty);
        set => SetValue(ColumnGapProperty, value);
    }

    public double RowGap
    {
        get => GetValue(RowGapProperty);
        set => SetValue(RowGapProperty, value);
    }

    public Thickness ContentMargin
    {
        get => GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public bool IsScrollEnabled
    {
        get => GetValue(IsScrollEnabledProperty);
        set => SetValue(IsScrollEnabledProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        StopProgressiveMountTimer();
        _layoutPanel?.Children.Clear();

        base.OnApplyTemplate(e);
        _layoutPanel = e.NameScope.Get<ShowCaseMasonryPanel>(MainPanelPart);
        if (_layoutPanel != null)
        {
            _nextProgressiveMountIndex = 0;
            if (OperatingSystem.IsBrowser())
            {
                _nextProgressiveMountIndex = MountShowCaseItems(0, BrowserInitialShowCaseItemCount);
                StartProgressiveMountTimer();
            }
            else
            {
                MountShowCaseItems(0, Children.Count);
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        StopProgressiveMountTimer();
        base.OnDetachedFromVisualTree(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (OperatingSystem.IsBrowser() &&
            _layoutPanel is not null &&
            _progressiveMountTimer is null &&
            _nextProgressiveMountIndex < Children.Count)
        {
            StartProgressiveMountTimer();
        }

        return base.MeasureOverride(availableSize);
    }

    private int MountShowCaseItems(int startIndex, int maxCount)
    {
        if (_layoutPanel is null || maxCount <= 0)
        {
            return startIndex;
        }

        var mountedCount = 0;
        var index        = startIndex;
        while (index < Children.Count && mountedCount < maxCount)
        {
            if (Children[index] is ShowCaseItem item)
            {
                _layoutPanel.Children.Add(item);
                mountedCount++;
            }

            index++;
        }

        return index;
    }

    private void StartProgressiveMountTimer()
    {
        if (_nextProgressiveMountIndex >= Children.Count)
        {
            return;
        }

        _progressiveMountTimer = new DispatcherTimer(
            s_browserProgressiveMountInterval,
            DispatcherPriority.SystemIdle,
            Dispatcher);
        _progressiveMountTimer.Tick += HandleProgressiveMountTimerTick;
        _progressiveMountTimer.Start();
    }

    private void HandleProgressiveMountTimerTick(object? sender, EventArgs e)
    {
        if (!IsEffectivelyVisible)
        {
            StopProgressiveMountTimer();
            return;
        }

        _nextProgressiveMountIndex = MountShowCaseItems(_nextProgressiveMountIndex, 1);
        if (_nextProgressiveMountIndex >= Children.Count)
        {
            StopProgressiveMountTimer();
        }
    }

    private void StopProgressiveMountTimer()
    {
        if (_progressiveMountTimer is null)
        {
            return;
        }

        _progressiveMountTimer.Stop();
        _progressiveMountTimer.Tick -= HandleProgressiveMountTimerTick;
        _progressiveMountTimer = null;
    }

    internal virtual void NotifyAboutToActive()
    {
    }

    internal virtual void NotifyActivated()
    {
    }

    internal virtual void NotifyAboutToDeactivated()
    {
    }

    internal virtual void NotifyDeactivated()
    {
    }
}
