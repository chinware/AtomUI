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
    public const string LastNodeAndReversingDescription = "When the timeline is incomplete and ongoing, put a pending node at the end. Set Pending to a truthy value to display the pending item. You can customize the pending content and pending icon. IsReverse is used for reversing nodes.";
    public const string AlternateTitle = "Alternate";
    public const string AlternateDescription = "Alternate timeline.";
    public const string LabelTitle = "Label";
    public const string LabelDescription = "Use label show time alone.";
    public const string RightAlternateTitle = "Right alternate";
    public const string RightAlternateDescription = "Right alternate timeline.";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiated = "2024-01-01 AtomUI Officially Initiated";
    public const string P2ContentN2024N08N12AfterMoreThanN7Months = "2024-08-12 After more than 7 months of development, AtomUI is officially open-source. Welcome everyone to follow us.";
    public const string P2ContentN2024N10N01ReleaseOfTheN0N0 = "2024-10-01 Release of the 0.0.1 Preview Version";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN1 = "2024-01-01 AtomUI Officially Initiated. 1";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN2 = "2024-01-01 AtomUI Officially Initiated. 2";
    public const string P2ContentN2024N01N01AtomuiOfficiallyInitiatedN3 = "2024-01-01 AtomUI Officially Initiated. 3";
    public const string P2ContentToggleReverse = "Toggle Reverse";
    public const string P2ContentRecording = "Recording...";
    public const string P2ContentLeft = "Left";
    public const string P2ContentRight = "Right";
    public const string P2ContentAlternate = "Alternate";
    public const string P2ContentAtomuiOfficiallyInitiated = "AtomUI Officially Initiated";
    public const string P2ContentCreateAServicesSite = "Create a services site";
    public const string P2ContentQinwareWebsiteOnline = "Qinware website online";
    public const string P2ContentNetworkProblemsBeingSolved = "Network problems being solved";

    protected override Type GetResourceKindType() => typeof(TimelineShowCaseLangResourceKind);
}
