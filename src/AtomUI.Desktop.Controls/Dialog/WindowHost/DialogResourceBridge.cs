using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogResourceBridge : ResourceProvider, IDisposable
{
    private StyledElement? _dialog;

    internal DialogResourceBridge(StyledElement dialog)
    {
        _dialog = dialog;
        ((IResourceHost)dialog).ResourcesChanged += HandleResourcesChanged;
    }

    public override bool HasResources => _dialog is not null;

    public override bool TryGetResource(
        object key,
        ThemeVariant? theme,
        out object? value)
    {
        var dialog = _dialog;
        if (dialog is null)
        {
            value = null;
            return false;
        }

        return dialog.TryFindResource(key, theme, out value);
    }

    public void Dispose()
    {
        var dialog = Interlocked.Exchange(ref _dialog, null);
        if (dialog is not null)
        {
            ((IResourceHost)dialog).ResourcesChanged -= HandleResourcesChanged;
        }
    }

    private void HandleResourcesChanged(object? sender, ResourcesChangedEventArgs e)
    {
        if (_dialog is not null)
        {
            RaiseResourcesChanged();
        }
    }
}
