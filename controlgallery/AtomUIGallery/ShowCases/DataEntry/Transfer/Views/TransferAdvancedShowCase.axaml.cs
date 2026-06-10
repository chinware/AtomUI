using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Transfer;

public partial class TransferAdvancedShowCase : GalleryReactiveUserControl<TransferViewModel>
{
    public TransferAdvancedShowCase()
    {
        InitializeComponent();
    }

    private void ReloadAdvancedTransferItems(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TransferViewModel viewModel)
        {
            AdvanceTransfer.TargetKeys = viewModel.AdvanceTransferDefaultTargetKeys;
        }
    }
}
