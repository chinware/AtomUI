using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.zh_CN, TransferShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Transfer 的基础用法需要提供源数据、目标 keys 数组，以及渲染和部分回调函数。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioAdvanced = "高级";
    public const string ScenarioTreeStatus = "树与状态";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "在两个集合之间移动条目，支持搜索、分页和树形数据。";
    public const string PageDescription = "Transfer 在左侧展示候选条目，在右侧展示已选条目。它支持单向移动、筛选、分页、自定义条目模板和基于树的数据源。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyItemsSource = "穿梭列表展示的数据集合。";
    public const string ApiPropertyTargetKeys = "当前位于目标列表中的条目 key。";
    public const string ApiPropertySourceTitle = "源列表上方显示的标题。";
    public const string ApiPropertyTargetTitle = "目标列表上方显示的标题。";
    public const string ApiPropertyIsOneWay = "仅允许条目从源列表移动到目标列表。";
    public const string ApiPropertyIsFilterEnabled = "显示用于筛选穿梭条目的搜索框。";
    public const string ApiPropertyFilterPlaceholderText = "筛选输入框中显示的占位文本。";
    public const string ApiPropertyFilterValueSelector = "选择默认筛选逻辑使用的文本。";
    public const string ApiPropertyListWidth = "每个穿梭列表的宽度。";
    public const string ApiPropertyListHeight = "每个穿梭列表的高度。";
    public const string ApiPropertyPageSize = "每页展示的穿梭条目数量。";
    public const string ApiPropertyStatus = "穿梭框表面的校验状态样式。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameListWidth = "穿梭列表默认宽度。";
    public const string TokenNameListWidthLG = "大尺寸穿梭列表宽度。";
    public const string TokenNameListHeight = "穿梭列表默认高度。";
    public const string TokenNameItemHeight = "每个穿梭条目行的高度。";
    public const string TokenNameItemPadding = "每个穿梭条目行的垂直内边距。";
    public const string TokenNameHeaderHeight = "穿梭列表头部高度。";
    public const string TokenNameHeaderPadding = "穿梭列表头部内边距。";
    public const string TokenNamePaginationMargin = "穿梭分页区域外边距。";
    public const string TokenNameDataGridSelectionHeaderMargin = "DataGrid 穿梭选择头部使用的外边距。";
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
    public const string P2SourceTitle = "源列表";
    public const string P2TargetTitle = "目标列表";
    public const string P2TextText = "-";
    public const string P2HeaderName = "姓名";
    public const string P2HeaderTag = "标签";
    public const string P2HeaderDescription = "描述";
    public const string P2ContentLeftButtonReload = "重新加载左侧";
    public const string P2ContentRightButtonReload = "重新加载右侧";

    public const string P2OnContentDisable = "禁用";

    public const string P2OffContentEnable = "启用";

    public const string P2FilterPlaceholderTextSearchHere = "在此搜索";

    public const string P2ToSourceButtonTextToLeft = "移到左侧";

    public const string P2ToTargetButtonTextToRight = "移到右侧";

    public const string P2OnContentOnyWay = "单向";

    public const string P2OffContentOnyWay = "单向";
    public const string P2ItemContentFormat = "内容{0}";
    public const string P2ItemDescriptionFormat = "内容{0}的描述";
    public const string P2TagCat = "猫";
    public const string P2TagDog = "狗";
    public const string P2TagBird = "鸟";

    protected override Type GetResourceKindType() => typeof(TransferShowCaseLangResourceKind);
}
