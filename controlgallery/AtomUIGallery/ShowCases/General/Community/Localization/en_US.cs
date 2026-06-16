using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Community;

[LanguageProvider(LanguageCode.en_US, CommunityPage.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string AboutAtomUI = "About AtomUI";
    public const string VisionLabel = "Vision:";
    public const string VisionText = "To become a world-class provider of productivity software";
    public const string MissionLabel = "Mission:";
    public const string MissionText = "Make digital world builders more efficient";
    public const string HomePage = "Home Page";
    public const string WeChatOfficial = "WeChat Press";
    public const string WeChatOfficialDescription = "Follow release updates";
    public const string TelegramGroup = "Telegram";
    public const string TelegramGroupDescription = "Join the Telegram group";
    public const string WeChatGroup = "WeChat Group";
    public const string WeChatGroupDescription = "Join the WeChat group";
    public const string QQGroup = "QQ Group";
    public const string QQGroupDescription = "Join the QQ group";

    protected override Type GetResourceKindType() => typeof(CommunityPageLangResourceKind);
}
