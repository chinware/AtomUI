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

    protected override Type GetResourceKindType() => typeof(PopupConfirmShowCaseLangResourceKind);
}
