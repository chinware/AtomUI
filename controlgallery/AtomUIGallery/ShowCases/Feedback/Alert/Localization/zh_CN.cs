using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Alert;

[LanguageProvider(LanguageCode.zh_CN, AlertShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "用于短消息提示的最简单用法。";
    public const string MoreTypesTitle = "更多类型";
    public const string MoreTypesDescription = "Alert 有 success、info、warning、error 四种类型。";
    public const string ClosableTitle = "可关闭";
    public const string ClosableDescription = "显示关闭按钮。";
    public const string DescriptionTitle = "含描述信息";
    public const string DescriptionDescription = "为提示消息添加额外描述。";
    public const string CustomActionTitle = "自定义操作";
    public const string CustomActionDescription = "自定义操作。";
    public const string LoopBannerTitle = "循环横幅";
    public const string LoopBannerDescription = "展示循环横幅。";

    protected override Type GetResourceKindType() => typeof(AlertShowCaseLangResourceKind);
}
