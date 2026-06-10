using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.en_US, GroupBoxShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic usage of the GroupBox control.";
    public const string HeaderPositionTitle = "Header title Position";
    public const string HeaderPositionDescription = "GroupBox Header supports three position types: Left, Center and Right.";
    public const string HeaderStyleTitle = "Header title style";
    public const string HeaderStyleDescription = "GroupBox Header supports customizing some properties of color and font.";
    public const string HeaderIconTitle = "Header Icon";
    public const string HeaderIconDescription = "GroupBox Header supports specifying Icon.";
    public const string P2HeaderTitleTitleInfo = "Title Info";
    public const string P2TextContentOfGroupBox = "Content of group box";

    protected override Type GetResourceKindType() => typeof(GroupBoxShowCaseLangResourceKind);
}
