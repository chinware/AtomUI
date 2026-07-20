using Avalonia.Platform;

namespace AtomUI.Theme.Definitions;

public sealed class AvaloniaAssetThemeDefinitionResolver : IThemeDefinitionResolver
{
    private readonly IReadOnlyList<IThemeDefinitionSource> _sources;

    public AvaloniaAssetThemeDefinitionResolver(
        string id,
        IReadOnlyList<Uri> resourceUris,
        string sourceRevision)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(resourceUris);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceRevision);

        var sources = new IThemeDefinitionSource[resourceUris.Count];
        for (var index = 0; index < resourceUris.Count; index++)
        {
            var uri = resourceUris[index] ??
                      throw new ArgumentException("Resource URI cannot be null.", nameof(resourceUris));
            if (!uri.IsAbsoluteUri || !string.Equals(uri.Scheme, "avares", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Theme resource URI '{uri}' must be an absolute avares URI.",
                    nameof(resourceUris));
            }
            sources[index] = new AvaloniaAssetThemeDefinitionSource(uri, sourceRevision);
        }

        Id       = id;
        _sources = Array.AsReadOnly(sources);
    }

    public string Id { get; }
    public bool SupportsReload => false;

    public ThemeDefinitionResolveResult Resolve(ThemeDefinitionResolveContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new ThemeDefinitionResolveResult(_sources, Array.Empty<ThemeDiagnostic>());
    }

    private sealed class AvaloniaAssetThemeDefinitionSource(
        Uri uri,
        string revision) : IThemeDefinitionSource
    {
        public string SourceIdentity { get; } = uri.ToString();
        public string SourceRevision { get; } = revision;

        public Stream OpenRead() => AssetLoader.Open(uri);
    }
}
