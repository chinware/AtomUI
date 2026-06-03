using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Palette;

public partial class PaletteShowCase : ReactiveUserControl<PaletteViewModel>
{
    public const string LanguageId = nameof(PaletteShowCase);

    public PaletteShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
