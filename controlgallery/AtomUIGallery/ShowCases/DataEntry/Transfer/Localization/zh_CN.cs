using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.zh_CN, TransferShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Transfer 的基础用法需要提供源数据、目标 keys 数组，以及渲染和部分回调函数。";
    public const string OneWayTitle = "单向模式";
    public const string OneWayDescription = "使用 oneWay 让 Transfer 呈现单向样式。";
    public const string SearchTitle = "搜索";
    public const string SearchDescription = "带搜索框的 Transfer。";
    public const string AdvancedTitle = "高级用法";
    public const string AdvancedDescription = "Transfer 的高级用法。可以自定义穿梭按钮标签、列宽和列高，以及页脚中展示的内容。";
    public const string PaginationTitle = "分页";
    public const string PaginationDescription = "通过分页承载大量条目。";
    public const string TreeTransferTitle = "树形穿梭框";
    public const string TreeTransferDescription = "使用 Tree 组件自定义渲染列表。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Transfer 添加状态，可设置为错误或警告。";

    protected override Type GetResourceKindType() => typeof(TransferShowCaseLangResourceKind);
}
