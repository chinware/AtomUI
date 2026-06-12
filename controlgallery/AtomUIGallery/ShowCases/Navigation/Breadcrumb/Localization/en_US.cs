using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Breadcrumb;

[LanguageProvider(LanguageCode.en_US, BreadcrumbShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Show the current page location in a navigation hierarchy.";
    public const string PageDescription = "Breadcrumb helps users understand where they are and move back through parent levels. It supports icons, custom separators, route context, URI navigation, and item templates.";
    public const string InfoNamespaceLabel = "Namespace:";
    public const string InfoPackageLabel = "Package:";
    public const string InfoBaseClassLabel = "Base class:";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertySeparator = "Default separator rendered between breadcrumb items.";
    public const string ApiPropertySeparatorTemplate = "Template used to render breadcrumb separators.";
    public const string ApiPropertyIsMotionEnabled = "Enables or disables breadcrumb item motion transitions.";
    public const string ApiPropertyNavigateRequest = "Raised when a breadcrumb item with navigation context is clicked.";
    public const string ApiPropertyIcon = "Icon displayed before the breadcrumb item content.";
    public const string ApiPropertyNavigateContext = "Custom navigation payload passed through NavigateRequest.";
    public const string ApiPropertyNavigateUri = "URI launched when the breadcrumb item is clicked.";
    public const string ApiPropertyItemSeparator = "Separator override for an individual breadcrumb item.";
    public const string ApiPropertyItemSeparatorTemplate = "Separator template override for an individual breadcrumb item.";
    public const string ApiPropertyItemDataContent = "Content value used when generating breadcrumb items from data.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameIconSize = "Icon size used inside breadcrumb items.";
    public const string TokenNameItemColor = "Text color for normal breadcrumb items.";
    public const string TokenNameLastItemColor = "Text color for the last breadcrumb item.";
    public const string TokenNameLinkColor = "Text color for navigable breadcrumb links.";
    public const string TokenNameLinkHoverColor = "Text color used when hovering a navigable item.";
    public const string TokenNameLinkHoverBgColor = "Background color used when hovering a navigable item.";
    public const string TokenNameBreadcrumbItemContentPadding = "Padding around each breadcrumb item content region.";
    public const string TokenNameSeparatorColor = "Color used to render breadcrumb separators.";
    public const string TokenNameSeparatorMargin = "Margin around breadcrumb separators.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "The simplest use.";
    public const string WithIconTitle = "With an Icon";
    public const string WithIconDescription = "The icon should be placed in front of the text.";
    public const string WithParamsTitle = "With Params";
    public const string WithParamsDescription = "With route params.";
    public const string ConfiguringSeparatorTitle = "Configuring the Separator";
    public const string ConfiguringSeparatorDescription = "The separator can be customized by setting the separator property: separator equals >";
    public const string ConfiguringSeparatorIndependentlyTitle = "Configuring the Separator Independently";
    public const string ConfiguringSeparatorIndependentlyDescription = "Customize separator for each other.";
    public const string GenerateByTemplateTitle = "Generate BreadcrumbItem by template";
    public const string GenerateByTemplateDescription = "Generate BreadcrumbItem by template.";
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
