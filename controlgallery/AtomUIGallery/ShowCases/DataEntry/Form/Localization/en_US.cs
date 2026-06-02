using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Form;

[LanguageProvider(LanguageCode.en_US, FormShowCase.LanguageId)]
internal class en_US : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(FormShowCaseLangResourceKind);
}
