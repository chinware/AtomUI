using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class EmptyShowCase : ReactiveUserControl<EmptyViewModel>
{
    public EmptyShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}