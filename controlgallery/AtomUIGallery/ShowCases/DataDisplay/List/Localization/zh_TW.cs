using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.List;

[LanguageProvider(LanguageCode.zh_TW, ListShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAdvanced = "進階";
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎用法示例。";
    public const string SelectionTitle = "選擇";
    public const string SelectionDescription = "可以設置單選、多選或不可選擇。";
    public const string GroupTitle = "分組";
    public const string GroupDescription = "可以根據條件對數據分組。";
    public const string ItemDisabledTitle = "禁用項";
    public const string ItemDisabledDescription = "禁用列表項。";
    public const string EmptyTitle = "空狀態";
    public const string EmptyDescription = "無數據時顯示空狀態指示。";
    public const string FilterTitle = "篩選";
    public const string FilterDescription = "可以按條件篩選數據；這裡篩選包含“色”的項。";
    public const string OrderedTitle = "排序";
    public const string OrderedDescription = "可以根據指定屬性排序。";
    public const string SimpleListBoxTitle = "簡單列表框控件";
    public const string SimpleListBoxDescription = "基礎用法示例。";
    public const string SimpleListBoxItemsSourceTitle = "簡單列表框控件";
    public const string SimpleListBoxItemsSourceDescription = "基礎 ItemsSource 和模板用法示例。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的列表。";
    public const string PaginationListTitle = "分頁列表";
    public const string PaginationListDescription = "分頁列表。";
    public const string P2PlaceholderTextSearch = "搜索";
    public const string P2FilterValue = "色";
    public const string P2TextSelectionMode = "選擇模式：";
    public const string P2ContentSingle = "單選";
    public const string P2ContentMultiple = "多選";
    public const string P2ContentToggle = "切換選擇";
    public const string P2ColorBlue = "藍色";
    public const string P2ColorGreen = "綠色";
    public const string P2ColorRed = "紅色";
    public const string P2ColorYellow = "黃色";
    public const string P2ColorOrange = "橙色";
    public const string P2ColorPurple = "紫色";
    public const string P2ColorPink = "粉色";
    public const string P2ColorBrown = "棕色";
    public const string P2ColorWhite = "白色";
    public const string P2ColorBlack = "黑色";
    public const string P2ColorGray = "灰色";
    public const string P2ColorTurquoise = "青綠色";
    public const string P2ColorViolet = "紫羅蘭色";
    public const string P2ColorMagenta = "品紅色";
    public const string P2ColorMaroon = "栗色";
    public const string P2ColorNavy = "海軍藍";
    public const string P2ColorBeige = "米色";
    public const string P2ColorCyan = "青色";
    public const string P2ColorLavender = "薰衣草色";
    public const string P2ColorOlive = "橄欖色";
    public const string P2GroupBasicColors = "基礎顏色";
    public const string P2GroupNeutralColors = "中性色";
    public const string P2GroupSpecificShades = "特定色調";
    public const string P2ContentRacingCarSpraysBurningFuelIntoCrowd = "賽車向人群噴出燃燒的燃料。";
    public const string P2ContentJapanesePrincessToWedCommoner = "日本公主將嫁給平民。";
    public const string P2ContentAustralianWalksN100kmAfterOutbackCrash = "澳大利亞人在內陸車禍後步行 100 公里。";
    public const string P2ContentManChargedOverMissingWeddingGirl = "男子因婚禮女孩失蹤案被起訴。";
    public const string P2ContentLosAngelesBattlesHugeWildfires = "洛杉磯抗擊大規模山火。";
    public const string P2ContentDynamicItem = "動態項";
    public const string P2ContentPaginationItemFormat = "內容 {0}";

    public const string P2ContentAddItem = "添加項";

    public const string P2ContentRemoveItem = "移除項";

    protected override Type GetResourceKindType() => typeof(ListShowCaseLangResourceKind);
}

