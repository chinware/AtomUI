using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
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
                              BasicCaseGrid.ItemsSource                  = null;
                              SelectionDataGrid.ItemsSource              = null;
                              FilterAndSortGrid.ItemsSource              = null;
                              FilterInTreeGrid.ItemsSource               = null;
                              MultiSorterDataGrid.ItemsSource            = null;
                              ResetFilterAndSortGrid.ItemsSource         = null;
                              DragResizeColumn.ItemsSource               = null;
                              LargeSizeDataGrid.ItemsSource              = null;
                              MiddleSizeDataGrid.ItemsSource             = null;
                              SmallSizeDataGrid.ItemsSource              = null;
                              CustomHeaderAndFooterDataGrid.ItemsSource  = null;
                              ExpandableDataGrid.ItemsSource             = null;
                              OrderSpecificColumnDataGrid.ItemsSource    = null;
                              RowAndColumnHeaderDataGrid.ItemsSource     = null;
                              GroupHeaderDataGrid.ItemsSource            = null;
                              HideColumnDataGrid.ItemsSource             = null;
                              FixedHeaderDataGrid.ItemsSource            = null;
                              FixedColumnsDataGrid1.ItemsSource          = null;
                              FixedColumnsDataGrid2.ItemsSource          = null;
                              FixedColumnsAndHeadersDataGrid.ItemsSource = null;
                              DragColumnDataGrid1.ItemsSource            = null;
                              DragColumnDataGrid2.ItemsSource            = null;
                              DragColumnDataGrid3.ItemsSource            = null;
                              DragRowDataGrid1.ItemsSource               = null;
                              DragRowDataGrid2.ItemsSource               = null;
                              CustomEmptyDataGrid.ItemsSource            = null;
                              EditableCellsDataGrid.ItemsSource          = null;
                              BasicPagingCaseGrid.ItemsSource            = null;
                              
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
            viewModel.EditableCellsDataSource.Add(new DataGridBaseInfo()
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
}