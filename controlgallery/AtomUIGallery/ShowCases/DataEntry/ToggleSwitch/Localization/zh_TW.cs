using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.zh_TW, ToggleSwitchShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變數";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "在兩個互斥狀態之間切換，並支持文本、圖標、加載和尺寸變體。";
    public const string PageDescription = "ToggleSwitch 用於即時的開關選擇，支持禁用、加載、自定義開關內容、圖標內容、尺寸變體、動效和波紋反饋。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyIsChecked = "從 ToggleButton 繼承的當前選中狀態。";
    public const string ApiPropertyGrooveBackground = "覆蓋開關軌道背景畫刷。";
    public const string ApiPropertyOnContent = "開關選中時顯示的內容。";
    public const string ApiPropertyOnContentTemplate = "用於渲染 OnContent 的模板。";
    public const string ApiPropertyOffContent = "開關未選中時顯示的內容。";
    public const string ApiPropertyOffContentTemplate = "用於渲染 OffContent 的模板。";
    public const string ApiPropertySizeType = "控制 Large、Middle、Small 或 Custom 開關尺寸。";
    public const string ApiPropertyIsLoading = "顯示加載指示器和等待交互狀態。";
    public const string ApiPropertyIsMotionEnabled = "啟用或禁用開關動效。";
    public const string ApiPropertyIsWaveSpiritEnabled = "啟用或禁用波紋反饋。";
    public const string TokenColumnToken = "變數";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameTrackHeight = "默認開關軌道高度。";
    public const string TokenNameTrackHeightSM = "小號開關軌道高度。";
    public const string TokenNameTrackMinWidth = "默認開關軌道最小寬度。";
    public const string TokenNameTrackMinWidthSM = "小號開關軌道最小寬度。";
    public const string TokenNameTrackPadding = "開關軌道內部邊距。";
    public const string TokenNameHandleBg = "開關把手背景色。";
    public const string TokenNameHandleShadow = "開關把手陰影。";
    public const string TokenNameHandleSize = "默認開關把手尺寸。";
    public const string TokenNameHandleSizeSM = "小號開關把手尺寸。";
    public const string TokenNameInnerMinMargin = "默認內容布局的最小內部邊距。";
    public const string TokenNameInnerMaxMargin = "默認內容布局的最大內部邊距。";
    public const string TokenNameInnerMinMarginSM = "小號內容布局的最小內部邊距。";
    public const string TokenNameInnerMaxMarginSM = "小號內容布局的最大內部邊距。";
    public const string TokenNameIconSize = "默認內容圖標尺寸。";
    public const string TokenNameIconSizeSM = "小號內容圖標尺寸。";
    public const string TokenNameSwitchColor = "開關激活色。";
    public const string TokenNameSwitchDisabledOpacity = "禁用開關狀態使用的透明度。";
    public const string TokenNameExtraInfoFontSize = "默認開關文本字號。";
    public const string TokenNameExtraInfoFontSizeSM = "小號開關文本字號。";
    public const string TokenNameLoadingAnimationDuration = "加載指示器動畫周期。";
    public const string TokenNameOffStateLoadIndicatorColor = "關閉狀態下的加載指示器顏色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "Switch 的禁用狀態。";
    public const string TextAndIconTitle = "文本和圖標";
    public const string TextAndIconDescription = "帶文本和圖標。";
    public const string TwoSizesTitle = "尺寸";
    public const string TwoSizesDescription = "SizeType 控制預設和 Custom 開關尺寸模式。";
    public const string LoadingTitle = "加載中";
    public const string LoadingDescription = "標記開關的等待狀態。";
    public const string P2ContentToggleDisabled = "切換禁用";
    public const string P2ContentToggleLoading = "切換加載";
    public const string P2ContentCustom = "自定義";

    public const string P2OnContentOn = "開";

    public const string P2OffContentOff = "關";

    public const string P2OnContentText = "開";

    public const string P2OffContentText = "關";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
