using System.Collections.Generic;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(3);
            languageProviders.Add(new AtomUI.Desktop.Controls.DataGridLocalization.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.DataGridLocalization.zh_CN());
            languageProviders.Add(new AtomUI.Desktop.Controls.DataGridLocalization.zh_TW());
            return languageProviders;
        }
    }
}