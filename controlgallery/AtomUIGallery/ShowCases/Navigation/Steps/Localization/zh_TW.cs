using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.zh_TW, StepsShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioInteractive = "交互";
    public const string ScenarioVertical = "垂直";
    public const string ScenarioDotClickable = "點狀與可點擊";
    public const string ScenarioNavigation = "導航";
    public const string ScenarioProgress = "進度";
    public const string ScenarioInline = "內聯";
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

