using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.zh_CN, IconShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string P2HeaderOutlined = "线框风格";
    public const string P2HeaderFilled = "填充风格";
    public const string P2HeaderTwoTone = "双色风格";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}
