using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using DynamicData;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridShowCase : GalleryReactiveUserControl<DataGridViewModel>
{
    public const string LanguageId = nameof(DataGridShowCase);

    private const string BasicScenario       = "Basic";
    private const string InteractionScenario = "Interaction";
    private const string FilteringScenario   = "Filtering";
    private const string StructureScenario   = "Structure";
    private const string FixedScenario       = "Fixed";
    private const string DragScenario        = "Drag";
    private const string EditingScenario     = "Editing";
    private const string PagingScenario      = "Paging";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public DataGridShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearScenarioContent();
        if (DataContext is DataGridViewModel viewModel)
        {
            DataGridShowCaseDataSources.ClearAll(viewModel);
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private void ClearScenarioContent()
    {
        foreach (var item in ScenarioTabs.Items.OfType<AtomUI.Desktop.Controls.TabItem>())
        {
            item.Content = null;
        }
        _scenarioCache.Clear();
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario       => new DataGridBasicShowCase(),
            InteractionScenario => new DataGridInteractionShowCase(),
            FilteringScenario   => new DataGridFilteringShowCase(),
            StructureScenario   => new DataGridStructureShowCase(),
            FixedScenario       => new DataGridFixedShowCase(),
            DragScenario        => new DataGridDragShowCase(),
            EditingScenario     => new DataGridEditingShowCase(),
            PagingScenario      => new DataGridPagingShowCase(),
            _                   => throw new InvalidOperationException($"Unknown DataGrid scenario: {scenario}")
        };
    }
}

public abstract class DataGridScenarioShowCase : GalleryReactiveUserControl<DataGridViewModel>
{
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is DataGridViewModel viewModel)
        {
            InitializeScenario(viewModel);
        }
    }

    protected abstract void InitializeScenario(DataGridViewModel viewModel);
}

internal static class DataGridShowCaseDataSources
{
    public static void ClearAll(DataGridViewModel viewModel)
    {
        viewModel.BasicCaseDataSource              = null;
        viewModel.FilterAndSorterDataSource        = null;
        viewModel.MultiSorterDataSource            = null;
        viewModel.ExpandableRowDataSource          = null;
        viewModel.GroupHeaderDataSource            = null;
        viewModel.FixedHeaderDataSource            = null;
        viewModel.FixedColumnsDataSource           = null;
        viewModel.FixedColumnsAndHeadersDataSource = null;
        viewModel.DragColumnDataSource             = null;
        viewModel.DragRowDataSource                = null;
        viewModel.DragRowManyDataSource            = null;
        viewModel.CustomEmptyDataSource            = null;
        viewModel.EditableCellsDataSource          = null;
        viewModel.EditableRowsDataSource           = null;
        viewModel.PagingGridDataSource             = null;
    }

    public static void EnsureBasicDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.BasicCaseDataSource is not null)
        {
            return;
        }

        viewModel.BasicCaseDataSource = new();
        List<DataGridBaseInfo> items =
        [
            new DataGridBaseInfo
            {
                Key   = "1", Name = "John Brown", Age = 32, Address = "New York No. 1 Lake Park",
                Money = "￥300,000.00",
                Tags =
                [
                    new TagInfo { Name = "NICE", Color      = "green" },
                    new TagInfo { Name = "DEVELOPER", Color = "geekblue" }
                ]
            },
            new DataGridBaseInfo
            {
                Key   = "2", Name = "Jim Green", Age = 42, Address = "London No. 1 Lake Park",
                Money = "￥1,256,000.00",
                Tags =
                [
                    new TagInfo { Name = "LOSER", Color = "volcano" }
                ]
            },
            new DataGridBaseInfo
            {
                Key   = "3", Name = "Joe Black", Age = 32, Address = "Sydney No. 1 Lake Park",
                Money = "￥120,000.00",
                Tags =
                [
                    new TagInfo { Name = "COOL", Color    = "green" },
                    new TagInfo { Name = "TEACHER", Color = "geekblue" }
                ]
            }
        ];
        viewModel.BasicCaseDataSource.AddRange(items);
    }

    public static void EnsureFilterAndSorterDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.FilterAndSorterDataSource is not null)
        {
            return;
        }

        viewModel.FilterAndSorterDataSource = new();
        List<DataGridBaseInfo> items =
        [
            new DataGridBaseInfo { Key = "1", Name = "John Brown", Age = 32, Address = "New York No. 1 Lake Park" },
            new DataGridBaseInfo { Key = "2", Name = "Jim Green", Age  = 42, Address = "London No. 1 Lake Park" },
            new DataGridBaseInfo { Key = "3", Name = "Joe Black", Age  = 32, Address = "Sydney No. 1 Lake Park" },
            new DataGridBaseInfo { Key = "4", Name = "Joe Red", Age    = 32, Address = "London No. 2 Lake Park" }
        ];
        viewModel.FilterAndSorterDataSource.AddRange(items);
    }

    public static void EnsureMultiSorterDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.MultiSorterDataSource is not null)
        {
            return;
        }

        viewModel.MultiSorterDataSource = new();
        List<MultiSorterDataType> items =
        [
            new MultiSorterDataType { Key = "1", Name = "John Brown", Chinese = 98, Math = 60, English = 70 },
            new MultiSorterDataType { Key = "2", Name = "Jim Green", Chinese  = 98, Math = 66, English = 89 },
            new MultiSorterDataType { Key = "3", Name = "Joe Black", Chinese  = 98, Math = 90, English = 70 },
            new MultiSorterDataType { Key = "3", Name = "Jim Red", Chinese    = 88, Math = 99, English = 89 },
        ];
        viewModel.MultiSorterDataSource.AddRange(items);
    }

    public static void EnsureExpandableRowDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.ExpandableRowDataSource is not null)
        {
            return;
        }

        viewModel.ExpandableRowDataSource = new();
        List<ExpandableRowDataType> items =
        [
            new ExpandableRowDataType
            {
                Key         = "1", Name = "John Brown", Age = 32, Address = "New York No. 1 Lake Park",
                Description = "My name is John Brown, I am 32 years old, living in New York No. 1 Lake Park."
            },
            new ExpandableRowDataType
            {
                Key         = "2", Name = "Jim Green", Age = 42, Address = "London No. 1 Lake Park",
                Description = "London No. 1 Lake Park"
            },
            new ExpandableRowDataType
            {
                Key         = "3", Name = "Joe Black", Age = 32, Address = "Sydney No. 1 Lake Park",
                Description = "My name is Joe Black, I am 32 years old, living in Sydney No. 1 Lake Park."
            },
            new ExpandableRowDataType
            {
                Key         = "5", Name = "Joe Red", Age = 78, Address = "London No. 2 Lake Park",
                Description = "My name is Joe Black, I am 78 years old, London No. 2 Lake Park"
            }
        ];
        viewModel.ExpandableRowDataSource.AddRange(items);
    }

    public static void EnsureGroupHeaderDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.GroupHeaderDataSource is not null)
        {
            return;
        }

        viewModel.GroupHeaderDataSource = new();
        var items = new List<GroupHeaderDataType>();
        for (var i = 0; i < 6; i++)
        {
            items.Add(new GroupHeaderDataType
            {
                Key            = i.ToString(),
                Name           = "John Brown",
                Age            = i + 1,
                Street         = "Lake Park",
                Building       = "C",
                Number         = 2035,
                CompanyAddress = "Lake Street 42",
                CompanyName    = "SoftLake Co",
                Gender         = "M"
            });
        }

        viewModel.GroupHeaderDataSource.AddRange(items);
    }

    public static void EnsureFixedHeaderDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.FixedHeaderDataSource is not null)
        {
            return;
        }

        viewModel.FixedHeaderDataSource = new();
        var items = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items.Add(new DataGridBaseInfo
            {
                Key     = i.ToString(),
                Name    = $"Edward King {i}",
                Age     = 32,
                Address = $"London No. 1 Lake Park {i}",
            });
        }

        viewModel.FixedHeaderDataSource.AddRange(items);
    }

    public static void EnsureFixedColumnsDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.FixedColumnsDataSource is not null)
        {
            return;
        }

        viewModel.FixedColumnsDataSource = new();
        List<DataGridBaseInfo> items =
        [
            new DataGridBaseInfo { Key = "1", Name = "John Brown", Age = 32, Address = "New York No. 1 Lake Park" },
            new DataGridBaseInfo { Key = "2", Name = "Jim Green", Age  = 42, Address = "London No. 1 Lake Park" },
        ];
        viewModel.FixedColumnsDataSource.AddRange(items);
    }

    public static void EnsureFixedColumnsAndHeadersDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.FixedColumnsAndHeadersDataSource is not null)
        {
            return;
        }

        viewModel.FixedColumnsAndHeadersDataSource = new();
        var items = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items.Add(new DataGridBaseInfo
            {
                Key     = i.ToString(),
                Name    = $"Edward King {i}",
                Age     = 32,
                Address = $"London No. 1 Lake Park {i}",
            });
        }

        viewModel.FixedColumnsAndHeadersDataSource.AddRange(items);
    }

    public static void EnsureDragColumnDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.DragColumnDataSource is not null)
        {
            return;
        }

        viewModel.DragColumnDataSource = new();
        var items = new List<DragColumnDataType>
        {
            new()
            {
                Name    = "John Brown",
                Gender  = "male",
                Age     = 32,
                Email   = "John Brown@example.com",
                Address = "London No. 1 Lake Park"
            },
            new()
            {
                Name    = "Jim Green",
                Gender  = "female",
                Age     = 42,
                Email   = "jimGreen@example.com",
                Address = "London No. 1 Lake Park"
            },
            new()
            {
                Name    = "Joe Black",
                Gender  = "female",
                Age     = 32,
                Email   = "JoeBlack@example.com",
                Address = "Sidney No. 1 Lake Park"
            },
            new()
            {
                Name    = "George Hcc",
                Gender  = "male",
                Age     = 20,
                Email   = "george@example.com",
                Address = "Sidney No. 1 Lake Park"
            }
        };
        viewModel.DragColumnDataSource.AddRange(items);
    }

    public static void EnsureDragRowDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.DragRowDataSource is null)
        {
            viewModel.DragRowDataSource = new();
            List<DataGridBaseInfo> items =
            [
                new DataGridBaseInfo { Name = "John Brown", Age  = 32, Address = "London No. 1 Lake Park" },
                new DataGridBaseInfo { Name = "Jim Green", Age   = 42, Address = "London No. 1 Lake Park" },
                new DataGridBaseInfo { Name = "Joe Black", Age   = 32, Address = "Sidney No. 1 Lake Park" },
                new DataGridBaseInfo { Name = "George Hcc", Age  = 20, Address = "Sidney No. 1 Lake Park" }
            ];
            viewModel.DragRowDataSource.AddRange(items);
        }

        if (viewModel.DragRowManyDataSource is not null)
        {
            return;
        }

        viewModel.DragRowManyDataSource = new();
        var manyItems = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            manyItems.Add(new DataGridBaseInfo
            {
                Name    = "John Brown",
                Age     = 32,
                Address = $"London No. {i + 1} Lake Park"
            });
        }
        viewModel.DragRowManyDataSource.AddRange(manyItems);
    }

    public static void EnsureCustomEmptyDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.CustomEmptyDataSource is not null)
        {
            return;
        }

        viewModel.CustomEmptyDataSource = new();
        List<DataGridBaseInfo> items =
        [
            new DataGridBaseInfo { Name = "John Brown", Age = 32, Address = "London No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "Jim Green", Age  = 42, Address = "London No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "Joe Black", Age  = 32, Address = "Sidney No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "George Hcc", Age = 18, Address = "Sidney No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "Joe Black", Age  = 32, Address = "Sidney No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "George Hcc", Age = 44, Address = "Sidney No. 2 Lake Park" }
        ];
        viewModel.CustomEmptyDataSource.AddRange(items);
    }

    public static void EnsureEditableCellsDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.EditableCellsDataSource is not null)
        {
            return;
        }

        viewModel.EditableCellsDataSource = new();
        List<DataGridBaseInfo> items =
        [
            new DataGridBaseInfo { Name = "John Brown", Age = 32, Address = "London No. 1 Lake Park" },
            new DataGridBaseInfo { Name = "Jim Green", Age  = 42, Address = "London No. 3 Lake Park" }
        ];
        viewModel.EditableCellsDataSource.AddRange(items);
    }

    public static void EnsureEditableRowsDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.EditableRowsDataSource is not null)
        {
            return;
        }

        viewModel.EditableRowsDataSource = new();
        var items = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items.Add(new DataGridBaseInfo
            {
                Name    = $"Edward {i + 1}",
                Age     = 32,
                Address = $"London Park no. {i + 1}"
            });
        }
        viewModel.EditableRowsDataSource.AddRange(items);
    }

    public static void EnsurePagingGridDataSource(DataGridViewModel viewModel)
    {
        if (viewModel.PagingGridDataSource is not null)
        {
            return;
        }

        viewModel.PagingGridDataSource = new();
        viewModel.PagingGridDataSource.AddRange(RandomDataGenerator.GenerateRandomData(100));
    }
}

internal static class RandomDataGenerator
{
    private static readonly Random Random = new();

    private static readonly string[] FirstNames = ["James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth"];
    private static readonly string[] LastNames = ["Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez"];
    private static readonly string[] Cities = ["New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose"];
    private static readonly string[] Streets = ["Main St", "Park Ave", "Elm St", "Oak St", "Pine Rd", "Maple Ave", "Cedar Ln", "Washington Blvd", "Lake Shore Dr", "Sunset Blvd"];
    private static readonly string[] TagNames = ["VIP", "NEW", "STAFF", "MANAGER", "DEVELOPER", "DESIGNER", "ANALYST", "LEADER", "EXPERT", "TRAINEE", "VIP", "SPECIAL"];
    private static readonly string[] Colors = ["red", "volcano", "orange", "gold", "yellow", "lime", "green", "cyan", "blue", "geekblue", "purple"];

    public static List<DataGridBaseInfo> GenerateRandomData(int count = 10)
    {
        return Enumerable.Range(1, count).Select(i => new DataGridBaseInfo
        {
            Key     = i.ToString(),
            Name    = $"{FirstNames[Random.Next(FirstNames.Length)]} {LastNames[Random.Next(LastNames.Length)]}",
            Age     = Random.Next(18, 66),
            Address = $"{Random.Next(1, 10000)} {Streets[Random.Next(Streets.Length)]}, {Cities[Random.Next(Cities.Length)]}",
            Money   = $"￥{Random.Next(1000, 10000000):N2}",
            Tags    = GenerateRandomTags(Random.Next(1, 4))
        }).ToList();
    }

    private static List<TagInfo> GenerateRandomTags(int count)
    {
        return Enumerable.Range(0, count).Select(_ => new TagInfo
        {
            Name  = TagNames[Random.Next(TagNames.Length)],
            Color = Colors[Random.Next(Colors.Length)]
        }).ToList();
    }
}
