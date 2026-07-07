using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.ProgressBar;

public partial class ProgressBarShowCase : GalleryReactiveUserControl<ProgressBarViewModel>
{
    public const string LanguageId = nameof(ProgressBarShowCase);

    public ProgressBarShowCase()
    {
        InitializeComponent();
    }

}
