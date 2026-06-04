using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.zh_TW, RateShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string HalfStarTitle = "半星";
    public const string HalfStarDescription = "支持選擇半星。";
    public const string ShowCopywritingTitle = "顯示文案";
    public const string ShowCopywritingDescription = "在評分組件中添加文案。";
    public const string ReadOnlyTitle = "只讀";
    public const string ReadOnlyDescription = "只讀狀態，不能使用鼠標交互。";
    public const string ClearStarTitle = "清除星級";
    public const string ClearStarDescription = "支持設置再次點擊時允許清除星級。";
    public const string OtherCharacterTitle = "其他字符";
    public const string OtherCharacterDescription = "將默認星形替換為其他字符，例如字母、數字、圖標字體，甚至中文文字。";
    public const string P2TextIsallowclearTrue = "允許清除：true";
    public const string P2TextIsallowclearFalse = "允許清除：false";
    public const string P2TooltipTerrible = "糟糕";
    public const string P2TooltipBad = "不好";
    public const string P2TooltipNormal = "一般";
    public const string P2TooltipGood = "好";
    public const string P2TooltipWonderful = "很棒";

    protected override Type GetResourceKindType() => typeof(RateShowCaseLangResourceKind);
}

