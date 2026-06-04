using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.ImagePreviewerLang;

[LanguageProvider(LanguageCode.zh_TW, ImagePreviewerToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string Preview = "預覽";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerLangResourceKind);
}