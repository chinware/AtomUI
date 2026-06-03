using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.zh_CN, RateShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string HalfStarTitle = "半星";
    public const string HalfStarDescription = "支持选择半星。";
    public const string ShowCopywritingTitle = "显示文案";
    public const string ShowCopywritingDescription = "在评分组件中添加文案。";
    public const string ReadOnlyTitle = "只读";
    public const string ReadOnlyDescription = "只读状态，不能使用鼠标交互。";
    public const string ClearStarTitle = "清除星级";
    public const string ClearStarDescription = "支持设置再次点击时允许清除星级。";
    public const string OtherCharacterTitle = "其他字符";
    public const string OtherCharacterDescription = "将默认星形替换为其他字符，例如字母、数字、图标字体，甚至中文文字。";
    public const string P2TextIsallowclearTrue = "允许清除：true";
    public const string P2TextIsallowclearFalse = "允许清除：false";
    public const string P2TooltipTerrible = "糟糕";
    public const string P2TooltipBad = "不好";
    public const string P2TooltipNormal = "一般";
    public const string P2TooltipGood = "好";
    public const string P2TooltipWonderful = "很棒";

    protected override Type GetResourceKindType() => typeof(RateShowCaseLangResourceKind);
}
