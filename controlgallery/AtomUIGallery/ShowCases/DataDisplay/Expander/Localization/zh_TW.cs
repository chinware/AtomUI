using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Expander;

[LanguageProvider(LanguageCode.zh_TW, ExpanderShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAppearance = "外觀";
    public const string ScenarioBehavior = "行為";
    public const string ExpanderTitle = "展開器";
    public const string ExpanderDescription = "默認情況下，最簡單的用法是向下展開。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "Ant Design 支持默認、大號和小號三種 Expander 尺寸。需要大號或小號時分別設置 size 屬性；省略 size 屬性時使用默認尺寸。";
    public const string ExpandingDirectionTitle = "展開方向";
    public const string ExpandingDirectionDescription = "內容區域支持四個方向展開。";
    public const string NestedPanelTitle = "嵌套面板";
    public const string NestedPanelDescription = "在 Collapse 中嵌套 Expander。";
    public const string BorderlessTitle = "無邊框";
    public const string BorderlessDescription = "無邊框樣式的 Expander。";
    public const string NoArrowTitle = "隱藏箭頭";
    public const string NoArrowDescription = "可以向 CollapsePanel 傳入 IsShowExpandIcon={False} 來隱藏箭頭圖標。";
    public const string ExpandIconLocationTitle = "展開圖標位置";
    public const string ExpandIconLocationDescription = "展開圖標可以放在前方或後方。";
    public const string GhostCollapseTitle = "幽靈展開器";
    public const string GhostCollapseDescription = "讓背景透明。";
    public const string CollapsibleTitle = "可折疊觸發區域";
    public const string CollapsibleDescription = "通過 collapsible 指定折疊觸發區域。";
    public const string CustomPaddingTitle = "自定義標題和內容間距";
    public const string CustomPaddingDescription = "請設置自定義標題邊距和內容間距。";
    public const string P2HeaderThisIsPanelHeaderN1 = "這是面板標題 1";
    public const string P2TitleDefaultSize = "默認尺寸";
    public const string P2HeaderThisIsDefaultSizePanelHeader = "這是默認尺寸面板標題";
    public const string P2TitleSmallSize = "小尺寸";
    public const string P2HeaderThisIsSmallSizePanelHeader = "這是小尺寸面板標題";
    public const string P2TitleLargeSize = "大尺寸";
    public const string P2HeaderThisIsLargeSizePanelHeader = "這是大尺寸面板標題";
    public const string P2HeaderThisIsPanelHeader = "這是面板標題";
    public const string P2HeaderThisIsNestedPanelHeader = "這是嵌套面板標題";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking = "只能點擊文字折疊此面板";
    public const string P2HeaderThisPanelCanOnlyBeCollapsedByClicking2 = "只能點擊圖標折疊此面板";
    public const string P2HeaderThisPanelCanTBeCollapsed = "此面板不能折疊";
    public const string P2TextADogIsATypeOfDomesticatedAnimal = "狗是一種被馴養的動物。它以忠誠和可靠著稱，在世界各地許多家庭中都是受歡迎的成員。";
    public const string P2TextExpandDirection = "展開方向：";
    public const string P2ContentDown = "向下";
    public const string P2ContentUp = "向上";
    public const string P2ContentLeft = "向左";
    public const string P2ContentRight = "向右";
    public const string P2TextExpandIconPosition = "展開圖標位置：";
    public const string P2ContentStart = "開始";
    public const string P2ContentEnd = "結束";

    protected override Type GetResourceKindType() => typeof(ExpanderShowCaseLangResourceKind);
}

