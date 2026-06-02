using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Separator;

public partial class SeparatorShowCase : ReactiveUserControl<SeparatorViewModel>
{
    public SeparatorShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
