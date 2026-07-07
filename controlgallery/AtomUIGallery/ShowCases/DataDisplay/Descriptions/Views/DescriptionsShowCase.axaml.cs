using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Descriptions;

public partial class DescriptionsShowCase : GalleryReactiveUserControl<DescriptionsViewModel>
{
    public const string LanguageId = nameof(DescriptionsShowCase);

    public DescriptionsShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                viewModel.DescriptionsSizeType = SizeType.Large;
            }
        });
    }

    private void SizeTypeCheckChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton { IsChecked: true, Tag: SizeType sizeType })
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                viewModel.DescriptionsSizeType = sizeType;
            }
        }
    }
}
