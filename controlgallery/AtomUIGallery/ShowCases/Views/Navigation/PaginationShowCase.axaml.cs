using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class PaginationShowCase : ReactiveUserControl<PaginationViewModel>
{
    public PaginationShowCase()
    {
        InitializeComponent();
    }
}