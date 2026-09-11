using System.Globalization;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedAxamlUsageInput
{
    private const string Xaml2006Namespace =
        "http://schemas.microsoft.com/winfx/2006/xaml";
    private static readonly HashSet<string> s_usageKinds = new(
        ["Element", "TargetType", "DataType", "Selector", "XType", "UnitRoot", "PackageRoot"],
        StringComparer.Ordinal);

    internal static bool TryParse(string content, out XElement? root, out string error)
    {
        try
        {
            root = XDocument.Parse(content, LoadOptions.None).Root;
        }
        catch (System.Xml.XmlException exception)
        {
            root = null;
            error = exception.Message;
            return false;
        }
        return Validate(root, out error);
    }

    internal static string GetAttribute(XElement element, string name)
    {
        return element.Attribute(name)?.Value.Trim() ?? string.Empty;
    }

    internal static int GetIntAttribute(XElement element, string name)
    {
        return int.TryParse(
            GetAttribute(element, name),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var value) && value >= 0
            ? value
            : 0;
    }

    internal static bool IsXamlLanguageNamespace(string namespaceUri)
    {
        return string.Equals(namespaceUri, Xaml2006Namespace, StringComparison.Ordinal);
    }

    internal static string ResolveMetadataName(
        LinkedRegistrationManifestCatalog catalog,
        string namespaceUri,
        string localName)
    {
        if (namespaceUri.StartsWith("using:", StringComparison.Ordinal))
        {
            return namespaceUri.Substring("using:".Length) + "." + localName;
        }
        if (namespaceUri.StartsWith("clr-namespace:", StringComparison.Ordinal))
        {
            var namespaceValue = namespaceUri.Substring("clr-namespace:".Length);
            var separator = namespaceValue.IndexOf(';');
            if (separator >= 0)
            {
                namespaceValue = namespaceValue.Substring(0, separator);
            }
            return namespaceValue + "." + localName;
        }
        if (catalog.XmlNamespaces.TryGetValue(namespaceUri, out var namespaces))
        {
            var candidates = namespaces.Select(item => item + "." + localName)
                                       .Where(catalog.ControlMaps.ContainsKey)
                                       .Distinct(StringComparer.Ordinal)
                                       .ToArray();
            return candidates.Length == 1 ? candidates[0] : string.Empty;
        }
        return string.Empty;
    }

    internal static INamedTypeSymbol? ResolveKnownType(
        Compilation compilation,
        LinkedRegistrationManifestCatalog catalog,
        string metadataName,
        string namespaceUri,
        string localName)
    {
        if (metadataName.Length != 0 &&
            compilation.GetTypeByMetadataName(metadataName) is { } knownType)
        {
            return knownType;
        }
        if (!catalog.XmlNamespaces.TryGetValue(namespaceUri, out var namespaces))
        {
            return null;
        }
        var candidates = namespaces.Select(clrNamespace =>
                                       compilation.GetTypeByMetadataName(clrNamespace + "." + localName))
                                   .OfType<INamedTypeSymbol>()
                                   .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                                   .ToArray();
        return candidates.Length == 1 ? candidates[0] : null;
    }

    internal static HashSet<string> ResolveCandidatePackages(
        LinkedRegistrationManifestCatalog catalog,
        string metadataName,
        string namespaceUri,
        string localName)
    {
        var packages = new HashSet<string>(StringComparer.Ordinal);
        if (metadataName.Length != 0)
        {
            var candidateEnd = metadataName.LastIndexOf('.');
            var candidateNamespace = candidateEnd > 0
                ? metadataName.Substring(0, candidateEnd)
                : string.Empty;
            foreach (var control in catalog.ControlMaps.Values)
            {
                var namespaceEnd = control.MetadataName.LastIndexOf('.');
                var controlNamespace = namespaceEnd > 0
                    ? control.MetadataName.Substring(0, namespaceEnd)
                    : string.Empty;
                if (string.Equals(controlNamespace, candidateNamespace, StringComparison.Ordinal))
                {
                    packages.Add(control.PackageId);
                }
            }
        }
        if (catalog.XmlNamespacePackages.TryGetValue(namespaceUri, out var namespacePackages))
        {
            packages.UnionWith(namespacePackages);
        }
        return packages;
    }

    private static bool Validate(XElement? root, out string error)
    {
        if (root is null || root.Name != "AtomUIAxamlUsage")
        {
            error = "root element must be AtomUIAxamlUsage";
            return false;
        }
        var version = root.Attribute("Version")?.Value;
        if (!string.Equals(version, "1", StringComparison.Ordinal))
        {
            error = $"unsupported AXAML usage protocol version '{version ?? "<missing>"}'";
            return false;
        }
        foreach (var element in root.Elements())
        {
            if (element.Name == "Usage")
            {
                if (!HasAttributes(
                        element,
                        "Source",
                        "Line",
                        "Column",
                        "Kind",
                        "NamespaceUri",
                        "LocalName",
                        "TypeName",
                        "Identity"))
                {
                    error = "Usage is missing a required attribute";
                    return false;
                }
                var kind = element.Attribute("Kind")!.Value;
                if (!s_usageKinds.Contains(kind))
                {
                    error = $"unknown AXAML Usage Kind '{kind}'";
                    return false;
                }
                if (element.Attribute("Source")!.Value.Length == 0 ||
                    !TryGetNonNegativeInt(element, "Line") ||
                    !TryGetNonNegativeInt(element, "Column"))
                {
                    error = "Usage contains an invalid source location";
                    return false;
                }
            }
            else if (element.Name == "Uncertainty")
            {
                if (!HasAttributes(
                        element,
                        "Source",
                        "Line",
                        "Column",
                        "Reason",
                        "Value",
                        "PackageId"))
                {
                    error = "Uncertainty is missing a required attribute";
                    return false;
                }
                if (element.Attribute("Source")!.Value.Length == 0 ||
                    element.Attribute("Reason")!.Value.Length == 0 ||
                    !TryGetNonNegativeInt(element, "Line") ||
                    !TryGetNonNegativeInt(element, "Column"))
                {
                    error = "Uncertainty contains an invalid source location or reason";
                    return false;
                }
            }
            else
            {
                error = $"unknown AXAML usage element '{element.Name}'";
                return false;
            }
        }
        error = string.Empty;
        return true;
    }

    private static bool HasAttributes(XElement element, params string[] names)
    {
        return names.All(name => element.Attribute(name) is not null);
    }

    private static bool TryGetNonNegativeInt(XElement element, string name)
    {
        return int.TryParse(
                   element.Attribute(name)?.Value,
                   NumberStyles.None,
                   CultureInfo.InvariantCulture,
                   out var value) &&
               value >= 0;
    }

}
