namespace AtomUI.Localization.Build;

internal sealed class LanguagePackageManifest
{
    internal LanguagePackageManifest(
        string packageId,
        string language,
        IReadOnlyList<LanguagePackageCatalogEntry> catalogs)
    {
        PackageId = packageId;
        Language = language;
        Catalogs = catalogs;
    }

    internal string PackageId { get; }

    internal string Language { get; }

    internal IReadOnlyList<LanguagePackageCatalogEntry> Catalogs { get; }
}

internal sealed class LanguagePackageCatalogEntry
{
    internal LanguagePackageCatalogEntry(
        string moduleId,
        string catalogId,
        int contractVersion,
        string path,
        string sourceFingerprint)
    {
        ModuleId = moduleId;
        CatalogId = catalogId;
        ContractVersion = contractVersion;
        Path = path;
        SourceFingerprint = sourceFingerprint;
    }

    internal string ModuleId { get; }

    internal string CatalogId { get; }

    internal int ContractVersion { get; }

    internal string Path { get; }

    internal string SourceFingerprint { get; }
}
