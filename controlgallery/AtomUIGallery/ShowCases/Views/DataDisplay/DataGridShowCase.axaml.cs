using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DataGridShowCase : ReactiveUserControl<DataGridViewModel>
{
    public DataGridShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is DataGridViewModel viewModel)
            {
                InitBasicShowCaseDataSource(viewModel);
                InitFilterAndSorterDataSource(viewModel);
                InitMultiSorterDataSource(viewModel);
                InitExpandableRowDataSource(viewModel);
                InitGroupDataDataSource(viewModel);
                InitFixedHeaderDataSource(viewModel);
                InitFixedColumnsDataSource(viewModel);
                InitFixedColumnsAndHeadersDataSource(viewModel);
                InitDragColumnDataSource(viewModel);
                InitDragRowDataSource(viewModel);
                InitCustomEmptyDataSource(viewModel);
                InitEditableCellsDataSource(viewModel);
                InitEditableRowsDataSource(viewModel);
                InitPagingGridDataSource(viewModel);
                
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.BasicCaseGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.SelectionDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FilterAndSorterDataSource, v => v.FilterAndSortGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FilterAndSorterDataSource, v => v.FilterInTreeGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.MultiSorterDataSource, v => v.MultiSorterDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.ResetFilterAndSortGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.DragResizeColumn.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.LargeSizeDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.MiddleSizeDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.SmallSizeDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.CustomHeaderAndFooterDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.ExpandableRowDataSource, v => v.ExpandableDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.ExpandableRowDataSource, v => v.OrderSpecificColumnDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.ExpandableRowDataSource, v => v.RowAndColumnHeaderDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.GroupHeaderDataSource, v => v.GroupHeaderDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicCaseDataSource, v => v.HideColumnDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FixedHeaderDataSource, v => v.FixedHeaderDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FixedColumnsDataSource, v => v.FixedColumnsDataGrid1.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FixedColumnsDataSource, v => v.FixedColumnsDataGrid2.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FixedColumnsAndHeadersDataSource, v => v.FixedColumnsAndHeadersDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DragColumnDataSource, v => v.DragColumnDataGrid1.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DragColumnDataSource, v => v.DragColumnDataGrid2.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DragColumnDataSource, v => v.DragColumnDataGrid3.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DragRowDataSource, v => v.DragRowDataGrid1.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DragRowManyDataSource, v => v.DragRowDataGrid2.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.CustomEmptyDataSource, v => v.CustomEmptyDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.EditableCellsDataSource, v => v.EditableCellsDataGrid.ItemsSource).DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.PagingGridDataSource, v => v.BasicPagingCaseGrid.ItemsSource).DisposeWith(disposables);
                ExtendedSelection.IsCheckedChanged += SelectionModeCheckedChanged;
                SingleSelection.IsCheckedChanged   += SelectionModeCheckedChanged;
            
                SortAgeBtn.Click                += HandleSortAgeBtnClick;
                ClearFiltersBtn.Click           += HandleClearFiltersBtnClick;
                ClearFiltersAndSortersBtn.Click += HandleClearFiltersAndSortersBtnClick;
                ColumnCheckBox1.IsChecked       =  true;
                ColumnCheckBox2.IsChecked       =  true;
                ColumnCheckBox3.IsChecked       =  true;
                ColumnCheckBox4.IsChecked       =  true;
                ColumnCheckBox5.IsChecked       =  true;
                ColumnCheckBox6.IsChecked       =  true;
            
                ColumnCheckBox1.IsCheckedChanged += HandleColumnVisibleChanged;
                ColumnCheckBox2.IsCheckedChanged += HandleColumnVisibleChanged;
                ColumnCheckBox3.IsCheckedChanged += HandleColumnVisibleChanged;
                ColumnCheckBox4.IsCheckedChanged += HandleColumnVisibleChanged;
                ColumnCheckBox5.IsCheckedChanged += HandleColumnVisibleChanged;
                ColumnCheckBox6.IsCheckedChanged += HandleColumnVisibleChanged;
            
                ShowTopPaginationCheckBox.IsCheckedChanged       += HandleShowTopPaginationCheckBoxChanged;
                ShowBottomPaginationCheckBox.IsCheckedChanged    += HandleShowBottomPaginationCheckBoxChanged;
                TopPaginationOptionGroup.OptionCheckedChanged    += HandleTopPaginationAlignChanged;
                BottomPaginationOptionGroup.OptionCheckedChanged += HandleBottomPaginationAlignChanged;
                
                Disposable.Create(() =>
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
                              
                              ExtendedSelection.IsCheckedChanged -= SelectionModeCheckedChanged;
                              SingleSelection.IsCheckedChanged   -= SelectionModeCheckedChanged;
            
                              SortAgeBtn.Click                -= HandleSortAgeBtnClick;
                              ClearFiltersBtn.Click           -= HandleClearFiltersBtnClick;
                              ClearFiltersAndSortersBtn.Click -= HandleClearFiltersAndSortersBtnClick;
                              
                              ColumnCheckBox1.IsCheckedChanged -= HandleColumnVisibleChanged;
                              ColumnCheckBox2.IsCheckedChanged -= HandleColumnVisibleChanged;
                              ColumnCheckBox3.IsCheckedChanged -= HandleColumnVisibleChanged;
                              ColumnCheckBox4.IsCheckedChanged -= HandleColumnVisibleChanged;
                              ColumnCheckBox5.IsCheckedChanged -= HandleColumnVisibleChanged;
                              ColumnCheckBox6.IsCheckedChanged -= HandleColumnVisibleChanged;
            
                              ShowTopPaginationCheckBox.IsCheckedChanged       -= HandleShowTopPaginationCheckBoxChanged;
                              ShowBottomPaginationCheckBox.IsCheckedChanged    -= HandleShowBottomPaginationCheckBoxChanged;
                              TopPaginationOptionGroup.OptionCheckedChanged    -= HandleTopPaginationAlignChanged;
                              BottomPaginationOptionGroup.OptionCheckedChanged -= HandleBottomPaginationAlignChanged;
                          })
                          .DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
    
    private void HandleTopPaginationAlignChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            BasicPagingCaseGrid.TopPaginationAlign = PaginationAlign.Start;
        }
        else if (args.Index == 1)
        {
            BasicPagingCaseGrid.TopPaginationAlign = PaginationAlign.Center;
        }
        else
        {
            BasicPagingCaseGrid.TopPaginationAlign = PaginationAlign.End;
        }
    }
    
    private void HandleBottomPaginationAlignChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            BasicPagingCaseGrid.BottomPaginationAlign = PaginationAlign.Start;
        }
        else if (args.Index == 1)
        {
            BasicPagingCaseGrid.BottomPaginationAlign = PaginationAlign.Center;
        }
        else
        {
            BasicPagingCaseGrid.BottomPaginationAlign = PaginationAlign.End;
        }
    }
    
    private void HandleShowTopPaginationCheckBoxChanged(object? sender, RoutedEventArgs args)
    {
        if (ShowTopPaginationCheckBox.IsChecked == true)
        {
            BasicPagingCaseGrid.PaginationVisibility |= DataGridPaginationVisibility.Top;
        }
        else
        {
            BasicPagingCaseGrid.PaginationVisibility &= ~DataGridPaginationVisibility.Top;
        }
    }
    
    private void HandleShowBottomPaginationCheckBoxChanged(object? sender, RoutedEventArgs args)
    {
        if (ShowBottomPaginationCheckBox.IsChecked == true)
        {
            BasicPagingCaseGrid.PaginationVisibility |= DataGridPaginationVisibility.Bottom;
        }
        else
        {
            BasicPagingCaseGrid.PaginationVisibility &= ~DataGridPaginationVisibility.Bottom;
        }
    }
    
    private void SelectionModeCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            if (radioButton == ExtendedSelection && ExtendedSelection.IsChecked.HasValue &&
                ExtendedSelection.IsChecked.Value)
            {
                SelectionDataGrid.SelectionMode = DataGridSelectionMode.Extended;
            }
            else if (radioButton == SingleSelection && SingleSelection.IsChecked.HasValue &&
                     SingleSelection.IsChecked.Value)
            {
                SelectionDataGrid.SelectionMode = DataGridSelectionMode.Single;
            }
        }
    }
    
    private void HandleColumnVisibleChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            var columns     = HideColumnDataGrid.Columns;
            var name        = checkBox.Name;
            var columnIndex = -1;
            if (name == "ColumnCheckBox1")
            {
                columnIndex = 0;
            }
            else if (name == "ColumnCheckBox2")
            {
                columnIndex = 1;
            }
            else if (name == "ColumnCheckBox3")
            {
                columnIndex = 2;
            }
            else if (name == "ColumnCheckBox4")
            {
                columnIndex = 3;
            }
            else if (name == "ColumnCheckBox5")
            {
                columnIndex = 4;
            }
            else if (name == "ColumnCheckBox6")
            {
                columnIndex = 5;
            }
            
            if (columnIndex != -1)
            {
                var column = columns[columnIndex];
                column.IsVisible = checkBox.IsChecked == true;
            }
        }
    }
    
    private void HandleSortAgeBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.Sort(1, ListSortDirection.Descending);
    }
    
    private void HandleClearFiltersBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.ClearFilters();
    }
    
    private void HandleClearFiltersAndSortersBtnClick(object? sender, RoutedEventArgs? eventArgs)
    {
        ResetFilterAndSortGrid.ClearFilters();
        ResetFilterAndSortGrid.ClearSort();
    }
    
    private void HandleToggleEmptyGridItemsSource(object? sender, RoutedEventArgs? eventArgs)
    {
        if (CustomEmptyDataGrid.ItemsSource != null)
        {
            CustomEmptyDataGrid.ItemsSource = null;
        }
        else
        {
            if (DataContext is DataGridViewModel viewModel)
            {
                CustomEmptyDataGrid.ItemsSource = viewModel.CustomEmptyDataSource;
            }
        }
    }
    
    private void HandleToggleLoadingState(object? sender, RoutedEventArgs? eventArgs)
    {
        CustomEmptyDataGrid.IsOperating = !CustomEmptyDataGrid.IsOperating;
    }
    
    private static int CellsEditableNewRowIndex = 1;
    
    private void HandleAddARowToCellsEditableGrid(object? sender, RoutedEventArgs? eventArgs)
    {
        if (DataContext is DataGridViewModel viewModel)
        {
            viewModel.EditableCellsDataSource?.Add(new DataGridBaseInfo()
            {
                Address = $"London, Park Lane no. {CellsEditableNewRowIndex}",
                Name    = $"Edward King {CellsEditableNewRowIndex}",
                Age     = 32
            });
            CellsEditableNewRowIndex++;
        }
    }
    
    private void HandleRemoveRowCellsEditableGrid(object? sender, RoutedEventArgs? eventArgs)
    {
        if (sender is PopupConfirm popupConfirm)
        {
            if (popupConfirm.DataContext is int index)
            {
                EditableCellsDataGrid.CollectionView?.RemoveAt(index);
            }
        }
    }
    
    private void InitBasicShowCaseDataSource(DataGridViewModel viewModel)
    {
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

    private void InitFilterAndSorterDataSource(DataGridViewModel viewModel)
    {
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

    private void InitMultiSorterDataSource(DataGridViewModel viewModel)
    {
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

    private void InitExpandableRowDataSource(DataGridViewModel viewModel)
    {
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

    public void InitGroupDataDataSource(DataGridViewModel viewModel)
    {
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

    private void InitFixedHeaderDataSource(DataGridViewModel viewModel)
    {
        viewModel.FixedHeaderDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items.Add(new DataGridBaseInfo()
            {
                Key     = i.ToString(),
                Name    = $"Edward King {i}",
                Age     = 32,
                Address = $"London No. 1 Lake Park {i}",
            });
        }

        viewModel.FixedHeaderDataSource.AddRange(items);
    }

    private void InitFixedColumnsDataSource(DataGridViewModel viewModel)
    {
        viewModel.FixedColumnsDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>()
        {
            new DataGridBaseInfo { Key = "1", Name = "John Brown", Age = 32, Address = "New York No. 1 Lake Park" },
            new DataGridBaseInfo { Key = "2", Name = "Jim Green", Age  = 42, Address = "London No. 1 Lake Park" },
        };
        viewModel.FixedColumnsDataSource.AddRange(items);
    }

    private void InitFixedColumnsAndHeadersDataSource(DataGridViewModel viewModel)
    {
        viewModel.FixedColumnsAndHeadersDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items.Add(new DataGridBaseInfo()
            {
                Key     = i.ToString(),
                Name    = $"Edward King {i}",
                Age     = 32,
                Address = $"London No. 1 Lake Park {i}",
            });
        }

        viewModel.FixedColumnsAndHeadersDataSource.AddRange(items);
    }

    private void InitDragColumnDataSource(DataGridViewModel viewModel)
    {
        viewModel.DragColumnDataSource = new();
        List<DragColumnDataType> items = new List<DragColumnDataType>();
        items.Add(new DragColumnDataType()
        {
            Name    = "John Brown",
            Gender  = "male",
            Age     = 32,
            Email   = "John Brown@example.com",
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DragColumnDataType()
        {
            Name    = "Jim Green",
            Gender  = "female",
            Age     = 42,
            Email   = "jimGreen@example.com",
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DragColumnDataType()
        {
            Name    = "Joe Black",
            Gender  = "female",
            Age     = 32,
            Email   = "JoeBlack@example.com",
            Address = "Sidney No. 1 Lake Park"
        });
        items.Add(new DragColumnDataType()
        {
            Name    = "George Hcc",
            Gender  = "male",
            Age     = 20,
            Email   = "george@example.com",
            Address = "Sidney No. 1 Lake Park"
        });
        viewModel.DragColumnDataSource.AddRange(items);
    }

    private void InitDragRowDataSource(DataGridViewModel viewModel)
    {
        viewModel.DragRowDataSource     = new();
        viewModel.DragRowManyDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        items.Add(new DataGridBaseInfo()
        {
            Name    = "John Brown",
            Age     = 32,
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Jim Green",
            Age     = 42,
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Joe Black",
            Age     = 32,
            Address = "Sidney No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "George Hcc",
            Age     = 20,
            Address = "Sidney No. 1 Lake Park"
        });
        
        viewModel.DragRowDataSource.AddRange(items);

        var items1 = new List<DataGridBaseInfo>();
        for (var i = 0; i < 30; i++)
        {
            items1.Add(new DataGridBaseInfo()
            {
                Name    = "John Brown",
                Age     = 32,
                Address = $"London No. {i + 1} Lake Park"
            });
        }
        viewModel.DragRowManyDataSource.AddRange(items1);
    }

    private void InitCustomEmptyDataSource(DataGridViewModel viewModel)
    {
        viewModel.CustomEmptyDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        items.Add(new DataGridBaseInfo()
        {
            Name    = "John Brown",
            Age     = 32,
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Jim Green",
            Age     = 42,
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Joe Black",
            Age     = 32,
            Address = "Sidney No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "George Hcc",
            Age     = 18,
            Address = "Sidney No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Joe Black",
            Age     = 32,
            Address = "Sidney No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "George Hcc",
            Age     = 44,
            Address = "Sidney No. 2 Lake Park"
        });
        viewModel.CustomEmptyDataSource.AddRange(items);
    }

    private void InitEditableCellsDataSource(DataGridViewModel viewModel)
    {
        viewModel.EditableCellsDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        items.Add(new DataGridBaseInfo()
        {
            Name    = "John Brown",
            Age     = 32,
            Address = "London No. 1 Lake Park"
        });
        items.Add(new DataGridBaseInfo()
        {
            Name    = "Jim Green",
            Age     = 42,
            Address = "London No. 3 Lake Park"
        });
        viewModel.EditableCellsDataSource.AddRange(items);
    }

    private void InitEditableRowsDataSource(DataGridViewModel viewModel)
    {
        viewModel.EditableRowsDataSource = new();
        List<DataGridBaseInfo> items = new List<DataGridBaseInfo>();
        for (int i = 0; i < 30; i++)
        {
            items.Add(new  DataGridBaseInfo
            {
                Name    = $"Edward {i + 1}",
                Age    = 32,
                Address = $"London Park no. {i + 1}"
            });
        }
        viewModel.EditableRowsDataSource.AddRange(items);
    }
    
    private void InitPagingGridDataSource(DataGridViewModel viewModel)
    {
        viewModel.PagingGridDataSource = new();
        viewModel.PagingGridDataSource.AddRange(RandomDataGenerator.GenerateRandomData(100));
    }
}

static class RandomDataGenerator
{
    private static readonly Random Random = new Random();
    
    private static readonly string[] FirstNames = { "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda", "William", "Elizabeth" };
    private static readonly string[] LastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
    private static readonly string[] Cities = { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
    private static readonly string[] Streets = { "Main St", "Park Ave", "Elm St", "Oak St", "Pine Rd", "Maple Ave", "Cedar Ln", "Washington Blvd", "Lake Shore Dr", "Sunset Blvd" };
    private static readonly string[] TagNames = { "VIP", "NEW", "STAFF", "MANAGER", "DEVELOPER", "DESIGNER", "ANALYST", "LEADER", "EXPERT", "TRAINEE", "VIP", "SPECIAL" };
    private static readonly string[] Colors = { "red", "volcano", "orange", "gold", "yellow", "lime", "green", "cyan", "blue", "geekblue", "purple" };

    public static List<DataGridBaseInfo> GenerateRandomData(int count = 10)
    {
        return Enumerable.Range(1, count).Select(i => new DataGridBaseInfo
        {
            Key = i.ToString(),
            Name = $"{FirstNames[Random.Next(FirstNames.Length)]} {LastNames[Random.Next(LastNames.Length)]}",
            Age = Random.Next(18, 66),
            Address = $"{Random.Next(1, 10000)} {Streets[Random.Next(Streets.Length)]}, {Cities[Random.Next(Cities.Length)]}",
            Money = $"￥{Random.Next(1000, 10000000):N2}",
            Tags = GenerateRandomTags(Random.Next(1, 4))
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