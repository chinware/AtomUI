using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Timeline;

[LanguageProvider(LanguageCode.en_US, TimelineShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Basic usage example.";
    public const string ColorTitle = "Color";
    public const string ColorDescription = "Set the color of circles. green means completed or success status, red means warning or error, and blue means ongoing or other default status, gray for unfinished or disabled status.";
    public const string LastNodeAndReversingTitle = "Last node and Reversing";
    public const string LastNodeAndReversingDescription = "When the timeline is incomplete and ongoing, put a ghost node at last. Set pending as truthy value to enable displaying pending item. You can customize the pending content by passing a React Element. Meanwhile, pendingDot={a React Element} is used to customize the dot of the pending item. reverse={true} is used for reversing nodes.";
    public const string AlternateTitle = "Alternate";
    public const string AlternateDescription = "Alternate timeline.";
    public const string LabelTitle = "Label";
    public const string LabelDescription = "Use label show time alone.";
    public const string RightAlternateTitle = "Right alternate";
    public const string RightAlternateDescription = "Right alternate timeline.";

    protected override Type GetResourceKindType() => typeof(TimelineShowCaseLangResourceKind);
}
