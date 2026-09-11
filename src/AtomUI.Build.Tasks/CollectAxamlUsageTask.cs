using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class CollectAxamlUsageTask : ITask
{
    private const string Xaml2006Namespace = "http://schemas.microsoft.com/winfx/2006/xaml";

    [Required]
    public ITaskItem[] AxamlFiles { get; set; } = [];

    [Required]
    public string ProjectDirectory { get; set; } = string.Empty;

    [Required]
    public string OutputPath { get; set; } = string.Empty;

    public string ProjectPackageId { get; set; } = string.Empty;

    public ITaskItem[] UnitRoots { get; set; } = [];

    public ITaskItem[] PackageRoots { get; set; } = [];

    [Output]
    public ITaskItem[] UsageCandidates { get; set; } = [];

    [Output]
    public ITaskItem[] Uncertainties { get; set; } = [];

    public IBuildEngine BuildEngine { get; set; } = null!;

    public ITaskHost HostObject { get; set; } = null!;

    public bool Execute()
    {
        var usages = new List<UsageEntry>();
        var uncertainties = new List<UncertaintyEntry>();
        var projectPackageId = ProjectPackageId.Trim();
        foreach (var item in AxamlFiles.OrderBy(GetLogicalPath, StringComparer.Ordinal))
        {
            CollectFile(item, projectPackageId, usages, uncertainties);
        }

        foreach (var root in UnitRoots.OrderBy(static item => item.ItemSpec, StringComparer.Ordinal))
        {
            usages.Add(UsageEntry.ForRoot("UnitRoot", root.ItemSpec));
        }
        foreach (var root in PackageRoots.OrderBy(static item => item.ItemSpec, StringComparer.Ordinal))
        {
            usages.Add(UsageEntry.ForRoot("PackageRoot", root.ItemSpec));
        }

        var orderedUsages = usages.Distinct(UsageEntryComparer.Instance)
                                  .OrderBy(static usage => usage.Source, StringComparer.Ordinal)
                                  .ThenBy(static usage => usage.Line)
                                  .ThenBy(static usage => usage.Column)
                                  .ThenBy(static usage => usage.Kind, StringComparer.Ordinal)
                                  .ThenBy(static usage => usage.TypeName, StringComparer.Ordinal)
                                  .ThenBy(static usage => usage.Identity, StringComparer.Ordinal)
                                  .ToArray();
        var orderedUncertainties = uncertainties.Distinct(UncertaintyEntryComparer.Instance)
                                                .OrderBy(static item => item.Source, StringComparer.Ordinal)
                                                .ThenBy(static item => item.Line)
                                                .ThenBy(static item => item.Column)
                                                .ThenBy(static item => item.Reason, StringComparer.Ordinal)
                                                .ThenBy(static item => item.Value, StringComparer.Ordinal)
                                                .ThenBy(static item => item.PackageId, StringComparer.Ordinal)
                                                .ToArray();

        UsageCandidates = orderedUsages.Select(CreateUsageItem).Cast<ITaskItem>().ToArray();
        Uncertainties = orderedUncertainties.Select(CreateUncertaintyItem).Cast<ITaskItem>().ToArray();
        WriteOutput(orderedUsages, orderedUncertainties);
        return true;
    }

    private void CollectFile(
        ITaskItem item,
        string projectPackageId,
        ICollection<UsageEntry> usages,
        ICollection<UncertaintyEntry> uncertainties)
    {
        var source = GetLogicalPath(item);
        var isLooseAxaml = string.Equals(
            item.GetMetadata("AtomUILooseXaml"),
            "true",
            StringComparison.OrdinalIgnoreCase);
        AddMalformedOwnershipUncertainty(item, source, projectPackageId, uncertainties);

        try
        {
            var document = XDocument.Load(
                GetFullPath(item),
                LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            foreach (var element in document.Root?.DescendantsAndSelf() ?? [])
            {
                AddElementUsage(element, source, usages);
                AddAttributeUsages(element, source, usages);
                AddDynamicSourceUncertainty(element, source, projectPackageId, uncertainties);
            }

            if (isLooseAxaml)
            {
                AddLooseAxamlUncertainty(source, projectPackageId, uncertainties);
            }
        }
        catch (Exception exception) when (
            exception is IOException ||
            exception is UnauthorizedAccessException ||
            exception is XmlException)
        {
            if (isLooseAxaml)
            {
                uncertainties.Add(new UncertaintyEntry(
                    source,
                    0,
                    0,
                    "LooseAxaml",
                    source,
                    projectPackageId));
            }
            uncertainties.Add(new UncertaintyEntry(
                source,
                0,
                0,
                "MalformedXml",
                exception.Message,
                projectPackageId));
        }
    }

    private static void AddMalformedOwnershipUncertainty(
        ITaskItem item,
        string source,
        string projectPackageId,
        ICollection<UncertaintyEntry> uncertainties)
    {
        var ownership = item.GetMetadata("AtomUIRegistrationUnit");
        if (string.IsNullOrWhiteSpace(ownership) || IsValidOwnership(ownership))
        {
            return;
        }

        uncertainties.Add(new UncertaintyEntry(
            source,
            0,
            0,
            "MalformedOwnership",
            ownership,
            GetPackageIdFromOwnership(ownership, projectPackageId)));
    }

    private static void AddLooseAxamlUncertainty(
        string source,
        string projectPackageId,
        ICollection<UncertaintyEntry> uncertainties)
    {
        uncertainties.Add(new UncertaintyEntry(
            source,
            0,
            0,
            "LooseAxaml",
            source,
            projectPackageId));
    }

    private static void AddElementUsage(
        XElement element,
        string source,
        ICollection<UsageEntry> usages)
    {
        var localName = element.Name.LocalName;
        var propertySeparator = localName.IndexOf('.');
        if (propertySeparator > 0)
        {
            localName = localName.Substring(0, propertySeparator);
        }
        AddTypeUsage(
            element.Name.NamespaceName,
            localName,
            "Element",
            source,
            element,
            usages);
    }

    private static void AddAttributeUsages(
        XElement element,
        string source,
        ICollection<UsageEntry> usages)
    {
        foreach (var attribute in element.Attributes().Where(static attribute =>
                     !attribute.IsNamespaceDeclaration))
        {
            var kind = attribute.Name.LocalName;
            if (string.Equals(kind, "Selector", StringComparison.Ordinal))
            {
                AddSelectorUsages(element, attribute, source, usages);
            }

            AddXTypeUsages(element, attribute, source, usages);
            if (string.Equals(kind, "TargetType", StringComparison.Ordinal) ||
                string.Equals(kind, "DataType", StringComparison.Ordinal))
            {
                AddDirectTypeUsage(element, attribute.Value, kind, source, attribute, usages);
            }
        }
    }

    private static void AddDirectTypeUsage(
        XElement context,
        string value,
        string kind,
        string source,
        XObject location,
        ICollection<UsageEntry> usages)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith("{", StringComparison.Ordinal))
        {
            return;
        }

        if (TryResolveQName(context, trimmed, out var namespaceUri, out var localName))
        {
            AddTypeUsage(namespaceUri, localName, kind, source, location, usages);
        }
    }

    private static void AddXTypeUsages(
        XElement context,
        XAttribute attribute,
        string source,
        ICollection<UsageEntry> usages)
    {
        for (var start = attribute.Value.IndexOf('{');
             start >= 0;
             start = attribute.Value.IndexOf('{', start + 1))
        {
            var tokenStart = start + 1;
            var tokenEnd = tokenStart;
            while (tokenEnd < attribute.Value.Length &&
                   !char.IsWhiteSpace(attribute.Value[tokenEnd]) &&
                   attribute.Value[tokenEnd] != '}')
            {
                tokenEnd++;
            }
            if (tokenEnd == tokenStart)
            {
                continue;
            }

            var token = attribute.Value.Substring(tokenStart, tokenEnd - tokenStart);
            var separator = token.IndexOf(':');
            if (separator <= 0 ||
                !string.Equals(token.Substring(separator + 1), "Type", StringComparison.Ordinal) ||
                !string.Equals(
                    context.GetNamespaceOfPrefix(token.Substring(0, separator))?.NamespaceName,
                    Xaml2006Namespace,
                    StringComparison.Ordinal))
            {
                continue;
            }

            var valueEnd = attribute.Value.IndexOf('}', tokenEnd);
            if (valueEnd < 0)
            {
                continue;
            }

            var value = attribute.Value.Substring(tokenEnd, valueEnd - tokenEnd).Trim();
            if (TryResolveQName(context, value, out var namespaceUri, out var localName))
            {
                AddTypeUsage(namespaceUri, localName, "XType", source, attribute, usages);
            }
        }
    }

    private static void AddSelectorUsages(
        XElement context,
        XAttribute attribute,
        string source,
        ICollection<UsageEntry> usages)
    {
        var selector = attribute.Value;
        for (var separator = selector.IndexOf('|');
             separator > 0;
             separator = selector.IndexOf('|', separator + 1))
        {
            var prefixStart = separator - 1;
            while (prefixStart >= 0 && IsSelectorIdentifierCharacter(selector[prefixStart]))
            {
                prefixStart--;
            }
            prefixStart++;

            var typeEnd = separator + 1;
            while (typeEnd < selector.Length && IsSelectorIdentifierCharacter(selector[typeEnd]))
            {
                typeEnd++;
            }
            if (prefixStart == separator || typeEnd == separator + 1)
            {
                continue;
            }

            var prefix = selector.Substring(prefixStart, separator - prefixStart);
            var localName = selector.Substring(separator + 1, typeEnd - separator - 1);
            var namespaceUri = context.GetNamespaceOfPrefix(prefix)?.NamespaceName;
            if (!string.IsNullOrWhiteSpace(namespaceUri))
            {
                AddTypeUsage(namespaceUri!, localName, "Selector", source, attribute, usages);
            }
        }
    }

    private static void AddDynamicSourceUncertainty(
        XElement element,
        string source,
        string projectPackageId,
        ICollection<UncertaintyEntry> uncertainties)
    {
        if (!string.Equals(element.Name.LocalName, "ResourceInclude", StringComparison.Ordinal) &&
            !string.Equals(element.Name.LocalName, "StyleInclude", StringComparison.Ordinal))
        {
            return;
        }

        var sourceAttribute = element.Attributes().FirstOrDefault(static attribute =>
            string.Equals(attribute.Name.LocalName, "Source", StringComparison.Ordinal));
        if (sourceAttribute is null)
        {
            return;
        }

        var value = sourceAttribute.Value.Trim();
        if (value.StartsWith("{", StringComparison.Ordinal) ||
            !Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _))
        {
            var (line, column) = GetLocation(sourceAttribute);
            uncertainties.Add(new UncertaintyEntry(
                source,
                line,
                column,
                "DynamicResourceSource",
                value,
                projectPackageId));
        }
    }

    private static bool IsValidOwnership(string ownership)
    {
        var normalized = ownership.Replace('\\', '/').Trim();
        if (normalized.Length == 0 || normalized.StartsWith("/", StringComparison.Ordinal) ||
            normalized.EndsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        return normalized.Split('/').All(IsValidOwnershipSegment);
    }

    private static bool IsValidOwnershipSegment(string value)
    {
        return value.Length != 0 && value.All(static character =>
            char.IsLetterOrDigit(character) || character is '_' or '-' or '.');
    }

    private static string GetPackageIdFromOwnership(string ownership, string projectPackageId)
    {
        var normalized = ownership.Replace('\\', '/').Trim().TrimStart('/');
        var separator = normalized.IndexOf('/');
        if (separator <= 0)
        {
            return projectPackageId;
        }

        var packageId = normalized.Substring(0, separator);
        return IsValidOwnershipSegment(packageId) ? packageId : projectPackageId;
    }

    private static bool TryResolveQName(
        XElement context,
        string value,
        out string namespaceUri,
        out string localName)
    {
        var separator = value.IndexOf(':');
        if (separator > 0)
        {
            var prefix = value.Substring(0, separator);
            localName = value.Substring(separator + 1);
            namespaceUri = context.GetNamespaceOfPrefix(prefix)?.NamespaceName ?? string.Empty;
            return namespaceUri.Length != 0 && localName.Length != 0;
        }

        namespaceUri = context.GetDefaultNamespace().NamespaceName;
        localName = value;
        return localName.Length != 0;
    }

    private static void AddTypeUsage(
        string namespaceUri,
        string localName,
        string kind,
        string source,
        XObject location,
        ICollection<UsageEntry> usages)
    {
        if (string.IsNullOrWhiteSpace(localName))
        {
            return;
        }

        var (line, column) = GetLocation(location);
        usages.Add(new UsageEntry(
            source,
            line,
            column,
            kind,
            namespaceUri,
            localName,
            GetMetadataName(namespaceUri, localName),
            string.Empty));
    }

    private static string GetMetadataName(string namespaceUri, string localName)
    {
        if (namespaceUri.StartsWith("using:", StringComparison.Ordinal))
        {
            return namespaceUri.Substring("using:".Length) + "." + localName;
        }
        if (namespaceUri.StartsWith("clr-namespace:", StringComparison.Ordinal))
        {
            var namespaceValue = namespaceUri.Substring("clr-namespace:".Length);
            var assemblySeparator = namespaceValue.IndexOf(';');
            if (assemblySeparator >= 0)
            {
                namespaceValue = namespaceValue.Substring(0, assemblySeparator);
            }
            return namespaceValue + "." + localName;
        }
        return string.Empty;
    }

    private static bool IsSelectorIdentifierCharacter(char value)
    {
        return char.IsLetterOrDigit(value) || value is '_' or '+' or '`';
    }

    private static (int Line, int Column) GetLocation(XObject value)
    {
        return value is IXmlLineInfo lineInfo && lineInfo.HasLineInfo()
            ? (lineInfo.LineNumber, lineInfo.LinePosition)
            : (0, 0);
    }

    private void WriteOutput(
        IReadOnlyList<UsageEntry> usages,
        IReadOnlyList<UncertaintyEntry> uncertainties)
    {
        var root = new XElement("AtomUIAxamlUsage", new XAttribute("Version", "1"));
        foreach (var usage in usages)
        {
            root.Add(new XElement(
                "Usage",
                new XAttribute("Source", usage.Source),
                new XAttribute("Line", usage.Line),
                new XAttribute("Column", usage.Column),
                new XAttribute("Kind", usage.Kind),
                new XAttribute("NamespaceUri", usage.NamespaceUri),
                new XAttribute("LocalName", usage.LocalName),
                new XAttribute("TypeName", usage.TypeName),
                new XAttribute("Identity", usage.Identity)));
        }
        foreach (var uncertainty in uncertainties)
        {
            root.Add(new XElement(
                "Uncertainty",
                new XAttribute("Source", uncertainty.Source),
                new XAttribute("Line", uncertainty.Line),
                new XAttribute("Column", uncertainty.Column),
                new XAttribute("Reason", uncertainty.Reason),
                new XAttribute("Value", uncertainty.Value),
                new XAttribute("PackageId", uncertainty.PackageId)));
        }

        var content = new XDocument(root).ToString(SaveOptions.DisableFormatting);
        var directory = Path.GetDirectoryName(OutputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(OutputPath) || !string.Equals(File.ReadAllText(OutputPath), content, StringComparison.Ordinal))
        {
            File.WriteAllText(OutputPath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }
    }

    private static GeneratedTaskItem CreateUsageItem(UsageEntry usage)
    {
        var item = new GeneratedTaskItem(
            $"{usage.Source}:{usage.Line.ToString(CultureInfo.InvariantCulture)}:{usage.Column.ToString(CultureInfo.InvariantCulture)}:{usage.Kind}");
        item.SetMetadata("Source", usage.Source);
        item.SetMetadata("Line", usage.Line.ToString(CultureInfo.InvariantCulture));
        item.SetMetadata("Column", usage.Column.ToString(CultureInfo.InvariantCulture));
        item.SetMetadata("Kind", usage.Kind);
        item.SetMetadata("NamespaceUri", usage.NamespaceUri);
        item.SetMetadata("LocalName", usage.LocalName);
        item.SetMetadata("TypeName", usage.TypeName);
        item.SetMetadata("Identity", usage.Identity);
        return item;
    }

    private static GeneratedTaskItem CreateUncertaintyItem(UncertaintyEntry uncertainty)
    {
        var item = new GeneratedTaskItem(
            $"{uncertainty.Source}:{uncertainty.Line.ToString(CultureInfo.InvariantCulture)}:{uncertainty.Column.ToString(CultureInfo.InvariantCulture)}:{uncertainty.Reason}");
        item.SetMetadata("Source", uncertainty.Source);
        item.SetMetadata("Line", uncertainty.Line.ToString(CultureInfo.InvariantCulture));
        item.SetMetadata("Column", uncertainty.Column.ToString(CultureInfo.InvariantCulture));
        item.SetMetadata("Reason", uncertainty.Reason);
        item.SetMetadata("Value", uncertainty.Value);
        item.SetMetadata("PackageId", uncertainty.PackageId);
        return item;
    }

    private string GetLogicalPath(ITaskItem item)
    {
        var logicalPath = item.GetMetadata("Link");
        if (string.IsNullOrWhiteSpace(logicalPath))
        {
            logicalPath = item.ItemSpec;
            if (Path.IsPathRooted(logicalPath))
            {
                logicalPath = GetRelativePath(ProjectDirectory, logicalPath);
            }
        }
        return logicalPath.Replace('\\', '/').TrimStart('/');
    }

    private static string GetFullPath(ITaskItem item)
    {
        var path = item.GetMetadata("FullPath");
        return string.IsNullOrWhiteSpace(path) ? item.ItemSpec : path;
    }

    private static string GetRelativePath(string relativeTo, string path)
    {
        var baseUri = new Uri(AppendDirectorySeparator(Path.GetFullPath(relativeTo)));
        var pathUri = new Uri(Path.GetFullPath(path));
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(pathUri).ToString());
    }

    private static string AppendDirectorySeparator(string path)
    {
        return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? path
            : path + Path.DirectorySeparatorChar;
    }

    private sealed class UsageEntry
    {
        internal UsageEntry(
            string source,
            int line,
            int column,
            string kind,
            string namespaceUri,
            string localName,
            string typeName,
            string identity)
        {
            Source = source;
            Line = line;
            Column = column;
            Kind = kind;
            NamespaceUri = namespaceUri;
            LocalName = localName;
            TypeName = typeName;
            Identity = identity;
        }

        internal string Source { get; }
        internal int Line { get; }
        internal int Column { get; }
        internal string Kind { get; }
        internal string NamespaceUri { get; }
        internal string LocalName { get; }
        internal string TypeName { get; }
        internal string Identity { get; }

        internal static UsageEntry ForRoot(string kind, string identity)
        {
            return new UsageEntry("<Project>", 0, 0, kind, string.Empty, string.Empty, string.Empty, identity);
        }
    }

    private sealed class UncertaintyEntry
    {
        internal UncertaintyEntry(
            string source,
            int line,
            int column,
            string reason,
            string value,
            string packageId)
        {
            Source = source;
            Line = line;
            Column = column;
            Reason = reason;
            Value = value;
            PackageId = packageId;
        }

        internal string Source { get; }
        internal int Line { get; }
        internal int Column { get; }
        internal string Reason { get; }
        internal string Value { get; }
        internal string PackageId { get; }
    }

    private sealed class UsageEntryComparer : IEqualityComparer<UsageEntry>
    {
        internal static readonly UsageEntryComparer Instance = new();

        public bool Equals(UsageEntry? x, UsageEntry? y)
        {
            return x is not null && y is not null &&
                   x.Source == y.Source && x.Line == y.Line && x.Column == y.Column &&
                   x.Kind == y.Kind && x.NamespaceUri == y.NamespaceUri &&
                   x.LocalName == y.LocalName && x.TypeName == y.TypeName &&
                   x.Identity == y.Identity;
        }

        public int GetHashCode(UsageEntry obj)
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(obj.Source);
                hash = (hash * 397) ^ obj.Line;
                hash = (hash * 397) ^ obj.Column;
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.Kind);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.NamespaceUri);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.LocalName);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.TypeName);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.Identity);
                return hash;
            }
        }
    }

    private sealed class UncertaintyEntryComparer : IEqualityComparer<UncertaintyEntry>
    {
        internal static readonly UncertaintyEntryComparer Instance = new();

        public bool Equals(UncertaintyEntry? x, UncertaintyEntry? y)
        {
            return x is not null && y is not null &&
                   x.Source == y.Source && x.Line == y.Line && x.Column == y.Column &&
                   x.Reason == y.Reason && x.Value == y.Value && x.PackageId == y.PackageId;
        }

        public int GetHashCode(UncertaintyEntry obj)
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(obj.Source);
                hash = (hash * 397) ^ obj.Line;
                hash = (hash * 397) ^ obj.Column;
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.Reason);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.Value);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(obj.PackageId);
                return hash;
            }
        }
    }
}
