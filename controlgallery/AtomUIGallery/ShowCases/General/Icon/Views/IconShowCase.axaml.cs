using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Icon;

public partial class IconShowCase : ReactiveUserControl<IconViewModel>
{
    public IconShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
