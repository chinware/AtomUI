using System.Collections.ObjectModel;
using AtomUI.Controls;
using DynamicData;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

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

    public DataGridViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}

public class DataGridBaseInfo
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

public class MultiSorterDataType
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