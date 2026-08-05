using System.Collections.Generic;
using AtomUI.Theme.Language;
using Avalonia.Controls;

namespace AtomUI.Generated.AtomUI_Toolkits_GalleryBase;

    internal sealed class GalleryShowCaseHeaderEnUSLanguageProvider : LanguageProvider
    {
        public GalleryShowCaseHeaderEnUSLanguageProvider()
            : base(LanguageCode.en_US, "GalleryShowCaseHeader")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.BaseClassLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.en_US.BaseClassLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.NamespaceLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.en_US.NamespaceLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.PackageLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.en_US.PackageLabel;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class GalleryShowCaseHeaderZhCNLanguageProvider : LanguageProvider
    {
        public GalleryShowCaseHeaderZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "GalleryShowCaseHeader")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.BaseClassLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_CN.BaseClassLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.NamespaceLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_CN.NamespaceLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.PackageLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_CN.PackageLabel;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class GalleryShowCaseHeaderZhTWLanguageProvider : LanguageProvider
    {
        public GalleryShowCaseHeaderZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "GalleryShowCaseHeader")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.BaseClassLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_TW.BaseClassLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.NamespaceLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_TW.NamespaceLabel;
                dictionary[global::AtomUI.Toolkits.GalleryBase.Localization.GalleryShowCaseHeaderLangResourceKind.PackageLabel] = global::AtomUI.Toolkits.GalleryBase.Localization.zh_TW.PackageLabel;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(3);
            languageProviders.Add(new GalleryShowCaseHeaderEnUSLanguageProvider());
            languageProviders.Add(new GalleryShowCaseHeaderZhCNLanguageProvider());
            languageProviders.Add(new GalleryShowCaseHeaderZhTWLanguageProvider());
            return languageProviders;
        }
    }
