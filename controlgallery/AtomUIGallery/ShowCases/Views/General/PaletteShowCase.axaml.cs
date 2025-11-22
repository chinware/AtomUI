using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class PaletteShowCase : ReactiveUserControl<PaletteViewModel>
{
    public PaletteShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}