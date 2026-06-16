using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal static class LinuxCsdPopupSupport
{
    internal static void ConfigurePopupPlacement(Control anchor, Popup? popup, ref bool isConfigured, bool isEnabled = true)
    {
        if (!isEnabled || !TryConfigurePopupPlacement(anchor, popup))
        {
            ClearPopupPlacement(popup, ref isConfigured);
            return;
        }

        isConfigured = true;
    }

    internal static void ClearPopupPlacement(Popup? popup, ref bool isConfigured)
    {
        if (popup is null || !isConfigured)
        {
            return;
        }

        popup.PlacementTarget = null;
        popup.PlacementRect   = null;
        isConfigured          = false;
    }

    internal static IDisposable? UpdatePopupPlacementTracker(
        Control anchor,
        Popup? popup,
        IDisposable? current,
        Func<bool> isOpen,
        bool isEnabled = true)
    {
        if (!isEnabled ||
            popup is null ||
            !TryResolveLinuxCsdHostWindow(anchor, out _))
        {
            current?.Dispose();
            return null;
        }

        if (current is PopupPlacementRegistration registration &&
            registration.Matches(anchor, popup))
        {
            registration.Update();
            return current;
        }

        current?.Dispose();
        return new PopupPlacementRegistration(anchor, popup, isOpen);
    }

    internal static void ClearPopupPlacementTracker(ref IDisposable? current)
    {
        current?.Dispose();
        current = null;
    }

    internal static IDisposable? UpdateDismissRoot(
        Control owner,
        IDisposable? current,
        Func<bool> isOpen,
        Action dismiss)
    {
        return UpdateDismissRoot(
            owner,
            current,
            isOpen,
            dismiss,
            control => owner.IsLogicalAncestorOf(control));
    }

    internal static IDisposable? UpdateDismissRoot(
        Control owner,
        IDisposable? current,
        Func<bool> isOpen,
        Action dismiss,
        Func<ILogical, bool> isInside)
    {
        if (!TryResolveLinuxCsdHostWindow(owner, out var hostWindow))
        {
            current?.Dispose();
            return null;
        }

        if (current is DismissRootRegistration registration &&
            registration.Matches(owner, hostWindow))
        {
            return current;
        }

        current?.Dispose();
        return new DismissRootRegistration(owner, hostWindow, isOpen, dismiss, isInside);
    }

    internal static void ClearDismissRoot(ref IDisposable? current)
    {
        current?.Dispose();
        current = null;
    }

    private static bool TryConfigurePopupPlacement(Control anchor, Popup? popup)
    {
        if (popup is null)
        {
            return false;
        }

        if (!TryResolveLinuxCsdHostWindow(anchor, out var hostWindow))
        {
            return false;
        }

        if (!TryCreatePlacementRect(anchor, hostWindow, out var placementRect))
        {
            return false;
        }

        if (!ReferenceEquals(popup.PlacementTarget, hostWindow))
        {
            popup.PlacementTarget = hostWindow;
        }

        if (popup.PlacementRect != placementRect)
        {
            popup.PlacementRect = placementRect;
        }

        return true;
    }

    internal static bool TryResolveLinuxCsdHostWindow(Control control, out Window hostWindow)
    {
        hostWindow = null!;
        if (!OperatingSystem.IsLinux() || TopLevel.GetTopLevel(control) is not null)
        {
            return false;
        }

        var titleBar = control.FindLogicalAncestorOfType<WindowTitleBar>() ??
                       control.FindAncestorOfType<WindowTitleBar>();
        var window = titleBar?.HostWindow;
        if (window is not { IsCsdEnabled: true })
        {
            return false;
        }

        hostWindow = window;
        return true;
    }

    private static bool TryCreatePlacementRect(Control anchor, Window hostWindow, out Rect placementRect)
    {
        var position = anchor.TranslatePoint(default, hostWindow);
        if (position is null)
        {
            try
            {
                position = hostWindow.PointToClient(anchor.PointToScreen(default));
            }
            catch (ArgumentException)
            {
                placementRect = default;
                return false;
            }
        }

        placementRect = new Rect(position.Value, anchor.Bounds.Size);
        return true;
    }

    private sealed class PopupPlacementRegistration : IDisposable
    {
        private readonly Control _anchor;
        private readonly Popup _popup;
        private readonly Func<bool> _isOpen;
        private readonly IDisposable _boundsSubscription;
        private bool _isDisposed;

        public PopupPlacementRegistration(Control anchor, Popup popup, Func<bool> isOpen)
        {
            _anchor             = anchor;
            _popup              = popup;
            _isOpen             = isOpen;
            _boundsSubscription = _anchor.GetObservable(Visual.BoundsProperty)
                                         .Subscribe(_ => Update());
            _anchor.LayoutUpdated += HandleAnchorLayoutUpdated;
            Update();
        }

        public bool Matches(Control anchor, Popup popup)
        {
            return ReferenceEquals(_anchor, anchor) &&
                   ReferenceEquals(_popup, popup);
        }

        public void Update()
        {
            if (!_isDisposed && _isOpen())
            {
                TryConfigurePopupPlacement(_anchor, _popup);
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _anchor.LayoutUpdated -= HandleAnchorLayoutUpdated;
            _boundsSubscription.Dispose();
            _isDisposed = true;
        }

        private void HandleAnchorLayoutUpdated(object? sender, EventArgs e)
        {
            Update();
        }
    }

    private sealed class DismissRootRegistration : IDisposable
    {
        private readonly Control _owner;
        private readonly Window _root;
        private readonly Func<bool> _isOpen;
        private readonly Action _dismiss;
        private readonly Func<ILogical, bool> _isInside;
        private bool _isDisposed;

        public DismissRootRegistration(
            Control owner,
            Window root,
            Func<bool> isOpen,
            Action dismiss,
            Func<ILogical, bool> isInside)
        {
            _owner    = owner;
            _root     = root;
            _isOpen   = isOpen;
            _dismiss  = dismiss;
            _isInside = isInside;
            _root.AddHandler(InputElement.PointerPressedEvent,
                HandleRootPointerPressed,
                RoutingStrategies.Tunnel);
            _root.Deactivated += HandleRootDeactivated;
        }

        public bool Matches(Control owner, Window root)
        {
            return ReferenceEquals(_owner, owner) &&
                   ReferenceEquals(_root, root);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _root.RemoveHandler(InputElement.PointerPressedEvent, HandleRootPointerPressed);
            _root.Deactivated -= HandleRootDeactivated;
            _isDisposed = true;
        }

        private void HandleRootPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (_isOpen() &&
                e.Source is ILogical control &&
                !_isInside(control))
            {
                _dismiss();
            }
        }

        private void HandleRootDeactivated(object? sender, EventArgs e)
        {
            _dismiss();
        }
    }
}
