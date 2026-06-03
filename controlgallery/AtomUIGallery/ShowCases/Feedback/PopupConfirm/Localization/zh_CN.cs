using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.PopupConfirm;

[LanguageProvider(LanguageCode.zh_CN, PopupConfirmShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础示例支持确认框的标题和描述属性。";
    public const string LocaleTextTitle = "本地化文本";
    public const string LocaleTextDescription = "设置 okText 和 cancelText 属性来自定义按钮标签。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "提供 12 种弹出位置。";
    public const string CustomizeIconTitle = "自定义图标";
    public const string CustomizeIconDescription = "设置 icon 属性来自定义图标。";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "确定要删除这个任务吗？";
    public const string P2OkTextOk = "确定";
    public const string P2CancelTextCancel = "取消";
    public const string P2TitleDeleteTheTask = "删除任务";
    public const string P2ContentDelete = "删除";
    public const string P2ContentLt = "LT";
    public const string P2ContentLeft = "左侧";
    public const string P2ContentLb = "LB";
    public const string P2ContentTl = "TL";
    public const string P2ContentTop = "顶部";
    public const string P2ContentTr = "TR";
    public const string P2ContentRt = "RT";
    public const string P2ContentRight = "右侧";
    public const string P2ContentRb = "RB";
    public const string P2ContentBl = "BL";
    public const string P2ContentBottom = "底部";
    public const string P2ContentBr = "BR";

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}
