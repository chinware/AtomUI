using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.FlexPanel;

[LanguageProvider(LanguageCode.zh_CN, FlexPanelShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicLayoutTitle = "基础布局";
    public const string BasicLayoutDescription = "基础用法。";
    public const string AlignmentTitle = "对齐";
    public const string AlignmentDescription = "设置 justify 和 align。";
    public const string GapTitle = "间距";
    public const string GapDescription = "设置项目之间的间距。";
    public const string AlignSelfTitle = "单项对齐";
    public const string AlignSelfDescription = "使用 Flex.AlignSelf 覆盖单个项目的对齐方式。";
    public const string AutoWrapTitle = "自动换行";
    public const string AutoWrapDescription = "项目会自动换行。";
    public const string OrderTitle = "排序";
    public const string OrderDescription = "使用 Flex.Order 改变项目顺序。";
    public const string BasisTitle = "基础尺寸";
    public const string BasisDescription = "使用 Flex.Basis 控制初始尺寸。";
    public const string FlexGrowTitle = "Flex 增长";
    public const string FlexGrowDescription = "使用 Flex.Grow 分配剩余空间。";
    public const string FlexShrinkTitle = "Flex 收缩（加权）";
    public const string FlexShrinkDescription = "收缩量按每个项目的基础尺寸加权。";
    public const string CombinationTitle = "组合布局";
    public const string CombinationDescription = "使用嵌套 FlexPanel 构建复杂布局。";
    public const string PlaygroundTitle = "Flex 演练场";
    public const string PlaygroundDescription = "调整所有 flex 属性。";

    protected override Type GetResourceKindType() => typeof(FlexPanelShowCaseLangResourceKind);
}
