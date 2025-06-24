using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class LanguageProviderPool
    {
        internal static IList<AbstractLanguageProvider> GetLanguageProviders()
        {
            List<AbstractLanguageProvider> languageProviders = new List<AbstractLanguageProvider>();
            languageProviders.Add(new AtomUI.Controls.DataGridLang.en_US());
            languageProviders.Add(new AtomUI.Controls.DataGridLang.zh_CN());
            return languageProviders;
        }
    }
}