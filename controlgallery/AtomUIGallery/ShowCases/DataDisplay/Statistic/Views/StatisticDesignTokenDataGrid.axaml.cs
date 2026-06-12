using System;

namespace AtomUIGallery.ShowCases.Statistic;

public partial class StatisticDesignTokenDataGrid : GalleryReactiveUserControl<StatisticViewModel>
{
    public StatisticDesignTokenDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is StatisticViewModel viewModel)
        {
            viewModel.EnsureDesignTokenRows();
            DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows;
        }
        else
        {
            DesignTokenDataGrid.ItemsSource = null;
        }
    }
}
