using AtomUI.Localization;
using AtomUI.Theme;

namespace AtomUI;

public interface IAtomUIBuilder
{
    IThemeManagerBuilder Theme { get; }

    ILocalizationBuilder Localization { get; }
}
