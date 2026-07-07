using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Spin;

public partial class SpinShowCase : GalleryReactiveUserControl<SpinViewModel>
{
    public const string LanguageId = nameof(SpinShowCase);

    public SpinShowCase()
    {
        InitializeComponent();
    }

}
