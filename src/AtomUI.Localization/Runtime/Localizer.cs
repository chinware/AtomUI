namespace AtomUI.Localization;

internal sealed class Localizer : ILocalizer
{
    private readonly LanguageRuntimeContext _context;

    internal Localizer(LanguageRuntimeContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string Get<TResourceKind>(TResourceKind key)
        where TResourceKind : struct, Enum
    {
        var revision = _context.Current;
        var resolved = Resolve(revision, key);
        return resolved.Entry.Text;
    }

    public string Format<TResourceKind>(TResourceKind key, params object?[] arguments)
        where TResourceKind : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(arguments);
        var revision = _context.Current;
        var resolved = Resolve(revision, key);
        if (resolved.Entry.CompositeFormat is null)
        {
            return resolved.Entry.Text;
        }

        try
        {
            return string.Format(
                revision.State.FormattingCulture,
                resolved.Entry.CompositeFormat,
                arguments);
        }
        catch (FormatException exception)
        {
            throw new LanguageCatalogException(
                $"Formatting Catalog '{resolved.Catalog.CatalogId}' unit " +
                $"'{resolved.Unit.Name}' ({resolved.Unit.Id}) for language " +
                $"'{revision.State.CurrentLanguage.Value}' failed.",
                exception);
        }
    }

    private ResolvedLanguageValue Resolve<TResourceKind>(
        LanguageRuntimeRevision revision,
        TResourceKind key)
        where TResourceKind : struct, Enum
    {
        var resourceKindType = typeof(TResourceKind);
        if (!_context.Registry.TryGetCatalogSlot(resourceKindType, out var catalogSlot))
        {
            throw new LanguageCatalogException(
                $"Resource enum type '{resourceKindType.FullName}' is not registered as a language Catalog.");
        }

        var catalog = _context.Registry.Catalogs[catalogSlot];
        if (catalog is not LanguageCatalogDescriptor<TResourceKind> typedCatalog)
        {
            throw new LanguageCatalogException(
                $"Catalog '{catalog.CatalogId}' is registered with incompatible resource enum type " +
                $"'{catalog.ResourceKindType.FullName}'.");
        }
        if (!typedCatalog.TryGetUnitSlot(key, out var unitSlot))
        {
            throw new LanguageCatalogException(
                $"A value of resource enum type '{resourceKindType.FullName}' is not mapped to any unit " +
                $"in Catalog '{catalog.CatalogId}'.");
        }

        return new ResolvedLanguageValue(
            catalog,
            catalog.Units[unitSlot],
            revision.Snapshot.GetEntry(catalogSlot, unitSlot));
    }

    private readonly record struct ResolvedLanguageValue(
        LanguageCatalogDescriptor Catalog,
        LanguageCatalogUnitDescriptor Unit,
        LanguageSnapshotEntry Entry);
}
