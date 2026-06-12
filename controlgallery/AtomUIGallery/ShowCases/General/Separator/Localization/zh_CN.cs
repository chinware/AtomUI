using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.zh_CN, SeparatorShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "通用";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用水平或垂直分割线分隔内容区块。";
    public const string PageDescription = "Separator 用于建立内容之间的视觉节奏，支持标题文本、普通文本样式、垂直分割、线型变体和不同间距尺寸。";
    public const string InfoNamespaceLabel = "命名空间:";
    public const string InfoPackageLabel = "包:";
    public const string InfoBaseClassLabel = "基类:";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyTitle = "显示在水平分割线中的文本。";
    public const string ApiPropertyTitlePosition = "设置标题在分割线左侧、居中或右侧显示。";
    public const string ApiPropertyTitleColor = "用于渲染分割线标题文本的画刷。";
    public const string ApiPropertyLineColor = "用于渲染分割线线条的画刷。";
    public const string ApiPropertyOrientation = "控制分割线是水平还是垂直方向。";
    public const string ApiPropertyOrientationMargin = "标题靠左或靠右时，与最近边缘之间的距离。";
    public const string ApiPropertyVariant = "在线、点线和虚线之间切换分割线样式。";
    public const string ApiPropertyLineWidth = "不受渲染缩放影响的分割线线宽。";
    public const string ApiPropertyIsPlain = "使用普通正文样式显示标题，而不是标题样式。";
    public const string ApiPropertySizeType = "控制水平分割线的间距密度。";
    public const string ApiPropertyVerticalSeparatorOrientation = "VerticalSeparator 会把 Separator 的方向覆盖为垂直。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameTextPaddingInline = "标题两侧的内联内间距，单位为 em。";
    public const string TokenNameOrientationMarginPercent = "未指定 orientation margin 时，标题到边缘的默认比例。";
    public const string TokenNameVerticalMarginInline = "垂直分割线使用的水平外间距。";
    public const string TokenNameHorizontalMarginBlockSM = "小号水平分割线的垂直外间距。";
    public const string TokenNameHorizontalMarginBlock = "水平分割线的默认垂直外间距。";
    public const string TokenNameHorizontalMarginBlockLG = "大号水平分割线的垂直外间距。";
    public const string TokenNameHorizontalWithTextGutterMargin = "带标题水平分割线使用的垂直外间距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string HorizontalTitle = "水平分割线";
    public const string HorizontalDescription = "Separator 默认是水平分割线，可以在 Separator 中添加文本。";
    public const string DividerWithTitleTitle = "带标题的分割线";
    public const string DividerWithTitleDescription = "带内部标题的分割线，可设置 orientation='left/right' 来对齐标题。";
    public const string PlainTextTitle = "无标题样式文本";
    public const string PlainTextDescription = "通过设置 plain 属性，可以使用非标题样式的分割线文本。";
    public const string SpacingSizeTitle = "设置分割线间距";
    public const string SpacingSizeDescription = "设置间距大小。";
    public const string VerticalTitle = "垂直分割线";
    public const string VerticalDescription = "使用 type='vertical' 可以让分割线垂直显示。";
    public const string VariantTitle = "线型";
    public const string VariantDescription = "分割线默认使用实线样式，也可以改为虚线或点线。";
    public const string P2TitleText = "文本";
    public const string P2TitleLeftText = "左侧文本";
    public const string P2TitleRightText = "右侧文本";
    public const string P2TitleLeftTextWithN0Orientationmargin = "orientationMargin 为 0 的左侧文本";
    public const string P2TitleRightTextWithN50pxOrientationmargin = "orientationMargin 为 50px 的右侧文本";
    public const string P2TitleLeftText2 = "左侧文本";
    public const string P2TitleRightText2 = "右侧文本";
    public const string P2TitleSolid = "实线";
    public const string P2TitleDotted = "点线";
    public const string P2TitleDashed = "虚线";
    public const string P2TextLoremIpsumDolorSitAmetConsecteturAdipiscingElit = "这是一段用于演示分割线效果的示例文本。分割线可以组织内容层次，让页面结构更加清晰。";
    public const string P2TextItem1 = "项目 1";
    public const string P2TextItem2 = "项目 2";
    public const string P2TextItem3 = "项目 3";

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}
