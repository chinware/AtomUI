using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Pagination;

public partial class PaginationShowCase : ReactiveUserControl<PaginationViewModel>
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
