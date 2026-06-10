using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Form;

[LanguageProvider(LanguageCode.en_US, FormShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Basic Form data control. Includes layout, initial values, validation and submit.";
    public const string FormMethodsTitle = "Form methods";
    public const string FormMethodsDescription = "Call form method with Form.";
    public const string CustomizedFormControlsTitle = "Customized Form Controls";
    public const string CustomizedFormControlsDescription = "Customized or third-party form controls can be used in Form, too. Controls must implement IFormItemAware:";
    public const string OtherFormControlsTitle = "Other Form Controls";
    public const string OtherFormControlsDescription = "Demonstration of validation configuration for form controls which are not shown in the demos above.";
    public const string DynamicFormItemTitle = "Dynamic Form Item";
    public const string DynamicFormItemDescription = "Add or remove form items dynamically. add function support config initial value.";
    public const string FormLayoutTitle = "Form Layout";
    public const string FormLayoutDescription = "There are three layout for form: horizontal, vertical, inline.";
    public const string LabelCanWrapTitle = "label can wrap";
    public const string LabelCanWrapDescription = "Turn on labelWrap to wrap label if text is long.";
    public const string InlineLoginFormTitle = "Inline Login Form";
    public const string InlineLoginFormDescription = "Inline login form is often used in navigation bar.";
    public const string LoginFormTitle = "Login Form";
    public const string LoginFormDescription = "Normal login form which can contain more elements.";
    public const string RegistrationTitle = "Registration";
    public const string RegistrationDescription = "Fill in this form to create a new account for you.";
    public const string FormVariantsTitle = "Form variants";
    public const string FormVariantsDescription = "Change the variant of all components in the form, options include: outlined filled borderless and underlined.";
    public const string RequiredStyleTitle = "Required style";
    public const string RequiredStyleDescription = "Switch required or optional style with requiredMark.";
    public const string NoBlockRuleTitle = "No block rule";
    public const string NoBlockRuleDescription = "rule with warningOnly will not block form submit.";
    public const string ValidateTriggerTitle = "Validate Trigger";
    public const string ValidateTriggerDescription = "For the async validation scenario, high frequency of verification will cause backend pressure. You can change the verification timing through validateTrigger, or change the verification frequency through validateDebounce, or set the verification short circuit through validateFirst.";
    public const string ValidateOnlyTitle = "Validate Only";
    public const string ValidateOnlyDescription = "Dynamic adjust submit button's disabled status by validateOnly of validateFields.";
    public const string TimeRelatedControlsTitle = "Time-related Controls";
    public const string TimeRelatedControlsDescription = "The value of time-related components is a dayjs object, which we need to pre-process it before we submit to server.";
    public const string CustomizedValidationTitle = "Customized Validation";
    public const string CustomizedValidationDescription = "We provide properties like validateStatus help hasFeedback to customize your own validate status and message, without using Form.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioLayout = "Layout";
    public const string ScenarioStates = "States";
    public const string ScenarioValidation = "Validation";
    public const string ScenarioDynamic = "Dynamic";
    public const string ScenarioPresets = "Presets";
    public const string ScenarioControls = "Controls";
    public const string P2PlaceholderTextSelectAOptionAndChangeInputTextAbove = "Select a option and change input text above";
    public const string P2HeaderMale = "male";
    public const string P2HeaderFemale = "female";
    public const string P2HeaderOther = "other";
    public const string P2TextChina = "China";
    public const string P2PlaceholderTextPleaseSelectACountry = "Please select a country";
    public const string P2HeaderChina = "China";
    public const string P2HeaderUSA = "U.S.A";
    public const string P2PlaceholderTextPleaseSelectFavouriteColors = "Please select favourite colors";
    public const string P2HeaderRed = "Red";
    public const string P2HeaderGreen = "Green";
    public const string P2HeaderBlue = "Blue";
    public const string P2PlaceholderTextUsername = "Username";
    public const string P2PlaceholderTextPassword = "Password";
    public const string P2TooltipWhatDoYouWantOthersToCallYou = "What do you want others to call you?";
    public const string P2HeaderZhejiang = "Zhejiang";
    public const string P2HeaderHangzhou = "Hangzhou";
    public const string P2HeaderWestLake = "West Lake";
    public const string P2HeaderJiangsu = "Jiangsu";
    public const string P2HeaderNanjing = "Nanjing";
    public const string P2HeaderZhongHuaMen = "Zhong Hua Men";
    public const string P2PlaceholderTextWebsite = "website";
    public const string P2HeaderMale2 = "Male";
    public const string P2HeaderFemale2 = "Female";
    public const string P2HeaderOther2 = "Other";
    public const string P2HeaderDemo = "Demo";
    public const string P2HeaderLight = "Light";
    public const string P2HeaderBamboo = "Bamboo";
    public const string P2PlaceholderTextSelectDate = "Select date";
    public const string P2PlaceholderTextStartDate = "Start date";
    public const string P2SecondaryPlaceholderTextEndDate = "End date";
    public const string P2HeaderParentN1 = "Parent 1";
    public const string P2HeaderChildN1N1 = "Child 1-1";
    public const string P2HeaderGrandchildN1N1N1 = "Grandchild 1-1-1";
    public const string P2HeaderChildN1N2 = "Child 1-2";
    public const string P2HeaderParentN2 = "Parent 2";
    public const string P2HeaderChildN2N1 = "Child 2-1";
    public const string P2PlaceholderTextPleaseSelect = "Please select";
    public const string P2TooltipThisIsARequiredField = "This is a required field";
    public const string P2PlaceholderTextInputPlaceholder = "input placeholder";
    public const string P2TooltipTooltipWithCustomizeIcon = "Tooltip with customize icon";
    public const string P2HeaderBbb = "BBB";
    public const string P2HeaderAaa = "AAA";
    public const string P2PlaceholderTextValidateRequiredOnblur = "Validate required onBlur";
    public const string P2PlaceholderTextValidateRequiredDebounceAfterN1s = "Validate required debounce after 1s";
    public const string P2PlaceholderTextValidateOneByOne = "Validate one by one";
    public const string P2PlaceholderTextSelectDate2 = "Select date!";
    public const string P2PlaceholderTextStartTime = "Start time";
    public const string P2PlaceholderTextUnavailableChoice = "unavailable choice";
    public const string P2PlaceholderTextWarning = "warning";
    public const string P2PlaceholderTextIMTheContentIsBeingValidated = "I'm the content is being validated";
    public const string P2PlaceholderTextIMTheContent = "I'm the content";
    public const string P2PlaceholderTextSelectTime = "Select time";
    public const string P2PlaceholderTextIMSelect = "I'm Select";
    public const string P2HeaderOptionN1 = "Option 1";
    public const string P2HeaderOptionN2 = "Option 2";
    public const string P2HeaderOptionN3 = "Option 3";
    public const string P2HeaderXx = "xx";
    public const string P2PlaceholderTextIMTreeselect = "I'm TreeSelect";
    public const string P2PlaceholderTextWithAllowclear = "with allowClear";
    public const string P2PlaceholderTextWithInputPassword = "with input password";
    public const string P2PlaceholderTextWithInputPasswordAndAllowclear = "with input password and allowClear";
    public const string P2ContentRememberMe = "Remember me";
    public const string P2ContentItemN1 = "item 1";
    public const string P2ContentItemN2 = "item 2";
    public const string P2ContentItemN3 = "item 3";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";
    public const string P2ContentE = "E";
    public const string P2ContentF = "F";
    public const string P2ContentClickToUpload = "Click to Upload";
    public const string P2ContentAddField = "Add field";
    public const string P2ContentAddFieldAtHead = "Add field at head";
    public const string P2TextFormLayout = "Form Layout :";
    public const string P2ContentHorizontal = "Horizontal";
    public const string P2ContentVertical = "Vertical";
    public const string P2ContentInline = "Inline";
    public const string P2ContentForgotPassword = "Forgot password";
    public const string P2TextOr = "Or";
    public const string P2ContentRegisterNow = "Register now!";
    public const string P2TextIHaveReadThe = "I have read the";
    public const string P2ContentAgreement = "agreement";
    public const string P2ContentFormDisabled = "Form disabled";
    public const string P2ContentCheckbox = "Checkbox";
    public const string P2ContentApple = "Apple";
    public const string P2ContentPear = "Pear";
    public const string P2TextUpload = "Upload";
    public const string P2ContentButton = "Button";
    public const string P2TextFormVariant = "Form variant:";
    public const string P2ContentOutlined = "outlined";
    public const string P2ContentFilled = "filled";
    public const string P2ContentBorderless = "borderless";
    public const string P2ContentUnderlined = "underlined";
    public const string P2TextRequiredMark = "Required Mark :";
    public const string P2ContentDefault = "Default";
    public const string P2ContentOptional = "Optional";
    public const string P2ContentHidden = "Hidden";
    public const string P2ContentCustomize = "Customize";
    public const string P2ContentRequired = "required";
    public const string P2ContentOptional2 = "optional";
    public const string P2ContentSmall = "Small";
    public const string P2ContentMiddle = "Middle";
    public const string P2ContentLarge = "Large";
    public const string P2ContentFill = "Fill";

    public const string P2LabelTextUrl = "Url";

    public const string P2MessagePleaseEnterURL = "Please enter URL!";

    public const string P2MessageURLIsNotAValidUrl = "URL is not a valid url!";

    public const string P2MessageURLMustBeAtLeastN6Characters = "URL must be at least 6 characters!";

    public const string P2LabelTextFieldA = "Field A";

    public const string P2MessageFieldAMustBeUpToN3Characters = "Field A must be up to 3 characters!";

    public const string P2LabelTextFieldB = "FieldB";

    public const string P2LabelTextFieldC = "FieldC";

    public const string P2MessageFieldAMustBeUpToN6Characters = "Field A must be up to 6 characters!";

    public const string P2MessageContinueInputToExceedN6Chars = "Continue input to exceed 6 chars!";

    public const string P2LabelTextName = "Name";

    public const string P2MessagePleaseEnterName = "Please enter Name!";

    public const string P2LabelTextAge = "Age";

    public const string P2MessagePleaseEnterAge = "Please enter Age!";

    public const string P2LabelTextDatePicker = "DatePicker";

    public const string P2MessagePleaseSelectTime = "Please select time!";
    public const string P2HelpShouldBeCombinationOfNumbersAndAlphabets = "Should be combination of numbers and alphabets";
    public const string P2HelpTheInformationIsBeingValidated = "The information is being validated";
    public const string P2HelpSomethingBreaksTheRule = "Something breaks the rule.";
    public const string P2HelpNeedToBeChecked = "Need to be checked.";
    public const string P2HelpShouldHaveSomething = "Should have something";
    public const string P2HelpLongUpload = "Long upload help text";

    public const string P2LabelTextDatePickerShowTime = "DatePicker[showTime]";

    public const string P2LabelTextRangePicker = "RangePicker";

    public const string P2LabelTextRangePickerShowTime = "RangePicker[showTime]";

    public const string P2LabelTextTimePicker = "TimePicker";

    public const string P2LabelTextFail = "Fail";

    public const string P2LabelTextWarning = "Warning";

    public const string P2LabelTextValidating = "Validating";

    public const string P2LabelTextSuccess = "Success";

    public const string P2LabelTextError = "Error";

    public const string P2LabelTextHorizontal = "horizontal";

    public const string P2LabelTextVertical = "vertical";

    public const string P2LabelTextVertical2 = "vertical2";

    public const string P2LabelTextNormalLabel = "Normal label";

    public const string P2MessagePleaseInputYourUsername = "Please input your username!";

    public const string P2LabelTextASuperLongLabelText = "A super long label text";

    public const string P2LabelTextPassengers = "Passengers";

    public const string P2MessagePleaseInputPassengerSNameOrDeleteThisField = "Please input passenger's name or delete this field!";

    public const string P2LabelTextPassengers1 = "Passengers1";

    public const string P2LabelTextCheckbox = "Checkbox";

    public const string P2LabelTextRadio = "Radio";

    public const string P2LabelTextInput = "Input";

    public const string P2LabelTextSelect = "Select";

    public const string P2LabelTextTreeSelect = "TreeSelect";

    public const string P2LabelTextCascader = "Cascader";

    public const string P2LabelTextInputNumber = "InputNumber";

    public const string P2LabelTextTextArea = "TextArea";

    public const string P2LabelTextSwitch = "Switch";

    public const string P2LabelTextUpload = "Upload";

    public const string P2LabelTextButton = "Button";

    public const string P2LabelTextSlider = "Slider";

    public const string P2LabelTextColorPicker = "ColorPicker";

    public const string P2LabelTextRate = "Rate";

    public const string P2LabelTextMentions = "Mentions";

    public const string P2LabelTextTree = "Tree";

    public const string P2MessagePleaseInput = "Please input!";

    public const string P2LabelTextUsername = "Username";

    public const string P2LabelTextPassword = "Password";

    public const string P2MessagePleaseInputYourPassword = "Please input your password!";

    public const string P2LabelTextNote = "Note";

    public const string P2MessagePleaseEnterNote = "Please enter Note";

    public const string P2LabelTextGender = "Gender";

    public const string P2MessagePleaseEnterGender = "Please enter Gender";

    public const string P2LabelTextCustomizeGender = "Customize Gender";

    public const string P2MessagePleaseEnterCustomizeGender = "Please enter Customize Gender";

    public const string P2LabelTextPrice = "Price";

    public const string P2MessagePriceMustBeGreaterThanZero = "Price must be greater than zero!";

    public const string P2LabelTextPlainText = "Plain Text";

    public const string P2MessagePleaseSelectYourCountry = "Please select your country!";

    public const string P2LabelTextSelectMultiple = "Select[multiple]";

    public const string P2MessagePleaseSelectYourFavouriteColors = "Please select your favourite colors!";

    public const string P2LabelTextRadioGroup = "radio-group";

    public const string P2LabelTextOptionButtonGroup = "option-button-group";

    public const string P2LabelTextCheckboxGroup = "Checkbox.Group";

    public const string P2LabelTextDragger = "Dragger";

    public const string P2MessageColorIsRequired = "color is required!";

    public const string P2ContentLogIn = "Log in";

    public const string P2LabelTextEmail = "Email";

    public const string P2MessagePleaseInputYourEMail = "Please input your E-mail!";

    public const string P2LabelTextConfirmPassword = "Confirm Password";

    public const string P2MessagePleaseConfirmYourPassword = "Please confirm your password!";

    public const string P2LabelTextNickname = "Nickname";

    public const string P2MessagePleaseInputYourNickname = "Please input your nickname!";

    public const string P2LabelTextHabitualResidence = "Habitual Residence";

    public const string P2MessagePleaseSelectYourHabitualResidence = "Please select your habitual residence!";

    public const string P2LabelTextPhoneNumber = "Phone Number";

    public const string P2MessagePleaseInputYourPhoneNumber = "Please input your phone number!";

    public const string P2LabelTextDonation = "Donation";

    public const string P2MessagePleaseInputDonationAmount = "Please input donation amount!";

    public const string P2LabelTextWebsite = "Website";

    public const string P2MessagePleaseInputWebsite = "Please input website!";

    public const string P2LabelTextIntro = "Intro";

    public const string P2MessagePleaseInputIntro = "Please input Intro!";

    public const string P2MessagePleaseSelectGender = "Please select gender!";

    public const string P2LabelTextCaptcha = "Captcha";

    public const string P2MessagePleaseInputTheCaptchaYouGot = "Please input the captcha you got!";

    public const string P2ContentRegister = "Register";

    public const string P3DynamicPassengerLabelFormat = "passengers_{0}";

    public const string P3SubmitSuccessMessage = "Submit success!";

    public const string P3SubmitFailedMessage = "Submit failed!";

    public const string P3GenderNoteFormat = "Hi, {0}!";

    public const string P3GetCaptchaButtonText = "Get captcha";

    public const string P3CurrencyRmbHeader = "RMB";

    public const string P3CurrencyDollarHeader = "Dollar";

    protected override Type GetResourceKindType() => typeof(FormShowCaseLangResourceKind);
}
