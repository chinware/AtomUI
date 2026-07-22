using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Notification;

[LanguageProvider(LanguageCode.zh_TW, NotificationShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Notification 的最簡單用法。";
    public const string DurationTitle = "自動關閉時長";
    public const string DurationDescription = "Duration 可指定通知保持打開的時間。時間結束後通知會自動關閉。未指定時默認值為 4.5 秒；設置為 TimeSpan.Zero 時通知不會自動關閉。";
    public const string WithIconTitle = "帶圖標通知";
    public const string WithIconDescription = "左側帶圖標的通知框。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "通知框可通過 placement 從視口頂部、底部、左上、右上、左下或右下出現。";
    public const string CustomizedIconTitle = "自定義圖標";
    public const string CustomizedIconDescription = "圖標可以自定義為任意圖標節點。";
    public const string ProgressTitle = "顯示進度";
    public const string ProgressDescription = "為自動關閉的通知顯示進度條。";
    public const string ComponentCategory = "反饋";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "用於較長反饋和非阻塞更新的桌面通知。";
    public const string PageDescription = "Notification 在窗口邊緣顯示信息更豐富的反饋卡片，支持語義類型、自定義圖標、彈出位置、手動時長、懸停暫停和自動關閉進度。";
    public const string ScenarioExamples = "示例";
    public const string ApiMethodManagerShow = "顯示 INotification 實例並應用可選樣式類。";
    public const string P2ContentShowNotification = "顯示通知";
    public const string P2ContentOpenTheNotificationBox = "打開通知框";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentInfo = "信息";
    public const string P2ContentWarning = "警告";
    public const string P2ContentError = "錯誤";
    public const string P2ContentTop = "頂部";
    public const string P2ContentBottom = "底部";
    public const string P2ContentTopleft = "左上";
    public const string P2ContentTopright = "右上";
    public const string P2ContentBottomleft = "左下";
    public const string P2ContentBottomright = "右下";
    public const string P2ContentPauseOnHover = "懸停時暫停";
    public const string P2ContentDonTPauseOnHover = "懸停時不暫停";
    public const string P2NotificationTitle = "通知標題";
    public const string P2NotificationTopTitle = "頂部通知";
    public const string P2NotificationBottomTitle = "底部通知";
    public const string P2NotificationTopLeftTitle = "左上通知";
    public const string P2NotificationTopRightTitle = "右上通知";
    public const string P2NotificationBottomLeftTitle = "左下通知";
    public const string P2NotificationBottomRightTitle = "右下通知";
    public const string P2NotificationHello = "你好，AtomUI/Avalonia！";
    public const string P2NotificationContent = "這是通知內容。這是通知內容。這是通知內容。";
    public const string P2NotificationNeverCloseContent = "我不會自動關閉。這是一段特意寫得很長的描述，包含很多字符和詞語。";

}
