using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Pagination;

public partial class PaginationShowCase : ReactiveUserControl<PaginationViewModel>
{
    public PaginationShowCase()
    {
        this.WhenActivated(disposables =>
        {
        });
        InitializeComponent();
    }
}
