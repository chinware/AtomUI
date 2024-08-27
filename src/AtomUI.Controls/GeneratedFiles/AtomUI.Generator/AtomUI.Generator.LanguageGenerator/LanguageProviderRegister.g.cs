namespace AtomUI.Theme
{
    internal class LanguageProviderRegister
    {
        internal static void Register()
        {
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.PopupConfirmLang.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.PopupConfirmLang.zh_CN());
        }
    }
}