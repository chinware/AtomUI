using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class IconShowCase : ReactiveUserControl<IconViewModel>
{
    public IconShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}