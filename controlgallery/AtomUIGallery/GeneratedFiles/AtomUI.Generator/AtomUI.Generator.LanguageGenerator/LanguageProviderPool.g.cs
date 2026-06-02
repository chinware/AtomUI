using System.Collections.Generic;
using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Theme.Language
{
    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(8);
            languageProviders.Add(new AtomUIGallery.ShowCases.AboutUs.en_US());
            languageProviders.Add(new AtomUIGallery.ShowCases.AboutUs.zh_CN());
            languageProviders.Add(new AtomUIGallery.ShowCases.Localization.ShowCaseScenarioLang.en_US());
            languageProviders.Add(new AtomUIGallery.ShowCases.Localization.ShowCaseScenarioLang.zh_CN());
            languageProviders.Add(new AtomUIGallery.Workspace.Localization.CaseNavigationLang.en_US());
            languageProviders.Add(new AtomUIGallery.Workspace.Localization.CaseNavigationLang.zh_CN());
            languageProviders.Add(new AtomUIGallery.Workspace.Localization.WorkspaceWindowLang.en_US());
            languageProviders.Add(new AtomUIGallery.Workspace.Localization.WorkspaceWindowLang.zh_CN());
            return languageProviders;
        }
    }
}