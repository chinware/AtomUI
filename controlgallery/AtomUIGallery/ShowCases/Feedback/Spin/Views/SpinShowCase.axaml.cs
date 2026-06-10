using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Spin;

public partial class SpinShowCase : GalleryReactiveUserControl<SpinViewModel>
{
    public const string LanguageId = nameof(SpinShowCase);

    public SpinShowCase()
    {
        InitializeComponent();
    }
}
