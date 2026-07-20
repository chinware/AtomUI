namespace AtomUI.Theme.Definitions;

public interface IThemeDefinitionResolver
{
    string Id { get; }
    bool SupportsReload { get; }

    ThemeDefinitionResolveResult Resolve(ThemeDefinitionResolveContext context);
}
