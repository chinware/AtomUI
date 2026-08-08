using System.Xml.Linq;

namespace AtomUI.Localization.Build;

internal static class LanguagePackageManifestWriter
{
    internal static string Write(LanguagePackageManifest manifest)
    {
        if (manifest is null)
        {
            throw new ArgumentNullException(nameof(manifest));
        }

        var root = new XElement(
            "languagePack",
            new XAttribute("packageId", manifest.PackageId),
            new XAttribute("language", manifest.Language));
        foreach (var catalog in manifest.Catalogs
                                        .OrderBy(static entry => entry.ModuleId, StringComparer.Ordinal)
                                        .ThenBy(static entry => entry.CatalogId, StringComparer.Ordinal))
        {
            var element = new XElement(
                "catalog",
                new XAttribute("moduleId", catalog.ModuleId),
                new XAttribute("catalogId", catalog.CatalogId),
                new XAttribute("contractValidation", catalog.ContractValidation),
                new XAttribute("path", catalog.Path),
                new XAttribute("sourceFingerprint", catalog.SourceFingerprint));
            if (catalog.ContractVersion is { } contractVersion)
            {
                element.Add(new XAttribute("contractVersion", contractVersion));
            }
            root.Add(element);
        }

        return DeterministicXml.Write(
            new XDocument(new XDeclaration("1.0", "utf-8", null), root));
    }
}
