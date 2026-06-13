using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_CN, PaginationShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用页码、页大小、总数和快速跳转浏览长列表。";
    public const string PageDescription = "Pagination 将大型数据集拆分为可预测的页面，支持对齐方式、页大小选择、快速跳转、总数信息、迷你尺寸以及简洁只读或可编辑模式。";
    public const string InfoNamespaceLabel = "命名空间:";
    public const string InfoPackageLabel = "包:";
    public const string InfoBaseClassLabel = "基类:";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyTotal = "所有页面中的记录总数。";
    public const string ApiPropertyCurrentPage = "当前页码，从 1 开始。";
    public const string ApiPropertyPageSize = "每页表示的记录数量。";
    public const string ApiPropertyPageCount = "根据总数和页大小计算出的页数。";
    public const string ApiPropertyIsHideOnSinglePage = "当所有记录能放入单页时隐藏分页。";
    public const string ApiPropertyAlign = "将分页内容对齐到起始、中间或末尾。";
    public const string ApiPropertySizeType = "控制默认或小号分页密度。";
    public const string ApiPropertyIsMotionEnabled = "启用或禁用动效过渡。";
    public const string ApiPropertyCurrentPageChanged = "当前页变化时触发。";
    public const string ApiPropertyIsShowSizeChanger = "显示页大小选择器。";
    public const string ApiPropertyIsShowQuickJumper = "显示快速跳转输入框。";
    public const string ApiPropertyIsShowTotalInfo = "显示总记录信息。";
    public const string ApiPropertyTotalInfoTemplate = "用于格式化总数信息文本的模板。";
    public const string ApiPropertyIsReadOnly = "控制 SimplePagination 使用只读展示还是可编辑跳转输入。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameItemBg = "分页项背景色。";
    public const string TokenNameItemSize = "默认分页项尺寸。";
    public const string TokenNameItemActiveBg = "激活分页项背景色。";
    public const string TokenNameItemSizeSM = "小号分页项尺寸。";
    public const string TokenNameItemLinkBg = "分页项链接背景色。";
    public const string TokenNameItemActiveBgDisabled = "禁用激活分页项背景色。";
    public const string TokenNameItemActiveColorDisabled = "禁用激活分页项文本色。";
    public const string TokenNameItemInputBg = "分页输入控件背景色。";
    public const string TokenNameInputOutlineOffset = "分页输入控件轮廓偏移量。";
    public const string TokenNamePaginationLayoutSpacing = "普通分页布局水平间距。";
    public const string TokenNamePaginationLayoutMiniSpacing = "迷你分页布局水平间距。";
    public const string TokenNamePaginationQuickJumperInputWidth = "普通快速跳转输入框宽度。";
    public const string TokenNamePaginationMiniQuickJumperInputWidth = "迷你快速跳转输入框宽度。";
    public const string TokenNamePaginationItemPaddingInline = "分页项横向内间距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string BasicTitle = "基础分页";
    public const string BasicDescription = "基础分页。";
    public const string AlignTitle = "对齐方式";
    public const string AlignDescription = "支持左对齐、居中对齐和右对齐三种对齐方式。";
    public const string MoreTitle = "更多页码";
    public const string MoreDescription = "更多页码。";
    public const string MiniSizeTitle = "迷你尺寸";
    public const string MiniSizeDescription = "迷你尺寸分页。";
    public const string TotalNumberTitle = "总数";
    public const string TotalNumberDescription = "可以通过设置 showTotal 展示数据总量。";
    public const string SimpleModeTitle = "简洁模式";
    public const string SimpleModeDescription = "简洁模式。";

    protected override Type GetResourceKindType() => typeof(PaginationShowCaseLangResourceKind);
}
