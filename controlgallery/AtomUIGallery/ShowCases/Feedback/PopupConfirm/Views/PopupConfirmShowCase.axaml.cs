using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public partial class PopupConfirmShowCase : ReactiveUserControl<PopupConfirmViewModel>
{
    public const string LanguageId = nameof(PopupConfirmShowCase);

    public PopupConfirmShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}