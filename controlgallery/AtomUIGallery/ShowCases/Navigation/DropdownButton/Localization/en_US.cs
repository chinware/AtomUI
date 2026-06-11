using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.en_US, DropdownButtonShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic dropdown menu.";
    public const string ButtonTypesTitle = "Button Types";
    public const string ButtonTypesDescription = "Support centralized button type.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "You could display an arrow.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Support 6 placements.";
    public const string P2HeaderCut = "Cut";
    public const string P2HeaderCopy = "Copy";
    public const string P2HeaderDelete = "Delete";
    public const string P2HeaderPaste = "Paste";
    public const string P2HeaderPasteFromHistory = "Paste from History";
    public const string P2ContentHoverMe = "Hover me";
    public const string P2ContentEditFile = "Edit File";
    public const string P2ContentBottomLeft = "BottomLeft";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentBottomRight = "BottomRight";
    public const string P2ContentTopLeft = "TopLeft";
    public const string P2ContentTop = "Top";
    public const string P2ContentTopRight = "TopRight";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
