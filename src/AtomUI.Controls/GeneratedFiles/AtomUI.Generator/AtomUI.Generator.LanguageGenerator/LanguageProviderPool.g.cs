using System.Collections.Generic;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>();
            languageProviders.Add(new AtomUI.Controls.DatePickerLang.en_US());
            languageProviders.Add(new AtomUI.Controls.DatePickerLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.DialogLang.en_US());
            languageProviders.Add(new AtomUI.Controls.DialogLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.Localization.en_US());
            languageProviders.Add(new AtomUI.Controls.Localization.zh_CN());
            languageProviders.Add(new AtomUI.Controls.PaginationLang.en_US());
            languageProviders.Add(new AtomUI.Controls.PaginationLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.PopupConfirmLang.en_US());
            languageProviders.Add(new AtomUI.Controls.PopupConfirmLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.QRCodeLang.en_US());
            languageProviders.Add(new AtomUI.Controls.QRCodeLang.zh_CN());
            languageProviders.Add(new AtomUI.Controls.TimePickerLang.en_US());
            languageProviders.Add(new AtomUI.Controls.TimePickerLang.zh_CN());
            return languageProviders;
        }
    }
}