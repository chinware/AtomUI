using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using AvaloniaDefaultMenuInteractionHandler = Avalonia.Controls.Platform.DefaultMenuInteractionHandler;

public class DefaultMenuInteractionHandler : AvaloniaDefaultMenuInteractionHandler, IMenuInteractionHandler
{
    public DefaultMenuInteractionHandler(bool isContextMenu)
        : base(isContextMenu)
    {
        _delayRun = (callback, delay) => DispatcherTimer.RunOnce(callback, delay);
    }

    public DefaultMenuInteractionHandler(
        bool isContextMenu,
        IInputManager? inputManager,
        Action<Action, TimeSpan> delayRun)
        : base(isContextMenu, inputManager, delayRun)
    {
        _delayRun = AdaptDelayRun(delayRun);
    }

    private readonly Func<Action, TimeSpan, IDisposable> _delayRun;
    private MenuBase? _owner;
    private MenuItem? _pendingOpenTarget;
    private IDisposable? _pendingOpenDelay;
    private long _openIntentGeneration;
    private MenuItem? _pendingCloseTarget;
    private IDisposable? _pendingCloseDelay;
    private long _closeIntentGeneration;

    protected override void KeyDown(object? sender, KeyEventArgs e)
    {
        var wasHandled = e.Handled;
        base.KeyDown(sender, e);

        if (!wasHandled && e.Handled)
        {
            CancelPendingHoverOperations();
        }
    }

    protected override void AccessKeyPressed(object? sender, RoutedEventArgs e)
    {
        CancelPendingHoverOperations();
        base.AccessKeyPressed(sender, e);
    }

    protected override void PointerEntered(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            base.PointerEntered(sender, e);
            return;
        }

        if (item.IsTopLevel)
        {
            CancelPendingHoverOperations();
            base.PointerEntered(sender, e);
            return;
        }

        if (item.HasSubMenu)
        {
            CancelPendingClose(item);
            SelectItemAndAncestors(item);
            if (!item.IsSubMenuOpen)
            {
                ScheduleOpen(item);
            }
        }
        else
        {
            CancelPendingOpen();
            CancelPendingClose();
            SelectItemAndAncestors(item);

            var openedSibling = FindOpenedSibling(item);
            if (openedSibling != null)
            {
                ScheduleClose(openedSibling);
            }
        }
    }

    protected override void PointerMoved(object? sender, PointerEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item == null)
        {
            return;
        }

        var transformedBounds = item.GetTransformedBounds();
        if (transformedBounds == null)
        {
            return;
        }

        var point = e.GetCurrentPoint(null);

        if (point.Properties.IsLeftButtonPressed
            && transformedBounds.Value.Contains(point.Position) == false)
        {
            var sourceControl  = e.Source as Control;
            var scrollBarThumb = sourceControl.FindAncestorOfType<ScrollBarThumb>();
            if (scrollBarThumb != null)
            {
                return;
            }

            e.Pointer.Capture(null);
        }
    }

    protected override void PointerExited(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            base.PointerExited(sender, e);
            return;
        }

        if (item.IsTopLevel)
        {
            CancelPendingHoverOperations();
            base.PointerExited(sender, e);
            return;
        }

        if (!item.IsSubMenuOpen)
        {
            CancelPendingOpen(item);
            ClearParentSelection(item);
            return;
        }

        if (item.IsPointerOverSubMenu)
        {
            CancelPendingClose(item);
        }
        else
        {
            ScheduleClose(item);
        }
    }

    protected override void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        CancelPendingHoverOperations();
        base.PointerPressed(sender, e);
    }

    void IMenuInteractionHandler.Attach(MenuBase menu)
    {
        base.Attach(menu);
        AttachOwner(menu);
    }

    void IMenuInteractionHandler.Detach(MenuBase menu)
    {
        try
        {
            base.Detach(menu);
        }
        finally
        {
            DetachOwner(menu);
        }
    }

    internal void CancelPendingHoverOperations()
    {
        CancelPendingOpen();
        CancelPendingClose();
    }

    internal static MenuItem? GetMenuItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
            {
                return null;
            }

            if (item is MenuItem menuItem)
            {
                return menuItem;
            }

            item = item.Parent;
        }
    }

    private static Func<Action, TimeSpan, IDisposable> AdaptDelayRun(
        Action<Action, TimeSpan> delayRun)
    {
        return (callback, delay) =>
        {
            Action? callbackGate = callback;

            void InvokeCallback()
            {
                Interlocked.Exchange(ref callbackGate, null)?.Invoke();
            }

            try
            {
                delayRun(InvokeCallback, delay);
            }
            catch
            {
                Interlocked.Exchange(ref callbackGate, null);
                throw;
            }

            return Disposable.Create(() => Interlocked.Exchange(ref callbackGate, null));
        };
    }

    private void AttachOwner(MenuBase menu)
    {
        _owner = menu;
        menu.Closed += OwnerClosed;
    }

    private void DetachOwner(MenuBase menu)
    {
        if (!ReferenceEquals(_owner, menu))
        {
            return;
        }

        menu.Closed -= OwnerClosed;
        _owner = null;
        CancelPendingHoverOperations();
    }

    private void OwnerClosed(object? sender, RoutedEventArgs e)
    {
        if (ReferenceEquals(e.Source, _owner))
        {
            CancelPendingHoverOperations();
        }
    }

    private void ScheduleOpen(MenuItem target)
    {
        if (ReferenceEquals(_pendingOpenTarget, target))
        {
            return;
        }

        CancelPendingOpen();
        CancelPendingClose(target);
        var generation       = _openIntentGeneration;
        _pendingOpenTarget   = target;
        _pendingOpenDelay    = null;

        IDisposable delay;
        try
        {
            delay = _delayRun(() => CommitOpen(target, generation), MenuShowDelay);
        }
        catch
        {
            CancelPendingOpen(target);
            throw;
        }

        if (IsCurrentOpenIntent(target, generation))
        {
            _pendingOpenDelay = delay;
        }
        else
        {
            delay.Dispose();
        }
    }

    private void CommitOpen(MenuItem target, long generation)
    {
        if (!TakeOpenIntent(target, generation))
        {
            return;
        }

        if (BelongsToOwner(target) &&
            target.HasSubMenu &&
            !target.IsSubMenuOpen &&
            IsSelectedContainer(target))
        {
            target.Open();
        }
    }

    private void ScheduleClose(MenuItem target)
    {
        if (ReferenceEquals(_pendingCloseTarget, target))
        {
            return;
        }

        CancelPendingOpen(target);
        CancelPendingClose();
        var generation       = _closeIntentGeneration;
        _pendingCloseTarget  = target;
        _pendingCloseDelay   = null;

        IDisposable delay;
        try
        {
            delay = _delayRun(() => CommitClose(target, generation), MenuShowDelay);
        }
        catch
        {
            CancelPendingClose(target);
            throw;
        }

        if (IsCurrentCloseIntent(target, generation))
        {
            _pendingCloseDelay = delay;
        }
        else
        {
            delay.Dispose();
        }
    }

    private void CommitClose(MenuItem target, long generation)
    {
        if (!TakeCloseIntent(target, generation))
        {
            return;
        }

        if (BelongsToOwner(target) &&
            target.IsSubMenuOpen &&
            !target.IsPointerOver &&
            !target.IsPointerOverSubMenu)
        {
            target.Close();
        }
    }

    private bool TakeOpenIntent(MenuItem target, long generation)
    {
        if (!IsCurrentOpenIntent(target, generation))
        {
            return false;
        }

        var delay = _pendingOpenDelay;
        _pendingOpenTarget = null;
        _pendingOpenDelay  = null;
        ++_openIntentGeneration;
        delay?.Dispose();
        return true;
    }

    private bool TakeCloseIntent(MenuItem target, long generation)
    {
        if (!IsCurrentCloseIntent(target, generation))
        {
            return false;
        }

        var delay = _pendingCloseDelay;
        _pendingCloseTarget = null;
        _pendingCloseDelay  = null;
        ++_closeIntentGeneration;
        delay?.Dispose();
        return true;
    }

    private bool IsCurrentOpenIntent(MenuItem target, long generation)
    {
        return generation == _openIntentGeneration &&
               ReferenceEquals(_pendingOpenTarget, target);
    }

    private bool IsCurrentCloseIntent(MenuItem target, long generation)
    {
        return generation == _closeIntentGeneration &&
               ReferenceEquals(_pendingCloseTarget, target);
    }

    private void CancelPendingOpen(MenuItem target)
    {
        if (ReferenceEquals(_pendingOpenTarget, target))
        {
            CancelPendingOpen();
        }
    }

    private void CancelPendingOpen()
    {
        var delay = _pendingOpenDelay;
        _pendingOpenTarget = null;
        _pendingOpenDelay  = null;
        ++_openIntentGeneration;
        delay?.Dispose();
    }

    private void CancelPendingClose(MenuItem target)
    {
        if (_pendingCloseTarget is { } pendingTarget &&
            IsSelfOrDescendantOf(target, pendingTarget))
        {
            CancelPendingClose();
        }
    }

    private void CancelPendingClose()
    {
        var delay = _pendingCloseDelay;
        _pendingCloseTarget = null;
        _pendingCloseDelay  = null;
        ++_closeIntentGeneration;
        delay?.Dispose();
    }

    private static bool IsSelfOrDescendantOf(MenuItem target, MenuItem ancestor)
    {
        StyledElement? current = target;
        while (current != null)
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }

    private bool BelongsToOwner(MenuItem target)
    {
        if (_owner == null)
        {
            return false;
        }

        StyledElement? current = target;
        while (current != null)
        {
            if (ReferenceEquals(current, _owner))
            {
                return true;
            }

            current = current.Parent;
        }

        return false;
    }

    private static bool IsSelectedContainer(MenuItem target)
    {
        if (target.Parent is not SelectingItemsControl parent)
        {
            return false;
        }

        var targetIndex = parent.IndexFromContainer(target);
        return targetIndex >= 0 &&
               parent.SelectedIndex == targetIndex &&
               ReferenceEquals(parent.ContainerFromIndex(targetIndex), target);
    }

    private static void SelectItemAndAncestors(MenuItem target)
    {
        var current = target;
        while (current.Parent is SelectingItemsControl parent)
        {
            var targetIndex = parent.IndexFromContainer(current);
            if (targetIndex < 0 || !ReferenceEquals(parent.ContainerFromIndex(targetIndex), current))
            {
                return;
            }

            parent.SelectedIndex = targetIndex;
            if (parent is not MenuItem parentItem)
            {
                return;
            }

            current = parentItem;
        }
    }

    private static void ClearParentSelection(MenuItem target)
    {
        if (target.Parent is not SelectingItemsControl parent)
        {
            return;
        }

        var targetIndex = parent.IndexFromContainer(target);
        if (targetIndex >= 0 &&
            parent.SelectedIndex == targetIndex &&
            ReferenceEquals(parent.ContainerFromIndex(targetIndex), target))
        {
            parent.SelectedIndex = -1;
        }
    }

    private static MenuItem? FindOpenedSibling(MenuItem target)
    {
        if (target.Parent is not SelectingItemsControl parent)
        {
            return null;
        }

        for (var index = 0; index < parent.ItemCount; index++)
        {
            if (parent.ContainerFromIndex(index) is MenuItem sibling &&
                !ReferenceEquals(sibling, target) &&
                sibling.IsSubMenuOpen)
            {
                return sibling;
            }
        }

        return null;
    }
}
