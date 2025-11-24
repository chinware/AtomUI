using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DescriptionsShowCase : ReactiveUserControl<DescriptionsViewModel>
{
    public DescriptionsShowCase()
    {
        if (DataContext is DescriptionsViewModel viewModel)
        {
            viewModel.DescriptionsSizeType = SizeType.Large;
        }

        InitializeComponent();
        // MiddleSizeRadioButton.IsCheckedChanged += SizeTypeCheckChanged;
        // DefaultSizeRadioButton.IsCheckedChanged += SizeTypeCheckChanged;
        // SmallSizeRadioButton.IsCheckedChanged   += SizeTypeCheckChanged;
    }
    
    // private void SizeTypeCheckChanged(object? sender, RoutedEventArgs e)
    // {
    //     if (sender is RadioButton radioButton && radioButton.IsChecked == true)
    //     {
    //         if (DataContext is DescriptionsViewModel viewModel)
    //         {
    //             if (radioButton == DefaultSizeRadioButton)
    //             {
    //                 viewModel.DescriptionsSizeType = SizeType.Large;
    //             }
    //             else if (radioButton == MiddleSizeRadioButton)
    //             {
    //                 viewModel.DescriptionsSizeType = SizeType.Middle;
    //             }
    //             else
    //             {
    //                 viewModel.DescriptionsSizeType = SizeType.Small;
    //             }
    //         }
    //     }
    // }
}