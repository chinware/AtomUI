using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

internal sealed class ThemeContextLease : IDisposable
{
    private static readonly AttachedProperty<ThemeContextLease?> LeaseProperty =
        AvaloniaProperty.RegisterAttached<ThemeConfigProvider, TopLevel, ThemeContextLease?>(
            "ThemeContextLease");

    private TopLevel? _host;
    private ThemeContext? _ownerContext;
    private ThemeContextResourceBridge? _bridge;

    private ThemeContextLease(
        TopLevel host,
        ThemeContext ownerContext,
        ThemeContextResourceBridge bridge)
    {
        _host         = host;
        _ownerContext = ownerContext;
        _bridge       = bridge;
        ownerContext.Published += HandleContextPublished;
        if (host is Window window)
        {
            window.Closed += HandleWindowClosed;
        }
    }

    internal static ThemeContextLease Attach(
        TopLevel host,
        ThemeContext ownerContext)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(ownerContext);

        var previous = host.GetValue(LeaseProperty);
        var bridge = new ThemeContextResourceBridge(ownerContext);
        ThemeContextLease? lease = null;
        try
        {
            host.Resources.MergedDictionaries.Add(bridge);
            host.SetValue(ThemeScope.ContextProperty, ownerContext);
            host.SetCurrentValue(
                TopLevel.RequestedThemeVariantProperty,
                ToAvaloniaVariant(ownerContext.Appearance));
            lease = new ThemeContextLease(host, ownerContext, bridge);
            host.SetValue(LeaseProperty, lease);
        }
        catch
        {
            host.Resources.MergedDictionaries.Remove(bridge);
            bridge.Dispose();
            if (lease is not null)
            {
                lease.ReleaseSubscriptions();
            }
            throw;
        }

        previous?.Dispose();
        return lease;
    }

    internal bool IsOwnedBy(ThemeContext context)
    {
        return ReferenceEquals(_ownerContext, context);
    }

    public void Dispose()
    {
        var host = Interlocked.Exchange(ref _host, null);
        var context = Interlocked.Exchange(ref _ownerContext, null);
        var bridge = Interlocked.Exchange(ref _bridge, null);
        if (host is null)
        {
            return;
        }

        if (context is not null)
        {
            context.Published -= HandleContextPublished;
        }
        if (host is Window window)
        {
            window.Closed -= HandleWindowClosed;
        }
        if (bridge is not null)
        {
            host.Resources.MergedDictionaries.Remove(bridge);
            bridge.Dispose();
        }

        if (ReferenceEquals(host.GetValue(LeaseProperty), this))
        {
            host.ClearValue(LeaseProperty);
            host.ClearValue(ThemeScope.ContextProperty);
            host.ClearValue(TopLevel.RequestedThemeVariantProperty);
        }
    }

    private void HandleContextPublished(object? sender, EventArgs args)
    {
        var host = _host;
        var context = _ownerContext;
        if (host is not null && context is not null)
        {
            host.SetCurrentValue(
                TopLevel.RequestedThemeVariantProperty,
                ToAvaloniaVariant(context.Appearance));
        }
    }

    private void HandleWindowClosed(object? sender, EventArgs args)
    {
        Dispose();
    }

    private void ReleaseSubscriptions()
    {
        if (_ownerContext is not null)
        {
            _ownerContext.Published -= HandleContextPublished;
        }
        if (_host is Window window)
        {
            window.Closed -= HandleWindowClosed;
        }
    }

    private static ThemeVariant ToAvaloniaVariant(ThemeAppearance appearance)
    {
        return appearance == ThemeAppearance.Dark
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
    }
}
