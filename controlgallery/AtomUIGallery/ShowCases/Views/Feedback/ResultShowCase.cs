using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class ResultShowCase : ReactiveUserControl<ResultViewModel>
{
    public ResultShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}