using System;

namespace AtomUIGallery.ShowCases.Notification;

public partial class NotificationApiDataGrid : GalleryReactiveUserControl<NotificationViewModel>
{
    public NotificationApiDataGrid()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is NotificationViewModel viewModel)
        {
            viewModel.EnsureApiRows();
            ApiDataGrid.ItemsSource = viewModel.ApiRows;
        }
        else
        {
            ApiDataGrid.ItemsSource = null;
        }
    }
}
