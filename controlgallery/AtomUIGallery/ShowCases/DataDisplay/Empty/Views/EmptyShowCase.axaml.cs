
using Avalonia;
using Avalonia.Controls;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.Empty;

public partial class EmptyShowCase : GalleryReactiveUserControl<EmptyViewModel>
{
    public const string LanguageId = nameof(EmptyShowCase);

    public EmptyShowCase()
    {
        InitializeComponent();
    }

}
