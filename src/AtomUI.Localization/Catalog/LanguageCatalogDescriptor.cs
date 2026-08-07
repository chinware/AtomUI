using System.Collections.ObjectModel;

namespace AtomUI.Localization;

public abstract class LanguageCatalogDescriptor
{
    private readonly ReadOnlyCollection<LanguageCatalogUnitDescriptor> _units;

    private protected LanguageCatalogDescriptor(
        string catalogId,
        int contractVersion,
        Type resourceKindType,
        IReadOnlyList<LanguageCatalogUnitDescriptor> units)
    {
        LanguageCatalogIdentity.Validate(catalogId);
        if (contractVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(contractVersion),
                contractVersion,
                "A Catalog contract version must be positive.");
        }

        ArgumentNullException.ThrowIfNull(resourceKindType);
        ArgumentNullException.ThrowIfNull(units);
        if (units.Count == 0)
        {
            throw new ArgumentException("A language Catalog must contain at least one unit.", nameof(units));
        }

        var unitArray = new LanguageCatalogUnitDescriptor[units.Count];
        var unitKeys = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < units.Count; index++)
        {
            var unit = units[index] ?? throw new ArgumentException(
                "A language Catalog cannot contain a null unit descriptor.",
                nameof(units));
            if (!unitKeys.Add(unit.Key))
            {
                throw new ArgumentException(
                    $"Language unit Key '{unit.Key}' is duplicated in Catalog '{catalogId}'.",
                    nameof(units));
            }

            unitArray[index] = unit;
        }

        CatalogId = catalogId;
        ContractVersion = contractVersion;
        ResourceKindType = resourceKindType;
        _units = Array.AsReadOnly(unitArray);
    }

    public string CatalogId { get; }

    public int ContractVersion { get; }

    public Type ResourceKindType { get; }

    public IReadOnlyList<LanguageCatalogUnitDescriptor> Units => _units;

    internal abstract bool TryGetUnitSlot(object resourceKey, out int unitSlot);
}

public sealed class LanguageCatalogDescriptor<TResourceKind> : LanguageCatalogDescriptor
    where TResourceKind : struct, Enum
{
    private readonly Func<TResourceKind, int> _unitSlotResolver;

    public LanguageCatalogDescriptor(
        string catalogId,
        int contractVersion,
        IReadOnlyList<LanguageCatalogUnitDescriptor> units,
        Func<TResourceKind, int> unitSlotResolver)
        : base(catalogId, contractVersion, typeof(TResourceKind), units)
    {
        _unitSlotResolver = unitSlotResolver ?? throw new ArgumentNullException(nameof(unitSlotResolver));
    }

    public bool TryGetUnitSlot(TResourceKind resourceKey, out int unitSlot)
    {
        unitSlot = _unitSlotResolver(resourceKey);
        return (uint)unitSlot < (uint)Units.Count;
    }

    internal override bool TryGetUnitSlot(object resourceKey, out int unitSlot)
    {
        if (resourceKey is TResourceKind typedKey)
        {
            return TryGetUnitSlot(typedKey, out unitSlot);
        }

        unitSlot = -1;
        return false;
    }
}

internal static class LanguageCatalogIdentity
{
    internal static void Validate(string catalogId)
    {
        ArgumentNullException.ThrowIfNull(catalogId);
        if (string.IsNullOrWhiteSpace(catalogId) ||
            catalogId[0] == ':' ||
            catalogId[^1] == ':' ||
            catalogId.IndexOf(':') < 0)
        {
            throw new ArgumentException(
                "A Catalog ID must use the '{LanguageModuleId}:{FullyQualifiedMetadataName}' format.",
                nameof(catalogId));
        }
    }
}
