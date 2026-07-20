namespace AtomUI.Theme.Definitions;

internal static class CoreThemeDefinitionResolver
{
    internal const string ResolverId = "AtomUI.Core.BuiltInThemes";

    internal static IThemeDefinitionResolver Create()
    {
        return new AvaloniaAssetThemeDefinitionResolver(
            ResolverId,
            [new Uri("avares://AtomUI.Core/Assets/Themes/DaybreakBlue.theme.xml")],
            typeof(CoreThemeDefinitionResolver).Assembly.GetName().Version?.ToString() ?? "0");
    }
}
