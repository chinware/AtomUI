using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridPagingShowCase : DataGridScenarioShowCase
{
    public DataGridPagingShowCase()
    {
        InitializeComponent();
        ShowTopPaginationCheckBox.IsCheckedChanged       += HandleShowTopPaginationCheckBoxChanged;
        ShowBottomPaginationCheckBox.IsCheckedChanged    += HandleShowBottomPaginationCheckBoxChanged;
        TopPaginationOptionGroup.OptionCheckedChanged    += HandleTopPaginationAlignChanged;
        BottomPaginationOptionGroup.OptionCheckedChanged += HandleBottomPaginationAlignChanged;
    }

    protected override void InitializeScenario(DataGridViewModel viewModel)
    {
        DataGridShowCaseDataSources.EnsurePagingGridDataSource(viewModel);
        BasicPagingCaseGrid.ItemsSource = viewModel.PagingGridDataSource;
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
}
