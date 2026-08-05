using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Localization;

internal sealed class LanguageResourceProvider : ResourceProvider
{
    private readonly LanguageRuntimeContext _context;

    internal LanguageResourceProvider(LanguageRuntimeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public override bool HasResources => true;

    public override bool TryGetResource(
        object key,
        ThemeVariant? theme,
        out object? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var revision = _context.Current;
        if (!_context.Registry.TryGetCatalogSlot(key.GetType(), out var catalogSlot))
        {
            value = null;
            return false;
        }

        var catalog = _context.Registry.Catalogs[catalogSlot];
        if (!catalog.TryGetUnitSlot(key, out var unitSlot))
        {
            value = null;
            return false;
        }

        value = revision.Snapshot.GetEntry(catalogSlot, unitSlot).Text;
        return true;
    }

    internal void PublishResourcesChanged()
    {
        RaiseResourcesChanged();
    }
}
