using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AboutUs;

[LanguageProvider(LanguageCode.zh_CN, AboutUsPage.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string VisionLabel = "愿景：";
    public const string VisionText = "成为世界一流的生产力工具软件提供商";
    public const string MissionLabel = "使命：";
    public const string MissionText = "让数字世界建设者更高效";
    public const string HomePage = "官网";
    public const string WeChatOfficial = "微信公众号";
    public const string WeChatGroup = "微信交流群";
    public const string QQGroup = "QQ 交流群";

    protected override Type GetResourceKindType() => typeof(AboutUsPageLangResourceKind);
}
