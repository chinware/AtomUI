using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Timeline;

[LanguageProvider(LanguageCode.zh_TW, TimelineShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎用法示例。";
    public const string ColorTitle = "顏色";
    public const string ColorDescription = "設置圓圈顏色。綠色表示完成或成功，紅色表示警告或錯誤，藍色表示進行中或其他默認狀態，灰色表示未完成或禁用。";
    public const string LastNodeAndReversingTitle = "最後節點和反轉";
    public const string LastNodeAndReversingDescription = "當時間軸未完成且仍在進行時，可在最後放置一個待處理節點。將 Pending 設置為有效值可顯示待處理項；也可以自定義待處理內容和待處理圖標。IsReverse 用於反轉節點。";
    public const string AlternateTitle = "交替展示";
    public const string AlternateDescription = "交替展示的時間軸。";
    public const string LabelTitle = "標籤";
    public const string LabelDescription = "使用 label 單獨顯示時間。";
    public const string RightAlternateTitle = "右側交替展示";
    public const string RightAlternateDescription = "右側交替展示的時間軸。";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiated = "2024-01-01 AtomUI 正式啓動";
    public const string P2ContentN2024N08N12AfterMoreThanN7Months = "2024-08-12 經過 7 個多月的開發，AtomUI 正式開源。歡迎大家關注我們。";
    public const string P2ContentN2024N10N01ReleaseOfTheN0N0 = "2024-10-01 發佈 0.0.1 預覽版";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN1 = "2024-01-01 AtomUI 正式啓動。1";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN2 = "2024-01-01 AtomUI 正式啓動。2";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN3 = "2024-01-01 AtomUI 正式啓動。3";
    public const string P2ContentToggleReverse = "切換反轉";
    public const string P2ContentRecording = "記錄中...";
    public const string P2ContentLeft = "左側";
    public const string P2ContentRight = "右側";
    public const string P2ContentAlternate = "交替";
    public const string P2ContentAtomuiOfficiallyInitiated = "AtomUI 正式啓動";
    public const string P2ContentCreateAServicesSite = "創建服務站點";
    public const string P2ContentQinwareWebsiteOnline = "Qinware 網站上線";
    public const string P2ContentNetworkProblemsBeingSolved = "網絡問題正在解決";

    protected override Type GetResourceKindType() => typeof(TimelineShowCaseLangResourceKind);
}

