using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class LanguageProviderPool
    {
        internal static IList<AbstractLanguageProvider> GetLanguageProviders()
        {
            List<AbstractLanguageProvider> languageProviders = new List<AbstractLanguageProvider>();
            languageProviders.Add(new AtomUI.Controls.DatePickerLang.en_US());
            languageProviders.Add(new AtomUI.Controls.DatePickerLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.Localization.en_US());
            languageProviders.Add(new AtomUI.Controls.Localization.zh_CN());
            languageProviders.Add(new AtomUI.Controls.PaginationLang.en_US());
            languageProviders.Add(new AtomUI.Controls.PaginationLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.PopupConfirmLang.en_US());
            languageProviders.Add(new AtomUI.Controls.PopupConfirmLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.TimePickerLang.en_US());
            languageProviders.Add(new AtomUI.Controls.TimePickerLang.zh_CN());
            return languageProviders;
        }
    }
}