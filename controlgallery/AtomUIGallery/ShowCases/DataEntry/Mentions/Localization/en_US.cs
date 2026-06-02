using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.en_US, MentionsShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest use.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Mentions, there are four variants: outlined filled borderless and underlined.";
    public const string AsynchronousLoadingTitle = "Asynchronous loading";
    public const string AsynchronousLoadingDescription = "async.";
    public const string CustomizeTriggerTokenTitle = "Customize Trigger Token";
    public const string CustomizeTriggerTokenDescription = "Customize Trigger Token by prefix props. Default to @, array also supported.";
    public const string DisabledOrReadOnlyTitle = "disabled or readOnly";
    public const string DisabledOrReadOnlyDescription = "Configure disabled and readOnly.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Change the suggestions placement.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Mentions with status, which could be error or warning.";
    public const string AutoSizeTitle = "autoSize";
    public const string AutoSizeDescription = "Height autoSize.";
    public const string WithClearIconTitle = "With clear icon";
    public const string WithClearIconDescription = "Customize clear button.";

    protected override Type GetResourceKindType() => typeof(MentionsShowCaseLangResourceKind);
}
