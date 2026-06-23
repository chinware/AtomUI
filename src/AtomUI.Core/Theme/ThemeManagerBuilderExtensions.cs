namespace AtomUI.Theme;

public static class ThemeManagerBuilderExtensions
{
    /// <summary>
    /// Sets the initial application theme variant by composing the base theme id with built-in theme algorithms.
    /// Use this during <c>Application.Initialize()</c> when the first rendered frame should already use
    /// dark or compact resources.
    /// </summary>
    /// <param name="themeManagerBuilder">The AtomUI theme manager builder.</param>
    /// <param name="themeId">The base theme id, for example <see cref="IThemeManager.DEFAULT_THEME_ID"/>.</param>
    /// <param name="algorithms">The initial theme algorithms. Duplicate values are ignored and output order is normalized.</param>
    public static void WithDefaultTheme(this IThemeManagerBuilder themeManagerBuilder,
                                        string themeId,
                                        params ThemeAlgorithm[] algorithms)
    {
        ArgumentNullException.ThrowIfNull(themeManagerBuilder);
        ArgumentException.ThrowIfNullOrEmpty(themeId);

        var hasDark    = false;
        var hasCompact = false;
        foreach (var algorithm in algorithms)
        {
            switch (algorithm)
            {
                case ThemeAlgorithm.Default:
                    break;

                case ThemeAlgorithm.Dark:
                    hasDark = true;
                    break;

                case ThemeAlgorithm.Compact:
                    hasCompact = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(algorithms),
                        algorithm,
                        $"Unsupported theme algorithm: {algorithm}.");
            }
        }

        themeManagerBuilder.WithDefaultTheme(Theme.BuildThemeVariantName(themeId, hasDark, hasCompact));
    }
}
