using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Avatar;

[LanguageProvider(LanguageCode.zh_CN, AvatarShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "使用图片、图标或文本头像表示用户、团队或对象。";
    public const string PageDescription = "Avatar 用于在紧凑空间中展示身份信息。它支持圆形和方形、显式尺寸、文本自动缩放，以及头像组的折叠展示。";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyShape = "设置头像形状，可在圆形和方形之间切换。";
    public const string ApiPropertySize = "设置显式头像尺寸，并让控件进入自定义尺寸模式。";
    public const string ApiPropertySizeType = "使用共享尺寸刻度展示大号、默认和小号头像。";
    public const string ApiPropertyIcon = "使用图标作为头像内容。";
    public const string ApiPropertySrc = "加载 SVG 图片作为头像内容。";
    public const string ApiPropertyText = "展示文本内容，并在文本宽于头像时自动缩放。";
    public const string ApiPropertyGap = "控制文本自动缩放时左右两侧的间距。";
    public const string ApiPropertyMaxDisplayCount = "限制 AvatarGroup 中可见头像数量，并将剩余头像折叠到溢出头像中。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameAvatarToken = "Avatar 和 AvatarGroup 使用的组件 Token 映射。";
    public const string TokenNameContainerSize = "从共享控件高度派生的默认头像容器尺寸。";
    public const string TokenNameGroupSpace = "头像分组时使用的间距和重叠值。";
    public const string TokenNameAvatarColor = "占位背景上使用的默认头像前景色。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenStatusMapped = "映射";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "提供三种尺寸和两种形状。";
    public const string TypeTitle = "类型";
    public const string TypeDescription = "支持图片、图标和字母类型，后两种头像可自定义颜色和背景色。";
    public const string AutoSetFontSizeTitle = "自动设置字号";
    public const string AutoSetFontSizeDescription = "对于字母类型头像，当字母过长无法展示时，字号会根据头像宽度自动调整。也可以使用 gap 设置左右两侧的单位距离。";
    public const string AvatarGroupTitle = "头像组";
    public const string AvatarGroupDescription = "头像组展示。";
    public const string P2ContentU = "U";
    public const string P2ContentUser = "用户";
    public const string P2ContentChangeuser = "切换用户";
    public const string P2ContentChangegap = "切换间距";
    public const string P2ContentK = "K";
    public const string P2ContentA = "A";

    protected override Type GetResourceKindType() => typeof(AvatarShowCaseLangResourceKind);
}
