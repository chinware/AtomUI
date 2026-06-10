using System.Collections.Generic;
using AtomUI.Theme.Language;
using Avalonia.Controls;

namespace AtomUI.Theme.Language
{
    internal sealed class ColorPickerEnUSLanguageProvider : LanguageProvider
    {
        public ColorPickerEnUSLanguageProvider()
            : base(LanguageCode.en_US, "ColorPicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind.EmptyColorText] = global::AtomUI.Desktop.Controls.ColorPickerLang.en_US.EmptyColorText;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class ColorPickerZhCNLanguageProvider : LanguageProvider
    {
        public ColorPickerZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "ColorPicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind.EmptyColorText] = global::AtomUI.Desktop.Controls.ColorPickerLang.zh_CN.EmptyColorText;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class ColorPickerZhTWLanguageProvider : LanguageProvider
    {
        public ColorPickerZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "ColorPicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ColorPickerLangResourceKind.EmptyColorText] = global::AtomUI.Desktop.Controls.ColorPickerLang.zh_TW.EmptyColorText;
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
            languageProviders.Add(new ColorPickerEnUSLanguageProvider());
            languageProviders.Add(new ColorPickerZhCNLanguageProvider());
            languageProviders.Add(new ColorPickerZhTWLanguageProvider());
            return languageProviders;
        }
    }
}
