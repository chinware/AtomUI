using System.ComponentModel;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using Avalonia.Threading;
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
                Dispatcher.UIThread.Post(() => BasicCaseGrid.ItemsSource         = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SelectionDataGrid.ItemsSource     = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FilterAndSortGrid.ItemsSource     = viewModel.FilterAndSorterDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FilterInTreeGrid.ItemsSource      = viewModel.FilterAndSorterDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => MultiSorterDataGrid.ItemsSource   = viewModel.MultiSorterDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => ResetFilterAndSortGrid.ItemsSource = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragResizeColumn.ItemsSource = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => LargeSizeDataGrid.ItemsSource              = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => MiddleSizeDataGrid.ItemsSource             = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => SmallSizeDataGrid.ItemsSource              = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => CustomHeaderAndFooterDataGrid.ItemsSource  = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => ExpandableDataGrid.ItemsSource             = viewModel.ExpandableRowDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => OrderSpecificColumnDataGrid.ItemsSource    = viewModel.ExpandableRowDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => RowAndColumnHeaderDataGrid.ItemsSource     = viewModel.ExpandableRowDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => GroupHeaderDataGrid.ItemsSource            = viewModel.GroupHeaderDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => HideColumnDataGrid.ItemsSource             = viewModel.BasicCaseDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FixedHeaderDataGrid.ItemsSource            = viewModel.FixedHeaderDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FixedColumnsDataGrid1.ItemsSource = viewModel.FixedColumnsDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FixedColumnsDataGrid2.ItemsSource = viewModel.FixedColumnsDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => FixedColumnsAndHeadersDataGrid.ItemsSource = viewModel.FixedColumnsAndHeadersDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragColumnDataGrid1.ItemsSource = viewModel.DragColumnDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragColumnDataGrid2.ItemsSource = viewModel.DragColumnDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragColumnDataGrid3.ItemsSource = viewModel.DragColumnDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragRowDataGrid1.ItemsSource = viewModel.DragRowDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => DragRowDataGrid2.ItemsSource = viewModel.DragRowManyDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => CustomEmptyDataGrid.ItemsSource            = viewModel.CustomEmptyDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => EditableCellsDataGrid.ItemsSource          = viewModel.EditableCellsDataSource, DispatcherPriority.Background);
                Dispatcher.UIThread.Post(() => BasicPagingCaseGrid.ItemsSource = viewModel.PagingGridDataSource, DispatcherPriority.Background);
            }

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