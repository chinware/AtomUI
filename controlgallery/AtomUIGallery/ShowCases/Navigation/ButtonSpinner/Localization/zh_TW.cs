using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

[LanguageProvider(LanguageCode.zh_TW, ButtonSpinnerShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "帶按鈕操作柄的緊湊微調輸入控件。";
    public const string PageDescription = "ButtonSpinner 將輸入式內容區與遞增/遞減按鈕結合，支持前後置標籤、內部前後綴、狀態、尺寸和視覺形態。";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎的按鈕微調器。";
    public const string ThreeSizesTitle = "按鈕微調器尺寸";
    public const string ThreeSizesDescription = "按鈕微調器支持大號（40px）、默認（32px）、小號（24px）和使用本地尺寸值的 Custom。";
    public const string P2LabelSizeTypeLarge = "大號";
    public const string P2LabelSizeTypeMiddle = "中號";
    public const string P2LabelSizeTypeSmall = "小號";
    public const string P2LabelSizeTypeCustom = "Custom";
    public const string VariantsTitle = "不同形態";
    public const string VariantsDescription = "輸入框的不同形態。";
    public const string DisabledTitle = "禁用狀態";
    public const string DisabledDescription = "禁用狀態下的輸入框形態。";
    public const string PrePostTabTitle = "前置/後置標籤";
    public const string PrePostTabDescription = "前置和後置標籤的使用示例。";
    public const string PrefixSuffixTitle = "前綴和後綴";
    public const string PrefixSuffixDescription = "在輸入框內部添加前綴或後綴圖標。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為輸入框添加錯誤或警告狀態。";

    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyIsSpinEnabled = "啟用指針、鍵盤和滾輪觸發的微調操作。";
    public const string ApiPropertyIsButtonSpinnerVisible = "控制微調按鈕操作柄是否可見。";
    public const string ApiPropertyButtonSpinnerLocation = "設置微調按鈕操作柄位於左側或右側。";
    public const string ApiPropertyLeftAddOn = "顯示在輸入區域外左側的內容。";
    public const string ApiPropertyRightAddOn = "顯示在輸入區域外右側的內容。";
    public const string ApiPropertyInnerLeftContent = "顯示在輸入區域內數值前方的內容。";
    public const string ApiPropertyInnerRightContent = "顯示在輸入區域內數值後方的內容。";
    public const string ApiPropertySizeType = "控制按鈕微調器尺寸。";
    public const string ApiPropertyStyleVariant = "控制描邊、填充和無邊框視覺形態。";
    public const string ApiPropertyStatus = "應用校驗狀態樣式。";
    public const string ApiPropertyIsButtonSpinnerFloatable = "允許微調按鈕操作柄在交互前浮動在內容上方。";
    public const string ApiPropertyIsMotionEnabled = "在主題允許時啟用控件動效。";
    public const string ApiPropertySpinnerHandleWidth = "覆蓋微調按鈕操作柄寬度。";
    public const string ApiPropertySpin = "用戶請求遞增或遞減微調操作時觸發。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameControlWidth = "默認控件寬度。";
    public const string TokenNameHandleWidth = "微調按鈕操作柄寬度。";
    public const string TokenNameHandleIconSize = "微調按鈕操作柄圖標尺寸。";
    public const string TokenNameHandleBg = "微調按鈕操作柄背景色。";
    public const string TokenNameHandleActiveBg = "微調按鈕操作柄激活背景色。";
    public const string TokenNameHandleHoverColor = "微調按鈕操作柄懸浮前景色。";
    public const string TokenNameHandleBorderColor = "微調按鈕操作柄邊框色。";
    public const string TokenNameFilledHandleBg = "填充形態下的微調按鈕操作柄背景色。";
    public const string TokenNameInputFontSize = "繼承自 LineEdit 的默認輸入字號。";
    public const string TokenNameInputFontSizeLG = "繼承自 LineEdit 的大號輸入字號。";
    public const string TokenNameInputFontSizeSM = "繼承自 LineEdit 的小號輸入字號。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(ButtonSpinnerShowCaseLangResourceKind);
}
