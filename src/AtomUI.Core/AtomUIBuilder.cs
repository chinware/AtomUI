using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;

namespace AtomUI;

internal sealed class AtomUIBuilder : IAtomUIBuilder
{
    internal AtomUIBuilder(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        ThemeManagerBuilder = new ThemeManagerBuilder(application);
        LocalizationBuilder = new LocalizationBuilder();
    }

    public IThemeManagerBuilder Theme => ThemeManagerBuilder;

    public ILocalizationBuilder Localization => LocalizationBuilder;

    internal ThemeManagerBuilder ThemeManagerBuilder { get; }

    internal LocalizationBuilder LocalizationBuilder { get; }
}
