using System.Reactive.Disposables;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CarouselShowCase : ReactiveUserControl<CarouselViewModel>
{
    public CarouselShowCase()
    {
        this.WhenActivated(disposables =>
        {
            PositionOptionGroup.OptionCheckedChanged += HandlePositionOptionChanged;
            disposables.Add(Disposable.Create(() => PositionOptionGroup.OptionCheckedChanged -= HandlePositionOptionChanged));
        });
        InitializeComponent();
    }
    
    public void HandlePositionOptionChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is CarouselViewModel viewModel)
        {
            if (args.Index == 0)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Top;
            }
            else if (args.Index == 1)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Bottom;
            }
            else if (args.Index == 2)
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Left;
            }
            else
            {
                viewModel.PaginationPosition = CarouselPaginationPosition.Right;
            }   
        }
     
    }
}