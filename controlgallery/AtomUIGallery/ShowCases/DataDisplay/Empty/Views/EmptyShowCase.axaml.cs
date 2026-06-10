using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Empty;

public partial class EmptyShowCase : GalleryReactiveUserControl<EmptyViewModel>
{
    public const string LanguageId = nameof(EmptyShowCase);

    public EmptyShowCase()
    {
        InitializeComponent();
    }
}
