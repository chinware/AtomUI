
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorShowCase : GalleryReactiveUserControl<SeparatorViewModel>
{
    public const string LanguageId = nameof(SeparatorShowCase);

    public SeparatorShowCase()
    {
        InitializeComponent();
    }

}
