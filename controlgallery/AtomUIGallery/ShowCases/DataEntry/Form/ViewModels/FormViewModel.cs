using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Collections;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Form;

public class FormViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Form";
    
    public IScreen HostScreen { get; }
    
    public string? UrlPathSegment => ID.ToString();
    
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
    
    private SizeType _formSizeType = SizeType.Middle;

    public SizeType FormSizeType
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
}
