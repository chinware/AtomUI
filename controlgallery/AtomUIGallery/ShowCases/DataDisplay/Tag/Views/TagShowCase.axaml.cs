using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Tag;

public partial class TagShowCase : ReactiveUserControl<TagViewModel>
{
    public TagShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
