
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Pagination;

public partial class PaginationShowCase : GalleryReactiveUserControl<PaginationViewModel>
{
    public const string LanguageId = nameof(PaginationShowCase);

    public PaginationShowCase()
    {
        InitializeComponent();
    }

}
