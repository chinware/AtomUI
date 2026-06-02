using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Result;

public partial class ResultShowCase : ReactiveUserControl<ResultViewModel>
{
    public const string LanguageId = nameof(ResultShowCase);

    public ResultShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
