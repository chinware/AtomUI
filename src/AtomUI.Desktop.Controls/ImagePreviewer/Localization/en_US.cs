using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.ImagePreviewerLang;

[LanguageProvider(LanguageCode.en_US, ImagePreviewerToken.ID)]
internal class en_US : LanguageProvider
{
    public en_US()
        : base(LanguageCode.en_US, ImagePreviewerToken.ID)
    {
    }

    public const string Preview = "Preview";
    
    protected override Type GetResourceKindType() => typeof(ImagePreviewerLangResourceKind);
}