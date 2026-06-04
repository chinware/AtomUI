using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Select;

[LanguageProvider(LanguageCode.zh_TW, SelectShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎用法。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioOptions = "選項";
    public const string ScenarioAppearance = "外觀";
    public const string SearchFieldTitle = "帶搜索框的選擇器";
    public const string SearchFieldDescription = "展開時搜索選項。";
    public const string CustomSearchTitle = "自定義搜索";
    public const string CustomSearchDescription = "使用 filterOption 自定義搜索，對 Header 進行大小寫敏感搜索。";
    public const string MultipleSelectionTitle = "多選";
    public const string MultipleSelectionDescription = "多選，從已有項中選擇。";
    public const string TagsTitle = "標籤";
    public const string TagsDescription = "允許用戶從列表中選擇標籤或輸入自定義標籤。";
    public const string SizesTitle = "尺寸";
    public const string SizesDescription = "選擇器輸入區域默認高度為 32px。size 設置為 large 時高度為 40px，設置為 small 時高度為 24px。";
    public const string CustomDropdownOptionsTitle = "自定義下拉選項";
    public const string CustomDropdownOptionsDescription = "使用 optionRender 自定義下拉選項渲染。";
    public const string OptionGroupTitle = "選項分組";
    public const string OptionGroupDescription = "使用 OptGroup 對選項分組。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "Select 提供四種變體：描邊、填充、無邊框和下划線。";
    public const string HideAlreadySelectedTitle = "隱藏已選項";
    public const string HideAlreadySelectedDescription = "在下拉列表中隱藏已經選擇的選項。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Select 添加狀態，可設置為錯誤或警告。";
    public const string MaxCountTitle = "最大數量";
    public const string MaxCountDescription = "可以設置 maxCount 屬性控制最多可選項數量。超過限制後，選項會變為禁用狀態。";
    public const string ResponsiveMaxTagCountTitle = "響應式 maxTagCount";
    public const string ResponsiveMaxTagCountDescription = "在響應式場景中自動折疊為標籤。由於響應式計算有性能成本，不建議在大型表單中使用。";
    public const string PrefixAndSuffixTitle = "前綴和後綴";
    public const string PrefixAndSuffixDescription = "自定義 prefix 和 suffixIcon。";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2HeaderJack = "傑克";
    public const string P2HeaderLucy = "露西";
    public const string P2HeaderYiminghe = "一鳴鶴";
    public const string P2HeaderDisabled = "禁用";
    public const string P2HeaderTom = "湯姆";
    public const string P2PlaceholderTextSelectAPerson = "請選擇人員";
    public const string P2PlaceholderTextTagsMode = "標籤模式";
    public const string P2DescriptionChina = "中國";
    public const string P2HeaderChina = "中國";
    public const string P2DescriptionUsa = "美國";
    public const string P2HeaderUsa = "美國";
    public const string P2DescriptionJapan = "日本";
    public const string P2HeaderJapan = "日本";
    public const string P2DescriptionKorea = "韓國";
    public const string P2HeaderKorea = "韓國";
    public const string P2HeaderChloe = "克洛伊";
    public const string P2HeaderLucas = "盧卡斯";
    public const string P2PlaceholderTextOutline = "線框風格";
    public const string P2HeaderYiminghe2 = "一鳴鶴";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2PlaceholderTextInsertedAreRemoved = "已插入項會被移除";
    public const string P2HeaderApples = "蘋果";
    public const string P2HeaderNails = "釘子";
    public const string P2HeaderBananas = "香蕉";
    public const string P2HeaderHelicopters = "直升機";
    public const string P2HeaderAvaSwift = "艾娃·斯威夫特";
    public const string P2HeaderColeReed = "科爾·里德";
    public const string P2HeaderMiaBlake = "米婭·布萊克";
    public const string P2HeaderJakeStone = "傑克·斯通";
    public const string P2HeaderLilyLane = "莉莉·萊恩";
    public const string P2HeaderRyanChase = "瑞安·蔡斯";
    public const string P2HeaderZoeFox = "佐伊·福克斯";
    public const string P2HeaderAlexGrey = "亞歷克斯·格雷";
    public const string P2HeaderElleBlair = "艾爾·布萊爾";
    public const string P2PlaceholderTextSelectItem = "選擇項目...";
    public const string P2ContentLarge = "大號";
    public const string P2ContentDefault = "默認";
    public const string P2ContentSmall = "小號";
    public const string P2GroupManager = "經理";
    public const string P2GroupEngineer = "工程師";
    public const string P2ContentUser = "用戶";
    public const string P2TextLongLabelPrefix = "長標籤：";

    protected override Type GetResourceKindType() => typeof(SelectShowCaseLangResourceKind);
}

