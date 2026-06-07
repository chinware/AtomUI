using Avalonia.Controls;

namespace AtomUI.Controls;

internal static class TopLevelDeactivation
{
    internal static IDisposable? Subscribe(TopLevel? topLevel, EventHandler handler)
    {
        if (!RuntimePlatform.Features.SupportsWindowDeactivation ||
            topLevel is not WindowBase window)
        {
            return null;
        }

        window.Deactivated += handler;
        return new WindowDeactivationSubscription(window, handler);
    }

    private sealed class WindowDeactivationSubscription : IDisposable
    {
        private WindowBase? _window;
        private EventHandler? _handler;

        public WindowDeactivationSubscription(WindowBase window, EventHandler handler)
        {
            _window  = window;
            _handler = handler;
        }

        public void Dispose()
        {
            if (_window is not null && _handler is not null)
            {
                _window.Deactivated -= _handler;
                _window              =  null;
                _handler             =  null;
            }
        }
    }
}
