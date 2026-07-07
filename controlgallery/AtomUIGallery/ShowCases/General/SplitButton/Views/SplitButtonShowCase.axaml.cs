using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.SplitButton;

public partial class SplitButtonShowCase : GalleryReactiveUserControl<SplitButtonViewModel>
{
    public const string LanguageId = nameof(SplitButtonShowCase);

    public SplitButtonShowCase()
    {
        InitializeComponent();
    }

}
