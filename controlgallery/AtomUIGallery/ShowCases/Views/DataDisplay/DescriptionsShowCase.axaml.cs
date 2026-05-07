using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DescriptionsShowCase : ReactiveUserControl<DescriptionsViewModel>
{
    public DescriptionsShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                viewModel.DescriptionsSizeType = SizeType.Large;
            }

            MiddleSizeRadioButton.IsCheckedChanged += SizeTypeCheckChanged;
            DefaultSizeRadioButton.IsCheckedChanged += SizeTypeCheckChanged;
            SmallSizeRadioButton.IsCheckedChanged += SizeTypeCheckChanged;

            Disposable.Create(() =>
            {
                MiddleSizeRadioButton.IsCheckedChanged -= SizeTypeCheckChanged;
                DefaultSizeRadioButton.IsCheckedChanged -= SizeTypeCheckChanged;
                SmallSizeRadioButton.IsCheckedChanged -= SizeTypeCheckChanged;
            }).DisposeWith(disposables);
        });

        InitializeComponent();
    }

    private void SizeTypeCheckChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                if (radioButton == DefaultSizeRadioButton)
                {
                    viewModel.DescriptionsSizeType = SizeType.Large;
                }
                else if (radioButton == MiddleSizeRadioButton)
                {
                    viewModel.DescriptionsSizeType = SizeType.Middle;
                }
                else
                {
                    viewModel.DescriptionsSizeType = SizeType.Small;
                }
            }
        }
    }
}
