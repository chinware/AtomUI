using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.zh_TW, SeparatorShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string HorizontalTitle = "水平分割線";
    public const string HorizontalDescription = "Separator 默認是水平分割線，可以在 Separator 中添加文本。";
    public const string DividerWithTitleTitle = "帶標題的分割線";
    public const string DividerWithTitleDescription = "帶內部標題的分割線，可設置 orientation='left/right' 來對齊標題。";
    public const string PlainTextTitle = "無標題樣式文本";
    public const string PlainTextDescription = "通過設置 plain 屬性，可以使用非標題樣式的分割線文本。";
    public const string SpacingSizeTitle = "設置分割線間距";
    public const string SpacingSizeDescription = "設置間距大小。";
    public const string VerticalTitle = "垂直分割線";
    public const string VerticalDescription = "使用 type='vertical' 可以讓分割線垂直顯示。";
    public const string VariantTitle = "線型";
    public const string VariantDescription = "分割線默認使用實線樣式，也可以改為虛線或點線。";
    public const string P2TitleText = "文本";
    public const string P2TitleLeftText = "左側文本";
    public const string P2TitleRightText = "右側文本";
    public const string P2TitleLeftTextWithN0Orientationmargin = "orientationMargin 為 0 的左側文本";
    public const string P2TitleRightTextWithN50pxOrientationmargin = "orientationMargin 為 50px 的右側文本";
    public const string P2TitleLeftText2 = "左側文本";
    public const string P2TitleRightText2 = "右側文本";
    public const string P2TitleSolid = "實線";
    public const string P2TitleDotted = "點線";
    public const string P2TitleDashed = "虛線";
    public const string P2TextLoremIpsumDolorSitAmetConsecteturAdipiscingElit = "這是一段用於演示分割線效果的示例文本。分割線可以組織內容層次，讓頁面結構更加清晰。";
    public const string P2TextItem1 = "項目 1";
    public const string P2TextItem2 = "項目 2";
    public const string P2TextItem3 = "項目 3";

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}

