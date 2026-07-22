using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.zh_CN, TransferShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Transfer 的基础用法需要提供源数据、目标 keys 数组，以及渲染和部分回调函数。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioAdvanced = "高级";
    public const string ScenarioTreeStatus = "树与状态";
    public const string ScenarioExamples = "示例";
    public const string PageSubtitle = "在两个集合之间移动条目，支持搜索、分页和树形数据。";
    public const string PageDescription = "Transfer 在左侧展示候选条目，在右侧展示已选条目。它支持单向移动、筛选、分页、自定义条目模板和基于树的数据源。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string OneWayTitle = "单向模式";
    public const string OneWayDescription = "使用 oneWay 让 Transfer 呈现单向样式。";
    public const string SearchTitle = "搜索";
    public const string SearchDescription = "带搜索框的 Transfer。";
    public const string ControlledKeysTitle = "受控 key";
    public const string ControlledKeysDescription = "将 TargetKeys 和 SelectedKeys 绑定到 ObservableCollection。外部集合变更和 Transfer 交互会保持同步。";
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
    public const string P2ContentAddTargetKey = "添加 key 3 到目标";
    public const string P2ContentClearTargetKeys = "清空目标 key";
    public const string P2ContentSelectSourceKey = "选中 key 4";
    public const string P2TargetKeysCountLabel = "目标 key：";
    public const string P2SelectedKeysCountLabel = "选中 key：";

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

}
