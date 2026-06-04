using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.zh_TW, TooltipShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "提供 12 種彈出位置。";
    public const string ArrowTitle = "箭頭";
    public const string ArrowDescription = "支持顯示、隱藏或保持箭頭居中。";
    public const string ColorfulTooltipTitle = "多彩提示";
    public const string ColorfulTooltipDescription = "預設了一系列多彩提示樣式，可用於不同場景。";
    public const string P2TextTooltipWillShowOnMouseEnter = "鼠標移入時顯示提示。";
    public const string P2ContentShow = "顯示";
    public const string P2ContentHide = "隱藏";
    public const string P2ContentCenter = "居中";
    public const string P2TextPresets = "預設";
    public const string P2TextCustom = "自定義";

    public const string P2ToolTipTipPromptText = "提示文本";

    public const string P2ContentLT = "左上";

    public const string P2ContentLeft = "左側";

    public const string P2ContentLB = "左下";

    public const string P2ContentTL = "上左";

    public const string P2ContentTop = "上方";

    public const string P2ContentTR = "上右";

    public const string P2ContentRT = "右上";

    public const string P2ContentRight = "右側";

    public const string P2ContentRB = "右下";

    public const string P2ContentBL = "下左";

    public const string P2ContentBottom = "下方";

    public const string P2ContentBR = "下右";

    public const string P2ContentBlue = "藍色";

    public const string P2ContentRed = "紅色";

    public const string P2ContentVolcano = "火山色";

    public const string P2ContentOrange = "橙色";

    public const string P2ContentGold = "金色";

    public const string P2ContentYellow = "黃色";

    public const string P2ContentLime = "青檸色";

    public const string P2ContentGreen = "綠色";

    public const string P2ContentCyan = "青色";

    public const string P2ContentGeekBlue = "極客藍";

    public const string P2ContentPurple = "紫色";

    public const string P2ContentPink = "粉色";

    public const string P2ContentMagenta = "品紅";

    public const string P2ContentGrey = "灰色";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}

