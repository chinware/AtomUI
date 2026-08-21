using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Threading;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartHighlightSession : IDisposable
{
    private readonly List<AdornerEntry> _adorners = [];
    private readonly List<Popup> _popups = [];
    private Control[] _owners;
    private SemanticPartDescriptor? _part;
    private SemanticPartRegistry? _registry;
    private Visual[] _additionalRoots;
    private bool _refreshQueued;
    private bool _isDisposed;

    private SemanticPartHighlightSession(
        IReadOnlyList<Control> owners,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        IEnumerable<Visual>? additionalRoots)
    {
        _owners          = owners.ToArray();
        _part            = part;
        _registry        = registry;
        _additionalRoots = additionalRoots?.ToArray() ?? [];
    }

    public int TotalMatchCount { get; private set; }

    public int HighlightedTargetCount => _adorners.Count;

    public bool IsTruncated => HighlightedTargetCount < TotalMatchCount;

    public static SemanticPartHighlightSession Start(
        IReadOnlyList<Control> owners,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        IEnumerable<Visual>? additionalRoots = null)
    {
        ArgumentNullException.ThrowIfNull(owners);
        ArgumentNullException.ThrowIfNull(part);
        ArgumentNullException.ThrowIfNull(registry);
        if (owners.Count == 0)
        {
            throw new ArgumentException("At least one Semantic owner is required.", nameof(owners));
        }

        var session = new SemanticPartHighlightSession(owners, part, registry, additionalRoots);
        session.SubscribeToOwnerPopups();
        session.Refresh();
        return session;
    }

    public static SemanticPartHighlightSession Start(
        Control owner,
        SemanticPartDescriptor part,
        SemanticPartRegistry registry,
        IEnumerable<Visual>? additionalRoots = null)
    {
        ArgumentNullException.ThrowIfNull(owner);
        return Start([owner], part, registry, additionalRoots);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _refreshQueued = false;
        ClearAdorners();
        UnsubscribeFromOwnerPopups();
        _additionalRoots = [];
        _owners = [];
        _part = null;
        _registry = null;
        TotalMatchCount = 0;
    }

    private void SubscribeToOwnerPopups()
    {
        if (_part?.CrossVisualRoot != true)
        {
            return;
        }

        foreach (var owner in _owners)
        {
            if (owner is not TemplatedControl templatedOwner)
            {
                continue;
            }

            foreach (var popup in templatedOwner.GetTemplateDescendants().OfType<Popup>())
            {
                if (_popups.Contains(popup))
                {
                    continue;
                }

                popup.Opened += HandlePopupOpened;
                popup.Closed += HandlePopupClosed;
                _popups.Add(popup);
            }
        }
    }

    private void UnsubscribeFromOwnerPopups()
    {
        foreach (var popup in _popups)
        {
            popup.Opened -= HandlePopupOpened;
            popup.Closed -= HandlePopupClosed;
        }
        _popups.Clear();
    }

    private void HandlePopupOpened(object? sender, EventArgs e)
    {
        QueueRefresh();
    }

    private void HandlePopupClosed(object? sender, EventArgs e)
    {
        Refresh();
    }

    private void HandleTargetDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        QueueRefresh();
    }

    private void QueueRefresh()
    {
        if (_isDisposed || _refreshQueued)
        {
            return;
        }

        _refreshQueued = true;
        Dispatcher.UIThread.Post(ApplyQueuedRefresh, DispatcherPriority.Render);
    }

    private void ApplyQueuedRefresh()
    {
        if (!_refreshQueued)
        {
            return;
        }

        _refreshQueued = false;
        Refresh();
    }

    private void Refresh()
    {
        _refreshQueued = false;
        ClearAdorners();
        if (_isDisposed ||
            _owners.Length == 0 ||
            _part is not { } part ||
            _registry is not { } registry)
        {
            return;
        }

        var additionalRoots = CollectAdditionalRoots();
        var targets = new List<Visual>();
        var matches = new HashSet<Visual>();
        var totalMatchCount = 0;
        var budget = SemanticPartTargetResolver.DefaultTargetBudget;
        foreach (var owner in _owners)
        {
            var resolution = SemanticPartTargetResolver.Resolve(owner, part, registry, additionalRoots);
            totalMatchCount += resolution.TotalMatchCount;
            foreach (var target in resolution.Targets)
            {
                if (targets.Count >= budget)
                {
                    break;
                }

                if (matches.Add(target))
                {
                    targets.Add(target);
                }
            }
        }

        TotalMatchCount = totalMatchCount;

        for (var index = 0; index < targets.Count; index++)
        {
            var target = targets[index];
            var layer = AdornerLayer.GetAdornerLayer(target);
            if (layer is null)
            {
                continue;
            }

            var adorner = SemanticPartAdorner.Create(target, index == 0);
            layer.Children.Add(adorner);
            target.DetachedFromVisualTree += HandleTargetDetached;
            _adorners.Add(new AdornerEntry(target, layer, adorner));
        }
    }

    private IReadOnlyList<Visual> CollectAdditionalRoots()
    {
        if (_popups.Count == 0)
        {
            return _additionalRoots;
        }

        var roots = new List<Visual>(_additionalRoots.Length + _popups.Count);
        roots.AddRange(_additionalRoots);
        foreach (var popup in _popups)
        {
            if (popup.Child is { } child)
            {
                roots.Add(child);
            }
        }
        return roots;
    }

    private void ClearAdorners()
    {
        foreach (var entry in _adorners)
        {
            AdornerLayer.SetAdornedElement(entry.Adorner, null);
            if (entry.Layer.Children.Contains(entry.Adorner))
            {
                entry.Layer.Children.Remove(entry.Adorner);
            }
            entry.Target.DetachedFromVisualTree -= HandleTargetDetached;
        }
        _adorners.Clear();
        TotalMatchCount = 0;
    }

    private sealed record AdornerEntry(
        Visual Target,
        AdornerLayer Layer,
        SemanticPartAdorner Adorner);
}
