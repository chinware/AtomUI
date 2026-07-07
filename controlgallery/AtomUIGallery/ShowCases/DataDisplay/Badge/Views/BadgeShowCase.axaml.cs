using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Badge;

public partial class BadgeShowCase : GalleryReactiveUserControl<BadgeViewModel>
{
    public const string LanguageId = nameof(BadgeShowCase);

    public BadgeShowCase()
    {
        InitializeComponent();
    }

}
