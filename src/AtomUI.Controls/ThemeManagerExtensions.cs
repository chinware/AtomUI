using AtomUI.Theme;
using Avalonia;

namespace AtomUI.Controls;

internal static class ThemeManagerExtensions
{
    public static ThemeManager ConfigureAtomUIControls(this ThemeManager themeManager)
    {
        AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService());
        ControlThemeRegister.Register();
        ControlTokenTypeRegister.Register();
        LanguageProviderRegister.Register();
        return themeManager;
    }
}