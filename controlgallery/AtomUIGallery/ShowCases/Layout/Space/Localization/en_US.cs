using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.en_US, SpaceShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Crowded components horizontal spacing.";
    public const string VerticalSpaceTitle = "Vertical Space";
    public const string VerticalSpaceDescription = "Crowded components vertical spacing.";
    public const string SizeTitle = "Space Size";
    public const string SizeDescription = "Use size to set the spacing. Three sizes are preset: small, middle, and large. You can also customize the spacing. If size is not set, the spacing is small.";
    public const string AlignTitle = "Align";
    public const string AlignDescription = "Config item align.";
    public const string WrapTitle = "Wrap";
    public const string WrapDescription = "Auto wrap line.";
    public const string SplitTitle = "Split";
    public const string SplitDescription = "Crowded components split.";
    public const string CompactFormTitle = "Compact Mode for form component";
    public const string CompactFormDescription = "Compact Mode for form component.";
    public const string CompactButtonTitle = "Button Compact Mode";
    public const string CompactButtonDescription = "Button component compact example.";
    public const string VerticalCompactTitle = "Vertical Compact Mode";
    public const string VerticalCompactDescription = "Vertical Mode for Space.Compact, supports Button only.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioSize = "Size";
    public const string ScenarioAlign = "Align";
    public const string ScenarioCompactForm = "Compact Form";
    public const string ScenarioCompactButton = "Compact Button";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "Are you sure to delete this task?";
    public const string P2OkTextOk = "Ok";
    public const string P2CancelTextCancel = "Cancel";
    public const string P2TitleDeleteTheTask = "Delete the task";
    public const string P2HeaderCard = "Card";
    public const string P2HeaderReport = "Report";
    public const string P2HeaderMail = "Mail";
    public const string P2HeaderMobile = "Mobile";
    public const string P2HeaderN1stItem = "1st item";
    public const string P2HeaderN2ndItem = "2nd item";
    public const string P2HeaderN3rdItem = "3rd item";
    public const string P2HeaderZhejiang = "Zhejiang";
    public const string P2HeaderJiangsu = "Jiangsu";
    public const string P2TextXihuDistrictHangzhou = "Xihu District, Hangzhou";
    public const string P2TextN1 = "+1";
    public const string P2HeaderOption1 = "Option1";
    public const string P2HeaderOption2 = "Option2";
    public const string P2TextInputContent = "input content";
    public const string P2HeaderOption1N1 = "Option1-1";
    public const string P2HeaderOption2N1 = "Option2-1";
    public const string P2HeaderOption2N2 = "Option2-2";
    public const string P2HeaderBetween = "Between";
    public const string P2HeaderExcept = "Except";
    public const string P2PlaceholderTextMinimum = "Minimum";
    public const string P2PlaceholderTextText = "~";
    public const string P2PlaceholderTextMaximum = "Maximum";
    public const string P2HeaderSignUp = "Sign Up";
    public const string P2HeaderSignIn = "Sign In";
    public const string P2PlaceholderTextEmail = "Email";
    public const string P2HeaderTextN1 = "text 1";
    public const string P2HeaderTextN2 = "text 2";
    public const string P2PlaceholderTextSelectTime = "Select Time";
    public const string P2PlaceholderTextSelectAddress = "Select Address";
    public const string P2HeaderHangzhou = "Hangzhou";
    public const string P2HeaderWestLake = "West Lake";
    public const string P2HeaderLingyinShi = "Lingyin shi";
    public const string P2HeaderNanjing = "Nanjing";
    public const string P2HeaderZhongHuaMen = "Zhong Hua Men";
    public const string P2PlaceholderTextStartTime = "Start time";
    public const string P2SecondaryPlaceholderTextEndTime = "End time";
    public const string P2PlaceholderTextPleaseSelect = "Please select";
    public const string P2HeaderParentN1 = "parent 1";
    public const string P2HeaderParentN1N0 = "parent 1-0";
    public const string P2HeaderLeaf1 = "leaf1";
    public const string P2HeaderLeaf2 = "leaf2";
    public const string P2HeaderParentN1N1 = "parent 1-1";
    public const string P2HeaderLeaf3 = "leaf3";
    public const string P2PlaceholderTextInputHere = "input here";
    public const string P2PlaceholderTextAnotherInput = "another input";
    public const string P2TextCenter = "center";
    public const string P2ContentPrimary = "Primary";
    public const string P2TextBlock = "Block";
    public const string P2ContentButton = "Button";
    public const string P2ContentLink = "Link";
    public const string P2TextSpace = "Space:";
    public const string P2ContentClickToUpload = "Click to Upload";
    public const string P2ContentConfirm = "Confirm";
    public const string P2TextCardContent = "Card content";
    public const string P2ContentButtonN1 = "Button 1";
    public const string P2ContentButtonN2 = "Button 2";
    public const string P2ContentButtonN3 = "Button 3";
    public const string P2ContentButtonN4 = "Button 4";
    public const string P2ContentSubmit = "Submit";
    public const string P2ContentSearch = "Search";
    public const string P2ContentSmall = "Small";
    public const string P2ContentMiddle = "Middle";
    public const string P2ContentLarge = "Large";
    public const string P2ContentCustom = "Custom";
    public const string P2ContentDefault = "Default";
    public const string P2ContentDashed = "Dashed";

    public const string P2ToolTipTipCopyGitUrl = "copy git url";

    public const string P2ToolTipTipLike = "Like";

    public const string P2ToolTipTipComment = "Comment";

    public const string P2ToolTipTipStar = "Star";

    public const string P2ToolTipTipHeart = "Heart";

    public const string P2ToolTipTipShare = "Share";

    public const string P2ToolTipTipDownload = "Download";

    public const string P2ToolTipTipTooltip = "Tooltip";

    protected override Type GetResourceKindType() => typeof(SpaceShowCaseLangResourceKind);
}
