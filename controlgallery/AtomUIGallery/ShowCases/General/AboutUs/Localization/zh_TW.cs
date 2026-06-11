using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AboutUs;

[LanguageProvider(LanguageCode.zh_TW, AboutUsPage.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string VisionLabel = "願景：";
    public const string VisionText = "致力於成為世界一流的生產力工具軟件提供商";
    public const string MissionLabel = "使命：";
    public const string MissionText = "讓數字世界建設者更高效";
    public const string HomePage = "官網";
    public const string WeChatOfficial = "微信公眾號";
    public const string WeChatGroup = "微信交流群";
    public const string QQGroup = "QQ 交流群";

    protected override Type GetResourceKindType() => typeof(AboutUsPageLangResourceKind);
}
