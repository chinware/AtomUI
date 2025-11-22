using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class LineEditShowCase : ReactiveUserControl<LineEditViewModel>
{
    public LineEditShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}