using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TabStrip;

[LanguageProvider(LanguageCode.zh_TW, TabStripShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string TabStripBasicTitle = "基礎用法";
    public const string TabStripBasicDescription = "默認激活第一個標籤項。";
    public const string TabStripItemsSourceTitle = "通過 ItemSource 生成 TabStripItem";
    public const string TabStripItemsSourceDescription = "基於數據源和項目模板添加 TabStripItem。";
    public const string TabStripDisabledTitle = "禁用標籤";
    public const string TabStripDisabledDescription = "禁用某個標籤項。";
    public const string TabStripCenteredTitle = "居中顯示";
    public const string TabStripCenteredDescription = "標籤項居中顯示。";
    public const string TabStripIconTitle = "帶圖標";
    public const string TabStripIconDescription = "帶圖標的標籤項。";
    public const string TabStripSlideTitle = "滑動";
    public const string TabStripSlideDescription = "為了容納更多標籤，標籤可以左右滑動（或上下滑動）。";
    public const string TabStripCardTypeTitle = "卡片式標籤";
    public const string TabStripCardTypeDescription = "另一種標籤類型，不支持垂直模式。";
    public const string TabStripClosableTitle = "可關閉標籤";
    public const string TabStripClosableDescription = "支持可關閉的標籤設置。";
    public const string TabStripPositionTitle = "位置";
    public const string TabStripPositionDescription = "標籤位置可設為 left、right、top 或 bottom，在移動端會自動切換為 top。";
    public const string TabStripCardShapePositionTitle = "卡片形態位置";
    public const string TabStripCardShapePositionDescription = "標籤位置可設為 left、right、top 或 bottom，在移動端會自動切換為 top。";
    public const string TabStripSizeTitle = "尺寸";
    public const string TabStripSizeDescription = "大尺寸標籤通常用於頁頭，小尺寸可用於模態框。";
    public const string TabStripAddCloseTitle = "新增和關閉標籤";
    public const string TabStripAddCloseDescription = "隱藏默認加號圖標，並為自定義觸發器綁定事件。";
    public const string P2TextTabPosition = "標籤位置：";
    public const string P2ContentTop = "頂部";
    public const string P2ContentBottom = "底部";
    public const string P2ContentLeft = "左側";
    public const string P2ContentRight = "右側";
    public const string P2ContentSmall = "小號";
    public const string P2ContentMiddle = "中號";
    public const string P2ContentLarge = "大號";
    public const string P2ContentTabN1 = "標籤頁 1";
    public const string P2ContentTabN2 = "標籤頁 2";
    public const string P2ContentTabN3 = "標籤頁 3";
    public const string P2ContentTabN4 = "標籤頁 4";
    public const string P2ContentTabN5 = "標籤頁 5";
    public const string P2ContentTabN6 = "標籤頁 6";
    public const string P2ContentTabN7 = "標籤頁 7";
    public const string P2ContentTabN8 = "標籤頁 8";
    public const string P2ContentTabN9 = "標籤頁 9";
    public const string P2ContentTabN10 = "標籤頁 10";
    public const string P2ContentTabN11 = "標籤頁 11";
    public const string P2ContentTabN12 = "標籤頁 12";
    public const string P2ContentTabN13 = "標籤頁 13";
    public const string P2ContentTabN14 = "標籤頁 14";
    public const string P2ContentTabN15 = "標籤頁 15";
    public const string P2ContentTabN16 = "標籤頁 16";
    public const string P2ContentTabN17 = "標籤頁 17";
    public const string P2ContentTabN18 = "標籤頁 18";
    public const string P2ContentTabN19 = "標籤頁 19";
    public const string P2ContentTabN20 = "標籤頁 20";
    public const string P2ContentTabN21 = "標籤頁 21";
    public const string P2ContentTabN22 = "標籤頁 22";
    public const string P2ContentTabN23 = "標籤頁 23";
    public const string P2ContentNewTabFormat = "新增標籤 {0}";
    public const string P2TextTabContent = "標籤內容";

    protected override Type GetResourceKindType() => typeof(TabStripShowCaseLangResourceKind);
}

