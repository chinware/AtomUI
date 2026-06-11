using System.Collections.Generic;
using AtomUI.Theme.Language;
using Avalonia.Controls;

namespace AtomUI.Theme.Language
{
    internal sealed class DataGridEnUSLanguageProvider : LanguageProvider
    {
        public DataGridEnUSLanguageProvider()
            : base(LanguageCode.en_US, "DataGrid")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.AscendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.AscendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.CancelConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.CancelTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DeleteConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.DeleteConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DescendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.DescendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.Operating] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.Operating;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.SelectAllFilterItems] = global::AtomUI.Desktop.Controls.DataGridLocalization.en_US.SelectAllFilterItems;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DataGridZhCNLanguageProvider : LanguageProvider
    {
        public DataGridZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "DataGrid")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.AscendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.AscendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.CancelConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.CancelTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DeleteConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.DeleteConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DescendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.DescendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.Operating] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.Operating;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.SelectAllFilterItems] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_CN.SelectAllFilterItems;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DataGridZhTWLanguageProvider : LanguageProvider
    {
        public DataGridZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "DataGrid")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.AscendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.AscendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.CancelConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.CancelTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.CancelTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DeleteConfirmText] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.DeleteConfirmText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.DescendTooltip] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.DescendTooltip;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.Operating] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.Operating;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DataGridLangResourceKind.SelectAllFilterItems] = global::AtomUI.Desktop.Controls.DataGridLocalization.zh_TW.SelectAllFilterItems;
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
            languageProviders.Add(new DataGridEnUSLanguageProvider());
            languageProviders.Add(new DataGridZhCNLanguageProvider());
            languageProviders.Add(new DataGridZhTWLanguageProvider());
            return languageProviders;
        }
    }
}
