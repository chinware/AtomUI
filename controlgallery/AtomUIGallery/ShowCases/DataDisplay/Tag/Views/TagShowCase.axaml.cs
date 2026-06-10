using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Tag;

public partial class TagShowCase : GalleryReactiveUserControl<TagViewModel>
{
    public const string LanguageId = nameof(TagShowCase);

    public TagShowCase()
    {
        InitializeComponent();
    }
}
