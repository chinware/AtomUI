using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.zh_TW, GroupBoxShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "GroupBox 控件的基礎用法。";
    public const string AutoHeightTitle = "自動高度";
    public const string AutoHeightDescription = "GroupBox 未設定高度時會根據 Header、Padding 和內容期望尺寸自動撐開。";
    public const string HeaderPositionTitle = "標題位置";
    public const string HeaderPositionDescription = "GroupBox 標題支持左、中、右三種位置。";
    public const string HeaderStyleTitle = "標題樣式";
    public const string HeaderStyleDescription = "GroupBox 標題支持自定義顏色和字體等屬性。";
    public const string HeaderIconTitle = "標題圖標";
    public const string HeaderIconDescription = "GroupBox 標題支持指定圖標。";
    public const string P2HeaderTitleTitleInfo = "標題信息";
    public const string P2TextContentOfGroupBox = "分組框內容";
    public const string AutoHeightContentOverview = "下面的 GroupBox 沒有設定 Height，內容區域會隨著文字行數自動增長。";
    public const string AutoHeightContentDetail = "當內容來自 StackPanel、Grid 或顯式尺寸控件時，GroupBox 會使用內容的 DesiredSize 計算整體高度。";
    public const string AutoHeightContentFooter = "如果父容器設定了固定高度或 MaxHeight，仍然會按 Avalonia 佈局約束進行裁剪或滾動。";
    public const string ScenarioExamples = "範例";
    public const string PageSubtitle = "用帶標題的邊框容器組織相關內容。";
    public const string PageDescription =
        "GroupBox 通過帶邊框的容器包裹相關內容，並支持配置標題、位置、圖標和字體樣式。";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";

}
