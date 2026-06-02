using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Collapse;

[LanguageProvider(LanguageCode.zh_CN, CollapseShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string CollapseTitle = "折叠面板";
    public const string CollapseDescription = "默认情况下，可以同时展开任意数量的面板。本示例中第一个面板处于展开状态。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "Ant Design 支持默认、大号和小号三种折叠面板尺寸。需要大号或小号时分别设置 size 属性；省略 size 属性时使用默认尺寸。";
    public const string AccordionTitle = "手风琴";
    public const string AccordionDescription = "在手风琴模式下，同一时间只能展开一个面板。";
    public const string NestedPanelTitle = "嵌套面板";
    public const string NestedPanelDescription = "在 Collapse 内嵌套 Collapse。";
    public const string BorderlessTitle = "无边框";
    public const string BorderlessDescription = "无边框样式的 Collapse。";
    public const string NoArrowTitle = "隐藏箭头";
    public const string NoArrowDescription = "可以向 CollapsePanel 传入 IsShowExpandIcon={False} 来隐藏箭头图标。";
    public const string ExpandIconLocationTitle = "展开图标位置";
    public const string ExpandIconLocationDescription = "展开图标可以放在前方或后方。";
    public const string GhostCollapseTitle = "幽灵折叠面板";
    public const string GhostCollapseDescription = "让 Collapse 的背景透明。";
    public const string CollapsibleTitle = "可折叠触发区域";
    public const string CollapsibleDescription = "通过 collapsible 指定折叠触发区域。";
    public const string CustomPaddingTitle = "自定义标题和内容间距";
    public const string CustomPaddingDescription = "请设置自定义标题边距和内容间距。";

    protected override Type GetResourceKindType() => typeof(CollapseShowCaseLangResourceKind);
}
