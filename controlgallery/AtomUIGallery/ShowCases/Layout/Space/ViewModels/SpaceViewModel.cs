using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Space;

public class SpaceViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "SpaceShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private CustomizableSizeType _sizeType;

    public CustomizableSizeType SizeType
    {
        get => _sizeType;
        set => this.RaiseAndSetIfChanged(ref _sizeType, value);
    }

    private double _customSpacingValue;

    public double CustomSpacingValue
    {
        get => _customSpacingValue;
        set => this.RaiseAndSetIfChanged(ref _customSpacingValue, value);
    }

    private List<ISelectOption>? _provinceOptions;

    public List<ISelectOption>? ProvinceOptions
    {
        get => _provinceOptions;
        set => this.RaiseAndSetIfChanged(ref _provinceOptions, value);
    }

    private List<ISelectOption>? _basicOptions;

    public List<ISelectOption>? BasicOptions
    {
        get => _basicOptions;
        set => this.RaiseAndSetIfChanged(ref _basicOptions, value);
    }

    private List<ISelectOption>? _firstNestedOptions;

    public List<ISelectOption>? FirstNestedOptions
    {
        get => _firstNestedOptions;
        set => this.RaiseAndSetIfChanged(ref _firstNestedOptions, value);
    }

    private List<ISelectOption>? _secondNestedOptions;

    public List<ISelectOption>? SecondNestedOptions
    {
        get => _secondNestedOptions;
        set => this.RaiseAndSetIfChanged(ref _secondNestedOptions, value);
    }

    private List<ISelectOption>? _conditionOptions;

    public List<ISelectOption>? ConditionOptions
    {
        get => _conditionOptions;
        set => this.RaiseAndSetIfChanged(ref _conditionOptions, value);
    }

    private List<ISelectOption>? _authActionOptions;

    public List<ISelectOption>? AuthActionOptions
    {
        get => _authActionOptions;
        set => this.RaiseAndSetIfChanged(ref _authActionOptions, value);
    }

    private List<IAutoCompleteOption>? _autoCompleteTextOptions;

    public List<IAutoCompleteOption>? AutoCompleteTextOptions
    {
        get => _autoCompleteTextOptions;
        set => this.RaiseAndSetIfChanged(ref _autoCompleteTextOptions, value);
    }

    private List<ICascaderOption>? _addressCascaderOptions;

    public List<ICascaderOption>? AddressCascaderOptions
    {
        get => _addressCascaderOptions;
        set => this.RaiseAndSetIfChanged(ref _addressCascaderOptions, value);
    }

    private List<ITreeItemNode>? _treeSelectNodes;

    public List<ITreeItemNode>? TreeSelectNodes
    {
        get => _treeSelectNodes;
        set => this.RaiseAndSetIfChanged(ref _treeSelectNodes, value);
    }

    public SpaceViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
