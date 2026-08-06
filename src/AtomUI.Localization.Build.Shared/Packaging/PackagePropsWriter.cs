using System.Xml.Linq;

namespace AtomUI.Localization.Build;

internal static class PackagePropsWriter
{
    internal static string Write(
        string packageId,
        IReadOnlyList<LanguagePackageCatalogEntry> catalogs,
        string sourceKind = "StaticLanguagePack")
    {
        var itemGroup = new XElement("ItemGroup");
        foreach (var catalog in catalogs
                                        .OrderBy(static entry => entry.ModuleId, StringComparer.Ordinal)
                                        .ThenBy(static entry => entry.CatalogId, StringComparer.Ordinal))
        {
            itemGroup.Add(new XElement(
                "AtomUILanguage",
                new XAttribute(
                    "Include",
                    "$(MSBuildThisFileDirectory)../contentFiles/any/any/" + catalog.Path),
                new XAttribute("AtomUILanguageSourceKind", sourceKind),
                new XAttribute("AtomUILanguageSourceIdentity", packageId),
                new XAttribute("AtomUILanguageModuleId", catalog.ModuleId),
                new XAttribute("AtomUILanguageContractVersion", catalog.ContractVersion),
                new XAttribute("AtomUILanguagePackagePath", catalog.Path),
                new XAttribute("AtomUILanguageSourceFingerprint", catalog.SourceFingerprint)));
        }

        return DeterministicXml.Write(
            new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Project", itemGroup)));
    }
}
