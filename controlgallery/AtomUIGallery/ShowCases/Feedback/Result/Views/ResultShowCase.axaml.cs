using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Result;

public partial class ResultShowCase : ReactiveUserControl<ResultViewModel>
{
    public ResultShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
