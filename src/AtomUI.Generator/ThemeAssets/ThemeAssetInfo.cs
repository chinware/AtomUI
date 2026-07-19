using System.Text.RegularExpressions;
using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

internal sealed class ThemeAssetInfo
{
    private static readonly Regex s_identityPattern = new(
        @"ControlTokenScope\.Identity\s*=\s*""\{x:Static\s+[^:]+:(?<id>[A-Za-z_][A-Za-z0-9_]*)ThemeAsset\.Identity\}""",
        RegexOptions.CultureInvariant);

    private static readonly Regex s_controlTokenPattern = new(
        @"\{[^:{}]+:(?<id>[A-Za-z_][A-Za-z0-9_]*)TokenResource(?:\s|\})",
        RegexOptions.CultureInvariant);

    private static readonly Regex s_sharedTokenPattern = new(
        @"\{[^:{}]+:SharedTokenResource(?:\s|\})",
        RegexOptions.CultureInvariant);

    private ThemeAssetInfo(
        string path,
        string assetPath,
        SourceText source,
        bool usesTokens,
        bool usesSharedTokenResource,
        IReadOnlyList<string> identities,
        IReadOnlyList<string> ownControlTokenFamilies,
        IReadOnlyList<string> controlTokenFamilies)
    {
        Path                 = path;
        AssetPath            = assetPath;
        Source               = source;
        UsesTokens           = usesTokens;
        UsesSharedTokenResource = usesSharedTokenResource;
        Identities           = identities;
        OwnControlTokenFamilies = ownControlTokenFamilies;
        ControlTokenFamilies = controlTokenFamilies;
    }

    internal string Path { get; }
    internal string AssetPath { get; }
    internal SourceText Source { get; }
    internal bool UsesTokens { get; }
    internal bool UsesSharedTokenResource { get; }
    internal IReadOnlyList<string> Identities { get; }
    internal IReadOnlyList<string> OwnControlTokenFamilies { get; }
    internal IReadOnlyList<string> ControlTokenFamilies { get; }

    internal static ThemeAssetInfo Create(AdditionalText text, CancellationToken cancellationToken)
    {
        var source = text.GetText(cancellationToken) ?? SourceText.From(string.Empty);
        var content = source.ToString();
        var identities = Matches(s_identityPattern, content, "id");
        var ownFamilies = Matches(s_controlTokenPattern, content, "id")
                          .Where(static id => !string.Equals(id, "Shared", StringComparison.Ordinal))
                          .Distinct(StringComparer.Ordinal)
                          .OrderBy(static id => id, StringComparer.Ordinal)
                          .ToArray();
        var usesTokens = content.IndexOf("TokenResource", StringComparison.Ordinal) >= 0;
        return new ThemeAssetInfo(
            text.Path,
            NormalizeAssetPath(text.Path),
            source,
            usesTokens,
            s_sharedTokenPattern.IsMatch(content),
            identities,
            ownFamilies,
            ownFamilies);
    }

    internal Location CreateLocation()
    {
        var span = new TextSpan(0, Source.Length);
        return Location.Create(Path, span, Source.Lines.GetLinePositionSpan(span));
    }

    internal IEnumerable<Diagnostic> Validate(ISet<string> controls)
    {
        if (!UsesTokens)
        {
            yield break;
        }
        if (Identities.Count == 0)
        {
            if (!UsesSharedTokenResource || OwnControlTokenFamilies.Count == 0)
            {
                yield break;
            }
            yield return Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ThemeAssetMissingIdentity,
                CreateLocation(),
                AssetPath);
            yield break;
        }
        if (Identities.Count != 1)
        {
            yield return Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ThemeAssetConflictingIdentity,
                CreateLocation(),
                AssetPath);
            yield break;
        }

        var identity = Identities[0];
        if (!controls.Contains(identity))
        {
            yield return Diagnostic.Create(
                AtomUIDiagnosticDescriptors.ThemeAssetUnknownIdentity,
                CreateLocation(),
                AssetPath,
                identity);
            yield break;
        }

        if (UsesSharedTokenResource)
        {
            yield break;
        }

        foreach (var family in ControlTokenFamilies)
        {
            if (!string.Equals(family, identity, StringComparison.Ordinal))
            {
                yield return Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.ThemeAssetControlTokenMismatch,
                    CreateLocation(),
                    AssetPath,
                    identity,
                    family);
                yield break;
            }
        }
    }

    private static IReadOnlyList<string> Matches(Regex regex, string content, string group)
    {
        return regex.Matches(content)
                    .Cast<Match>()
                    .Select(match => match.Groups[group].Value)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(static value => value, StringComparer.Ordinal)
                    .ToArray();
    }

    private static string NormalizeAssetPath(string path)
    {
        var normalized = path.Replace('\\', '/');
        if (!System.IO.Path.IsPathRooted(path))
        {
            return normalized.TrimStart('/');
        }

        var sourceMarker = normalized.IndexOf("/src/", StringComparison.Ordinal);
        if (sourceMarker >= 0)
        {
            var projectStart = sourceMarker + 5;
            var relativeStart = normalized.IndexOf('/', projectStart);
            if (relativeStart >= 0 && relativeStart + 1 < normalized.Length)
            {
                return normalized.Substring(relativeStart + 1);
            }
        }
        return System.IO.Path.GetFileName(path);
    }
}
