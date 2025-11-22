using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SpinShowCase : ReactiveUserControl<SpinViewModel>
{
    public SpinShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}