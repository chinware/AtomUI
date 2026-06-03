using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.zh_CN, BreadcrumbShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
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
