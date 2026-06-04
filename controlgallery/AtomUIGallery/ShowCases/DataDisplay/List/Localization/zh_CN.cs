using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.List;

[LanguageProvider(LanguageCode.zh_CN, ListShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioAdvanced = "进阶";
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法示例。";
    public const string SelectionTitle = "选择";
    public const string SelectionDescription = "可以设置单选、多选或不可选择。";
    public const string GroupTitle = "分组";
    public const string GroupDescription = "可以根据条件对数据分组。";
    public const string ItemDisabledTitle = "禁用项";
    public const string ItemDisabledDescription = "禁用列表项。";
    public const string EmptyTitle = "空状态";
    public const string EmptyDescription = "无数据时显示空状态指示。";
    public const string FilterTitle = "筛选";
    public const string FilterDescription = "可以按条件筛选数据；这里筛选包含“色”的项。";
    public const string OrderedTitle = "排序";
    public const string OrderedDescription = "可以根据指定属性排序。";
    public const string SimpleListBoxTitle = "简单列表框控件";
    public const string SimpleListBoxDescription = "基础用法示例。";
    public const string SimpleListBoxItemsSourceTitle = "简单列表框控件";
    public const string SimpleListBoxItemsSourceDescription = "基础 ItemsSource 和模板用法示例。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的列表。";
    public const string PaginationListTitle = "分页列表";
    public const string PaginationListDescription = "分页列表。";
    public const string P2PlaceholderTextSearch = "搜索";
    public const string P2FilterValue = "色";
    public const string P2TextSelectionMode = "选择模式：";
    public const string P2ContentSingle = "单选";
    public const string P2ContentMultiple = "多选";
    public const string P2ContentToggle = "切换选择";
    public const string P2ColorBlue = "蓝色";
    public const string P2ColorGreen = "绿色";
    public const string P2ColorRed = "红色";
    public const string P2ColorYellow = "黄色";
    public const string P2ColorOrange = "橙色";
    public const string P2ColorPurple = "紫色";
    public const string P2ColorPink = "粉色";
    public const string P2ColorBrown = "棕色";
    public const string P2ColorWhite = "白色";
    public const string P2ColorBlack = "黑色";
    public const string P2ColorGray = "灰色";
    public const string P2ColorTurquoise = "青绿色";
    public const string P2ColorViolet = "紫罗兰色";
    public const string P2ColorMagenta = "品红色";
    public const string P2ColorMaroon = "栗色";
    public const string P2ColorNavy = "海军蓝";
    public const string P2ColorBeige = "米色";
    public const string P2ColorCyan = "青色";
    public const string P2ColorLavender = "薰衣草色";
    public const string P2ColorOlive = "橄榄色";
    public const string P2GroupBasicColors = "基础颜色";
    public const string P2GroupNeutralColors = "中性色";
    public const string P2GroupSpecificShades = "特定色调";
    public const string P2ContentRacingCarSpraysBurningFuelIntoCrowd = "赛车向人群喷出燃烧的燃料。";
    public const string P2ContentJapanesePrincessToWedCommoner = "日本公主将嫁给平民。";
    public const string P2ContentAustralianWalksN100kmAfterOutbackCrash = "澳大利亚人在内陆车祸后步行 100 公里。";
    public const string P2ContentManChargedOverMissingWeddingGirl = "男子因婚礼女孩失踪案被起诉。";
    public const string P2ContentLosAngelesBattlesHugeWildfires = "洛杉矶抗击大规模山火。";
    public const string P2ContentDynamicItem = "动态项";
    public const string P2ContentPaginationItemFormat = "内容 {0}";

    public const string P2ContentAddItem = "添加项";

    public const string P2ContentRemoveItem = "移除项";

    protected override Type GetResourceKindType() => typeof(ListShowCaseLangResourceKind);
}
