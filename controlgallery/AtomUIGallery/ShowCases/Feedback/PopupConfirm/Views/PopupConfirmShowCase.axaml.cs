using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public partial class PopupConfirmShowCase : ReactiveUserControl<PopupConfirmViewModel>
{
    public PopupConfirmShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}