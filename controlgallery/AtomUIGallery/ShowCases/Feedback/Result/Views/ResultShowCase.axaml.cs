
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Result;

public partial class ResultShowCase : GalleryReactiveUserControl<ResultViewModel>
{
    public const string LanguageId = nameof(ResultShowCase);

    public ResultShowCase()
    {
        InitializeComponent();
    }

}
