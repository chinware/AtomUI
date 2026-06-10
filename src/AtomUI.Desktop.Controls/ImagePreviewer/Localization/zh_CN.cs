using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.ImagePreviewerLang;

[LanguageProvider(LanguageCode.zh_CN, ImagePreviewerToken.ID)]
internal class zh_CN : LanguageProvider
{
    public zh_CN()
        : base(LanguageCode.zh_CN, ImagePreviewerToken.ID)
    {
    }

    public const string Preview = "预览";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerLangResourceKind);
}