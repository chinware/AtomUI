using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AboutUs;

[LanguageProvider(LanguageCode.en_US, AboutUsPage.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string VisionLabel = "Vision:";
    public const string VisionText = "Become a world-class productivity tool software provider";
    public const string MissionLabel = "Mission:";
    public const string MissionText = "Make digital world builders more efficient";
    public const string HomePage = "Home Page";
    public const string WeChatOfficial = "WeChat Official Account";
    public const string WeChatGroup = "WeChat Community";
    public const string QQGroup = "QQ Community";

    protected override Type GetResourceKindType() => typeof(AboutUsPageLangResourceKind);
}
