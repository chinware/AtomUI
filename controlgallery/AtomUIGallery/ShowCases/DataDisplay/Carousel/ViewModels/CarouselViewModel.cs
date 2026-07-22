using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Carousel;

public class CarouselViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Carousel";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private CarouselPaginationPosition _paginationPosition = CarouselPaginationPosition.Bottom;

    public CarouselPaginationPosition PaginationPosition
    {
        get => _paginationPosition;
        set => this.RaiseAndSetIfChanged(ref _paginationPosition, value);
    }

    public CarouselViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
