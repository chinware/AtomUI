using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.ColorPickerLang;

[LanguageProvider(LanguageCode.en_US, ColorPickerToken.ID)]
internal class en_US : LanguageProvider
{
    public en_US()
        : base(LanguageCode.en_US, ColorPickerToken.ID)
    {
    }

    public const string EmptyColorText = "Transparent";

    protected override Type GetResourceKindType() => typeof(ColorPickerLangResourceKind);
}