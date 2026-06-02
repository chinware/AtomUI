using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.en_US, BreadcrumbShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "The simplest use.";
    public const string WithIconTitle = "With an Icon";
    public const string WithIconDescription = "The icon should be placed in front of the text.";
    public const string WithParamsTitle = "With Params";
    public const string WithParamsDescription = "With route params.";
    public const string ConfiguringSeparatorTitle = "Configuring the Separator";
    public const string ConfiguringSeparatorIndependentlyTitle = "Configuring the Separator Independently";
    public const string ConfiguringSeparatorIndependentlyDescription = "Customize separator for each other.";
    public const string GenerateByTemplateTitle = "Generate BreadcrumbItem by template";
    public const string GenerateByTemplateDescription = "Generate BreadcrumbItem by template.";

    protected override Type GetResourceKindType() => typeof(BreadcrumbShowCaseLangResourceKind);
}
