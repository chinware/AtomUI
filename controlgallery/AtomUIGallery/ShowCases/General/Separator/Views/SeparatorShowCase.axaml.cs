using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorShowCase : GalleryReactiveUserControl<SeparatorViewModel>
{
    public const string LanguageId = nameof(SeparatorShowCase);

    public SeparatorShowCase()
    {
        InitializeComponent();
    }
}
