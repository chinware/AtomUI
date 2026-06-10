using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Pagination;

public partial class PaginationShowCase : GalleryReactiveUserControl<PaginationViewModel>
{
    public const string LanguageId = nameof(PaginationShowCase);

    public PaginationShowCase()
    {
        this.WhenActivated(disposables =>
        {
        });
        InitializeComponent();
    }
}
