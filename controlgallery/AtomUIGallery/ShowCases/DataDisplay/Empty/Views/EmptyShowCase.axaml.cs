using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Empty;

public partial class EmptyShowCase : ReactiveUserControl<EmptyViewModel>
{
    public EmptyShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
