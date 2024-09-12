namespace AtomUI.Theme
{
    internal class LanguageProviderRegister
    {
        internal static void Register()
        {
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.DatePickerLang.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.DatePickerLang.zh_CN());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.Localization.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.Localization.zh_CN());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.PopupConfirmLang.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.PopupConfirmLang.zh_CN());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.TimePickerLang.en_US());
            ThemeManager.Current.RegisterLanguageProvider(new AtomUI.Controls.TimePickerLang.zh_CN());
        }
    }
}