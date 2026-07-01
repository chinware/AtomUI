using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.zh_TW, StepsShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioInteractive = "交互";
    public const string ScenarioVertical = "垂直";
    public const string ScenarioDotClickable = "點狀與可點擊";
    public const string ScenarioNavigation = "導航";
    public const string ScenarioProgress = "進度";
    public const string ScenarioInline = "內聯";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "引導使用者理解有順序的任務和流程狀態。";
    public const string PageDescription = "Steps 用於展示任務序列、進度、導航狀態以及可選的步驟內容，適合需要清晰階段感的流程。";
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string ApiPropertyCurrentStep = "設定或繫結目前啟用步驟索引。";
    public const string ApiPropertyInitialStep = "設定控制項首次配置時的初始啟用步驟。";
    public const string ApiPropertyProgressValue = "顯示步驟項進度時目前步驟使用的百分比。";
    public const string ApiPropertyCurrentStepStatus = "套用到目前步驟的狀態。";
    public const string ApiPropertyOrientation = "控制水平或垂直步驟布局。";
    public const string ApiPropertyLabelPlacement = "控制標籤水平或垂直排列。";
    public const string ApiPropertySizeType = "控制步驟條視覺尺寸。";
    public const string ApiPropertyItemIndicatorType = "控制指示器樣式，例如預設或點狀。";
    public const string ApiPropertyStyle = "控制視覺風格，包括導航和內聯模式。";
    public const string ApiPropertyIsItemClickable = "允許使用者直接選擇步驟項。";
    public const string ApiPropertyIsShowItemProgress = "在目前步驟指示器內顯示進度。";
    public const string ApiPropertyContentTemplate = "目前步驟內容使用的樣板。";
    public const string ApiPropertyCurrentContent = "Steps 暴露的目前選中步驟內容。";
    public const string ApiPropertyStepsItemSubHeader = "顯示在步驟標題附近的輔助標題內容。";
    public const string ApiPropertyStepsItemDescription = "顯示在步驟標題下方的描述內容。";
    public const string ApiPropertyStepsItemIcon = "顯示在步驟指示器中的自定義圖標。";
    public const string ApiPropertyStepsItemStatus = "單個步驟項的顯式狀態。";
    public const string TokenNameDescriptionMaxWidth = "步驟描述區域的最大寬度。";
    public const string TokenNameIconSize = "預設步驟指示器容器尺寸。";
    public const string TokenNameIconFontSize = "預設步驟指示器圖標字號。";
    public const string TokenNameIconSizeSM = "小號步驟指示器尺寸。";
    public const string TokenNameDotSize = "點狀指示器尺寸。";
    public const string TokenNameDotCurrentSize = "目前點狀指示器尺寸。";
    public const string TokenNameDotLineThickness = "點狀連接線粗細。";
    public const string TokenNameHorizontalHeaderMargin = "水平步驟的標題外間距。";
    public const string TokenNameVerticalItemSpacing = "垂直步驟項之間的間距。";
    public const string TokenNameVerticalDescriptionPadding = "垂直步驟描述使用的內間距。";
    public const string TokenNameStepsNavActiveColor = "導航步驟使用的啟用色。";
    public const string TokenNameStepsProgressSize = "步驟項進度使用的尺寸。";
    public const string TokenNameInlineDotSize = "內聯步驟使用的點狀尺寸。";
    public const string TokenNameInlineItemPadding = "內聯步驟項使用的內間距。";
    public const string TokenNameProcessIconBgColor = "進行中步驟指示器背景色。";
    public const string TokenNameFinishTailColor = "已完成步驟連接線顏色。";
    public const string TokenNameErrorIconBgColor = "錯誤步驟指示器背景色。";
    public const string TokenScopeComponent = "元件";
    public const string TokenStatusStable = "穩定";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的步驟條。";
    public const string MiniVersionTitle = "迷你版本";
    public const string MiniVersionDescription = "將 SizeType 設置為 Small 可獲得迷你版本。";
    public const string WithIconTitle = "帶圖標";
    public const string WithIconDescription = "可以通過為項目設置 icon 屬性使用自定義圖標。";
    public const string SwitchStepTitle = "切換步驟";
    public const string SwitchStepDescription = "配合內容和按鈕展示流程進度。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直方向的簡單步驟條。";
    public const string VerticalMiniVersionTitle = "垂直迷你版本";
    public const string VerticalMiniVersionDescription = "垂直方向的簡單迷你步驟條。";
    public const string ErrorStatusTitle = "錯誤狀態";
    public const string ErrorStatusDescription = "通過 Steps 的 status 可以指定當前步驟狀態。";
    public const string DotStyleTitle = "點狀樣式";
    public const string DotStyleDescription = "帶進度點樣式的步驟條。";
    public const string DotStyleVerticalTitle = "垂直點狀樣式";
    public const string DotStyleVerticalDescription = "垂直方向帶進度點樣式的步驟條。";
    public const string ClickableTitle = "可點擊";
    public const string ClickableDescription = "設置 IsItemClickable=true 可讓步驟項可點擊。";
    public const string NavigationStepsTitle = "導航步驟";
    public const string NavigationStepsDescription = "導航式步驟。";
    public const string StepsWithProgressTitle = "帶進度的步驟";
    public const string StepsWithProgressDescription = "帶進度的步驟條。";
    public const string LabelPlacementTitle = "標籤位置";
    public const string LabelPlacementDescription = "將 labelPlacement 設置為 vertical。";
    public const string InlineStepsTitle = "內聯步驟";
    public const string InlineStepsDescription = "內聯類型步驟，適合在列表內容場景中展示對象的流程和當前狀態。";
    public const string P2DescriptionThisIsADescription = "這是一段描述。";
    public const string P2HeaderFinished = "已完成";
    public const string P2HeaderInProgress = "進行中";
    public const string P2HeaderWaiting = "等待中";
    public const string P2HeaderLogin = "登錄";
    public const string P2HeaderVerification = "驗證";
    public const string P2HeaderPay = "支付";
    public const string P2HeaderDone = "完成";
    public const string P2HeaderFirst = "第一項";
    public const string P2HeaderSecond = "第二項";
    public const string P2HeaderThird = "第三項";
    public const string P2HeaderStepN1 = "步驟 1";
    public const string P2HeaderStepN2 = "步驟 2";
    public const string P2HeaderStepN3 = "步驟 3";
    public const string P2HeaderStepN4 = "步驟 4";
    public const string P2HeaderFinishN1 = "完成 1";
    public const string P2HeaderFinishN2 = "完成 2";
    public const string P2HeaderCurrentProcess = "當前進行中";
    public const string P2HeaderWait = "等待";
    public const string P2SubHeaderLeftTime = "剩餘 00:00:08";
    public const string P2SubHeaderWaitingForLongTime = "等待較長時間";
    public const string P2TextAntDesignTitleN1 = "Ant Design 標題 1";
    public const string P2TextAntDesignADesignLanguageForBackgroundApplications = "Ant Design 是由 Ant UED 團隊提煉的後台應用設計語言";
    public const string P2TextAntDesignTitleN2 = "Ant Design 標題 2";
    public const string P2TextAntDesignTitleN3 = "Ant Design 標題 3";
    public const string P2TextAntDesignTitleN4 = "Ant Design 標題 4";

    public const string P2ContentFirstContent = "第一步內容";

    public const string P2ContentSecondContent = "第二步內容";

    public const string P2ContentLastContent = "最後一步內容";

    public const string P2ContentNext = "下一步";

    public const string P2ContentPrevious = "上一步";

    public const string P2ContentDone = "完成";

    protected override Type GetResourceKindType() => typeof(StepsShowCaseLangResourceKind);
}
