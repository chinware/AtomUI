using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tag;

[LanguageProvider(LanguageCode.zh_TW, TagShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎 Tag 用法。可通過 IsClosable 設置為可關閉，並通過 closeIcon 屬性自定義關閉按鈕；closeIcon 設置為 true 時顯示默認關閉按鈕。IsClosable Tag 支持 onClose 事件。";
    public const string ColorfulTagTitle = "多彩標籤";
    public const string ColorfulTagDescription = "預設了一系列多彩標籤樣式，可用於不同場景。也可以設置十六進制顏色字符串來自定義顏色。";
    public const string StatusTagTitle = "狀態標籤";
    public const string StatusTagDescription = "預設了五種不同顏色，可以設置 success、processing、error、default 和 warning 等 color 屬性表示具體狀態。";
    public const string IconTitle = "圖標";
    public const string IconDescription = "Tag 組件可以包含圖標。可以通過設置 icon 屬性，或在 Tag 內放置 Icon 組件實現。如果需要精確控製圖標的位置和佈局，應在 Tag 內放置 Icon 組件，而不是使用 icon 屬性。";
    public const string BorderlessTitle = "無邊框";
    public const string BorderlessDescription = "無邊框。";
    public const string P2ContentTagN1 = "標籤 1";
    public const string P2ContentLink = "鏈接";
    public const string P2ContentPreventDefault = "阻止默認行為";
    public const string P2ContentTagN2 = "標籤 2";
    public const string P2TextPresets = "預設";
    public const string P2ContentMagenta = "品紅色";
    public const string P2ContentRed = "紅色";
    public const string P2ContentVolcano = "火山色";
    public const string P2ContentOrange = "橙色";
    public const string P2ContentGold = "金色";
    public const string P2ContentLime = "青檸色";
    public const string P2ContentGreen = "綠色";
    public const string P2ContentCyan = "青色";
    public const string P2ContentBlue = "藍色";
    public const string P2ContentGeekblue = "極客藍";
    public const string P2ContentPurple = "紫色";
    public const string P2TextCustom = "自定義";
    public const string P2TextWithoutIcon = "無圖標";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentProcessing = "處理中";
    public const string P2ContentError = "錯誤";
    public const string P2ContentWarning = "警告";
    public const string P2ContentDefault = "默認";
    public const string P2ContentTwitter = "Twitter";
    public const string P2ContentYoutube = "Youtube";
    public const string P2ContentFacebook = "Facebook";
    public const string P2ContentLinkedin = "Linkedin";
    public const string P2ContentTag1 = "標籤1";
    public const string P2ContentTag2 = "標籤2";
    public const string P2ContentTag3 = "標籤3";
    public const string P2ContentTag4 = "標籤4";

    public const string P2TextMaterialIcon = "Material 圖標";

    protected override Type GetResourceKindType() => typeof(TagShowCaseLangResourceKind);
}

