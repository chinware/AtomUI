using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Expander;

[LanguageProvider(LanguageCode.zh_CN, ExpanderShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioAppearance = "外观";
    public const string ScenarioBehavior = "行为";
    public const string ExpanderTitle = "展开器";
    public const string ExpanderDescription = "默认情况下，最简单的用法是向下展开。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "Ant Design 支持默认、大号和小号三种 Expander 尺寸。需要大号或小号时分别设置 size 属性；省略 size 属性时使用默认尺寸。";
    public const string ExpandingDirectionTitle = "展开方向";
    public const string ExpandingDirectionDescription = "内容区域支持四个方向展开。";
    public const string NestedPanelTitle = "嵌套面板";
    public const string NestedPanelDescription = "在 Collapse 中嵌套 Expander。";
    public const string BorderlessTitle = "无边框";
    public const string BorderlessDescription = "无边框样式的 Expander。";
    public const string NoArrowTitle = "隐藏箭头";
    public const string NoArrowDescription = "可以向 CollapsePanel 传入 IsShowExpandIcon={False} 来隐藏箭头图标。";
    public const string ExpandIconLocationTitle = "展开图标位置";
    public const string ExpandIconLocationDescription = "展开图标可以放在前方或后方。";
    public const string GhostCollapseTitle = "幽灵展开器";
    public const string GhostCollapseDescription = "让背景透明。";
    public const string CollapsibleTitle = "可折叠触发区域";
    public const string CollapsibleDescription = "通过 collapsible 指定折叠触发区域。";
    public const string CustomPaddingTitle = "自定义标题和内容间距";
    public const string CustomPaddingDescription = "请设置自定义标题边距和内容间距。";
    public const string P2HeaderThisIsPanelHeaderN1 = "这是面板标题 1";
    public const string P2TitleDefaultSize = "默认尺寸";
    public const string P2HeaderThisIsDefaultSizePanelHeader = "这是默认尺寸面板标题";
    public const string P2TitleSmallSize = "小尺寸";
    public const string P2HeaderThisIsSmallSizePanelHeader = "这是小尺寸面板标题";
    public const string P2TitleLargeSize = "大尺寸";
    public const string P2HeaderThisIsLargeSizePanelHeader = "这是大尺寸面板标题";
    public const string P2HeaderThisIsPanelHeader = "这是面板标题";
    public const string P2HeaderThisIsNestedPanelHeader = "这是嵌套面板标题";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking = "只能点击文字折叠此面板";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking2 = "只能点击图标折叠此面板";
    public const string P2HeaderThisPanelCanTBeCollapsed = "此面板不能折叠";
    public const string P2TextADogIsATypeOfDomesticatedAnimal = "狗是一种被驯养的动物。它以忠诚和可靠著称，在世界各地许多家庭中都是受欢迎的成员。";
    public const string P2TextExpandDirection = "展开方向：";
    public const string P2ContentDown = "向下";
    public const string P2ContentUp = "向上";
    public const string P2ContentLeft = "向左";
    public const string P2ContentRight = "向右";
    public const string P2TextExpandIconPosition = "展开图标位置：";
    public const string P2ContentStart = "开始";
    public const string P2ContentEnd = "结束";

    protected override Type GetResourceKindType() => typeof(ExpanderShowCaseLangResourceKind);
}
