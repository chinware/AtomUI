using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.RadioButton;

[LanguageProvider(LanguageCode.en_US, RadioButtonShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioGroups = "Groups";
    public const string ScenarioOptions = "Options";
    public const string ScenarioStyles = "Styles";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest use.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled state of RadioButton.";
    public const string RadioGroupTitle = "Radio Group";
    public const string RadioGroupDescription = "A group of radio components.";
    public const string VerticalRadioGroupTitle = "Vertical Radio Group";
    public const string VerticalRadioGroupDescription = "Vertical Radio Group.";
    public const string ItemsSourceRadioGroupTitle = "Radio Group by Items source";
    public const string ItemsSourceRadioGroupDescription = "Radio Group.";
    public const string OptionButtonTitle = "Option Button";
    public const string OptionButtonDescription = "OptionButton Group.";
    public const string OptionButtonWithIconTitle = "Option Button with icon";
    public const string OptionButtonWithIconDescription = "OptionButton Group with icon.";
    public const string OptionStyleTitle = "option style";
    public const string OptionStyleDescription = "The combination of option button style.";
    public const string SolidOptionButtonTitle = "Solid option button";
    public const string SolidOptionButtonDescription = "Solid option button style.";
    public const string SizeTypeTitle = "Size type";
    public const string SizeTypeDescription = "There are three sizes available: large, medium, and small. It can coordinate with input box.";
    public const string P2TitleSizetype = "SizeType";
    public const string P2ContentRadio = "Radio";
    public const string P2ContentRadio1 = "Radio1";
    public const string P2ContentRadio2 = "Radio2";
    public const string P2ContentToggleDisabled = "toggle disabled";
    public const string P2TextLinechart = "LineChart";
    public const string P2TextDotchart = "DotChart";
    public const string P2TextBarchart = "BarChart";
    public const string P2TextPiechart = "PieChart";
    public const string P2ContentOptionA = "Option A";
    public const string P2ContentOptionB = "Option B";
    public const string P2ContentOptionC = "Option C";
    public const string P2ContentOptionD = "Option D";
    public const string P2ContentApple = "Apple";
    public const string P2ContentPear = "Pear";
    public const string P2ContentOrange = "Orange";
    public const string P2ContentMacos = "macOS";
    public const string P2ContentLinux = "Linux";
    public const string P2ContentWindows = "Windows";
    public const string P2ContentHangzhou = "Hangzhou";
    public const string P2ContentShanghai = "Shanghai";
    public const string P2ContentBeijing = "Beijing";
    public const string P2ContentChengdu = "Chengdu";

    protected override Type GetResourceKindType() => typeof(RadioButtonShowCaseLangResourceKind);
}
