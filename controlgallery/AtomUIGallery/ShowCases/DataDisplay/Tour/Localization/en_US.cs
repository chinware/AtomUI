using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tour;

[LanguageProvider(LanguageCode.en_US, TourShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string NonModalTitle = "Non-modal";
    public const string NonModalDescription = "Use mask={false} to make Tour non-modal. At the meantime it is recommended to use with type=primary to emphasize the guide itself.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Change the placement of the guide relative to the target, there are 12 placements available. When target={null} the guide will show in the center.";
    public const string CustomIndicatorTitle = "Custom indicator";
    public const string CustomIndicatorDescription = "Custom indicator.";
    public const string CustomMaskTitle = "Custom mask";
    public const string CustomMaskDescription = "Custom mask color.";
    public const string CustomActionTitle = "Custom action";
    public const string CustomActionDescription = "Custom action.";
    public const string CustomHighlightedAreaStyleTitle = "Custom highlighted area style";
    public const string CustomHighlightedAreaStyleDescription = "Using gap to control the radius of highlight area and the offset between highlight area and the element.";
    public const string P2TitleUploadFile = "Upload File";
    public const string P2DescriptionPutYourFilesHere = "Put your files here.";
    public const string P2TitleSave = "Save";
    public const string P2DescriptionSaveYourChanges = "Save your changes.";
    public const string P2TitleOtherActions = "Other Actions";
    public const string P2DescriptionClickToSeeOtherActions = "Click to see other actions.";
    public const string P2TitleCenter = "Center";
    public const string P2DescriptionDisplayedInTheCenterOfScreen = "Displayed in the center of screen.";
    public const string P2TitleRight = "Right";
    public const string P2DescriptionOnTheRightOfTarget = "On the right of target.";
    public const string P2TitleTop = "Top";
    public const string P2DescriptionOnTheTopOfTarget = "On the top of target.";
    public const string P2TitleLeft = "Left";
    public const string P2DescriptionOnTheLeftOfTarget = "On the left of target.";
    public const string P2ContentBeginTour = "Begin Tour";
    public const string P2ContentUpload = "Upload";
    public const string P2ContentSave = "Save";
    public const string P2ContentBeginNonModalTour = "Begin non-modal Tour";
    public const string P2ContentSkip = "Skip";
    public const string P2TextRadius = "Radius:";
    public const string P2TextHorizontalOffset = "Horizontal offset:";
    public const string P2TextVerticalOffset = "Vertical offset:";

    protected override Type GetResourceKindType() => typeof(TourShowCaseLangResourceKind);
}
