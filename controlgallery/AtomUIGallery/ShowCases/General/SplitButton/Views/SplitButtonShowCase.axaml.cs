using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.SplitButton;

public partial class SplitButtonShowCase : ReactiveUserControl<SplitButtonViewModel>
{
    public const string LanguageId = nameof(SplitButtonShowCase);

    public SplitButtonShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
