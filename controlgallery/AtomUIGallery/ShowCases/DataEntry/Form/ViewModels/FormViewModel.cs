using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Form;

public class FormViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Form";
    
    public IScreen HostScreen { get; }
    
    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<FormApiRow>? _apiRows;

    public ObservableCollection<FormApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    private ObservableCollection<FormDesignTokenRow>? _designTokenRows;

    public ObservableCollection<FormDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }
    
    private FormLayout _formLayout = FormLayout.Horizontal;

    public FormLayout FormLayout
    {
        get => _formLayout;
        set => this.RaiseAndSetIfChanged(ref _formLayout, value);
    }

    private bool _isFormDisabled = true;

    public bool IsFormDisabled
    {
        get => _isFormDisabled;
        set => this.RaiseAndSetIfChanged(ref _isFormDisabled, value);
    }
    
    private InputControlStyleVariant _formStyleVariant = InputControlStyleVariant.Outlined;

    public InputControlStyleVariant FormStyleVariant
    {
        get => _formStyleVariant;
        set => this.RaiseAndSetIfChanged(ref _formStyleVariant, value);
    }
    
    private FormRequiredMark _formRequiredMark = FormRequiredMark.Default;

    public FormRequiredMark FormRequiredMark
    {
        get => _formRequiredMark;
        set => this.RaiseAndSetIfChanged(ref _formRequiredMark, value);
    }
    
    private CustomizableSizeType _formSizeType = CustomizableSizeType.Middle;

    public CustomizableSizeType FormSizeType
    {
        get => _formSizeType;
        set => this.RaiseAndSetIfChanged(ref _formSizeType, value);
    }
    
    private List<SliderMark>? _sliderMarks;

    public List<SliderMark>? SliderMarks
    {
        get => _sliderMarks;
        set => this.RaiseAndSetIfChanged(ref _sliderMarks, value);
    }
    
    private IFormValues? _basicFormInitialValues;

    public IFormValues? BasicFormInitialValues
    {
        get => _basicFormInitialValues;
        set => this.RaiseAndSetIfChanged(ref _basicFormInitialValues, value);
    }

    private List<ISelectOption>? _genderOptions;

    public List<ISelectOption>? GenderOptions
    {
        get => _genderOptions;
        set => this.RaiseAndSetIfChanged(ref _genderOptions, value);
    }

    private List<ISelectOption>? _presetGenderOptions;

    public List<ISelectOption>? PresetGenderOptions
    {
        get => _presetGenderOptions;
        set => this.RaiseAndSetIfChanged(ref _presetGenderOptions, value);
    }

    private List<ISelectOption>? _countryOptions;

    public List<ISelectOption>? CountryOptions
    {
        get => _countryOptions;
        set => this.RaiseAndSetIfChanged(ref _countryOptions, value);
    }

    private List<ISelectOption>? _colorOptions;

    public List<ISelectOption>? ColorOptions
    {
        get => _colorOptions;
        set => this.RaiseAndSetIfChanged(ref _colorOptions, value);
    }

    private List<ISelectOption>? _demoSelectOptions;

    public List<ISelectOption>? DemoSelectOptions
    {
        get => _demoSelectOptions;
        set => this.RaiseAndSetIfChanged(ref _demoSelectOptions, value);
    }

    private List<ISelectOption>? _requiredStyleSelectOptions;

    public List<ISelectOption>? RequiredStyleSelectOptions
    {
        get => _requiredStyleSelectOptions;
        set => this.RaiseAndSetIfChanged(ref _requiredStyleSelectOptions, value);
    }

    private List<ISelectOption>? _validationSelectOptions;

    public List<ISelectOption>? ValidationSelectOptions
    {
        get => _validationSelectOptions;
        set => this.RaiseAndSetIfChanged(ref _validationSelectOptions, value);
    }

    private List<ICascaderOption>? _presetCascaderOptions;

    public List<ICascaderOption>? PresetCascaderOptions
    {
        get => _presetCascaderOptions;
        set => this.RaiseAndSetIfChanged(ref _presetCascaderOptions, value);
    }

    private List<ICascaderOption>? _demoCascaderOptions;

    public List<ICascaderOption>? DemoCascaderOptions
    {
        get => _demoCascaderOptions;
        set => this.RaiseAndSetIfChanged(ref _demoCascaderOptions, value);
    }

    private List<ICascaderOption>? _validationCascaderOptions;

    public List<ICascaderOption>? ValidationCascaderOptions
    {
        get => _validationCascaderOptions;
        set => this.RaiseAndSetIfChanged(ref _validationCascaderOptions, value);
    }

    private List<ITreeItemNode>? _demoTreeNodes;

    public List<ITreeItemNode>? DemoTreeNodes
    {
        get => _demoTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _demoTreeNodes, value);
    }

    private List<ITreeItemNode>? _validationTreeNodes;

    public List<ITreeItemNode>? ValidationTreeNodes
    {
        get => _validationTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _validationTreeNodes, value);
    }

    public FormViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new FormApiRow("FormLayout", Lang(FormShowCaseLangResourceKind.ApiPropertyFormLayout), "FormLayout", "purple", "Horizontal"),
            new FormApiRow("LabelColInfo", Lang(FormShowCaseLangResourceKind.ApiPropertyLabelColInfo), "GridLength", "cyan", "null"),
            new FormApiRow("WrapperColInfo", Lang(FormShowCaseLangResourceKind.ApiPropertyWrapperColInfo), "GridLength", "cyan", "null"),
            new FormApiRow("RequiredMark", Lang(FormShowCaseLangResourceKind.ApiPropertyRequiredMark), "FormRequiredMark", "purple", "Default"),
            new FormApiRow("ValidateTrigger", Lang(FormShowCaseLangResourceKind.ApiPropertyValidateTrigger), "FormValidateTrigger", "purple", "OnChanged"),
            new FormApiRow("IsValidateFeedbackEnabled", Lang(FormShowCaseLangResourceKind.ApiPropertyIsValidateFeedbackEnabled), "bool", "green", "false"),
            new FormApiRow("InitialValues", Lang(FormShowCaseLangResourceKind.ApiPropertyInitialValues), "IFormValues?", "cyan", "null"),
            new FormApiRow("Values", Lang(FormShowCaseLangResourceKind.ApiPropertyValues), "IFormValues", "cyan", "-"),
            new FormApiRow("FormItem.FieldName", Lang(FormShowCaseLangResourceKind.ApiPropertyFormItemFieldName), "string?", "cyan", "null"),
            new FormApiRow("FormItem.Validators", Lang(FormShowCaseLangResourceKind.ApiPropertyFormItemValidators), "IList<IFormValidator>", "cyan", "[]"),
            new FormApiRow("FormItem.ValidateTrigger", Lang(FormShowCaseLangResourceKind.ApiPropertyFormItemValidateTrigger), "FormValidateTrigger?", "purple", "null"),
            new FormApiRow("FormItem.Help", Lang(FormShowCaseLangResourceKind.ApiPropertyFormItemHelp), "object?", "cyan", "null"),
            new FormApiRow("Submitted", Lang(FormShowCaseLangResourceKind.ApiEventSubmitted), "event", "gold", "-"),
            new FormApiRow("Validated", Lang(FormShowCaseLangResourceKind.ApiEventValidated), "event", "gold", "-")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new FormDesignTokenRow("LabelRequiredMarkColor", Lang(FormShowCaseLangResourceKind.TokenNameLabelRequiredMarkColor), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("LabelColor", Lang(FormShowCaseLangResourceKind.TokenNameLabelColor), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("LabelFontSize", Lang(FormShowCaseLangResourceKind.TokenNameLabelFontSize), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("LabelColonMargin", Lang(FormShowCaseLangResourceKind.TokenNameLabelColonMargin), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("FormItemSpacing", Lang(FormShowCaseLangResourceKind.TokenNameFormItemSpacing), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("InlineItemSpacing", Lang(FormShowCaseLangResourceKind.TokenNameInlineItemSpacing), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("VerticalLabelPadding", Lang(FormShowCaseLangResourceKind.TokenNameVerticalLabelPadding), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FormDesignTokenRow("VerticalLabelMargin", Lang(FormShowCaseLangResourceKind.TokenNameVerticalLabelMargin), Lang(FormShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FormShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(FormShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(FormShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            FormShowCaseLangResourceKind.ApiPropertyFormLayout                 => en_US.ApiPropertyFormLayout,
            FormShowCaseLangResourceKind.ApiPropertyLabelColInfo               => en_US.ApiPropertyLabelColInfo,
            FormShowCaseLangResourceKind.ApiPropertyWrapperColInfo             => en_US.ApiPropertyWrapperColInfo,
            FormShowCaseLangResourceKind.ApiPropertyRequiredMark               => en_US.ApiPropertyRequiredMark,
            FormShowCaseLangResourceKind.ApiPropertyValidateTrigger            => en_US.ApiPropertyValidateTrigger,
            FormShowCaseLangResourceKind.ApiPropertyIsValidateFeedbackEnabled  => en_US.ApiPropertyIsValidateFeedbackEnabled,
            FormShowCaseLangResourceKind.ApiPropertyInitialValues              => en_US.ApiPropertyInitialValues,
            FormShowCaseLangResourceKind.ApiPropertyValues                     => en_US.ApiPropertyValues,
            FormShowCaseLangResourceKind.ApiPropertyFormItemFieldName          => en_US.ApiPropertyFormItemFieldName,
            FormShowCaseLangResourceKind.ApiPropertyFormItemValidators         => en_US.ApiPropertyFormItemValidators,
            FormShowCaseLangResourceKind.ApiPropertyFormItemValidateTrigger    => en_US.ApiPropertyFormItemValidateTrigger,
            FormShowCaseLangResourceKind.ApiPropertyFormItemHelp               => en_US.ApiPropertyFormItemHelp,
            FormShowCaseLangResourceKind.ApiEventSubmitted                     => en_US.ApiEventSubmitted,
            FormShowCaseLangResourceKind.ApiEventValidated                     => en_US.ApiEventValidated,
            FormShowCaseLangResourceKind.TokenNameLabelRequiredMarkColor       => en_US.TokenNameLabelRequiredMarkColor,
            FormShowCaseLangResourceKind.TokenNameLabelColor                   => en_US.TokenNameLabelColor,
            FormShowCaseLangResourceKind.TokenNameLabelFontSize                => en_US.TokenNameLabelFontSize,
            FormShowCaseLangResourceKind.TokenNameLabelColonMargin             => en_US.TokenNameLabelColonMargin,
            FormShowCaseLangResourceKind.TokenNameFormItemSpacing              => en_US.TokenNameFormItemSpacing,
            FormShowCaseLangResourceKind.TokenNameInlineItemSpacing            => en_US.TokenNameInlineItemSpacing,
            FormShowCaseLangResourceKind.TokenNameVerticalLabelPadding         => en_US.TokenNameVerticalLabelPadding,
            FormShowCaseLangResourceKind.TokenNameVerticalLabelMargin          => en_US.TokenNameVerticalLabelMargin,
            FormShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            FormShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                  => kind.ToString()
        };
    }
}

public sealed record FormApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record FormDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
