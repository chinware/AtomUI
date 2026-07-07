
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Tag;

public partial class TagShowCase : GalleryReactiveUserControl<TagViewModel>
{
    public const string LanguageId = nameof(TagShowCase);

    public TagShowCase()
    {
        InitializeComponent();
    }

}
