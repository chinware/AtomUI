using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Badge;

[LanguageProvider(LanguageCode.zh_TW, BadgeShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。count 為 0 時 Badge 會隱藏，但可以使用 showZero 顯示。";
    public const string OverflowCountTitle = "封頂數字";
    public const string OverflowCountDescription = "當 count 大於 overflowCount 時顯示 ${overflowCount}+。overflowCount 的默認值是 99。";
    public const string OffsetTitle = "偏移量";
    public const string OffsetDescription = "設置徽標點的偏移量，格式為 [left, top]，表示狀態點相對於默認位置左側和頂部的偏移。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "設置數字 Badge 的尺寸。";
    public const string StandaloneTitle = "獨立使用";
    public const string StandaloneDescription = "children 為空時可獨立使用。";
    public const string DynamicTitle = "動態變化";
    public const string DynamicDescription = "數字變化時會帶有動畫。";
    public const string RedBadgeTitle = "紅點徽標";
    public const string RedBadgeDescription = "簡單展示一個紅色徽標，不顯示具體數字。如果 count 等於 0，則不會顯示圓點。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "帶狀態的獨立徽標。";
    public const string RibbonTitle = "緞帶徽標";
    public const string RibbonDescription = "使用緞帶樣式徽標。";
    public const string ColorfulBadgeTitle = "多彩徽標";
    public const string ColorfulBadgeDescription = "預設了一系列多彩 Badge 樣式，可用於不同場景。也可以設置十六進制顏色字符串來自定義顏色。";
    public const string P2TextSuccess = "成功";
    public const string P2TextError = "錯誤";
    public const string P2TextDefault = "默認";
    public const string P2TextProcessing = "處理中";
    public const string P2TextWarning = "警告";
    public const string P2TextPolishExperience = "精益求精，打造體驗優秀的 UISDK";
    public const string P2TextJiachenPlan = "甲辰計劃雄起";
    public const string P2TextAvaloniaExcellent = "Avalonia 非常優秀";
    public const string P2TextHippies = "嬉皮士";
    public const string P2TitlePresets = "預設";
    public const string P2TextPink = "粉色";
    public const string P2TextRed = "紅色";
    public const string P2TextYellow = "黃色";
    public const string P2TextOrange = "橙色";
    public const string P2TextCyan = "青色";
    public const string P2TextGreen = "綠色";
    public const string P2TextBlue = "藍色";
    public const string P2TextPurple = "紫色";
    public const string P2TextGeekblue = "極客藍";
    public const string P2TextMagenta = "品紅";
    public const string P2TextVolcano = "火山色";
    public const string P2TextGold = "金色";
    public const string P2TextLime = "青檸";
    public const string P2TitleCustom = "自定義";
    public const string P2TextRgbN45N183N245 = "rgb(45, 183, 245)";
    public const string P2TextHslN102N53N61 = "hsl(102, 53%, 61%)";
    public const string P2TextRgbN15N141N230 = "rgb(15, 141, 230)";
    public const string P2ContentAdd = "增加";
    public const string P2ContentSub = "減少";
    public const string P2ContentRandom = "隨機";
    public const string P2TextPushesOpenTheWindow = "推開窗戶";
    public const string P2TextAndRaisesTheSpyglass = "並舉起望遠鏡。";

    public const string P2ContentLinkSomething = "鏈接內容";

    protected override Type GetResourceKindType() => typeof(BadgeShowCaseLangResourceKind);
}

