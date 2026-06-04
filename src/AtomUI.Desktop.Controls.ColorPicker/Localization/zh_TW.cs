using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.ColorPickerLang;

[LanguageProvider(LanguageCode.zh_TW, ColorPickerToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string EmptyColorText = "無色";

    protected override Type GetResourceKindType() => typeof(ColorPickerLangResourceKind);
}