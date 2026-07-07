using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Avatar;

public partial class AvatarShowCase : GalleryReactiveUserControl<AvatarViewModel>
{
    public const string LanguageId = nameof(AvatarShowCase);

    public AvatarShowCase()
    {
        InitializeComponent();
    }

}
