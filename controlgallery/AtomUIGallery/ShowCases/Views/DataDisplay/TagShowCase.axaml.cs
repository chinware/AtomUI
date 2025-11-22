using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class TagShowCase : ReactiveUserControl<TagViewModel>
{
    public TagShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}