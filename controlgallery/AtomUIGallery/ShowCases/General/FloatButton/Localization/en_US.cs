using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.FloatButton;

[LanguageProvider(LanguageCode.en_US, FloatButtonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string TypeTitle = "Type";
    public const string TypeDescription = "Change the type of the FloatButton with the type property.";
    public const string ShapeTitle = "Shape";
    public const string ShapeDescription = "Change the shape of the FloatButton with the shape property.";
    public const string TooltipTitle = "FloatButton with tooltip";
    public const string TooltipDescription = "Setting the tooltip property shows the FloatButton with a tooltip.";
    public const string DescriptionTitle = "Description";
    public const string DescriptionDescription = "Setting the description property allows you to show a FloatButton with a description.";
    public const string GroupTitle = "FloatButton Group";
    public const string GroupDescription = "When multiple buttons are used together, FloatButtonGroup is recommended. By setting the shape property of FloatButton.Group, you can change the group shape. The FloatButton.Group shape overrides the shape of FloatButtons inside.";
    public const string MenuModeTitle = "Menu mode";
    public const string MenuModeDescription = "Open menu mode with trigger, which could be hover or click.";
    public const string ControlledModeTitle = "Controlled mode";
    public const string ControlledModeDescription = "Set the component to controlled mode through open, which needs to be used together with trigger.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Customize animation placement. Four preset placements are provided: top, right, bottom, and left. The default placement is top.";
    public const string BadgeTitle = "Badge";
    public const string BadgeDescription = "FloatButton with Badge.";
    public const string BackTopTitle = "BackTop";
    public const string BackTopDescription = "BackTop makes it easy to go back to the top of the page.";

    protected override Type GetResourceKindType() => typeof(FloatButtonShowCaseLangResourceKind);
}
