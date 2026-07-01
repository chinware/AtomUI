using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.zh_CN, BreadcrumbShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "展示当前页面在导航层级中的位置。";
    public const string PageDescription = "Breadcrumb 帮助用户理解当前位置，并沿父级路径返回。它支持图标、自定义分隔符、路由上下文、URI 导航和条目模板。";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertySeparator = "渲染在面包屑项之间的默认分隔符。";
    public const string ApiPropertySeparatorTemplate = "用于渲染面包屑分隔符的模板。";
    public const string ApiPropertyIsMotionEnabled = "启用或禁用面包屑项的动效过渡。";
    public const string ApiPropertyNavigateRequest = "点击带导航上下文的面包屑项时触发。";
    public const string ApiPropertyIcon = "显示在面包屑项内容前的图标。";
    public const string ApiPropertyNavigateContext = "通过 NavigateRequest 传递的自定义导航数据。";
    public const string ApiPropertyNavigateUri = "点击面包屑项时打开的 URI。";
    public const string ApiPropertyItemSeparator = "单个面包屑项的分隔符覆盖值。";
    public const string ApiPropertyItemSeparatorTemplate = "单个面包屑项的分隔符模板覆盖值。";
    public const string ApiPropertyItemDataContent = "从数据生成面包屑项时使用的内容值。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameIconSize = "面包屑项中的图标尺寸。";
    public const string TokenNameItemColor = "普通面包屑项的文本颜色。";
    public const string TokenNameLastItemColor = "最后一个面包屑项的文本颜色。";
    public const string TokenNameLinkColor = "可导航面包屑链接的文本颜色。";
    public const string TokenNameLinkHoverColor = "可导航项 hover 时的文本颜色。";
    public const string TokenNameLinkHoverBgColor = "可导航项 hover 时的背景颜色。";
    public const string TokenNameBreadcrumbItemContentPadding = "每个面包屑项内容区域的内间距。";
    public const string TokenNameSeparatorColor = "面包屑分隔符颜色。";
    public const string TokenNameSeparatorMargin = "面包屑分隔符外间距。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "最简单的用法。";
    public const string WithIconTitle = "带图标";
    public const string WithIconDescription = "图标应放在文本前面。";
    public const string WithParamsTitle = "带参数";
    public const string WithParamsDescription = "带有路由参数的用法。";
    public const string ConfiguringSeparatorTitle = "配置分隔符";
    public const string ConfiguringSeparatorDescription = "可以通过设置 separator 属性来自定义分隔符：separator 等于 >";
    public const string ConfiguringSeparatorIndependentlyTitle = "单独配置分隔符";
    public const string ConfiguringSeparatorIndependentlyDescription = "为每一项单独自定义分隔符。";
    public const string GenerateByTemplateTitle = "通过模板生成 BreadcrumbItem";
    public const string GenerateByTemplateDescription = "通过模板生成 BreadcrumbItem。";
    public const string P2ContentHome = "Home";
    public const string P2ContentApplicationCenter = "Application Center";
    public const string P2ContentApplicationList = "Application List";
    public const string P2ContentAnApplication = "An Application";
    public const string P2ContentApplication = "Application";
    public const string P2ContentUsers = "Users";
    public const string P2ContentParam = "Param";
    public const string P2ContentLocation = "Location";

    protected override Type GetResourceKindType() => typeof(BreadcrumbShowCaseLangResourceKind);
}
