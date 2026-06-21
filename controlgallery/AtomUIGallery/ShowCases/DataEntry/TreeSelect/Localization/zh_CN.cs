using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeSelect;

[LanguageProvider(LanguageCode.zh_CN, TreeSelectShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioBehavior = "行为";
    public const string ScenarioAppearance = "外观";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "在紧凑输入框中从层级数据里选择值。";
    public const string PageDescription = "TreeSelect 将输入选择器与树形导航结合，适用于层级分类、组织节点、权限和其他嵌套选项集合。";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyItemsSource = "选择器展示的层级选项。";
    public const string ApiPropertySelectedItems = "当前选中的树节点。";
    public const string ApiPropertyIsMultiple = "允许选择多个树节点。";
    public const string ApiPropertyIsTreeCheckable = "在树形弹出层中显示复选框。";
    public const string ApiPropertyIsDefaultExpandAll = "弹出层打开时默认展开树节点。";
    public const string ApiPropertyIsAllowClear = "选中值后显示清除入口。";
    public const string ApiPropertyIsFilterEnabled = "允许从输入框筛选树节点。";
    public const string ApiPropertyDataLoader = "树节点展开时异步加载子节点。";
    public const string ApiPropertyPlacement = "控制弹出层相对于输入框的位置。";
    public const string ApiPropertyMaxCount = "选中值的最大数量，超过限制后更多选项会被禁用。";
    public const string ApiPropertyStyleVariant = "输入框视觉变体。";
    public const string ApiPropertyStatus = "选择器的校验状态样式。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameMinPopupWidth = "TreeSelect 使用的最小弹出层宽度。";
    public const string MultipleSelectionTitle = "多选";
    public const string MultipleSelectionDescription = "多选用法。";
    public const string GenerateFromTreeDataTitle = "由树数据生成";
    public const string GenerateFromTreeDataDescription = "可以使用 treeData 属性填充树结构，这是一种快速简单地提供树内容的方式。";
    public const string CheckableTitle = "可勾选";
    public const string CheckableDescription = "多选且可勾选。";
    public const string AsynchronousLoadingTitle = "异步加载";
    public const string AsynchronousLoadingDescription = "异步加载树节点。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "可以通过 placement 手动指定弹出层位置。";
    public const string ShowTreeLineTitle = "显示树线";
    public const string ShowTreeLineDescription = "使用 treeLine 显示线条样式。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "TreeSelect 提供四种变体：描边、填充、无边框和下划线。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 TreeSelect 添加状态，可设置为错误或警告。";
    public const string PrefixAndSuffixTitle = "前缀和后缀";
    public const string PrefixAndSuffixDescription = "自定义 prefix 和 suffixIcon。";
    public const string MaxCountTitle = "最大数量";
    public const string MaxCountDescription = "可以设置 maxCount 属性控制最多可选项数量。超过限制后，选项会变为禁用状态。";
    public const string SizeTypeTitle = "尺寸";
    public const string SizeTypeDescription = "TreeSelect 支持大号、中号、小号，也支持通过 Custom 配合本地高度和字号自定义。";
    public const string P2PlaceholderTextPleaseSelect = "请选择";
    public const string P2TextPlacement = "弹出位置：";
    public const string P2ContentTopleft = "左上";
    public const string P2ContentTopright = "右上";
    public const string P2ContentBottomleft = "左下";
    public const string P2ContentBottomright = "右下";
    public const string P2ContentLarge = "大号";
    public const string P2ContentDefault = "默认";
    public const string P2ContentSmall = "小号";
    public const string P2ContentCustom = "自定义";

    public const string P2OnContentShowIcon = "显示图标";

    public const string P2OffContentShowIcon = "显示图标";

    public const string P2OnContentTreeLine = "树线";

    public const string P2OffContentTreeLine = "树线";

    public const string P2OnContentShowLeafIcon = "显示叶子图标";

    public const string P2OffContentShowLeafIcon = "显示叶子图标";
    public const string P2AddOnPrefix = "前缀";
    public const string P2HeaderParent1 = "父节点 1";
    public const string P2HeaderParent10 = "父节点 1-0";
    public const string P2HeaderParent11 = "父节点 1-1";
    public const string P2HeaderLeaf1 = "叶子 1";
    public const string P2HeaderLeaf2 = "叶子 2";
    public const string P2HeaderLeaf3 = "叶子 3";
    public const string P2HeaderLeaf4 = "叶子 4";
    public const string P2HeaderLeaf5 = "叶子 5";
    public const string P2HeaderLeaf6 = "叶子 6";
    public const string P2HeaderLeaf11 = "叶子 11";
    public const string P2HeaderMyLeaf = "我的叶子";
    public const string P2HeaderYourLeaf = "你的叶子";
    public const string P2HeaderSss = "节点 SSS";
    public const string P2HeaderNode1 = "节点 1";
    public const string P2HeaderNode2 = "节点 2";
    public const string P2HeaderChildNode = "子节点";
    public const string P2HeaderChildNode1 = "子节点 1";
    public const string P2HeaderChildNode2 = "子节点 2";
    public const string P2HeaderChildNode3 = "子节点 3";
    public const string P2HeaderChildNode4 = "子节点 4";
    public const string P2HeaderChildNode5 = "子节点 5";
    public const string P2HeaderChildNode6 = "子节点 6";
    public const string P2HeaderChildNode7 = "子节点 7";
    public const string P2HeaderExpandToLoad = "展开加载";
    public const string P2HeaderTreeNode = "树节点";

    protected override Type GetResourceKindType() => typeof(TreeSelectShowCaseLangResourceKind);
}
