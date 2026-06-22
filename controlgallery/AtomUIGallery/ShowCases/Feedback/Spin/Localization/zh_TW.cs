using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Spin;

[LanguageProvider(LanguageCode.zh_TW, SpinShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "簡單的加載狀態。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "SpinIndicator 支持小號、中號、大號和自定義尺寸類型；自定義類型默認沿用中號尺寸指標，也可以覆蓋指示器尺寸指標。";
    public const string CustomIndicatorTitle = "自定義加載指示器";
    public const string CustomIndicatorDescription = "使用自定義加載指示器。";
    public const string CustomizedDescriptionTitle = "自定義描述";
    public const string CustomizedDescriptionDescription = "自定義描述。";
    public const string EmbeddedModeTitle = "嵌入模式";
    public const string EmbeddedModeDescription = "將內容嵌入 Spin 後會進入加載狀態。";
    public const string PageSubtitle = "用於異步內容的加載指示器。";
    public const string PageDescription = "Spin 和 SpinIndicator 用於區域、卡片或頁面等待數據時展示加載狀態，並支持提示文本、遮罩和自定義指示器。";
    public const string ComponentCategory = "反饋";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string InfoNamespaceLabel = "命名空間";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基類";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertySizeType = "嵌入式 Spin 指示器的預設尺寸。";
    public const string ApiPropertyTip = "顯示在加載指示器附近的提示文本。";
    public const string ApiPropertyIsTipVisible = "控制提示文本是否可見。";
    public const string ApiPropertyCustomIndicator = "替代內置圓點的自定義指示器內容。";
    public const string ApiPropertyCustomIndicatorTemplate = "用於渲染自定義指示器內容的模板。";
    public const string ApiPropertyMotionDuration = "旋轉動畫的持續時間。";
    public const string ApiPropertyMotionEasingCurve = "旋轉動畫使用的緩動曲線。";
    public const string ApiPropertyIsSpinning = "控制加載遮罩是否處於激活狀態。";
    public const string ApiPropertyIsMaskBlurEnabled = "加載時對被遮罩內容應用模糊效果。";
    public const string ApiPropertyIsMaskBackgroundEnabled = "在嵌入內容上方顯示遮罩背景。";
    public const string ApiPropertyIsMotionEnabled = "控制是否啟用動效過渡。";
    public const string ApiPropertyContent = "加載時被 Spin 覆蓋的嵌入內容。";
    public const string ApiPropertyIndicatorSizeType = "獨立 SpinIndicator 的預設尺寸。";
    public const string ApiPropertyIndicatorSize = "獨立指示器的整體尺寸，通常與自定義尺寸類型配合使用。";
    public const string ApiPropertyIndicatorDotSize = "內置圓點尺寸，通常與自定義尺寸類型配合使用。";
    public const string ApiPropertyIndicatorCustomIndicator = "獨立指示器的自定義內容。";
    public const string ApiPropertyIndicatorCustomIndicatorTemplate = "用於渲染獨立自定義指示器內容的模板。";
    public const string ApiPropertyIndicatorMotionDuration = "獨立指示器旋轉動畫的持續時間。";
    public const string ApiPropertyIndicatorMotionEasingCurve = "獨立指示器旋轉動畫使用的緩動曲線。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameDotSize = "中號指示器的圓點尺寸。";
    public const string TokenNameDotSizeSM = "小號指示器的圓點尺寸。";
    public const string TokenNameDotSizeLG = "大號指示器的圓點尺寸。";
    public const string TokenNameIndicatorSize = "中號指示器的整體尺寸。";
    public const string TokenNameIndicatorSizeSM = "小號指示器的整體尺寸。";
    public const string TokenNameIndicatorSizeLG = "大號指示器的整體尺寸。";
    public const string TokenNameIndicatorDuration = "指示器動畫週期時長。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string P2DescriptionFurtherDetailsAboutTheContextOfThisAlert = "關於這條提示上下文的更多詳細信息。";
    public const string P2TextLoadingState = "加載狀態：";
    public const string P2TipLoading = "加載中...";

    public const string P2MessageAlertMessageTitle = "提示消息標題";

    protected override Type GetResourceKindType() => typeof(SpinShowCaseLangResourceKind);
}
