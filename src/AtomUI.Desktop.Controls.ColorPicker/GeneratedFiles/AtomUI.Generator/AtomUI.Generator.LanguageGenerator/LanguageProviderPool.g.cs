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
            languageProviders.Add(new AtomUI.Desktop.Controls.ColorPickerLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.ColorPickerLang.zh_CN());
            return languageProviders;
        }
    }
}