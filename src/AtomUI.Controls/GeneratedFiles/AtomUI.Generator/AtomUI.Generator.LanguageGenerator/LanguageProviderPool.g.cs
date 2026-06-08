using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Controls.Localization.en_US))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Controls.Localization.zh_CN))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Controls.Localization.zh_TW))]
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(3);
            languageProviders.Add(new AtomUI.Controls.Localization.en_US());
            languageProviders.Add(new AtomUI.Controls.Localization.zh_CN());
            languageProviders.Add(new AtomUI.Controls.Localization.zh_TW());
            return languageProviders;
        }
    }
}