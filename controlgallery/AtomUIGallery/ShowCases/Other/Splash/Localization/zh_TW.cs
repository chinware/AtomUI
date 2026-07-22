using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Splash;

[LanguageProvider(LanguageCode.zh_TW, SplashShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string ComponentCategory = "其他";
    public const string ComponentStatusPreview = "預覽";
    public const string ComponentIntroducedVersion = "v6.0.7";
    public const string PageSubtitle = "在主工作區準備完成前呈現桌面啟動進度。";
    public const string PageDescription = "Splash 組合緊湊的品牌面板、載入指示、確定進度、狀態文案和可選頁腳。服務 API 可將它託管在無邊框啟動視窗中，視覺控件也可以直接在 Gallery 中預覽。";
    public const string BasicTitle = "基礎";
    public const string BasicDescription = "使用 Splash 視覺控件展示啟動文案和不確定載入指示。";
    public const string DeterminateTitle = "確定進度";
    public const string DeterminateDescription = "當啟動流程有可度量階段時，設定 Progress 並關閉 IsIndeterminate。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "啟動流程進入終態後，將 Status 切換為 Success 或 Error。";
    public const string ComposedTitle = "Logo、內容與頁腳";
    public const string ComposedDescription = "當產品需要更豐富的啟動元資訊時，可以模板化 Logo 和頁腳。";
    public const string WindowServiceTitle = "視窗服務";
    public const string WindowServiceDescription = "開啟真實桌面 Splash 視窗，執行 5 秒模擬啟動流程，然後自動關閉。";
    public const string P2SubtitleDesktopBoot = "桌面啟動流程";
    public const string P2MessagePreparingShell = "正在準備工作區";
    public const string P2DetailLoadingThemeAndResources = "正在載入主題、語言資源和快取狀態。";
    public const string P2MessageLoadingModules = "正在載入模組";
    public const string P2DetailProgress = "主題、圖示和路由目錄已就緒，正在初始化可選套件。";
    public const string P2MessageReady = "工作區已就緒";
    public const string P2DetailReady = "滿足最短展示時長後即可顯示主視窗。";
    public const string P2MessageError = "啟動失敗";
    public const string P2DetailError = "可透過 SetErrorAsync 在關閉前呈現阻塞型啟動錯誤。";
    public const string P2FooterStaticPreview = "Gallery 靜態預覽";
    public const string P2ContentModuleCore = "核心";
    public const string P2ContentModuleTheme = "主題";
    public const string P2ContentModuleGallery = "Gallery";
    public const string P2FooterDesktopOnly = "視窗服務面向桌面啟動流程。";
    public const string P2ContentShowWindowSplash = "顯示視窗 Splash";
    public const string P2WindowSplashSubtitle = "桌面啟動模擬";
    public const string P2WindowSplashMessageStarting = "正在啟動 Gallery 工作區";
    public const string P2WindowSplashDetailStarting = "臨時 Splash 視窗正在展示 5 秒模擬啟動流程。";
    public const string P2WindowSplashMessageLoadingTheme = "正在載入主題資源";
    public const string P2WindowSplashMessageLoadingControls = "正在註冊控件套件";
    public const string P2WindowSplashMessageLoadingRoutes = "正在準備路由目錄";
    public const string P2WindowSplashMessageWarmingCache = "正在預熱 Gallery 快取";
    public const string P2WindowSplashMessageFinalizing = "正在完成工作區準備";
    public const string P2WindowSplashDetailProgress = "這是 Gallery 範例裡的模擬載入步驟。";
    public const string P2WindowSplashMessageComplete = "Gallery 已就緒";
    public const string P2WindowSplashDetailComplete = "模擬啟動流程已完成，視窗即將關閉。";
    public const string P2WindowSplashFooter = "Desktop Extras 套件 / 視窗託管啟動面板";
    public const string ApiStaticShowAsync = "靜態便捷 API，委託給 Splash.DefaultService。";
    public const string ApiServiceShowAsync = "實例服務 API，適用於希望透過依賴注入控制啟動流程的應用。";
    public const string ApiMethodSetProgress = "更新 Splash 實例的進度、訊息和詳情。";
    public const string ApiOptionMinimumShowDuration = "Splash 視窗允許關閉前的最短展示時間。";

}
