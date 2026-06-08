using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Desktop.Controls.ColorPickerLang.en_US))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Desktop.Controls.ColorPickerLang.zh_CN))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicFields, typeof(AtomUI.Desktop.Controls.ColorPickerLang.zh_TW))]
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(3);
            languageProviders.Add(new AtomUI.Desktop.Controls.ColorPickerLang.en_US());
            languageProviders.Add(new AtomUI.Desktop.Controls.ColorPickerLang.zh_CN());
            languageProviders.Add(new AtomUI.Desktop.Controls.ColorPickerLang.zh_TW());
            return languageProviders;
        }
    }
}