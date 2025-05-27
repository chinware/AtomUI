using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class LanguageProviderPool
    {
        internal static IList<AbstractLanguageProvider> GetLanguageProviders()
        {
            List<AbstractLanguageProvider> languageProviders = new List<AbstractLanguageProvider>();
            return languageProviders;
        }
    }
}