using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.zh_TW, IconShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string P2HeaderOutlined = "線框風格";
    public const string P2HeaderFilled = "填充風格";
    public const string P2HeaderTwoTone = "雙色風格";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}

