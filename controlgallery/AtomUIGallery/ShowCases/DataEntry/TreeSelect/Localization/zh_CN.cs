using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TreeSelect;

[LanguageProvider(LanguageCode.zh_CN, TreeSelectShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的用法。";
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

    protected override Type GetResourceKindType() => typeof(TreeSelectShowCaseLangResourceKind);
}
