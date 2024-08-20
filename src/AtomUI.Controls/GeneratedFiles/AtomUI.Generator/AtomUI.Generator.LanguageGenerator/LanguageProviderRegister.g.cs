namespace AtomUI.Theme
{
    internal class LanguageProviderRegister
    {
        internal static void Register()
        {
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.Lang.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.LoadingLang.zh_CN());
        }
    }
}