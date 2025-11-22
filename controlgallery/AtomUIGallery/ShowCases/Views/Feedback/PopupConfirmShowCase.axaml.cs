using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class PopupConfirmShowCase : ReactiveUserControl<PopupConfirmViewModel>
{
    public PopupConfirmShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}