using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Notification;

[LanguageProvider(LanguageCode.zh_CN, NotificationShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Notification 的最简单用法。";
    public const string DurationTitle = "自动关闭时长";
    public const string DurationDescription = "Duration 可指定通知保持打开的时间。时间结束后通知会自动关闭。未指定时默认值为 4.5 秒；设置为 TimeSpan.Zero 时通知不会自动关闭。";
    public const string WithIconTitle = "带图标通知";
    public const string WithIconDescription = "左侧带图标的通知框。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "通知框可通过 placement 从视口的 top、bottom、topLeft、topRight、bottomLeft 或 bottomRight 出现。";
    public const string CustomizedIconTitle = "自定义图标";
    public const string CustomizedIconDescription = "图标可以自定义为任意图标节点。";
    public const string ProgressTitle = "显示进度";
    public const string ProgressDescription = "为自动关闭的通知显示进度条。";

    protected override Type GetResourceKindType() => typeof(NotificationShowCaseLangResourceKind);
}
