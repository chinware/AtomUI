using System.Collections.Generic;
using AtomUI.Theme.Language;
using Avalonia.Controls;

namespace AtomUI.Theme.Language
{
    internal sealed class CommonEnUSLanguageProvider : LanguageProvider
    {
        public CommonEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Common")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Controls.Localization.CommonLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Cancel] = global::AtomUI.Controls.Localization.en_US.Cancel;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Delete] = global::AtomUI.Controls.Localization.en_US.Delete;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Edit] = global::AtomUI.Controls.Localization.en_US.Edit;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Loading] = global::AtomUI.Controls.Localization.en_US.Loading;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.NoData] = global::AtomUI.Controls.Localization.en_US.NoData;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Ok] = global::AtomUI.Controls.Localization.en_US.Ok;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Optional] = global::AtomUI.Controls.Localization.en_US.Optional;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Reset] = global::AtomUI.Controls.Localization.en_US.Reset;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Save] = global::AtomUI.Controls.Localization.en_US.Save;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Submit] = global::AtomUI.Controls.Localization.en_US.Submit;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class CommonZhCNLanguageProvider : LanguageProvider
    {
        public CommonZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Common")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Controls.Localization.CommonLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Cancel] = global::AtomUI.Controls.Localization.zh_CN.Cancel;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Delete] = global::AtomUI.Controls.Localization.zh_CN.Delete;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Edit] = global::AtomUI.Controls.Localization.zh_CN.Edit;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Loading] = global::AtomUI.Controls.Localization.zh_CN.Loading;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.NoData] = global::AtomUI.Controls.Localization.zh_CN.NoData;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Ok] = global::AtomUI.Controls.Localization.zh_CN.Ok;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Optional] = global::AtomUI.Controls.Localization.zh_CN.Optional;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Reset] = global::AtomUI.Controls.Localization.zh_CN.Reset;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Save] = global::AtomUI.Controls.Localization.zh_CN.Save;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Submit] = global::AtomUI.Controls.Localization.zh_CN.Submit;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class CommonZhTWLanguageProvider : LanguageProvider
    {
        public CommonZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Common")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Controls.Localization.CommonLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Cancel] = global::AtomUI.Controls.Localization.zh_TW.Cancel;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Delete] = global::AtomUI.Controls.Localization.zh_TW.Delete;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Edit] = global::AtomUI.Controls.Localization.zh_TW.Edit;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Loading] = global::AtomUI.Controls.Localization.zh_TW.Loading;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.NoData] = global::AtomUI.Controls.Localization.zh_TW.NoData;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Ok] = global::AtomUI.Controls.Localization.zh_TW.Ok;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Optional] = global::AtomUI.Controls.Localization.zh_TW.Optional;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Reset] = global::AtomUI.Controls.Localization.zh_TW.Reset;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Save] = global::AtomUI.Controls.Localization.zh_TW.Save;
                dictionary[global::AtomUI.Controls.Localization.CommonLangResourceKind.Submit] = global::AtomUI.Controls.Localization.zh_TW.Submit;
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
            languageProviders.Add(new CommonEnUSLanguageProvider());
            languageProviders.Add(new CommonZhCNLanguageProvider());
            languageProviders.Add(new CommonZhTWLanguageProvider());
            return languageProviders;
        }
    }
}
