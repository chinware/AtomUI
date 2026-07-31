using AtomUIGallery.Localization;
using System.Collections;
using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "DataGrid";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public ObservableCollection<DataGridBaseInfo>? BasicCaseDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FilterAndSorterDataSource { get; set; }
    public ObservableCollection<MultiSorterDataType>? MultiSorterDataSource { get; set; }
    public ObservableCollection<ExpandableRowDataType>? ExpandableRowDataSource { get; set; }
    public ObservableCollection<GroupHeaderDataType>? GroupHeaderDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedHeaderDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedColumnsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedColumnsAndHeadersDataSource { get; set; }
    public ObservableCollection<DragColumnDataType>? DragColumnDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? DragRowDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? DragRowManyDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? CustomEmptyDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? EditableCellsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? EditableRowsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? PagingGridDataSource { get; set; }

    public ObservableCollection<DataGridFilterItem> NameFilters { get; }
    public ObservableCollection<DataGridFilterItem> AddressFilters { get; }

    private IList? _filterAndSorterSelectedNames = new ObservableCollection<object>();
    private IList? _filterAndSorterSelectedAddresses = new ObservableCollection<object>();
    private IList? _treeFilterSelectedNames = new ObservableCollection<object>();
    private IList? _treeFilterSelectedAddresses = new ObservableCollection<object>();
    private IList? _resetSelectedNames = new ObservableCollection<object>();
    private IList? _resetSelectedAddresses = new ObservableCollection<object>();

    public IList? FilterAndSorterSelectedNames
    {
        get => _filterAndSorterSelectedNames;
        set => this.RaiseAndSetIfChanged(ref _filterAndSorterSelectedNames, value);
    }

    public IList? FilterAndSorterSelectedAddresses
    {
        get => _filterAndSorterSelectedAddresses;
        set => this.RaiseAndSetIfChanged(ref _filterAndSorterSelectedAddresses, value);
    }

    public IList? TreeFilterSelectedNames
    {
        get => _treeFilterSelectedNames;
        set => this.RaiseAndSetIfChanged(ref _treeFilterSelectedNames, value);
    }

    public IList? TreeFilterSelectedAddresses
    {
        get => _treeFilterSelectedAddresses;
        set => this.RaiseAndSetIfChanged(ref _treeFilterSelectedAddresses, value);
    }

    public IList? ResetSelectedNames
    {
        get => _resetSelectedNames;
        set => this.RaiseAndSetIfChanged(ref _resetSelectedNames, value);
    }

    public IList? ResetSelectedAddresses
    {
        get => _resetSelectedAddresses;
        set => this.RaiseAndSetIfChanged(ref _resetSelectedAddresses, value);
    }

    public DataGridViewModel(IScreen screen)
    {
        HostScreen     = screen;
        NameFilters    = CreateNameFilters();
        AddressFilters = CreateAddressFilters();
    }

    private static ObservableCollection<DataGridFilterItem> CreateNameFilters()
    {
        return
        [
            new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextJoe), Value = "Joe" },
            new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextJim), Value = "Jim" },
            new DataGridFilterItem
            {
                Text  = Lang(DataGridShowCaseLangResourceKind.P2TextSubmenu),
                Value = "Submenu",
                Children =
                [
                    new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextGreen), Value = "Green" },
                    new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextBlack), Value = "Black" }
                ]
            }
        ];
    }

    private static ObservableCollection<DataGridFilterItem> CreateAddressFilters()
    {
        return
        [
            new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextLondon), Value = "London" },
            new DataGridFilterItem { Text = Lang(DataGridShowCaseLangResourceKind.P2TextNewYork), Value = "New York" }
        ];
    }

    private static string Lang(DataGridShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DataGridShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            _                                                                   => kind.ToString()
        };
    }
}

[GenerateDataMemberAccessors]
public partial class DataGridBaseInfo
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Money { get; set; } = string.Empty;
    public List<TagInfo> Tags { get; set; } = new();
}

public class TagInfo
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

[GenerateDataMemberAccessors]
public partial class MultiSorterDataType
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Chinese { get; set; }
    public int Math { get; set; }
    public int English { get; set; }
}

public class ExpandableRowDataType : DataGridBaseInfo
{
    public string Description { get; set; } = string.Empty;
}

public class GroupHeaderDataType
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Number { get; set; }
}

public class DragColumnDataType
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
