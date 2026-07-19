using System.Security.Cryptography;
using AtomUI.Theme.Schema;
using Avalonia.Platform;

namespace AtomUI.Theme.Definitions;

internal sealed class CompiledThemeCatalog
{
    private readonly Dictionary<string, CompiledThemeDefinition> _definitions;

    private CompiledThemeCatalog(
        Dictionary<string, CompiledThemeDefinition> definitions,
        CompiledThemeDefinition defaultDefinition)
    {
        _definitions = definitions;
        DefaultDefinition = defaultDefinition;
        AvailableThemes = Array.AsReadOnly(
            definitions.Values
                       .Select(static entry => entry.Info)
                       .OrderBy(static info => info.Id, StringComparer.Ordinal)
                       .ToArray());
    }

    internal CompiledThemeDefinition DefaultDefinition { get; }
    internal IReadOnlyList<ThemeInfo> AvailableThemes { get; }

    internal CompiledThemeDefinition Get(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return _definitions.TryGetValue(id, out var definition)
            ? definition
            : throw new ThemeLoadException($"Theme '{id}' was not found in the compiled catalog.");
    }

    internal static CompiledThemeCatalog LoadBuiltIn(ThemeSchemaRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        var uri = new Uri("avares://AtomUI.Core/Assets/Themes/DaybreakBlue.xml");
        using var stream = AssetLoader.Open(uri);
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        var bytes = buffer.ToArray();
        using var input = new MemoryStream(bytes, writable: false);
        var read = ThemeDocumentReader.Read(input, uri.ToString());
        if (!read.Success)
        {
            throw new ThemeLoadException(Describe(read.Diagnostics));
        }

        var bound = ThemeDefinitionBinder.Bind(read.Document!, registry);
        if (!bound.Success)
        {
            throw new ThemeLoadException(Describe(bound.Diagnostics));
        }

        var definition = bound.Definition!;
        var revision = new ThemeDefinitionRevision(
            uri.ToString(),
            "1",
            Convert.ToHexString(SHA256.HashData(bytes)));
        var entry = new CompiledThemeDefinition(
            new ThemeInfo(
                definition.Id,
                definition.Name,
                definition.EffectiveAppearance,
                definition.IsDefault),
            definition,
            revision);
        var definitions = new Dictionary<string, CompiledThemeDefinition>(StringComparer.Ordinal)
        {
            [definition.Id] = entry
        };
        return new CompiledThemeCatalog(definitions, entry);
    }

    private static string Describe(IReadOnlyList<ThemeDefinitionDiagnostic> diagnostics)
    {
        return string.Join(
            Environment.NewLine,
            diagnostics.Select(static diagnostic =>
                $"{diagnostic.Code} {diagnostic.FilePath}{diagnostic.Path}: {diagnostic.Message}"));
    }
}

internal sealed record CompiledThemeDefinition(
    ThemeInfo Info,
    BoundThemeDefinition Definition,
    ThemeDefinitionRevision Revision);
