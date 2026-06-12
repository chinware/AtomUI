using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Overview;

[LanguageProvider(LanguageCode.en_US, OverviewPage.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string InstallTitle = "Install AtomUI";
    public const string InstallDescription = "Add the desktop controls package to an Avalonia application.";
    public const string DotNetCliTab = ".NET CLI";
    public const string PackageReferenceTab = "PackageReference";
    public const string PackageManagerTab = "Package Manager";
    public const string Copy = "Copy";
    public const string LearningLinksTitle = "Keep learning";
    public const string LearningLinksDescription = "After installing the package, continue with the manual or the API reference.";
    public const string UserManualTitle = "User Manual";
    public const string UserManualDescription = "Learn the project concepts, getting started flow, and practical development guidance.";
    public const string ApiDocsTitle = "API Documentation";
    public const string ApiDocsDescription = "Look up AtomUI OSS types, members, and versioned API details.";
    public const string OpenLearningLink = "Open";

    protected override Type GetResourceKindType() => typeof(OverviewPageLangResourceKind);
}
