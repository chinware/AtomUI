using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class LanguageProviderPool
    {
        internal static IList<AbstractLanguageProvider> GetLanguageProviders()
        {
            List<AbstractLanguageProvider> languageProviders = new List<AbstractLanguageProvider>();
            languageProviders.Add(new AtomUI.Controls.DataGridLocalization.en_US());
            languageProviders.Add(new AtomUI.Controls.DataGridLocalization.zh_CN());
            return languageProviders;
        }
    }
}