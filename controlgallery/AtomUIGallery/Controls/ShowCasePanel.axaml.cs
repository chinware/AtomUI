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

    private bool _initialized;
    private Grid? _layoutPanel;
    private DispatcherTimer? _progressiveMountTimer;
    private int _nextProgressiveMountIndex;

    [Content]
    public AvaloniaControlList Children { get; } = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var effectCount = 0;
        foreach (var child in Children)
        {
            if (child is ShowCaseItem showCaseItem)
            {
                effectCount++;
                if (showCaseItem.IsOccupyEntireRow)
                {
                    effectCount++;
                }
            }
        }
        if (effectCount % 2 != 0)
        {
            var extra = new ShowCaseItem()
            {
                IsFake = true
            };
            Children.Add(extra);
        }
        base.OnApplyTemplate(e);
        _layoutPanel = e.NameScope.Get<Grid>(MainPanelPart);
        if (_layoutPanel != null && !_initialized)
        {
            var row = 0;
            var column = 0;

            for (var i = 0; i < Children.Count; ++i)
            {
                if (Children[i] is ShowCaseItem item)
                {
                    if (item.IsOccupyEntireRow)
                    {
                        if (column != 0)
                        {
                            row++;
                        }
                        Grid.SetRow(item, row++);

                        Grid.SetColumn(item, 0);
                        Grid.SetColumnSpan(item, 2);
                    }
                    else
                    {
                        Grid.SetRow(item, row);
                        Grid.SetColumn(item, column++);
                        if (column == 2)
                        {
                            row++;
                            column = 0;
                        }
                    }
                }
            }

            var rowDefinitions = new RowDefinitions();
            for (var i = 0; i < row; ++i)
            {
                rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }
            _layoutPanel.RowDefinitions = rowDefinitions;
            _initialized                = true;

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
            _initialized &&
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
                LogicalChildren.Add(item);
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
