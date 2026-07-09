using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Message;

[LanguageProvider(LanguageCode.zh_TW, MessageShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Message 的最簡單用法。";
    public const string OtherTypesTitle = "其他消息類型";
    public const string OtherTypesDescription = "success、error 和 warning 類型的消息。";
    public const string LoadingIndicatorTitle = "帶加載指示器的消息";
    public const string LoadingIndicatorDescription = "顯示一個全局加載指示器，並異步自動關閉。";
    public const string CallbackTitle = "回調";
    public const string CallbackDescription = "上面的示例會在舊消息即將關閉時顯示一條新消息。";
    public const string ComponentCategory = "反饋";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "用於輕量級操作反饋的全局提示消息。";
    public const string PageDescription = "Message 在當前窗口頂層顯示簡短反饋，適用於保存結果、校驗反饋、異步進度和連續完成提示。";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變量";
    public const string ApiColumnMember = "成員";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyMessageContent = "消息卡片顯示的文本內容。";
    public const string ApiPropertyMessageType = "消息語義類型，用於控制默認圖標和視覺狀態。";
    public const string ApiPropertyMessageIcon = "可選自定義圖標。未設置時會根據消息類型選擇默認圖標。";
    public const string ApiPropertyMessageExpiration = "自動關閉延遲。使用 TimeSpan.Zero 可保持消息直到手動關閉。";
    public const string ApiPropertyMessageOnClose = "消息關閉後觸發的回調。";
    public const string ApiPropertyManagerPosition = "窗口消息管理器使用的屏幕位置。";
    public const string ApiPropertyManagerMaxItems = "可見消息最大數量，超出限制時較早的可見消息會關閉。";
    public const string ApiPropertyManagerIsMotionEnabled = "是否啟用消息打開和關閉動效。";
    public const string ApiMethodManagerShow = "顯示一個 IMessage 實例，並應用可選樣式類。";
    public const string TokenColumnToken = "變量";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNameMessageContentBg = "消息卡片的背景色。";
    public const string TokenNameMessageContentPadding = "消息卡片內容區域的內邊距。";
    public const string TokenNameMessageCardHeight = "消息卡片預留的默認高度。";
    public const string TokenNameMessageIconSize = "消息狀態圖標尺寸。";
    public const string TokenNameMessageIconMargin = "消息狀態圖標的外邊距。";
    public const string TokenNameMessageTopMargin = "消息卡片堆疊時使用的頂層外邊距。";
    public const string P2ContentDisplayNormalMessage = "顯示普通消息";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentInfo = "信息";
    public const string P2ContentWarning = "警告";
    public const string P2ContentError = "錯誤";
    public const string P2ContentDisplayALoadingIndicator = "顯示加載指示器";
    public const string P2MessageHelloAtomUIAvalonia = "你好，AtomUI/Avalonia！";
    public const string P2MessageInformation = "這是一條信息消息。";
    public const string P2MessageSuccess = "這是一條成功消息。";
    public const string P2MessageWarning = "這是一條警告消息。";
    public const string P2MessageError = "這是一條錯誤消息。";
    public const string P2MessageActionInProgress = "操作進行中...";
    public const string P2MessageLoadingFinished = "加載完成";

}
