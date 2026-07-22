using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Breadcrumb;

public class BreadcrumbViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Breadcrumb";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<BreadcrumbItemData>? _breadcrumbItems = [];

    public List<BreadcrumbItemData>? BreadcrumbItems
    {
        get => _breadcrumbItems;
        set => this.RaiseAndSetIfChanged(ref _breadcrumbItems, value);
    }

    public BreadcrumbViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }

}
